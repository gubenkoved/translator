using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Parser.Core
{
    public class SyntaxTree
    {
        public SyntaxTreeNode Root { get; private set; }

        public SyntaxTree(SyntaxTreeNode root)
        {
            Root = root;
        }

        public SyntaxTreeNode FindFirst(Func<SyntaxTreeNode, bool> predicate)
        {
            return FindFirst(Root, predicate);
        }

        private SyntaxTreeNode FindFirst(SyntaxTreeNode node, Func<SyntaxTreeNode, bool> predicate)
        {
            if (predicate.Invoke(node) == true)
                return node;

            foreach (var child in node.Children)
            {
                var findResultInChild = FindFirst(child, predicate);

                if (findResultInChild != null)
                    return findResultInChild;
            }

            return null;
        }
    }
}
