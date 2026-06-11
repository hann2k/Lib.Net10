using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;

using Framework.Common.Singleton;
using Framework.Common.Enum;

namespace Framework.Common.Logger
{

    public class Log : Singleton<Log>
    {

        private readonly Queue<LogItem> LogBuffer = new Queue<LogItem>();
        private readonly object LogBufferLock = new object();

        /// <summary>
        /// 현재 인스턴스의 로깅레벨
        /// </summary>
        private LogType LogLevel = LogType.Debug;

        /// <summary>
        /// 기본 로그 경로
        /// </summary>
        private string LogDir = @".\Log";

        private Thread LogThread;
        private bool LogRunFlag = false;

		private string LogServerIP;
		private int LogServerPort;
		private readonly BasicLogClient LogClient = new BasicLogClient("LogClient");
		// private Queue<LogItem> LogList = new Queue<LogItem>();

		protected override void Constructor()
        {
        }

		public void SetLogLevel(LogType level) => this.LogLevel = level;

        /// <summary>
        /// 로그파일이 저장될 디렉토리를 설정한다. 디렉토리가 존재하지 않으면 생성한다.
        /// </summary>
        /// <param name="logDir"></param>
        public void SetLogDir(string logDir)
        {
            this.LogDir = logDir;
            if (!Directory.Exists(logDir))
            {
                Directory.CreateDirectory(logDir);
            }
        }

		public void Connect(string ip, int port)
		{
			this.LogServerIP = ip;
			this.LogServerPort = port;
			this.LogClient.SetLocalLogging();
			this.LogClient.Open(ip, port);
		}

		public void Stop()
        {
            while( this.LogBuffer.Count > 0 )
            {
                Thread.Sleep(100);
            }

			if (this.LogClient.IsConnected)
			{
				this.LogClient.Close();
			}

            this.LogRunFlag = false;
        }

		public string Fatal(string msg) => this.Fatal(msg, false);
		public string Fatal(string msg, bool remote) => this.WriteLog(LogType.Fatal, msg, remote);

		public string Drop(Exception ex) => string.Empty;// 버린다.

		public string Exception(Exception ex) => this.Exception(ex.ToString(), false);

		public string Exception(string msg) => this.Exception(msg, false);
		public string Exception(string msg, bool remote) => this.WriteLog(LogType.Exception, msg, remote);

		public string Error(string msg) => this.Error(msg, false);
		public string Error(string msg, bool remote) => this.WriteLog(LogType.Error, msg, remote);

		public string Warning(string msg) => this.Warning(msg, false);
		public string Warning(string msg, bool remote) => this.WriteLog(LogType.Warning, msg, remote);

		public string Info(string msg) => this.Info(msg, false);
		public string Info(string msg, bool remote) => this.WriteLog(LogType.Info, msg, remote);

		public string Sim(string msg) => this.Sim(msg, false);
		public string Sim(string msg, bool remote)
		{
			var s = this.WriteLog(LogType.Simul, msg, remote);
			Console.WriteLine(s);
			return s;
		}


		/// <summary>
        /// 디버그 로그를 기록할 때는 로그메시지에 호출한 소스코드의 파일명, 라인번호, 멤버명을 포함하여 기록한다.
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="memberName"></param>
        /// <param name="sourceFilePath"></param>
        /// <param name="sourceLineNumber"></param>
        /// <returns></returns>
        public string Debug(string msg,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
            msg = $"[{Path.GetFileName(sourceFilePath)}:{sourceLineNumber} {memberName}] {msg}";
            return this.WriteLog(LogType.Debug, msg, false);
        }

        /// <summary>
        /// 원격지로 디버그 로그를 전송할 때는 로그메시지에 호출한 소스코드의 파일명, 라인번호, 멤버명을 포함하여 전송한다.
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="remote"></param>
        /// <param name="memberName"></param>
        /// <param name="sourceFilePath"></param>
        /// <param name="sourceLineNumber"></param>
        /// <returns></returns>
		public string Debug(string msg, bool remote,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
            msg = $"[{Path.GetFileName(sourceFilePath)}:{sourceLineNumber} {memberName}] {msg}";
            return this.WriteLog(LogType.Debug, msg, remote);
        }

		/// <summary>
        /// 디버그 로그들을 기록할 때는 첫번째 로그메시지에 호출한 소스코드의 파일명, 라인번호, 멤버명을 포함하여 기록한다.
        /// </summary>
        /// <param name="msgs"></param>
        /// <param name="memberName"></param>
        /// <param name="sourceFilePath"></param>
        /// <param name="sourceLineNumber"></param>
        /// <returns></returns>
        public string Debug(string[] msgs,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
            // 첫 번째 메시지에는 호출한 소스코드의 파일명, 라인번호, 멤버명을 포함하여 기록한다.
			var t = this.WriteLog(LogType.Debug, $"[{Path.GetFileName(sourceFilePath)}:{sourceLineNumber} {memberName}] {msgs[0]}", false);
            foreach(var s in msgs.Skip(1))
            {
                // 나머지 메시지는 그대로 기록한다.
                t += this.WriteLog(LogType.Debug, s, false);
            }

            return t;
        }

        private string WriteLog(LogType type, string msg, bool remote)
        {
            if (this.LogRunFlag == false)
            {
                this.StartLogThread();
            }

            lock (this.LogBuffer)
            {
                if (this.LogLevel >= type)
                {
					var item = new LogItem(type, DateTime.Now, msg, remote);

					this.LogBuffer.Enqueue(item);

					return item.FullMessage;
                }
            }
            return string.Empty;
        }

        private void StartLogThread()
        {
            this.LogRunFlag = true;

            this.LogThread = new Thread(new ThreadStart(this.WriteLogFileThread))
            {
                IsBackground = true
            };

            this.LogThread.Start();
        }

        private void WriteLogFileThread()
        {
            while(this.LogRunFlag)
            {
				lock (this.LogBufferLock)
				{
					var count = this.LogBuffer.Count;

					if (count > 0)
					{
						var msgList = new List<string>();
						try
						{
							do
							{
								LogItem item = this.LogBuffer.Dequeue();
								if (item != null)
								{
									msgList.Add(item.FullMessage);
									if (this.LogClient.IsConnected && item.Remote)
									{
										this.LogClient.SendAdd(item.GetBytes());
									}
								}
							}
							while (--count > 0);

							if (msgList.Count > 0)
							{
								this.WriteLogFile(msgList);
							}
						}
						catch (Exception ex)
						{
							this.Exception(ex);
						}
					}
					else
					{
						Thread.Sleep(50);
					}
				}
            }
        }

        private void WriteLogFile(List<string> msgList)
        {
			// var LogDir = @".\Log";
            this.CheckCreateLogDir(LogDir);

			// 로그파일 경로를 매일 변경되도록 지정한다.
			// 로크파일의 크기가 너무 크면 인덱스를 증가한다.
			var filePath =  $"{LogDir}\\SystemLog_{DateTime.Now.ToString("yyyyMMdd")}.log";

            // Trace.WriteLine($"File Path : {filePath}");

            try
            {
                // 파일에 기록한다.
                using (var DestinationWriter = new StreamWriter(filePath, true, Encoding.UTF8))
                {
                    foreach (var msg in msgList)
                    {
                        DestinationWriter.WriteLine(msg);
                        // Console.WriteLine(msg);
                    }
                }
            }
            catch
            {
                // 파일 기록에 실패하면 Trace에 기록한다.
                foreach (var msg in msgList)
                {
                    Trace.WriteLine(msg);
                }
            }
        }

        public void CheckCreateLogDir(string LogDir)
        {
			var di = new DirectoryInfo(LogDir);

            try
            {
                if (!di.Exists)
                {
                    di.Create();
                    Trace.WriteLine("The directory was created successfully.");
                }

            }
            catch (Exception e)
            {
                Trace.WriteLine("The process failed: {0}", e.ToString());
            }
        }
    }
}
