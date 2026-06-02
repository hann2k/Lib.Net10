using System;
using System.Text;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Framework.Common.Buffer;

namespace Framework.Test.Common.Buffer
{
    /// <summary>
    /// UnitTest_Buffer의 요약 설명
    /// </summary>
    [TestClass]
    public class UnitTest_Buffer
    {

        private readonly Buffer<bool> BoolLst = new Buffer<bool>();

        public UnitTest_Buffer()
        {
            //
            // TODO: 여기에 생성자 논리를 추가합니다.
            //
        }

        /// <summary>
        ///현재 테스트 실행에 대한 정보 및 기능을
        ///제공하는 테스트 컨텍스트를 가져오거나 설정합니다.
        ///</summary>
        public TestContext TestContext { get; set; }

        #region 추가 테스트 특성
        //
        // 테스트를 작성할 때 다음 추가 특성을 사용할 수 있습니다.
        //
        // ClassInitialize를 사용하여 클래스의 첫 번째 테스트를 실행하기 전에 코드를 실행합니다.
        // [ClassInitialize()]
        // public static void MyClassInitialize(TestContext testContext) { }
        //
        // ClassCleanup을 사용하여 클래스의 테스트를 모두 실행한 후에 코드를 실행합니다.
        // [ClassCleanup()]
        // public static void MyClassCleanup() { }
        //
        // TestInitialize를 사용하여 각 테스트를 실행하기 전에 코드를 실행합니다.
        // [TestInitialize()]
        // public void MyTestInitialize() { }
        //
        // TestCleanup을 사용하여 각 테스트를 실행한 후에 코드를 실행합니다.
        // [TestCleanup()]
        // public void MyTestCleanup() { }
        //
        #endregion

        [TestInitialize()]
        public void MyTestInitialize()
        {
            this.BoolLst.Clear();
        }

        [TestMethod]
        public void Test_삭제_삭제시험_01()
        {
            this.BoolLst.Add(true);
            this.BoolLst.Add(true);
            this.BoolLst.Add(true);
            this.BoolLst.Add(true);
            this.BoolLst.Add(true);

            this.BoolLst.Clear();

            Assert.AreEqual(0, this.BoolLst.Length);
        }

        [TestMethod]
        public void Test_삭제_삭제시험_02()
        {
            this.BoolLst.Add(true);
            this.BoolLst.Add(true);
            this.BoolLst.Add(true);
            this.BoolLst.Add(true);
            this.BoolLst.Add(true);

            this.BoolLst.RemoveAt(0);

            Assert.AreEqual(4, this.BoolLst.Length);
        }

        [TestMethod]
        public void Test_삭제_삭제시험_03()
        {
            this.BoolLst.Add(true);
            this.BoolLst.Add(true);
            this.BoolLst.Add(true);
            this.BoolLst.Add(true);
            this.BoolLst.Add(true);

            // this.BoolLst.RemoveAt(5);

            Assert.ThrowsException<ArgumentOutOfRangeException>(() => this.BoolLst.RemoveAt(5));

            // Assert.AreEqual(4, this.BoolLst.Length);
        }

        [TestMethod]
        public void Test_삭제_삭제시험_04()
        {
            this.BoolLst.Add(true);
            this.BoolLst.Add(true);
            this.BoolLst.Add(true);
            this.BoolLst.Add(true);
            this.BoolLst.Add(true);

            this.BoolLst.Remove(false);

            Assert.AreEqual(5, this.BoolLst.Length);

            this.BoolLst.Remove(true);

            Assert.AreEqual(4, this.BoolLst.Length);
        }


        [TestMethod]
        public void Test_삽입_수량시험_01()
        {
            Assert.AreEqual(0, this.BoolLst.Length);

            this.BoolLst.Add(true);
            this.BoolLst.Add(true);
            this.BoolLst.Add(true);
            this.BoolLst.Add(true);
            this.BoolLst.Add(true);

            Assert.AreEqual(5, this.BoolLst.Length);

            this.BoolLst.AddRange(new bool[] { true, false, true });

            Assert.AreEqual(8, this.BoolLst.Length);

            List<bool> temp = new List<bool> {
                true,
                true,
                true
            };

            this.BoolLst.AddRange(temp);

            Assert.AreEqual(11, this.BoolLst.Length);
        }

        [TestMethod]
        public void Test_삽입_수량시험_02()
        {
            Assert.AreEqual(0, this.BoolLst.Length);

            this.BoolLst.Add(true);
            this.BoolLst.Add(true);
            this.BoolLst.Add(true);
            this.BoolLst.Add(true);
            this.BoolLst.Add(true);

            Assert.AreEqual(5, this.BoolLst.Length);

            this.BoolLst.AddRange(new bool[] { true, false, true });

            Assert.AreEqual(8, this.BoolLst.Length);

            List<bool> temp = new List<bool> {
                true,
                true,
                true
            };

            this.BoolLst.AddRange(temp);

            Assert.AreEqual(11, this.BoolLst.Length);

            this.BoolLst.AddRange(this.BoolLst);

            Assert.AreEqual(22, this.BoolLst.Length);
        }

        [TestMethod]
        public void Test_읽기수정_인덱스접근시험_01()
        {
            void PutOutofRange()
            {
                this.BoolLst[0] = true;
            }
            Assert.ThrowsException<ArgumentOutOfRangeException>(() => PutOutofRange());

            bool GetOutofRange(int i)
            {
                return this.BoolLst[i];
            }
            Assert.ThrowsException<ArgumentOutOfRangeException>(() => GetOutofRange(1));
        }

        [TestMethod]
        public void Test_읽기수정_인덱스접근시험_02()
        {
            this.BoolLst.Add(true);
            this.BoolLst.Add(true);
            this.BoolLst.Add(true);
            this.BoolLst.Add(true);
            this.BoolLst.Add(true);

            Assert.AreEqual(5, this.BoolLst.Length);
            Assert.AreEqual(true, this.BoolLst[0]);

            this.BoolLst[0] = false;

            Assert.AreEqual(false, this.BoolLst[0]);

        }
    }
}
