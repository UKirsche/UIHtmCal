using HTM.HTMBitmapProcessor;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
namespace TestBitMapProcessor
{
    
    
    /// <summary>
    ///Dies ist eine Testklasse für "DBLogWrapperTest" und soll
    ///alle DBLogWrapperTest Komponententests enthalten.
    ///</summary>
    [TestClass()]
    public class DBLogWrapperTest
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
        ///Ein Test für "InitializeWrapper"
        ///</summary>
        [TestMethod()]
        public DBLogWrapper InitializeWrapperTest()
        {
            DBLogWrapper target = new DBLogWrapper(); 
            target.InitializeWrapper();
            Assert.IsNotNull(target.LogProvider);
            Assert.IsNotNull(target.DBFactory);
            Assert.IsNotNull(target.LogConnection);

            return target;
        }

        /// <summary>
        ///Ein Test für "GetAppEventFields"
        ///</summary>
        [TestMethod()]
        public void GetAppEventFieldsTest()
        {
            DBLogWrapper target= InitializeWrapperTest();

            string[] expected = new string[5]; // TODO: Passenden Wert initialisieren
            string[] actual;
            actual = target.GetAppEventFields();
            Assert.AreEqual(expected.Length, actual.Length);
            Assert.IsTrue(actual[0].Length>0);

            target.UnInitializeWrapper();
        }



        /// <summary>
        ///Ein Test für "GetAppVocabulary"
        ///</summary>
        [TestMethod()]
        public void GetAppVocabularyTest()
        {
            DBLogWrapper target = InitializeWrapperTest();
            List<string> actual;
            actual = target.GetAppVocabulary();
            Assert.IsTrue(actual.Count>0);
        }
    }
}
