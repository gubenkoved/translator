using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Parser.Core.TreeWalker
{
    public enum WalkStepDirection
    {
        /// <summary>
        /// Up direction (to root)
        /// </summary>
        Bubbling,
        /// <summary>
        /// Down direction (from root to leafs)
        /// </summary>
        Tunelling
    }

    public class WalkStepInfo
    {
        public SyntaxTreeNode Node {get; private set;}
        public WalkStepDirection Direction { get; private set; }

        public WalkStepInfo(SyntaxTreeNode node, WalkStepDirection direction)
        {
            Node = node;

            Direction = direction;
        }
    }

    public static class DirectOrderWalker
    {
        public static List<WalkStepInfo> GetWalkOrder(SyntaxTree tree)
        {
            List<WalkStepInfo> order = new List<WalkStepInfo>();

            DirectOrderWalk(tree.Root, order);

            return order;
        }

        private static void DirectOrderWalk(SyntaxTreeNode node, List<WalkStepInfo> order)
        {
            order.Add(new WalkStepInfo(node, WalkStepDirection.Tunelling));

            foreach (var child in node.Children)
            {
                DirectOrderWalk(child, order);
            }

            order.Add(new WalkStepInfo(node, WalkStepDirection.Bubbling));
        }
    }
}
