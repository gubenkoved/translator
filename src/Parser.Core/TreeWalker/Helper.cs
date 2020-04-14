using System.Collections.Generic;

namespace Parser.Core.TreeWalker
{
    public static class WalkHelper
    {
        public static IEnumerable<SyntaxTreeNode> GetAllParents(SyntaxTreeNode node)
        {
            List<SyntaxTreeNode> parents = new List<SyntaxTreeNode>();

            SyntaxTreeNode cur = node;
            do
            {
                cur = cur.Parent;

                if (cur != null)
                    parents.Add(cur);
            
            } while (cur != null);

            return parents;
        }

        public static SyntaxTreeNode FindParentWithNonterminal(SyntaxTreeNode node, Nonterminal nonterminal)
        {
            SyntaxTreeNode cur = node;

            do
            {
                if (cur.Value.Equals(nonterminal))
                    return cur;

                cur = cur.Parent;
            } while (cur != null);

            return null;
        }
    }
}
