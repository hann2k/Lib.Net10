using Framework.Common.Logger;
using Framework.Common.Files;
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
			// 보안 점검 #5-1: 절대경로는 허용하되 상대경로의 '..' 디렉터리 탈출은 차단한다.
			this.INI_FILE = PathGuard.EnsureSafe(file);

			IEnumerable<string> t = File.ReadLines(this.INI_FILE, Encoding.UTF8);
			this.Lines = t.ToList();

			this.Sections.Clear();

			// Console.WriteLine($"----------------- Read File : {file}");

			for (var i = 0; i< this.Lines.Count; i++)
			{
				var line = this.Lines[i];

				// 보안 점검 #4: 악성/오타 설정 한 줄이 전체 로드를 중단시키지 않도록
				// 줄 단위로 방어한다(예외 발생 시 해당 줄만 건너뛰고 로깅).
				try
				{
					if (string.IsNullOrWhiteSpace(line))
					{
						// 빈 줄/공백 줄: 무시
						continue;
					}

					if (line[0] == '#')
					{
						// 주석 처리. 아무것도 하지 않음
						continue;
					}

					if (line[0] == '[' && line[line.Length - 1] == ']')
					{
						// 섹션 이름을 추출한다.
						var section = line.Substring(1, line.Length - 2);

						// 신규 Section 이름을 만나면 섹션 사전에 넣는다.
						if (!this.Sections.ContainsKey(section))
						{
							// 새 섹션을 만들고, 섹션 아이템 목록을 설정한다.
							this.Sections.Add(section, new List<Item>());
						}

						// 현재 Section 설정
						this.CurrentSection = section;
						continue;
					}

					// 설정된 Section에 Key Value 쌍을 저장
					// 보안 점검 #4: '=' 없는 줄은 items[1] 접근에서 IndexOutOfRange로 전체 로드가 중단됐다.
					// 구분자 없는 줄은 건너뛰고 로깅한다.
					var sep = line.IndexOf('=');
					if (sep < 0)
					{
						Log.Ins.Warning($"{this.INI_FILE} {i + 1}행: '=' 구분자가 없어 건너뜁니다. [{line}]");
						continue;
					}

					// 보안 점검 #4: Split('=', 2)로 값 안의 '='를 보존한다(첫 '='만 구분자로 사용).
					var items = line.Split('=', 2);
					var it = new Item(items[0], items[1], i);

					// 보안 점검 #4: 섹션 없이 등장한 key=value는 담을 곳이 없어 KeyNotFound로 크래시했다.
					if (string.IsNullOrEmpty(this.CurrentSection) || !this.Sections.ContainsKey(this.CurrentSection))
					{
						Log.Ins.Warning($"{this.INI_FILE} {i + 1}행: 섹션([Section]) 없이 등장한 항목이라 건너뜁니다. [{line}]");
						continue;
					}

					var idx = this.Sections[this.CurrentSection].FindIndex(n => n.Key == it.Key);
					if (idx < 0)
					{
						// 목록에서 못찾으면 아이템을 추가한다.
						this.Sections[this.CurrentSection].Add(it);
					}
					else
					{
						// 목록에 있으면 중복 키로 보고 첫 항목을 유지한다(기존 동작).
						Log.Ins.Warning($"{this.INI_FILE} {i + 1}행: 중복 키 '{it.Key}' 무시(첫 항목 유지). [{line}]");
					}
				}
				catch (Exception ex)
				{
					// 예기치 못한 형식 오류도 해당 줄만 건너뛴다(가용성 보장).
					Log.Ins.Warning($"{this.INI_FILE} {i + 1}행 파싱 오류로 건너뜁니다. [{line}] - {ex.Message}");
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
			// 보안 점검 #5-2: 절대경로는 허용하되 상대경로의 '..' 디렉터리 탈출은 차단한다.
			var path = PathGuard.EnsureSafe(this.INI_FILE);

			// Console.WriteLine($"AutoFile : {this.AUTO_FILE}");
			Console.WriteLine($"INI_FILE : {path}");

			StreamWriter sw = File.CreateText(path);
			foreach( var line in this.Lines)
			{
				sw.WriteLine(line);
			}
			sw.Close();

		}
	}
}
