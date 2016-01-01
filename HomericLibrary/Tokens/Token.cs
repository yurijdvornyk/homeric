using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HomericLibrary.Tokens
{
    public abstract class Token
    {
        public string Lexeme { get; protected set; }
        public int Priority { get; protected set; }

        public Token() : this(string.Empty, 0) { }

        public Token(string lexeme) : this(lexeme, 0) { }

        public Token(string lexeme, int priority)
        {
            Lexeme = lexeme;
            Priority = priority;
        }

        public override string ToString()
        {
            return Lexeme;
        }
    }
}