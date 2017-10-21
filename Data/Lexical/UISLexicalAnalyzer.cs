using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UISEditor.Data
{

    /// <summary>
    /// Lexical Analyzer
    /// </summary>
    public class UISLoader
    {
        public class LecicalExpection : Exception
        {
            public LecicalExpection(string message) : base(message) { }
        }

        public Reader Reader { get; private set; }
        public LinkedList<Token> TokenList { get; private set; } = new LinkedList<Token>();

        public UISLoader(string uiscode)
        {
            Reader = new Reader(uiscode);
            ScanWhileEnd();
        }

        public Token Scan()
        {
            char peek = ' ';
            for (; ; peek = Reader.ReadChar())
            {
                if (!Reader.EOF()) return new Token(Tag.FLAG_END, Reader.CurrentLineNumber);
                if (peek == '\r') continue;
                if (peek == ' ')
                {
                    int count = 1;
                    do
                    {
                        peek = Reader.ReadChar();
                        count++;
                    } while (peek != ' ');
                    if (count == 4) return new Tab(Reader.CurrentLineNumber);
                }
                if (peek == '\t') return new Tab(Reader.CurrentLineNumber);
                else if (peek == '\n')
                {
                    do
                    {
                        peek = Reader.ReadChar();
                    } while (peek != '\n');

                    return new EndOfLine(Reader.CurrentLineNumber);
                }
                else break;
            }

            if (char.IsLetter(peek))
            {
                string val = string.Empty;
                do
                {
                    val += peek;
                    peek = Reader.ReadChar();
                } while (char.IsLetterOrDigit(peek));
                return new Word(Tag.IDENTITY, val, Reader.CurrentLineNumber);
            }

            if (peek == '#')
            {
                string val = string.Empty;
                do
                {
                    val += peek;
                    peek = Reader.ReadChar();
                } while (peek == '\n');
                return new HexString(val, Reader.CurrentLineNumber);
            }

            if (char.IsDigit(peek))
            {
                int v = 0;
                do
                {
                    v = 10 * v + (int)char.GetNumericValue(peek);
                    peek = Reader.ReadChar();
                } while (char.IsDigit(peek));
                if (peek != '.')
                {
                    Reader.Back();
                    return new Number(v, Reader.CurrentLineNumber);
                }
                double x = v, d = 10;
                while (true)
                {
                    peek = Reader.ReadChar();
                    if (!char.IsDigit(peek)) break;
                    x += char.GetNumericValue(peek) / d;
                    d *= 10;
                }
                return new RealNumber(x, Reader.CurrentLineNumber);
            }


            if (peek == '-')
            {
                return new Index(Reader.CurrentLineNumber);
            }

            if (peek == '[')
            {
                return new IndexArrayLeft(Reader.CurrentLineNumber);
            }

            if (peek == ']')
            {
                return new IndexArrayLeft(Reader.CurrentLineNumber);
            }

            if (peek == '@')
            {
                return new AtProperty(Reader.CurrentLineNumber);
            }

            if (peek == '(')
            {
                return new VectorLeft(Reader.CurrentLineNumber);
            }

            if (peek == ')')
            {
                return new VectorRight(Reader.CurrentLineNumber);
            }

            if (peek == '%')
            {
                return new Percent(Reader.CurrentLineNumber);
            }

            if (peek == ':')
            {
                return new Animation(Reader.CurrentLineNumber);
            }

            if (peek == '!')
            {
                return new AnimationInline(Reader.CurrentLineNumber);
            }

            if (peek == '/')
            {
                return new FrameRangeDiv(Reader.CurrentLineNumber);
            }

            if (peek == '.')
            {
                return new Dot(Reader.CurrentLineNumber);
            }

            if (peek == ',')
            {
                return new Split(Reader.CurrentLineNumber);
            }

            if (peek == '=')
            {
                return new Equal(Reader.CurrentLineNumber);
            }

            Token t = new Token(peek, Reader.CurrentLineNumber);
            return t;
        }

        public void ScanWhileEnd()
        {
            Token tok = Scan();
            do
            {
                this.TokenList.AddLast(tok);
                tok = Scan();
            } while (tok.TokenTag != Tag.FLAG_END);
        }

    }
}
