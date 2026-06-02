using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Framework.Common.Singleton
{
    public abstract class Singleton<T>
        where T : class, new()
    {
        private static readonly Lazy<T> InstanceValue = new Lazy<T>(() => new T());

        /// <summary>
        /// Initializes a new instance of the <see cref="Singleton{T}"/> class.
        /// </summary>
        protected Singleton()
        {
            this.Constructor();
        }

        protected abstract void Constructor();

        /// <summary>
        /// Gets 싱글톤 클래스의 인스턴스.
        /// </summary>
        public static T Ins
        {
            get
            {
                return InstanceValue.Value;
            }
        }

        /// <summary>
        /// 비정상 동작한다. 사용금지.
        /// </summary>
        /// <returns></returns>
        public static T GetInstance()
        {
            throw new Exception("Singleton<T>.GetInstance() 이 코드는 원하는대로 동작하지 않는다. 수정전까지 사용금지.");
            // return Ins;
        }
    }

    public interface ISingleton<T> where T : class
    {
        void CreateSingle(T _class);
    }
}
