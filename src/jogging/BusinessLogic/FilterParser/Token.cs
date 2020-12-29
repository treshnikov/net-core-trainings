namespace BusinessLogic.FilterParser
{
    public class Token
    {
        public string Ident { get; }

        public Token(string ident)
        {
            Ident = ident;
        }
    }
}