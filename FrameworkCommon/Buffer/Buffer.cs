using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Framework.Common.Buffer
{
    /// <summary>
    /// 데이터 저장소
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class Buffer<T>
    {
        protected readonly List<T> BUFFER = new List<T>();

        public T this[int i] {
            get => this.BUFFER[i];
            set => this.BUFFER[i] = value;
        }

		/// <summary>
		/// 패킷 버퍼 크기
		/// </summary>
		public int Length => this.BUFFER.Count;

		/// <summary>
		/// 버퍼 지우기
		/// </summary>
		public void Clear() => this.BUFFER.Clear();

		/// <summary>
		/// 버퍼에 아이템 1개를 추가한다.
		/// </summary>
		/// <param name="header"></param>
		public void Add(T b) => this.BUFFER.Add(b);

		/// <summary>
		/// 버퍼에 배열을 추가한다.
		/// </summary>
		/// <param name="array"></param>
		public void AddRange(T[] array) => this.BUFFER.AddRange(array);

		/// <summary>
		/// 버퍼에 리스트를 추가한다.
		/// </summary>
		/// <param name="header"></param>
		public void AddRange(List<T> lst) => this.BUFFER.AddRange(lst);

		/// <summary>
		/// 버퍼에 리스트를 추가한다.
		/// </summary>
		/// <param name="lst"></param>
		public void AddRange(Buffer<T> lst) => this.BUFFER.AddRange(lst.BUFFER);

		/// <summary>
		/// 지정한 위치의 아이템을 삭제한다.
		/// </summary>
		/// <param name="index"></param>
		public void RemoveAt(int index) => this.BUFFER.RemoveAt(index);

		/// <summary>
		/// 지정한 아이템을 버퍼에서 검색하여 가장 처음의 아이템을 삭제한다.
		/// </summary>
		/// <param name="item"></param>
		public void Remove(T item) => this.BUFFER.Remove(item);

		public void RemoveAll(T item)
        {
            // this.BUFFER.RemoveAll(n=>n[item]==item);
        }
    }


    /// <summary>
    /// 데이터 저장소 - 데이터통신용 Byte버퍼
    /// </summary>
    public class PacketBuffer : Buffer<byte>
    {
		/// <summary>
		/// 버퍼의 내용을 바이트 배열로 추출한다.
		/// </summary>
		/// <returns></returns>
		public byte[] GetBytes() => this.BUFFER.ToArray();
	}
}
