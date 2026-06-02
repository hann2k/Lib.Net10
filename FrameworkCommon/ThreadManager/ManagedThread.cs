using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace Framework.Common.ThreadManager
{
    public abstract class ManagedThread : Scheduler
    {
        protected Thread Thread;
        
        /// <summary>
        /// true : Timer.
        /// false : Thread.
        /// </summary>
        public bool RunMode { get; set; } = false;

        /// <summary>
        /// 스레드를 시작한다.
        /// </summary>
        public override void Start()
        {
            if (this.Thread != null)
            {
                this.Thread.Start();
                this.Running = true;
            }
        }

        public override void Suspend()
        {
            if (this.Running)
            {
                this.Suspending = true;
            }
        }

        public override void Resume()
        {
            if (this.Running)
            {
                this.Suspending = false;
            }
        }

		/// <summary>
		/// 스레드를 종료한다.
		/// </summary>
		public override void Stop() => this.Running = false;
	}

    public abstract class ParameterlizedThread : ManagedThread
    {
        public ParameterlizedThread()
        {
			this.Thread = new Thread(new ParameterizedThreadStart(this.WrapThreadRunner)) {
				IsBackground = true
			};
		}

        protected virtual void MyThreadRunner(object param)
        {

        }

        protected virtual void WrapThreadRunner(object param)
        {
            
            while (this.Running)
            {
                try
                {
                    if (!base.Suspending)
                    {
                        this.MyThreadRunner(param);
                    }
                    else
                    {
                        Thread.Sleep(1);
                    }
                }
                catch(Exception ex)
                {
                    Logger.Log.Ins.Exception(ex);
                    break;
                }
            }
        }
    }

    public abstract class NoneParameterThread : ManagedThread
    {
        public NoneParameterThread()
        {
			this.Thread = new Thread( new ThreadStart( this.WrapThreadRunner ) ) {
				IsBackground = true
			};
		}

		protected virtual void CustomThreadRunner() => Thread.Sleep( 1000 );



		private void WrapThreadRunner()
        {
            while (this.Running)
            {
                try
                {
                    if (!base.Suspending)
                    {
                        if (base.RunMode)
                        {
                            base.OnTick();
                            Thread.Sleep(base.Interval-11);
                        }
                        else
                        {
                            this.CustomThreadRunner();
                            Thread.Sleep(1);
                        }
                    }
                    else
                    {
                        Thread.Sleep(1);
                    }
                }
                catch (Exception ex)
                {
                    Logger.Log.Ins.Exception(ex);
                    break;
                }
            }
        }
    }
}
