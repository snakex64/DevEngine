using DevEngine.FakeTypes.Class;
using DevEngine.Core.Class;
using DevEngine.Graph;
using DevEngine.FakeTypes.Method;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DevEngine.Core.Graph;
using DevEngine.FakeTypes.Project;
using DevEngine.RealTypes;
using System.IO;

namespace DevEngine.Tests
{
    [TestClass]
    public class TestBasicGraph
    {
        [TestMethod]
        public void TestBasicMethodContent()
        {
            var project = TestBasicDeclarationsAndTypes.GetBasicDevProject();

            var class3 = new DevClass(project, null, new DevClassName("DevEngine.Tests.Class3"), "folder");
            project.Classes.Add(class3);

            var method3 = new DevMethod(class3, "method3", isStatic: false, returnType: project.GetRealType<int>(), Core.Visibility.Public);
            class3.Methods.Add(method3);

            method3.Parameters.Add(new DevMethodParameter(project.GetRealType<int>(), name: "p1", isOut: false, isRef: false));
            method3.Parameters.Add(new DevMethodParameter(class3, "p2", isOut: false, isRef: false)); // demonstrate passing DevClasses


            var graph = new DevGraphDefinition(project, "DevEngine.Tests.Class3.method3", class3, DevGraphDefinitionType.Method, method3.Name); // name can be whatever really... 
            graph.InitializeEmptyForMethod(method3);
            method3.GraphDefinition = graph;


            // the only thing we do in the method is return something the input p1, no in-between nodes
            // so we connect the exec from the entry point directly to the exec of the exit point
            graph.ConnectNodesParameters(graph.EntryPoint.ExecNodeParameter, graph.ExitPoints.First().ExecNodeParameter);

            // don't forget to also connect the input p1 to the return value
            graph.ConnectNodesParameters(graph.EntryPoint.Outputs.First(x => x.Name == "p1"), graph.ExitPoints.First().ReturnNodeParameter);
        }

        [TestMethod]
        public void TestSaveLoad()
        {
            string projectFolder;
            {
                var project = TestBasicDeclarationsAndTypes.GetBasicDevProject();

                var class3 = new DevClass(project, null, new DevClassName("DevEngine.Tests.Class3"), "folder");
                project.Classes.Add(class3);

                var method3 = new DevMethod(class3, "method3", isStatic: true, returnType: project.GetRealType<int>(), Core.Visibility.Public);
                class3.Methods.Add(method3);

                method3.Parameters.Add(new DevMethodParameter(project.GetRealType<int>(), name: "p1", isOut: false, isRef: false));
                method3.Parameters.Add(new DevMethodParameter(class3, "p2", isOut: false, isRef: false)); // demonstrate passing DevClasses


                var graph = new DevGraphDefinition(project, "DevEngine.Tests.Class3.method3", class3, DevGraphDefinitionType.Method, method3.Name); // name can be whatever really... 
                graph.InitializeEmptyForMethod(method3);
                method3.GraphDefinition = graph;


                // we add a "Add" node to the graph
                var addNode = new Standard.Math.AddNode<int>(Guid.NewGuid(), "Add", project);
                graph.Nodes.Add(addNode);


                // connect the entry "p1" node parameter to both inputs of the "addNode"
                graph.ConnectNodesParameters(graph.EntryPoint.Outputs.First(x => x.Name == "p1"), addNode.Inputs.First(x => x.Name == "A"));
                graph.ConnectNodesParameters(graph.EntryPoint.Outputs.First(x => x.Name == "p1"), addNode.Inputs.First(x => x.Name == "B"));

                // connect the output from the "Add" to the exit node return parameter
                graph.ConnectNodesParameters(addNode.Outputs.Single(), graph.ExitPoints.First().ReturnNodeParameter);

                // connect the exec directly from the entry to the exit
                graph.ConnectNodesParameters(graph.EntryPoint.ExecNodeParameter, graph.ExitPoints.First().ExecNodeParameter);

                projectFolder = Path.Combine(Path.GetTempPath(), Path.GetFileNameWithoutExtension(Path.GetRandomFileName()));

                if (Directory.Exists(projectFolder))
                    Directory.Delete(projectFolder, true);

                Directory.CreateDirectory(projectFolder);

                project.Save(projectFolder);
            }


            var newProject = new DevProject("Test", new RealTypesProviderService());
            newProject.Load(projectFolder);

            var classToEvaluate = newProject.Classes.First(x => x.Key.Name == "Class3").Value;

            var evaluator = new Evaluator.DevGraphEvaluator();
            evaluator.Evaluate(new Core.DevObject(classToEvaluate, null), (DevMethod)classToEvaluate.Methods.First(x => x.Name == "method3"), new Dictionary<string, Core.DevObject>
            {
                ["p1"] = new Core.DevObject(newProject.GetRealType<int>(), 5),
                ["p2"] = new Core.DevObject(classToEvaluate, null),
            }, out var outputs);


            Assert.AreEqual(1, outputs.Count);
            Assert.AreEqual(10, outputs.First(x => x.Key == "return").Value?.Value);

        }
    }
}
