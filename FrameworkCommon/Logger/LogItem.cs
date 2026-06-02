using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Framework.Common.Enum;

namespace Framework.Common.Logger
{
	public class LogItem
	{
		public LogType Type { get; protected set; }
		public DateTime DateTime { get; protected set; }
		public string Message { get; protected set; }
		public bool Remote { get; protected set; } = false;
		public string FullMessage => $"[{this.Type}] {this.DateTime.ToString("HH:mm:ss.fff")} {this.Message}";
		public int NextPos { get; private set; } = 0;
		public bool Error { get; private set; } = false;

		public LogItem() { }

		public LogItem(byte[] b, int pos = 0)
		{
			var parsing = false;

			try
			{
				do
				{
					if (b.Length > 12)
					{
						var length = 0;

						// 헤더 검사
						if (b[pos+0] != 0xAA)
						{
							break;
						}

						// Date : 8Byte
						if (b[pos + 1] == 0xAA)
						{
							// 타입
							this.Type = (LogType)b[pos + 2];
							// 길이 검사
							length = BitConverter.ToUInt16(b, pos + 3);

							// 날짜 파싱
							var ticks = BitConverter.ToInt64(b, pos + 5);
							this.DateTime = new DateTime(ticks);

							// 메시지 파싱
							var strLen = length - 13;
							this.Message = ExtractStringFromByteArray(b, pos+13, strLen);

							// 파싱성공
							parsing = true;

						}
						// Date : 5Byte(Time)
						else if (b[pos + 1] == 0xAB)
						{
							// 타입
							this.Type = (LogType)b[pos + 2];
							// 길이 검사
							length = BitConverter.ToUInt16(b, pos + 3);

							// 날짜 파싱
							this.DateTime = DateTime.Today.AddHours((double)b[pos + 5]).AddMinutes(b[pos + 6]).AddSeconds(b[pos + 7]).AddMilliseconds(b[pos + 8] + (b[pos + 9] * 256));

							// 메시지 파싱
							var strLen = length - 10;
							this.Message = ExtractStringFromByteArray(b, pos+10, strLen);

							// 파싱성공
							parsing = true;
						}

						this.NextPos = parsing? pos + length : b.Length;
					}
				} while (false);
			}
			catch(Exception ex)
			{
				Console.WriteLine(ex.ToString());
				this.Error = true;
			}
			
			if(parsing == false)
			{
				this.Message = "";
				this.Type = LogType.LogError;
				this.DateTime = new DateTime(0);
			}
		}

		public LogItem(string msg, bool remote = false)
		{
			this.Type = LogType.Info;
			this.DateTime = DateTime.Now;
			this.Message = msg;
			this.Remote = remote;
		}

		public LogItem(LogType type, string msg, bool remote = false)
		{
			this.Type = type;
			this.DateTime = DateTime.Now;
			this.Message = msg;
			this.Remote = remote;
		}

		public LogItem(LogType type, DateTime t, string msg, bool remote = false)
		{
			this.Type = type;
			this.DateTime = t;
			this.Message = msg;
			this.Remote = remote;
		}

		public byte[] GetBytes()
		{
			var bs = new List<byte>();

			var header = new byte[] { 0xAA, 0xAA };
			var type = (byte)this.Type;
			var ticks = BitConverter.GetBytes(this.DateTime.Ticks);
			var msgs = Encoding.UTF8.GetBytes(this.Message);
			var length = (ushort)(2 + 1 + ticks.Length + msgs.Length + 2);

			bs.AddRange(header);
			bs.Add(type);
			bs.AddRange(BitConverter.GetBytes(length));
			bs.AddRange(ticks);
			bs.AddRange(msgs);

			return bs.Count == length ? bs.ToArray() : Array.Empty<byte>();
		}

		private static string ExtractStringFromByteArray(byte[] byteArray, int startIndex, int length)
		{
			// 지정된 범위의 byte 배열 추출
			var subArray = new byte[length];
			Array.Copy(byteArray, startIndex, subArray, 0, length);

			// UTF-8 인코딩으로 문자열 변환
			return Encoding.UTF8.GetString(subArray);
		}
	}
}
