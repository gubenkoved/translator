namespace Parser.Core
{
    public class TokenStreamState
    {
        internal int _currentIndex;

        internal TokenStreamState(int currentIndex)
        {
            _currentIndex = currentIndex;
        }
    }
}
