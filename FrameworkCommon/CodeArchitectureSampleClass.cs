using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Framework.Common.CsharpCodingConventions
{

    /* 이 문서는 향후 모든 c# 프로젝트에서 사용할 코드의 기본 규칙 예제입니다.
     * 
     * c#으로 개발할 때는 이 예제 코드를 참조하여, 코드 작성 규칙을 지켜 주시기 바랍니다.
     * 
     * 1. 모든 클래스와, 이벤트, 속성, 필드, 메서드는 XML 주석을 작성해야 합니다.
     * 2. 속성, 메서드 내부에서 선언되는 변수는 // 형태의 주석을 사용하여 변수를 설명해야 합니다.
     * 
     * XML 문서 주석에 대해서는 다음 링크를 참고하세요.
     * 
     * https://learn.microsoft.com/ko-kr/dotnet/csharp/language-reference/xmldoc/
     * 
     * 이 링크는 microsoft의 정책에 따라 언제든 변경될 수 있습니다.
     */

    /* 기준문서 : 방위사업청 무기체계 소프트웨어 개발 및 관리 매뉴얼 원안 ()
     * [부록 6] 무기체계 소프트웨어 코딩규칙 중 C# 부문 적용
     * 
     * https://learn.microsoft.com/ko-kr/dotnet/csharp/fundamentals/coding-style/identifier-names
     * https://learn.microsoft.com/ko-kr/dotnet/csharp/fundamentals/coding-style/coding-conventions
     */

    // 샘플 코드에 대한 경고 메시지 감춤
#pragma warning disable CS0067, CS0414
    // [CLSCompliant(false)]
    /// <summary>
    /// 모든 클래스는 그 목적을 작성해야 합니다.
    /// 여러줄로 작성할 때 줄바꿈 태그 : <br/>
    /// 
    /// 줄바꿈 시에는
    /// </summary>
    class CodeArchitectureSampleClass
    {
        #region 1. 이벤트 / 델리게이트

        /// <summary>
        /// 노드의 높이가 변경될 때 발생되는 이벤트
        /// </summary>
        public event EventHandler<EventArgs> XxxxxxxChanged;

        #endregion

        #region 2. 속성(Property) Getter, Setter


        #endregion

        #region 3. 필드(Field) - 멤버 전역 변수

        #region 3.1 단독형 멤버 변수 선언

        /// <summary>
        /// 이 노드가 RootNode인지 여부
        /// </summary>
        private readonly bool RootNode = false;

        #endregion

        #region 3.2 배열 또는 개채목록형 멤버 변수 선언

        /// <summary>
        /// 하위 노드 목록
        /// </summary>
        private readonly List<string> Nodes = new List<string>();

        #endregion

        #endregion

        #region 4. 메서드(Method)

        #region 1. 생성자 / 소멸자 (Constructor/Deconstructor)

        public CodeArchitectureSampleClass()
        {
        }

        public CodeArchitectureSampleClass(int a) : this()
        {
        }

        public CodeArchitectureSampleClass(int a, int b) : this(a)
        {
        }

        #endregion

        #region 2. 외부 공개 메소드

        #region 2.1 상속 메소드 구현
        #endregion

        #region 2.2 인터페이스 메소드 구현
        #endregion

        #region 2.3 신규 메소드 정의

        public void NewMethodName1_1(/* Parameter */)
        {

        }

        public virtual void NewMethodName1_2(/* Parameter */)
        {

        }

        protected void NewMethodName2_1(/* Parameter */)
        {

        }

        protected virtual void NewMethodName2_2(/* Parameter */)
        {

        }

        #endregion

        #endregion

        #region 3. 내부 이벤트 정의 메소드

        private void PrivateMemberName_EventType(object sender, EventArgs eventAgrs)
        {
            // 본문 선언
        }

        #endregion

        #region 4. 내부 비공개 메소드

        private void PrivateMethodName( /* 파라미터 */)
        {
            // 본문 선언
        }

        #endregion

        #endregion
    }
}
