using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lexer.Core;


namespace Parser.Core
{
    public class SyntaxTreeNode
    {
        public Symbol Value { get; set; }
        public SyntaxTreeNode Parent { get; set; }

        private List<SyntaxTreeNode> _children;
        public IEnumerable<SyntaxTreeNode> Children
        {
            get
            {
                return _children;
            }
        }
        public bool HasNoChildren
        {
            get
            {
                return _children.Count == 0;
            }
        }
        public int ChildrenCount
        {
            get
            {
                return _children.Count;
            }
        }
        /// <summary>
        /// Returns true, when it have only one child - epsilon (generalized terminal)
        /// </summary>
        public bool IsEpsilon
        {
            get
            {
                return _children.Count == 1 && _children[0].Value.Equals(GeneralizedTerminal.Epsilon);
            }
        }

        public SyntaxTreeNode(Symbol value)
        {
            Value = value;

            _children = new List<SyntaxTreeNode>();
        }

        public void AddChild(SyntaxTreeNode node)
        {
            node.Parent = this;

            _children.Add(node);
        }

        public int GetChildIndex(SyntaxTreeNode child)
        {
            return _children.FindIndex(n => n == child);
        }

        public SyntaxTreeNode LeftNode()
        {
            if (Parent != null)
            {
                int thisChildNumber = Parent.GetChildIndex(this);

                if (thisChildNumber > 0)
                    return Parent[thisChildNumber - 1];
            }

            return null;
        }
        public SyntaxTreeNode this[int childNum]
        {
            get
            {
                if (childNum < _children.Count)
                    return _children[childNum];

                return null;
            }
        }
        public SyntaxTreeNode this[Nonterminal nonterminal]
        {
            get
            {
                return _children.FirstOrDefault(child => child.Value.Equals(nonterminal));
            }
        }
        public SyntaxTreeNode this[TokenType terminalType]
        {
            get
            {
                return _children.FirstOrDefault(child => (child.Value is Terminal) && (child.Value as Terminal).Type == terminalType);
            }
        }

        public override string ToString()
        {
            return string.Format("{0}", Value.ToString());
        }
    }
}
