using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace CircularDependencyTool
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }
    }

    public class ViewModel
    {
        public IEnumerable Children { get; } = BuildGraphs();

        public static IEnumerable<Graph> BuildGraphs()
        {
            return System.IO.Directory
                .GetFiles("Graphs")
                .Select(GraphBuilder.BuildGraph);
        }
    }
}