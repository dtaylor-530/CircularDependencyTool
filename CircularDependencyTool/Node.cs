using System.Collections.Generic;
using System.Linq;
using MvvmFoundation.Wpf;

namespace CircularDependencyTool
{
    /// <summary>
    /// Represents an item in a graph.
    /// </summary>
    public class Node : ObservableObject
    {
        #region Constructor

        public Node(string id, double locationX, double locationY)
        {
            this.NodeDependencies = new List<Node>();
            this.CircularDependencies = new List<CircularDependency>();
            this.LocationX = locationX;
            this.LocationY = locationY;
            this.ID = id;
        }

        #endregion // Constructor

        #region Properties

        public List<CircularDependency> CircularDependencies { get; private set; }

        public bool HasCircularDependency
        {
            get { return this.CircularDependencies.Any(); }
        }

        public string ID { get; private set; }

        public double LocationX
        {
            get { return _locationX; }
            set
            {
                if (value == _locationX)
                    return;

                _locationX = value;

                base.RaisePropertyChanged("LocationX");
            }
        }

        public double LocationY
        {
            get { return _locationY; }
            set
            {
                if (value == _locationY)
                    return;

                _locationY = value;

                base.RaisePropertyChanged("LocationY");
            }
        }

        public List<Node> NodeDependencies { get; private set; }

        public double NodeWidth
        {
            get { return 40; }
        }

        public double NodeHeight
        {
            get { return 40; }
        }

        #endregion // Properties

        #region Methods

        public List<CircularDependency> FindCircularDependencies()
        {
            if (this.NodeDependencies.Count == 0)
                return null;

            var circularDependencies = new List<CircularDependency>();

            var stack = new Stack<NodeInfo>();
            stack.Push(new NodeInfo(this));

            while (stack.Any())
            {
                var current = stack.Peek().GetNextDependency();
                if (current != null)
                {
                    if (current.Node == this)
                    {
                        var nodes = stack.Select(info => info.Node);
                        circularDependencies.Add(new CircularDependency(nodes));
                    }
                    else
                    {
                        bool visited = stack.Any(info => info.Node == current.Node);
                        if (!visited)
                            stack.Push(current);
                    }
                }
                else
                {
                    stack.Pop();
                }
            }

            return circularDependencies;
        }

        private class NodeInfo
        {
            public NodeInfo(Node node)
            {
                this.Node = node;
                _index = 0;
            }

            public Node Node { get; private set; }

            public NodeInfo GetNextDependency()
            {
                if (_index < this.Node.NodeDependencies.Count)
                {
                    var nextNode = this.Node.NodeDependencies[_index++];
                    return new NodeInfo(nextNode);
                }

                return null;
            }

            int _index;
        }

        #endregion // Methods

        #region Fields

        double _locationX, _locationY;

        #endregion // Fields
    }
}