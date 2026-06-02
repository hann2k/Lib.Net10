using Framework.Common.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Framework.Common.Converter
{
	/// <summary>
	/// 비트관리기<br/>
	/// <br/>
	/// 최대 64비트까지의 비트관리를 지원한다.<br/>
	/// 비트마스크 연산으로 값을 채우고, 그것을 원하는 타입으로 추출할 수 있다.<br/>
	/// 비트마스크 연산으로 값을 채우고, 그것을 바이트 배열로 추출할 수 있다.<br/>
	/// 지정된 비트자리에 채워진 값을, 비트개수로 추출할 수 있다.<br/>
	/// <br/>
	/// 사용전 반드시 Clear()후 사용해야 한다.
	/// </summary>
	public class BitManager
    {
        #region BitMask

        private static readonly ulong[] BIT_MASK = {
            0x0000000000000001, 0x0000000000000003, 0x0000000000000007, 0x000000000000000F,
            0x000000000000001F, 0x000000000000003F, 0x000000000000007F, 0x00000000000000FF,
            0x00000000000001FF, 0x00000000000003FF, 0x00000000000007FF, 0x0000000000000FFF,
            0x0000000000001FFF, 0x0000000000003FFF, 0x0000000000007FFF, 0x000000000000FFFF,
            0x000000000001FFFF, 0x000000000003FFFF, 0x000000000007FFFF, 0x00000000000FFFFF,
            0x00000000001FFFFF, 0x00000000003FFFFF, 0x00000000007FFFFF, 0x0000000000FFFFFF,
            0x0000000001FFFFFF, 0x0000000003FFFFFF, 0x0000000007FFFFFF, 0x000000000FFFFFFF,
            0x000000001FFFFFFF, 0x000000003FFFFFFF, 0x000000007FFFFFFF, 0x00000000FFFFFFFF,
            0x00000001FFFFFFFF, 0x00000003FFFFFFFF, 0x00000007FFFFFFFF, 0x0000000FFFFFFFFF,
            0x0000001FFFFFFFFF, 0x0000003FFFFFFFFF, 0x0000007FFFFFFFFF, 0x000000FFFFFFFFFF,
            0x000001FFFFFFFFFF, 0x000003FFFFFFFFFF, 0x000007FFFFFFFFFF, 0x00000FFFFFFFFFFF,
            0x00001FFFFFFFFFFF, 0x00003FFFFFFFFFFF, 0x00007FFFFFFFFFFF, 0x0000FFFFFFFFFFFF,
            0x0001FFFFFFFFFFFF, 0x0003FFFFFFFFFFFF, 0x0007FFFFFFFFFFFF, 0x000FFFFFFFFFFFFF,
            0x001FFFFFFFFFFFFF, 0x003FFFFFFFFFFFFF, 0x007FFFFFFFFFFFFF, 0x00FFFFFFFFFFFFFF,
            0x01FFFFFFFFFFFFFF, 0x03FFFFFFFFFFFFFF, 0x07FFFFFFFFFFFFFF, 0x0FFFFFFFFFFFFFFF,
            0x1FFFFFFFFFFFFFFF, 0x3FFFFFFFFFFFFFFF, 0x7FFFFFFFFFFFFFFF, 0xFFFFFFFFFFFFFFFF,
        };

        #endregion

        private ulong Storage = 0;

        #region Getter

        public byte GetByte => (byte)this.Storage;

        public sbyte GetInt8 => (sbyte)this.Storage;
        public byte GetUInt8 => this.GetByte;

        public short GetInt16 => (short)this.Storage;
        public ushort GetUInt16 => (ushort)this.Storage;

        public int GetInt32 => (int)this.Storage;
        public uint GetUInt32 => (uint)this.Storage;

        public long GetInt64 => (long)this.Storage;
        public ulong GetUInt64 => this.Storage;

        public byte[] GetBytes(int byteCount)
        {
            var result = Array.Empty<byte>();

            switch (byteCount)
            {
                case 2:
                    result = BitConverter.GetBytes((ushort)this.Storage);
                    break;
                case 4:
                    result = BitConverter.GetBytes((uint)this.Storage);
                    break;
                case 8:
                    result = BitConverter.GetBytes((ulong)this.Storage);
                    break;

                default:
                    throw new ArgumentOutOfRangeException("2, 4, 8 만 사용가능합니다.");
            }

            return result;
        }

		#endregion

		#region Constructor

		public BitManager() => this.Storage = 0;
		public BitManager(byte b) : this((ulong)b) { }
        public BitManager(sbyte b) : this((ulong)b) { }
        public BitManager(ushort b) : this((ulong)b) { }
        public BitManager(short b) : this((ulong)b) { }
        public BitManager(uint b): this((ulong)b) { }
        public BitManager(int b) : this((ulong)b) { }
		public BitManager(ulong b) => this.Storage = b;
		public BitManager(long b) => this.Storage = (ulong)b;

		#endregion

		#region Setter

		private void MakeException(int maxBitCount, int Location, int BitCount)
        {
            var ableLocation = maxBitCount - BitCount;

            if (BitCount < 1 || BitCount > maxBitCount)
            {
                throw new ArgumentOutOfRangeException($"BitCount({BitCount}) 의 허용범위는 1 ~ {maxBitCount} 입니다.");
            }
        }

        public BitManager SetBitMask(int bitCount)
        {
            this.Storage = this.Storage & BIT_MASK[bitCount - 1];
            return this;
        }

		public BitManager PutBool(Limit_Bool b, int Location) => this.PutBool(b.Checked, Location);

		public BitManager PutBool(bool b, int Location)
        {
			var source = (byte)(b ? 1 : 0);
            return this.PutStorage(source, Location, 1);
        }

		public BitManager PutUInt8(byte source) => this.PutUInt8(source, 7, 8);

		public BitManager PutInt8(sbyte source) => this.PutInt8(source, 7, 8);

		public BitManager PutUInt8(byte[] array, int index) => this.PutUInt8(array[index]);

		public BitManager PutInt8(byte[] array, int index) => this.PutInt8((sbyte)array[index]);

		public BitManager PutUInt8(byte source, int Location, int BitCount)
        {
            this.MakeException(8, Location, BitCount);

            return this.PutStorage(source, Location, BitCount);
        }

		public BitManager PutInt8(sbyte source, int Location, int BitCount) => this.PutUInt8((byte)source, Location, BitCount);

		public BitManager PutUInt16(ushort source) => this.PutUInt16(source, 15, 16);

		public BitManager PutInt16(short source) => this.PutInt16(source, 15, 16);

		public BitManager PutUInt16(byte low, byte high)
        {
			var bb = new byte[2] { low, high };
            return this.PutUInt16( BitConverter.ToUInt16(bb, 0) );
        }

		public BitManager PutUInt16(byte[] array, int index) => this.PutUInt16(BitConverter.ToUInt16(array, index));

		public BitManager PutInt16(byte low, byte high)
        {
			var bb = new byte[2] { low, high };
            return this.PutInt16(BitConverter.ToInt16(bb, 0));
        }

		public BitManager PutInt16(byte[] array, int index) => this.PutInt16(BitConverter.ToInt16(array, index));

		public BitManager PutUInt16(ushort source, int Location, int BitCount)
        {
            this.MakeException(16, Location, BitCount);

            return this.PutStorage(source, Location, BitCount);
        }

		public BitManager PutInt16(short source, int Location, int BitCount) => this.PutUInt16((ushort)source, Location, BitCount);

		public BitManager PutUInt32(uint source) => this.PutUInt32(source, 31, 32);

		public BitManager PutInt32(int source) => this.PutInt32(source, 31, 32);

		public BitManager PutUInt32(byte b0, byte b1, byte b2, byte b3)
        {
			var bb = new byte[] { b0, b1, b2, b3 };
            return this.PutUInt32(BitConverter.ToUInt32(bb, 0));
        }

        public BitManager PutInt32(byte b0, byte b1, byte b2, byte b3)
        {
			var bb = new byte[] { b0, b1, b2, b3 };
            return this.PutInt32(BitConverter.ToInt32(bb, 0));
        }

		public BitManager PutUInt32(byte[] array, int index) => this.PutUInt32(BitConverter.ToUInt32(array, index));

		public BitManager PutInt32(byte[] array, int index) => this.PutInt32(BitConverter.ToInt32(array, index));

		public BitManager PutUInt32(uint source, int Location, int BitCount)
        {
            this.MakeException(32, Location, BitCount);

            return this.PutStorage(source, Location, BitCount);
        }

		public BitManager PutInt32(int source, int Location, int BitCount) => this.PutUInt32((uint)source, Location, BitCount);

		public BitManager PutUInt64(ulong source) => this.PutUInt64(source, 63, 64);

		public BitManager PutInt64(long source) => this.PutInt64(source, 63, 64);

		public BitManager PutUInt64(byte[] array, int index) => this.PutUInt64(BitConverter.ToUInt64(array, index));

		public BitManager PutInt64(byte[] array, int index) => this.PutInt64(BitConverter.ToInt64(array, index));

		public BitManager PutUInt64(ulong source, int Location, int BitCount)
        {
            this.MakeException(64, Location, BitCount);

            return this.PutStorage(source, Location, BitCount);
        }

		public BitManager PutInt64(long source, int Location, int BitCount) => this.PutUInt64((ulong)source, Location, BitCount);

		private BitManager PutStorage(ulong source, int Location, int BitCount)
        {
			var shift = 0;

            if ( Location == 0 )
            {
                shift = 0;
            }
            else if ( Location > 0)
            {
                shift = Location - BitCount + 1;
            }
            else
            {
                throw new ArgumentOutOfRangeException();
            }

            // Console.WriteLine($"Location : {Location}");
            // Console.WriteLine($"BitCount : {BitCount}");
            // Console.WriteLine($"Shift : {shift}");
            
            this.Storage = this.Storage | ((source & BitManager.BIT_MASK[BitCount - 1]) << shift);

            return this;
        }

        #endregion

        #region Extractor

        public bool ExtractBool(int Location)
        {
            // byte b = (byte)this.Extract(Location, 1);

            return (byte)this.Extract(Location, 1) == 1 ? true : false;
        }

		public byte ExtractUInt8(int Location, int bitCount) => (byte)this.Extract(Location, bitCount);

		public sbyte ExtractInt8(int Location, int bitCount) => (sbyte)this.Extract(Location, bitCount);

		public ushort ExtractUInt16(int Location, int bitCount) => (ushort)this.Extract(Location, bitCount);

		public short ExtractInt16(int Location, int bitCount) => (short)this.Extract(Location, bitCount);

		public uint ExtractUInt32(int Location, int bitCount) => (uint)this.Extract(Location, bitCount);

		public int ExtractInt32(int Location, int bitCount) => (int)this.Extract(Location, bitCount);

		public ulong ExtractUInt64(int Location, int bitCount) => this.Extract(Location, bitCount);

		public long ExtractInt64(int Location, int bitCount) => (long)this.Extract(Location, bitCount);

		private ulong Extract(int Location, int bitCount)
        {
			var cp = this.Storage;
			var shift = 0;
            if ( Location == 0)
            {
                shift = 0;
            }
            else if ( Location > 0)
            {
                shift = Location - bitCount + 1;
            }
            else
            {
                throw new IndexOutOfRangeException("비트 위치는 0이하가 될 수 없습니다.");
            }


            return (cp >> shift) & BIT_MASK[bitCount -1] ;
        }

        #endregion

        public BitManager Clear()
        {
            this.Storage = 0L;
            return this;
        }
    }
}
