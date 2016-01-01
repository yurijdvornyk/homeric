namespace HomericLibrary.Tokens.Values
{
    public class Number : Value
    {
        public Number(string lexeme) : base(lexeme) { }

        public Number(double lexeme) : this(lexeme.ToString()) { }
    }
}
