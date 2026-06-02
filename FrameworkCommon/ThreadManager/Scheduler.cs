using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Framework.Common.ThreadManager
{
    public abstract class Scheduler
    {
        public event EventHandler Tick;

        protected bool Running = true;
        protected bool Suspending = false;

        public int Interval { get; set; } = 1000;

		protected virtual void OnTick() => this.Tick?.Invoke( this, EventArgs.Empty );

		/// <summary>
		/// 스레드를 시작한다.
		/// </summary>
		public abstract void Start();

        /// <summary>
        /// 스레드를 대기 상태로 설정한다.
        /// </summary>
        public virtual void Wait()
        {
            this.Start();
            this.Suspending = true;
        }

        /// <summary>
        /// 스레드를 잠시 중단한다.
        /// </summary>
        public abstract void Suspend();

        /// <summary>
        /// 중단된 스레드를 재개한다.
        /// </summary>
        public abstract void Resume();

        /// <summary>
        /// 스레드를 완전히 종료한다.
        /// </summary>
        public abstract void Stop();
    }
}
