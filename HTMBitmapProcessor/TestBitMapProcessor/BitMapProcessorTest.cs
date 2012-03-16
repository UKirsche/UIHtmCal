using HTM.HTMBitmapProcessor;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace TestBitMapProcessor
{
    
    
    /// <summary>
    ///Dies ist eine Testklasse für "BitMapProcessorTest" und soll
    ///alle BitMapProcessorTest Komponententests enthalten.
    ///</summary>
    [TestClass()]
    public class BitMapProcessorTest
    {


        private TestContext testContextInstance;

        /// <summary>
        ///Ruft den Testkontext auf, der Informationen
        ///über und Funktionalität für den aktuellen Testlauf bietet, oder legt diesen fest.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        #region Zusätzliche Testattribute
        // 
        //Sie können beim Verfassen Ihrer Tests die folgenden zusätzlichen Attribute verwenden:
        //
        //Mit ClassInitialize führen Sie Code aus, bevor Sie den ersten Test in der Klasse ausführen.
        //[ClassInitialize()]
        //public static void MyClassInitialize(TestContext testContext)
        //{
        //}
        //
        //Mit ClassCleanup führen Sie Code aus, nachdem alle Tests in einer Klasse ausgeführt wurden.
        //[ClassCleanup()]
        //public static void MyClassCleanup()
        //{
        //}
        //
        //Mit TestInitialize können Sie vor jedem einzelnen Test Code ausführen.
        //[TestInitialize()]
        //public void MyTestInitialize()
        //{
        //}
        //
        //Mit TestCleanup können Sie nach jedem einzelnen Test Code ausführen.
        //[TestCleanup()]
        //public void MyTestCleanup()
        //{
        //}
        //
        #endregion


        /// <summary>
        ///Ein erster Test für "Step".
        ///</summary>
        [TestMethod()]
        public void StepTest()
        {
            BitMapProcessor target = new BitMapProcessor();
            target.Initialize();
            target.Step();
            int[,] actual = target.GetOutPut();
            Assert.IsNotNull(actual);
            Assert.IsTrue(actual.GetLength(0)==10);
            Assert.IsTrue(actual.GetLength(1) == 15);


        }
    }
}
