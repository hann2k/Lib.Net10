using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Framework.Common.Logger;

using System.Runtime.InteropServices;
using Framework.Common.DTO;
using Framework.Common.Converter;

// using Framework.Common.Singleton;

namespace Framework.Common.Config
{
    public abstract class INI_Manager
    {
        // INI 쓰기
        [DllImport("kernel32")]
        private static extern long WritePrivateProfileString(string IpAppName, string key, string val, string filePath);

        // INI 읽기 ( StringBuilder )
        [DllImport("kernel32")]
        private static extern int GetPrivateProfileString(string IpAppName, string key, string def, StringBuilder retVal, int size, string filePath);

        public IniError Error { get; private set; } = new IniError();

        // INI 위치
        /// <summary>
        /// INI 기본위치. 현재폴더의 setting.ini 파일
        /// </summary>
        protected internal string INI_FILE = Environment.CurrentDirectory + @"\setting.ini";

        protected internal string AUTO_FILE = string.Empty;

		public string IniFile => this.INI_FILE;

		/// <summary>
		/// Ini 위치 수동 지정
		/// </summary>
		/// <param name="file"></param>
		protected void SetIniFile(string file) => this.INI_FILE = file;

		public void SetAutoFile(string file)
        {
            if (System.IO.File.Exists(file))
            {
                this.AUTO_FILE = file;
                this.SetIniFile(file);
            }
            else
            {
                this.Error.Fatal = true;
                this.Error.SetReason( $"설정파일 '{file}' 이 없습니다.");
            }
        }

		public void SetAutoFile() => this.SetIniFile(this.AUTO_FILE);

		protected internal string GetIniValue(string Section, string Key)
        {
            var temp = new StringBuilder(255);
			var i = GetPrivateProfileString(Section, Key, "", temp, 255, this.INI_FILE);

			// Console.WriteLine($"이건 뭐지 : {i} {temp.ToString()}[{temp.Length}]");
			
            if (temp.Length == i)
            {
                return temp.ToString();
            }
            else
            {
                Log.Ins.Error($"{this.INI_FILE} {Section}.{Key} 길이 불일치 [{i}] [{temp.Length}]");
                return string.Empty;
            }
        }

		protected internal void SetIniValue(string Section, string Key, string value) => WritePrivateProfileString(Section, Key, value, this.INI_FILE);

		
	}

    /// <summary>
    /// Ini 파일 관리자. 작성할 때 파일이 없으면 새로 생성한다.
    /// </summary>
    public abstract class IniConfig : INI_UTF8
    {
        protected string Section;
        protected string Key;

        public bool WriteableOnly = false;

        public IniConfig()
        {

        }

        #region Ini 읽기

        private void WriteException(Exception e, string Section, string Key, string Value)
        {
            Log.Ins.Exception(e);
            Log.Ins.Exception($"{base.INI_FILE} [{Section}] {Key}={Value} ");

            base.Error.Light = true;
            this.Error.SetReason($"'{base.INI_FILE}' 파일의 [{Section}]{Key}={Value} 항목을 읽는 중에 오류가 발생하였습니다. 로그파일을 참조하세요.");
        }

        public bool GetBool(string Section, string Key, bool Default = false)
        {
			var Value = this.GetIniValue(Section, Key);
			return StringToNumbers.ToBool(Value, Default);

            //try
            //{
            //    this.Section = Section;
            //    this.Key = Key;
                
            //    return Convert.ToBoolean(Value);
            //}
            //catch (Exception e)
            //{
            //    this.WriteException(e, Section, Key, Value);

            //    return Default;
            //}
        }

        public byte GetByte(string Section, string Key, byte Default = 0)
        {
			var Value = this.GetIniValue(Section, Key);
			return StringToNumbers.ToByte(Value, Default);

			//try
   //         {
   //             this.Section = Section;
   //             this.Key = Key;

   //             return Convert.ToByte(Value);
   //         }
   //         catch (Exception e)
   //         {
   //             this.WriteException(e, Section, Key, Value);
   //             return Default;
   //         }
        }

        public short GetInt16(string Section, string Key, short Default = 0)
        {
			var Value = this.GetIniValue(Section, Key);
			return StringToNumbers.ToInt16(Value, Default);

            //try
            //{
            //    this.Section = Section;
            //    this.Key = Key;

            //    return Convert.ToInt16(Value);
            //}
            //catch (Exception e)
            //{
            //    this.WriteException(e, Section, Key, Value);

            //    return Default;
            //}
        }

        public ushort GetUInt16(string Section, string Key, ushort Default = 0)
        {
			var Value = this.GetIniValue(Section, Key);
			return StringToNumbers.ToUInt16(Value, Default);

			//try
   //         {
   //             this.Section = Section;
   //             this.Key = Key;

   //             return Convert.ToUInt16(Value);
   //         }
   //         catch (Exception e)
   //         {
   //             this.WriteException(e, Section, Key, Value);

   //             return Default;
   //         }
        }

        public int GetInt32(string Section, string Key, int Default = 0)
        {
			var Value = this.GetIniValue(Section, Key);
			return StringToNumbers.ToInt32(Value, Default);

			//try
   //         {
   //             this.Section = Section;
   //             this.Key = Key;


   //             return Convert.ToInt32(Value);
   //         }
   //         catch (Exception e)
   //         {
   //             this.WriteException(e, Section, Key, Value);

   //             return Default;
   //         }
        }

        public uint GetUInt32(string Section, string Key, uint Default = 0)
        {
			var Value = this.GetIniValue(Section, Key);

            try
            {
                this.Section = Section;
                this.Key = Key;


                return Convert.ToUInt32(Value);
            }
            catch (Exception e)
            {
                this.WriteException(e, Section, Key, Value);

                return Default;
            }
        }

		public float GetFloat(string Section, string Key, float Default = 0) => (float)this.GetDouble(Section, Key, Default);

		public double GetDouble(string Section, string Key, double Default = 0)
        {
			var Value = this.GetIniValue(Section, Key);
			return StringToNumbers.ToDouble(Value, Default);
			//try
   //         {
   //             this.Section = Section;
   //             this.Key = Key;

			//	StringToNumbers.ToDouble(Value, Default);
   //             return Convert.ToDouble(Value);
   //         }
   //         catch (Exception e)
   //         {
   //             this.WriteException(e, Section, Key, Value);

   //             return Default;
   //         }
        }

        public decimal GetDecimal(string Section, string Key, decimal Default = 0)
        {
			var Value = this.GetIniValue(Section, Key);

            try
            {
                this.Section = Section;
                this.Key = Key;


                return Convert.ToDecimal(Value);
            }
            catch (Exception e)
            {
                this.WriteException(e, Section, Key, Value);

                return Default;
            }
        }

        public string GetString(string Section, string Key, string Default = "")
        {
			var Value = this.GetIniValue(Section, Key);

            try
            {
                this.Section = Section;
                this.Key = Key;

                return Value;
            }
            catch (Exception e)
            {
                this.WriteException(e, Section, Key, Value);

                return Default;
            }
        }

        public Limit_Bool GetLimit_Bool(string Section, string Key)
        {
            this.Section = Section;
            this.Key = Key;

			var item = new Limit_Bool();
            item.Set(this.GetBool(Section, Key));

            return item;
        }

        public Limit_Byte GetLimit_Byte(string Section, string Key)
        {
            this.Section = Section;
            this.Key = Key;

			var item = new Limit_Byte();
            item.Set(this.GetString(Section, Key));

            return item;
        }

		#endregion

		#region Ini 쓰기

		public void SetBool(string Section, string Key, bool value) => this.SetIniValue(Section, Key, value.ToString());

		public void SetByte(string Section, string Key, byte value) => this.SetIniValue(Section, Key, value.ToString());

		public void SetInt16(string Section, string Key, short value) => this.SetIniValue(Section, Key, value.ToString());

		public void SetUInt16(string Section, string Key, ushort value) => this.SetIniValue(Section, Key, value.ToString());

		public void SetInt32(string Section, string Key, int value) => this.SetIniValue(Section, Key, value.ToString());

		public void SetUInt32(string Section, string Key, uint value) => this.SetIniValue(Section, Key, value.ToString());

		public void SetFloat(string Section, string Key, float value) => this.SetIniValue(Section, Key, value.ToString());

		public void SetDouble(string Section, string Key, double value) => this.SetIniValue(Section, Key, value.ToString());

		public void SetDecimal(string Section, string Key, decimal value) => this.SetIniValue(Section, Key, value.ToString());

		public void SetString(string Section, string Key, string value) => this.SetIniValue(Section, Key, value);

		#endregion

		#region Ini File Read / Write

		/// <summary>
		/// DTo를 지정된 Ini에 저장한다.
		/// </summary>
		/// <param name="dto"></param>
		public virtual void WriteFile(Dto dto) => base.WriteFile();

		/// <summary>
		/// 지정한 Ini 파일에, DTO를 저장한다.
		/// </summary>
		/// <param name="dto"></param>
		/// <param name="iniFile"></param>
		public void WriteFile(Dto dto, string iniFile)
        {
			var tempIni = this.INI_FILE;
            this.INI_FILE = iniFile;

            this.WriteFile(dto);

            this.INI_FILE = tempIni;
        }

        /// <summary>
        /// 지정된 INI파일을 읽는다.
        /// </summary>
        /// <returns></returns>
        public abstract Dto ReadFile();

        /// <summary>
        /// INI파일을 지정하여 별도로 읽는다.
        /// </summary>
        /// <param name="iniFile"></param>
        /// <returns></returns>
        public Dto ReadFile(string iniFile)
        {
			var ini = this.INI_FILE;

            this.INI_FILE = iniFile;
            Dto d = this.ReadFile();

            this.INI_FILE = ini;

            return d;
        }

        #endregion
    }

    /// <summary>
    /// INI 사용시 오류를 판정하기 위한 클래스
    /// 오류의 종류를 정의한다.
    /// 오류가 발생한 이유를 기술한다.
    /// </summary>
    public class IniError : Framework.Common.Error.Error
    {
        public bool Fatal = false;
        public bool Light = false;
        public bool Warning = false;
    }
}
