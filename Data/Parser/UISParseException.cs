using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UISEditor.Data.Lexical;

namespace UISEditor.Data.Parser
{
    public static partial class UISParser
    {
        public class TokenWrongException : Exception
        {
            public TokenWrongException(Token token, params Tag[] except) : base("Ln " + token.Line + ", " + token.TokenTag.ToString() + " should " + except[0])
            {

            }
        }
        public class ParseException : Exception
        {
            public ParseException(Word token, string expect) : base($"Ln {token.Line}, {token.Lexeme} should {expect}")
            {

            }
        }
    }
}
