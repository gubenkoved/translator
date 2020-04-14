namespace Parser.Core
{
    public class Nonterminal : Symbol
    {
        public string Kind { get; private set; }

        public Nonterminal(string kind)
        {
            Kind = kind;
        }

        public override Symbol CreateCopy()
        {
            return new Nonterminal(Kind);
        }

        public override string ToString()
        {
            return string.Format("{0}", Kind);
        }

        internal static readonly LambdaEqualityComparer<Nonterminal> ByKindEqualityComprarer
            = new LambdaEqualityComparer<Nonterminal>(
                (n1, n2) => n1.Kind == n2.Kind,
                (n) => n.Kind.GetHashCode());
    }
}