using Framework.Common.Logger;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Framework.Common.Config
{
	public abstract class INI_UTF8
	{
		public class Item
		{
			public string Key { get; private set; }
			public string Value { get; private set; }
			public int Index { get; private set; }

			public Item(string key, string value, int index)
			{
				this.Key = key;
				this.Value = value;
				this.Index = index;
			}

			public void SetValue(string value) => this.Value = value;
		}

		public IniError Error { get; private set; } = new IniError();

		/// <summary>
		/// INI 기본위치. 현재폴더의 setting.ini 파일
		/// </summary>
		protected internal string INI_FILE = Environment.CurrentDirectory + @"\setting.ini";

		protected internal string AUTO_FILE = string.Empty;

		public string IniFile => this.INI_FILE;

		// INI파일을 읽은 원문 저장
		private List<string> Lines = null;

		// 섹션별 사전
		public readonly Dictionary<string, List<Item>> Sections = new Dictionary<string, List<Item>>();
		private string CurrentSection = string.Empty;

		/// <summary>
		/// Ini 위치 수동 지정
		/// </summary>
		/// <param name="file"></param>
		protected void SetIniFile(string file)
		{
			this.INI_FILE = file;

			IEnumerable<string> t = File.ReadLines(this.INI_FILE, Encoding.UTF8);
			this.Lines = t.ToList();

			this.Sections.Clear();

			// Console.WriteLine($"----------------- Read File : {file}");

			for (var i = 0; i< this.Lines.Count; i++)
			{
				var line = this.Lines[i];

				if(line.Length == 0)
				{
					// 빈줄 아무것도 하지 않음.
				}
				else if (line[0] == '#')
				{
					// 주석 처리. 아무것도 하지 않음
				}
				else if (line[0] == '[' && line[line.Length-1] == ']' )
				{
					// 섹션 이름을 추출한다.
					var section = line.Substring(1, line.Length - 2);

					// 신규 Section 이름을 만나면 섹션 사전에 넣는다.
					if ( !this.Sections.ContainsKey(section) )
					{
						// 새 섹션을 만들고, 섹션 아이템 목록을 설정한다.
						this.Sections.Add(section, new List<Item>());
						// Console.WriteLine($"New Section : [{section}]");
					}

					// 현재 Section 설정
					this.CurrentSection = section;
				}
				else
				{
					// 설정된 Section에 Key Value 쌍을 저장
					var items = line.Split('=');
					var it = new Item(items[0], items[1], i);

					var idx = this.Sections[this.CurrentSection].FindIndex(n => n.Key == it.Key);
					if (idx < 0 )
					{
						// 목록에서 못찾으면 아이템을 추가한다.
						this.Sections[this.CurrentSection].Add(it);
						// Console.WriteLine($"Add Item : {it.Key}={it.Value} [{it.Index}]");
					}
					else
					{
						// 목록에 있으면 충돌 예외를 발생시킨다.
						// throw new DuplicateKeyException();
						Console.WriteLine($"충돌 Item : {it.Key}={it.Value} [{it.Index}]");
					}
				}
			}
		}

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
				this.Error.SetReason($"설정파일 '{file}' 이 없습니다.");
			}
		}

		public void SetAutoFile() => this.SetIniFile(this.AUTO_FILE);

		protected internal string GetIniValue(string Section, string Key)
		{
			var i = this.Sections[Section].FindIndex(n => n.Key == Key);

			if (i > -1)
			{
				Item temp = this.Sections[Section][i];
				return temp.Value;
			}
			else
			{
				Log.Ins.Error($"{this.INI_FILE} {Section}.{Key} 이 없습니다.");
				return string.Empty;
			}
		}

		protected internal void SetIniValue(string section, string key, string value)
		{
			var i = this.Sections[section].FindIndex(n => n.Key == key);
			if (i > -1)
			{
				Item temp = this.Sections[section][i];
				temp.SetValue(value);

				this.Lines[temp.Index] = $"{temp.Key}={temp.Value}";
				// Console.WriteLine(this.Lines[temp.Index]);
			}
		}

		protected void WriteFile()
		{
			// Console.WriteLine($"AutoFile : {this.AUTO_FILE}");
			Console.WriteLine($"INI_FILE : {this.INI_FILE}");

			StreamWriter sw = File.CreateText(this.INI_FILE);
			foreach( var line in this.Lines)
			{
				sw.WriteLine(line);
			}
			sw.Close();
			
		}
	}
}
