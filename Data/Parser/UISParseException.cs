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
        public class TokenWrongException : UISRuntimeException
        {
            public TokenWrongException(Token token, params Tag[] except) : base($"{token.TokenTag.ToString()} should {except[0]}")
            {

            }
        }
        public class ParseException : UISRuntimeException
        {
            public ParseException(Word token, string expect) : base($"{token.Lexeme} should {expect}")
            {

            }
        }
    }
}
