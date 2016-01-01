using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HomericLibrary.Tokens
{
    class Operator : Token
    {
        public Operator(string lexeme): base(lexeme)
        {
            switch (lexeme)
            {
                case "+":
                    Priority = 1;
                    break;

                case "-":
                    Priority = 1;
                    break;

                case "*":
                    Priority = 2;
                    break;

                case "/":
                    Priority = 2;
                    break;

                case "^":
                    Priority = 3;
                    break;

                case "%":
                    Priority = 2;
                    break;

                default:
                    Priority = 0;
                    break;
            }
        }
    }
}
