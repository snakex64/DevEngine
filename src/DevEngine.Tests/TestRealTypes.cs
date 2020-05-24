using DevEngine.Class;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;

namespace DevEngine.Tests
{
    [TestClass]
    public class TestRealTypes
    {
        private class TestType1
        {
            public string Property1 { get; private set; }

            public int Method1(int parameter1, string parameter2) => throw new NotImplementedException();
        }
        private class TestType2: TestType1
        {
            public string Property2 { get; private set; }

            public int Method2(int parameter1, TestType1 parameter2) => throw new NotImplementedException();
        }

        [TestMethod]
        public void ImportBasicType()
        {
            var project = TestBasicDeclarationsAndTypes.GetBasicDevProject();

            Assert.IsNotNull(project.GetRealType<TestType2>());
        }

        [TestMethod]
        public void TestAssignTo()
        {
            var project = TestBasicDeclarationsAndTypes.GetBasicDevProject();

            var testType1 = project.GetRealType<TestType1>();
            var testType2 = project.GetRealType<TestType2>();

            Assert.IsFalse(testType1.CanBeAssignedTo(testType2));
            Assert.IsTrue(testType2.CanBeAssignedTo(testType1));
        }

        [TestMethod]
        public void TestAssignTo_InheritFromRealType()
        {
            var project = TestBasicDeclarationsAndTypes.GetBasicDevProject();


            var testType1 = project.GetRealType<TestType1>();
            var testType2 = project.GetRealType<TestType2>();
            var testType3 = new DevClass(testType1, "Tests.TestType3");
            project.Classes.Add(testType3);

            Assert.IsTrue(testType3.CanBeAssignedTo(testType1));
            Assert.IsFalse(testType1.CanBeAssignedTo(testType3));
            Assert.IsFalse(testType3.CanBeAssignedTo(testType2));
        }
    }
}
