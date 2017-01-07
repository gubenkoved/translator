using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lexer.Core;

namespace Parser.Core
{
    public abstract class Symbol
    {
        public Dictionary<string, object> Attributes { get; private set; }
        public object this[string key]
        {
            get
            {
                if (!Attributes.ContainsKey(key))
                    return null;
                
                return Attributes[key];
            }
            set
            {
                Attributes[key] = value;
            }
        }

        protected Symbol()
        {
            Attributes = new Dictionary<string, object>();
        }

        public abstract Symbol CreateCopy();

        public bool IsTerminalWithValue(string value)
        {
            if (this is ConcreteTerminal && ((ConcreteTerminal)this).Token.Value == value)
                return true;

            return false;
        }
        public bool IsTerminalWithType(TokenType type)
        {
            if (this is Terminal && ((Terminal)this).Type == type)
                return true;

            return false;
        }

        public override bool Equals(object obj)
        {
            if (obj is Symbol)
                return Symbol.EqualityComprarer.Equals(this, (Symbol)obj);

            return false;
        }
        public override int GetHashCode()
        {
            return 0;
        }

        /// <summary>
        /// General equality comprarer which uses:
        /// - by kind equality for non-terminals
        /// - by type equaility for generalized terminals
        /// - by token value equality for concrete terminals
        /// </summary>
        internal static readonly IEqualityComparer<Symbol> EqualityComprarer
            = new LambdaEqualityComparer<Symbol>(
                (s1, s2) =>
                {
                    if (s1 is Nonterminal && s2 is Nonterminal)
                        return Nonterminal.ByKindEqualityComprarer.EqualsPredicate.Invoke((Nonterminal)s1, (Nonterminal)s2);

                    if (s1 is GeneralizedTerminal && s2 is GeneralizedTerminal)
                        return GeneralizedTerminal.ByTypeEqualityComprarer.EqualsPredicate.Invoke((GeneralizedTerminal)s1, (GeneralizedTerminal)s2);

                    if (s1 is ConcreteTerminal && s2 is ConcreteTerminal)
                        return ConcreteTerminal.ByValueEqualityComprarer.EqualsPredicate.Invoke((ConcreteTerminal)s1, (ConcreteTerminal)s2);

                    return false;
                },
                s => 0);
    }
}
