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

namespace DevEngine.Tests
{
    [TestClass]
    public class TestEvaluator
    {
        [TestMethod]
        public void TestBasicMethodContent()
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

            var evaluator = new Evaluator.DevGraphEvaluator();
            evaluator.Evaluate(new Core.DevObject(class3, null), method3, new Dictionary<string, Core.DevObject>
            {
                ["p1"] = new Core.DevObject(project.GetRealType<int>(), 5),
                ["p2"] = new Core.DevObject(class3, null),
            }, out var outputs);


            Assert.AreEqual(1, outputs.Count);
            Assert.AreEqual(10, outputs.First( x=> x.Key == "return").Value?.Value);
        }
    }
}
