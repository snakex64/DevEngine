using DevEngine.Class;
using DevEngine.Core.Class;
using DevEngine.Graph;
using DevEngine.Method;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DevEngine.Tests
{
    [TestClass]
    public class TestBasicGraph
    {
        [TestMethod]
        public void TestBasicMethodContent()
        {
            var project = TestBasicDeclarationsAndTypes.GetBasicDevProject();

            var class3 = new DevClass(project, null, new DevClassName("DevEngine.Tests.Class3"));
            project.Classes.Add(class3);

            var method3 = new DevMethod(class3, "method3", isStatic: false, returnType: project.GetRealType<int>(), Core.Visibility.Public);
            class3.Methods.Add(method3);

            method3.Parameters.Add(new DevMethodParameter(project.GetRealType<int>(), name: "p1", isOut: false, isRef: false));
            method3.Parameters.Add(new DevMethodParameter(class3, "p2", isOut: false, isRef: false)); // demonstrate passing DevClasses


            var graph = new DevGraphDefinition(project, "DevEngine.Tests.Class3.method3"); // name can be whatever really... 
            graph.InitializeEmptyForMethod(method3);
            method3.GraphDefinition = graph;


            // the only thing we do in the method is return something the input p1, no in-between nodes
            // so we connect the exec from the entry point directly to the exec of the exit point
            graph.ConnectNodesParameters(graph.EntryPoint.ExecNodeParameter, graph.ExitPoints.First().ExecNodeParameter);

            // don't forget to also connect the input p1 to the return value
            graph.ConnectNodesParameters(graph.EntryPoint.Outputs.First(x => x.Name == "p1"), graph.ExitPoints.First().ReturnNodeParameter);
        }
    }
}
