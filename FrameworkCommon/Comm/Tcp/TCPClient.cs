using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

using Framework.Common.Converter;
using Framework.Common.Event;
using Framework.Common.Logger;

namespace Framework.Common.Comm
{
    /// <summary>
    /// Asynchronous TCP client with priority and normal send queues.
    /// </summary>
    public abstract class TCPClient
    {
        private readonly Queue<byte[]> SendQueue = new Queue<byte[]>();
        private readonly Queue<byte[]> FirstSendQueue = new Queue<byte[]>();
        private readonly SemaphoreSlim SendSignal = new SemaphoreSlim(0);

        private TcpClient Client;
        private NetworkStream Nstream;
        private CancellationTokenSource CommunicationCancellation;
        private Task Sending = Task.CompletedTask;
        private Task Receiving = Task.CompletedTask;
        private int ActiveSendCount;

        protected readonly string ClientID = string.Empty;
        protected readonly Queue<byte[]> ReceivedPacketQueue = new Queue<byte[]>();
        protected bool RemoteLogging = true;
        protected int SendCount;
        protected int RcvCount;

        protected abstract int TcpPacketSize { get; }

        protected TCPClient()
        {
        }

        protected TCPClient(string id)
        {
            this.ClientID = id;
        }

        public event EventHandler<ConnectionStateArgs> ClientConnected;
        public event EventHandler<ConnectionStateArgs> ClientDisconnected;
        public event EventHandler<EventPingArgs> EventPing;

        public bool SimMode { get; private set; }
        public bool IsConnected { get; private set; }

        public void DebugModeOn() => this.SimMode = true;

        public void SetLocalLogging() => this.RemoteLogging = false;

        public bool Open(string serverIP, int serverPort) => this.Open(serverIP, serverPort, true);

        public bool Open(string serverIP, int serverPort, bool connect = true) =>
            this.OpenAsync(serverIP, serverPort, connect).GetAwaiter().GetResult();

        public async Task<bool> OpenAsync(string serverIP, int serverPort, bool connect = true)
        {
            if (this.IsConnected)
            {
                return true;
            }

            try
            {
                if (!this.SimMode)
                {
                    Log.Ins.Debug($" -------------------- {this.ClientID}.Connect()", this.RemoteLogging);

                    this.Client = new TcpClient {
                        ReceiveTimeout = 1000,
                    };
                    await this.Client.ConnectAsync(serverIP, serverPort).ConfigureAwait(false);
                    this.Nstream = this.Client.GetStream();
                }

                this.IsConnected = true;
                this.CommunicationCancellation = new CancellationTokenSource();
                this.Sending = this.SendAsync(this.CommunicationCancellation.Token);
                this.Receiving = this.SimMode
                    ? Task.CompletedTask
                    : this.ReceiveAsync(this.CommunicationCancellation.Token);

                Log.Ins.Info($"{this.ClientID}.Connected()", this.RemoteLogging);
                this.ClientConnected?.Invoke(this, new ConnectionStateArgs { Connected = true });
            }
            catch (Exception ex)
            {
                Log.Ins.Exception(ex);
                this.IsConnected = false;
            }

            return this.IsConnected;
        }

        public void Close() => this.CloseAsync().GetAwaiter().GetResult();

        public async Task CloseAsync()
        {
            if (!this.IsConnected)
            {
                return;
            }

            while (this.HasPendingSend())
            {
                await Task.Delay(10).ConfigureAwait(false);
            }

            this.IsConnected = false;
            this.CommunicationCancellation?.Cancel();
            this.Nstream?.Close();
            this.Client?.Close();

            await this.WaitForCommunicationTasksAsync().ConfigureAwait(false);

            this.CommunicationCancellation?.Dispose();
            this.CommunicationCancellation = null;

            Log.Ins.Info($"{this.ClientID}.Disconnected()");
            this.ClientDisconnected?.Invoke(this, new ConnectionStateArgs { Connected = false });
        }

        public void SendFirst(byte[] buffer)
        {
            Log.Ins.Info("Priority send: " + ByteConverter.ToHexString(buffer), this.RemoteLogging);
            this.Enqueue(this.FirstSendQueue, buffer);
        }

        public void Send(byte[] buffer) => this.Enqueue(this.FirstSendQueue, buffer);

        public void Send(string message) => this.Send(ByteConverter.ToBytes(message));

        public void SendAdd(byte[] buffer) => this.Enqueue(this.SendQueue, buffer);

        protected abstract void ParseReceivedData();

        protected abstract List<byte[]> SplitRcvDataByPacket(byte[] buffer, int size);

        protected virtual void OnEventPing(EventPingArgs e) => this.EventPing?.Invoke(this, e);

        private void Enqueue(Queue<byte[]> queue, byte[] buffer)
        {
            lock (queue)
            {
                queue.Enqueue(buffer);
            }

            this.SendSignal.Release();
        }

        private bool HasPendingSend()
        {
            lock (this.FirstSendQueue)
            {
                if (this.FirstSendQueue.Count > 0)
                {
                    return true;
                }
            }

            lock (this.SendQueue)
            {
                return this.SendQueue.Count > 0 || Volatile.Read(ref this.ActiveSendCount) > 0;
            }
        }

        private bool TryDequeue(out byte[] buffer, out bool first)
        {
            lock (this.FirstSendQueue)
            {
                if (this.FirstSendQueue.Count > 0)
                {
                    buffer = this.FirstSendQueue.Dequeue();
                    first = true;
                    return true;
                }
            }

            lock (this.SendQueue)
            {
                if (this.SendQueue.Count > 0)
                {
                    buffer = this.SendQueue.Dequeue();
                    first = false;
                    return true;
                }
            }

            buffer = null;
            first = false;
            return false;
        }

        private async Task SendAsync(CancellationToken cancellationToken)
        {
            try
            {
                while (!cancellationToken.IsCancellationRequested)
                {
                    await this.SendSignal.WaitAsync(cancellationToken).ConfigureAwait(false);

                    if (!this.TryDequeue(out var buffer, out var first))
                    {
                        continue;
                    }

                    Interlocked.Increment(ref this.ActiveSendCount);
                    try
                    {
                        await this.SendAsync(buffer, first, cancellationToken).ConfigureAwait(false);
                    }
                    catch (Exception ex)
                    {
                        Log.Ins.Exception(ex);
                    }
                    finally
                    {
                        Interlocked.Decrement(ref this.ActiveSendCount);
                    }
                }
            }
            catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
            {
            }
        }

        private async Task SendAsync(byte[] buffer, bool first, CancellationToken cancellationToken)
        {
            if (this.SimMode)
            {
                Log.Ins.Sim($"{this.ClientID}.Send(simul,{this.SendCount}) : {ByteConverter.ToHexString(buffer)}", this.RemoteLogging);
                return;
            }

            await this.Nstream.WriteAsync(buffer.AsMemory(), cancellationToken).ConfigureAwait(false);
            await this.Nstream.FlushAsync(cancellationToken).ConfigureAwait(false);

            var mode = first ? " [priority]" : string.Empty;
            Log.Ins.Info($"{this.ClientID}.Send({this.SendCount}) : {ByteConverter.ToHexString(buffer)}{mode}", this.RemoteLogging);
            this.SendCount = this.SendCount >= int.MaxValue - 1 ? 0 : this.SendCount + 1;
        }

        private async Task ReceiveAsync(CancellationToken cancellationToken)
        {
            var buffer = new byte[this.TcpPacketSize];

            try
            {
                while (!cancellationToken.IsCancellationRequested)
                {
                    var readSize = await this.Nstream.ReadAsync(buffer.AsMemory(), cancellationToken).ConfigureAwait(false);
                    if (readSize == 0)
                    {
                        break;
                    }

                    Log.Ins.Info($"{this.ClientID}.Receive({this.RcvCount}, {readSize}) : {ByteConverter.ToHexString(buffer, readSize)}", this.RemoteLogging);
                    this.RcvCount = this.RcvCount >= int.MaxValue - 1 ? 0 : this.RcvCount + 1;

                    foreach (var packet in this.SplitRcvDataByPacket(buffer, readSize))
                    {
                        this.ReceivedPacketQueue.Enqueue(packet);
                    }

                    this.ParseReceivedData();
                }
            }
            catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
            {
            }
            catch (ObjectDisposedException)
            {
            }
            catch (Exception ex)
            {
                Log.Ins.Exception($"{this.ClientID} receive loop exception");
                Log.Ins.Exception(ex);
                this.ClientDisconnected?.Invoke(this, new ConnectionStateArgs { Connected = false });
            }
        }

        private async Task WaitForCommunicationTasksAsync()
        {
            try
            {
                await Task.WhenAll(this.Sending, this.Receiving).ConfigureAwait(false);
            }
            catch (OperationCanceledException)
            {
            }
        }
    }
}
