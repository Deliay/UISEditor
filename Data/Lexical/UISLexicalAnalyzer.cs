using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UISEditor.Data.Lexical
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
        private bool invalue = false;

        public UISLoader(string uiscode)
        {
            Reader = new Reader(uiscode);
            ScanWhileEnd();
        }

        public Token Scan()
        {
            char peek;
            for (; ; )
            {
                peek = Reader.ReadChar();
                if (!Reader.EOF()) return new Token(Tag.FLAG_END, Reader.CurrentLineNumber);
                if (peek == ' ' && !invalue)
                {
                    int count = 1;
                    do
                    {
                        peek = Reader.ReadChar();
                        count++;
                    } while (peek == ' ' && Reader.EOF());
                    if (count >= 4)
                    {
                        Reader.Back();
                        return new Tab(Reader.CurrentLineNumber);
                    }
                }
                if (peek == '\r') continue;
                if (peek == '\t') return new Tab(Reader.CurrentLineNumber);
                else if (peek == '\n')
                {
                    do
                    {
                        peek = Reader.ReadChar();
                    } while ((peek == '\n') && Reader.EOF());
                    Reader.Back();
                    invalue = false;
                    return new EndOfLine(Reader.CurrentLineNumber);
                }
                else if (peek == '#' && Reader.LastChar() == '\n')
                {
                    return new Comment(Reader.CurrentLineNumber, Reader.ReadLine());
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
                } while (char.IsLetterOrDigit(peek) || (invalue && peek == ' '));
                Reader.Back();
                return new Word(Tag.IDENTITY, val, Reader.CurrentLineNumber);
            }

            if (peek == '#')
            {
                string val = "" + peek;
                val += Reader.ReadChar();
                val += Reader.ReadChar();
                val += Reader.ReadChar();
                val += Reader.ReadChar();
                val += Reader.ReadChar();
                val += Reader.ReadChar();
                return new HexString(val, Reader.CurrentLineNumber);
            }

            if (char.IsDigit(peek))
            {
                int v = 0;
                do
                {
                    v = 10 * v + (int)char.GetNumericValue(peek);
                    peek = Reader.ReadChar();
                } while (char.IsDigit(peek) && Reader.EOF());
                if (peek != '.')
                {
                    Reader.Back();
                    return new Number(v, Reader.CurrentLineNumber);
                }

                double x = v, d = 10;
                while (true && Reader.EOF())
                {
                    peek = Reader.ReadChar();
                    if (!char.IsDigit(peek)) break;
                    x += char.GetNumericValue(peek) / d;
                    d *= 10;
                }

                Reader.Back();
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
                return new IndexArrayRight(Reader.CurrentLineNumber);
            }

            if (peek == '@')
            {
                int line = Reader.CurrentLineNumber;
                string lex = Reader.ReadLine();
                string id = lex.Substring(0, lex.IndexOf(' '));
                string value = lex.Substring(id.Length);
                return new AtProperty(line, id, value);
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
                invalue = true;
                return new Equal(Reader.CurrentLineNumber);
            }

            if(peek == '$')
            {
                return new Increase(Reader.CurrentLineNumber);
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
            } while (Reader.EOF());
            TokenList.AddLast(new EndOfLine(Reader.CurrentLineNumber));
            TokenList.AddLast(new EOF(Reader.CurrentLineNumber));
        }

    }
}
