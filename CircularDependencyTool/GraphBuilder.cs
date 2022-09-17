﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace CircularDependencyTool
{
    public class GraphBuilder
    {
        // NOTE: This method is invoked from AppWindow.xaml
        public static IEnumerable<Graph> BuildGraphs()
        {
            return new List<Graph>
            {
                BuildGraph("Simple.xml"),
                BuildGraph("Complex.xml"),
                BuildGraph("VeryCircular.xml"),                                
                BuildGraph("SimpleCircular.xml"),
                BuildGraph("ComplexCircular.xml"),                
                BuildGraph("SuperCircular.xml")
            };
        }

        static Graph BuildGraph(string xmlFileName)
        {
            string path = string.Format(@"Graphs\{0}", xmlFileName);
            XDocument xdoc = XDocument.Load(path);

            // Create a graph.
            var graphElem = xdoc.Element("graph");
            string title = graphElem.Attribute("title").Value;
            var graph = new Graph(title);

            var nodeElems = graphElem.Elements("node").ToList();

            // Create all of the nodes and add them to the graph.
            foreach (XElement nodeElem in nodeElems)
            {
                string id = nodeElem.Attribute("id").Value;
                double x = (double)nodeElem.Attribute("x");
                double y = (double)nodeElem.Attribute("y");

                var node = new Node(id, x, y);
                graph.Nodes.Add(node);
            }

            // Associate each node with its dependencies.
            foreach (Node node in graph.Nodes)
            {
                var nodeElem = nodeElems.First(elem => elem.Attribute("id").Value == node.ID);
                var dependencyElems = nodeElem.Elements("dependency");
                foreach (XElement dependencyElem in dependencyElems)
                {
                    string depID = dependencyElem.Attribute("id").Value;

                    if (depID == node.ID)
                        throw new Exception("A node cannot be its own dependency.  Node ID = " + depID);

                    var dependency = graph.Nodes.FirstOrDefault(n => n.ID == depID);
                    if (dependency != null)
                        node.NodeDependencies.Add(dependency);
                }
            }

            // Tell the graph to inspect itself for circular dependencies.
            graph.CheckForCircularDependencies();

            return graph;
        }
    }
}