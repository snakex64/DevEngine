using DevEngine.Class;
using DevEngine.Core.Class;
using DevEngine.Method;
using DevEngine.Project;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DevEngine.Tests
{
    [TestClass]
    public class TestBasicDeclarationsAndTypes
    {
        private DevProject GetBasicDevProject()
        {

            var project = new DevProject();

            var class1 = new DevClass(null, new Core.Class.DevClassName("DevEngine.Tests.Class1"));
            var class2 = new DevClass(null, new Core.Class.DevClassName("DevEngine.Tests.Class2"));
            project.Classes.Add(class1);
            project.Classes.Add(class2);

            var method1 = new DevMethod(class1, "method1", false);
            class1.Methods.Add(method1);

            method1.Parameters.Add(new DevMethodParameter(class2, "p1", false, false));
            method1.Parameters.Add(new DevMethodParameter(class2, "p2", false, false));

            return project;
        }

        [TestMethod]
        public void TestGetMethod()
        {
            var project = GetBasicDevProject();

            var class1 = project.Classes["DevEngine.Tests.Class1"];

            var method1 = class1.Methods.GetMethod("method1");

            Assert.IsNotNull(method1);
            Assert.AreEqual("method1", method1.Name);
            Assert.AreEqual("DevEngine.Tests.Class1", method1.DeclaringClass.Name.FullNameWithNamespace);
        }


        [TestMethod]
        public void TestClassName()
        {
            var className = new DevClassName("namespace1", "name1");

            Assert.AreEqual("namespace1", className.Namespace);
            Assert.AreEqual("name1", className.Name);
            Assert.AreEqual("namespace1.name1", className.FullNameWithNamespace);
            Assert.IsTrue(className.StartsWithNamespace("namespace1"));
        }
    }
}
