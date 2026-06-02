using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Framework.Common.ThreadManager
{
    public class TimerManager : Scheduler
    {
        private Timer Timer;

        /// <summary>
        /// 타이머 실행되기까지 대기 시간
        /// </summary>
        public int DueTime { get; set; } = 0;

        /// <summary>
        /// 타이머 대기시간중 표시 플래그
        /// </summary>
        public bool DueTimeFlag { get; private set; } = true;

        /// <summary>
        /// 타이머 실행 주기
        /// </summary>
        public int Period { get; set; } = 1000;

        public TimerManager()
        {

        }

        public TimerManager(int dueTime, int period)
        {
            this.DueTime = dueTime;
            this.Period = period;
        }

        public override void Resume()
        {
            // throw new NotImplementedException();
        }

        public override void Start()
        {
            this.DueTimeFlag = true;
            this.Timer = new Timer(this.TimerCallBack, null, this.DueTime, this.Period);
        }

        public override void Stop()
        {
            this.Timer.Dispose();
            this.DueTimeFlag = true;
        }

        public override void Suspend()
        {
            // throw new NotImplementedException();
        }

        protected virtual void TimerCallBack(object obj)
        {
            base.OnTick();
            this.DueTimeFlag = false;
        }
    }

    public class Power3SecReceiver : TimerManager
    {
        private readonly List<int> rcv = new List<int>();

        public Power3SecReceiver()
        {
            base.DueTime = 3000;
            base.Period = 3000;
        }

        protected override void TimerCallBack(object obj)
        {
            base.TimerCallBack(obj);
        }

        public void Store(short rcv)
        {
            this.rcv.Add(rcv);
        }

        public void Clear()
        {
            this.rcv.Clear();
        }

        public short Max()
        {
            int max = 0;

            if (this.rcv.Count > 0)
            {
                max = this.rcv.Max();
                this.Clear();
            }

            return (short)max;
        }
    }
}
