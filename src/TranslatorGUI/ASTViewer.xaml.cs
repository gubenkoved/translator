using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Parser.Core;
using CodeGeneration;

namespace TranslatorGUI
{
    /// <summary>
    /// Interaction logic for ASTViewer.xaml
    /// </summary>
    public partial class ASTViewer : Window
    {        
        public ASTViewer()
        {
            InitializeComponent();
        }

        public void LoadCode(IntermediateCode code)
        {
            CodeList.ItemsSource = code.Instructions;
        }

        public void LoadTree(SyntaxTree tree)
        {
            TreeView.Items.Clear();

            var rootItem = Grab(tree.Root);

            RecursiveGrabber(rootItem, tree.Root);

            TreeView.Items.Add(rootItem);
        }

        public TreeViewItem Grab(SyntaxTreeNode node)
        {
            var item = new TreeViewItem();

            if (node.Value is ConcreteTerminal)
                item.Header = string.Format("{{{0}}}", node.Value);
            else
                item.Header = node.Value.ToString().ToUpper();

            string attributes = string.Join(", ", node.Value.Attributes.Select(p => string.Format("{0}={1}", p.Key, GetStringRepresentation(p.Value))));
            
            if (node.Value.Attributes.Count > 0)
                item.Header += "[" + attributes + "]";

            return item;
        }

        public string GetStringRepresentation(object obj)
        {
            if (obj == null)
                return "null";

            if (obj is IEnumerable<object>)
            {
                return string.Format("({0})", string.Join(", ", (IEnumerable<object>)obj));
            }

            return obj.ToString();
        }

        public void RecursiveGrabber(TreeViewItem item, SyntaxTreeNode node)
        {
            foreach (var child in node.Children)
            {
                var childItem = Grab(child);
                item.Items.Add(childItem);
                RecursiveGrabber(childItem, child);
            }
        }
    }
}
