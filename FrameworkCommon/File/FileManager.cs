using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Framework.Common.Files
{
    public enum FileType
    {
        Text,
        Binary
    }

    /// <summary>
    /// 지정한 파일을 반드시 존재하도록 디렉토리와 파일을 만들어준다.
    /// 천천히 작업한다.
    /// </summary>
    public class ForcesFileExist
    {
        // private string File;

        public ForcesFileExist(string filename, FileType type)
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// 텍스트 파일 접근 클래스
    /// </summary>
    public class TextFileHandler
    {
        //private string FilePath;
        //private string FileName;
        //private string FileExt;

        //private StreamWriter Sw;
    }
}
