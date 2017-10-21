using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UISEditor.Data
{
    /*
     * uis -> cmds elements
     * 
     * elements -> elements element
     * element -> customEs | funcEs | aniEs
     * 
     * cmd -> @id string
     * cmds -> cmds cmd | empty
     * 
     * customEs -> customEs customE | empty
     * funcEs -> funcEs funcE | empty
     * aniEs -> aniEs aniE | empty
     * 
     * *index -> number
     * *indexs -> indexs, index | empty
 
     * *customE -> _id \n props | _id-lists \n props
     * *funcE -> id \n props | id-lists \n props
     * *aniE -> :id \n aniCollects
     * *lists -> [indexs]
     * 
     * *prop -> tab id = value
     * *props -> props prop | empty
     * 
     * 
     * *aniProp -> aniName=value
     * *aniProps -> aniProps,aniProp
     * *aniCollects -> \n name=aniName \t \n aniCollects \n tab aniProps 
     * 
     * *animationRepeat -> number | r number | number, number | number, r number
     * *animationTime -> number | number, number | number, +number
     * *value -> term | string | vector | hexcolor | filename | framefile | motion
     * 
     * *vector -> term, term | (term, term)
     * *term -> expr - expr | expr + expr | expr
     * *expr -> number | px | percent
     * 
     * *motion -> !aniProps
     * *percent -> number%
     * *px -> number"px"
     * 
     * *filename -> string.string
     * *framefile -> string / number - number
     * */
    public enum Tag
    {
        START_FLAG = 0,
        LINE_END = '\n',
        Split = ',',
        Equal = '=',
        AtProp = '@',
        Add = '+',
        Index = '-',
        UserDef = '_',
        LeftPar = '(',
        RightPar = ')',
        LeftBar = '[',
        RightBar = ']',
        Percent = '%',
        Animation = ':',
        Dot = '.',
        AnimationInline = '!',
        FrameRangeDiv = '/',
        FLAG_END = 256,
        ANIMATION,
        NUMBER,
        REAL,
        STRING,
        HEX_STRING,
        IDENTITY,
        UNIT,
        TAB,

    }

    public class Reader
    {
        string codes;
        int currentPos = 0, currentLine = 0, currentLinePos = 0;
        public Reader(string code)
        {
            codes = code;
        }

        public int CurrentPosition { get => currentLinePos; }
        public int CurrentLineNumber { get => currentLine; }
        public int CurrentLinePosition { get => currentLinePos; }

        public char ReadChar()
        {
            char result = codes[currentPos++];
            if (result == (char)Tag.LINE_END)
            {
                currentLine++;
                currentLinePos = -1;
            }
            currentLinePos++;
            return codes[currentPos++];
        }

        public char PeekChar()
        {
            return codes[currentPos];
        }

        public bool isTerminalSymbol(char ch)
        {
            return Enum.IsDefined(typeof(Tag), ch);
        }

        public bool EOF()
        {
            return currentPos + 1 <= codes.Length;
        }

        public string ReadLine()
        {
            string cache = string.Empty;
            char cur = ReadChar();
            while (cur != (char)Tag.LINE_END)
            {
                cache += (cur);
                cur = ReadChar();
            }
            return cache;
        }

        public void Back() => currentPos--;

    }

    public class EndOfLine : Token
    {
        public EndOfLine(int line) : base(Tag.LINE_END, line)
        {
        }
    }

    public class IndexArrayLeft : Operator
    {
        public IndexArrayLeft( int line) : base(Tag.LeftBar, "[", line)
        {
        }
    }

    public class IndexArrayRight : Operator
    {
        public IndexArrayRight(int line) : base(Tag.RightBar, "]", line)
        {
        }
    }

    public class AtProperty : Operator
    {
        public AtProperty(int line) : base(Tag.AtProp, "@", line)
        {
        }
    }

    public class VectorLeft : Operator
    {
        public VectorLeft(int line) : base(Tag.LeftPar, "(", line)
        {
        }
    }

    public class Dot : Operator
    {
        public Dot(int line) : base(Tag.Dot, ".", line)
        {
        }
    }

    public class Equal : Operator
    {
        public Equal(int line) : base(Tag.Equal, "=", line)
        {
        }
    }

    public class UserDef : Operator
    {
        public UserDef(int line) : base(Tag.UserDef, "_", line)
        {
        }
    }

    public class Percent : Operator
    {
        public Percent(int line) : base(Tag.Percent, "%", line)
        {
        }
    }

    public class Animation : Operator
    {
        public Animation(int line) : base(Tag.Animation, ":", line)
        {
        }
    }

    public class AnimationInline : Operator
    {
        public AnimationInline(int line) : base(Tag.AnimationInline, "!", line)
        {
        }
    }

    public class FrameRangeDiv : Operator
    {
        public FrameRangeDiv(int line) : base(Tag.FrameRangeDiv, "/", line)
        {
        }
    }

    public class VectorRight : Operator
    {
        public VectorRight(int line) : base(Tag.RightPar, ")", line)
        {
        }
    }

    public class Index : Operator
    {
        public Index(int line) : base(Tag.Index, "-", line)
        {
        }
    }

    public class Split : Operator
    {
        public Split(int line) : base(Tag.Split, ",", line)
        {
        }
    }

    public class Operator : Word
    {
        public Operator(Tag tag, string lex, int line) : base(tag, lex, line)
        {
        }
    }

    public class UString : Word
    {
        public UString(string value, int line) : base(Tag.STRING, value, line)
        {

        }
    }

    public class HexString : Word
    {
        public HexString(string value, int line) : base(Tag.HEX_STRING, value, line)
        {

        }
    }


    public class Number : Token
    {
        public int Value { get; private set; }
        public Number(int value, int line) : base(Tag.NUMBER, line)
        {
            Value = value;
        }
    }

    public class RealNumber : Token
    {
        public double Value { get; private set; }
        public RealNumber(double value, int line) : base(Tag.REAL, line)
        {
            Value = value;
        }
    }

    public class Word : Token
    {
        public string Lexeme { get; private set; }
        public Word(Tag t, string lex, int line) : base(t, line)
        {
            Lexeme = lex;
        }

        public override string ToString() => Lexeme;
    }

    public class Tab : Token
    {
        public Tab(int line) : base(Tag.TAB, line)
        {
        }
    }

    public class Token
    {
        public int Line { get; private set; }
        public Tag TokenTag { get; private set; }
        public Token(Tag tag, int line)
        {
            Line = line;
            TokenTag = tag;
        }
        public Token(char tag, int line) : this((Tag) tag, line)
        {
        }
    }

    /// <summary>
    /// Lecical Analyzer
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
            for(; ;peek = Reader.ReadChar())
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

            if(char.IsLetter(peek))
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

            if(peek == '[')
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
            do { 
                this.TokenList.AddLast(tok);
                tok = Scan();
            } while (tok.TokenTag != Tag.FLAG_END);
        }

    }

    class TokenReader
    {
        List<Token> tokens;
        int currentIndex = 0;

        public TokenReader(IEnumerable<Token> tokens)
        {
            this.tokens = new List<Token>(tokens);
        }

        public Token ReadLast()
        {
            return tokens[currentIndex - 2];
        }

        public Token Read()
        {
            return tokens[currentIndex++];

        }

        public Token ReadNext()
        {
            return tokens[currentIndex];
        }

        public void Reset()
        {
            currentIndex = 0;
        }
    }

    /// <summary>
    /// Token Parser
    /// </summary>
    public static class UISParser
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

        private static UISLoader Loader;
        private static TokenReader Reader { get; set; }
        private static Token look;
        private static UISInstance instance;

        static UISParser()
        {
            InitConstraint();
        }

        public static void ParseFile(FileInfo UISFile)
        {
            using (StreamReader sr = UISFile.OpenText())
            {
                string code = sr.ReadToEnd();
                Loader = new UISLoader(code);
            }
        }

        public static void ParseCode(String code)
        {
            Loader = new UISLoader(code);
        }

        private static void move()
        {
            look = Reader.Read();
            while (look.TokenTag == Tag.LINE_END) look = Reader.Read();
        }

        private static bool Test(params Tag[] tags)
        {
            return tags.Contains(look.TokenTag);
        }

        private static bool TestGrammar(params Tag[] tags)
        {
            if (tags.Contains(look.TokenTag)) return true;
            else throw new TokenWrongException(look, tags);
        }

        private static bool Expect(params Tag[] tags)
        {
            if (tags.Contains(look.TokenTag))
            {
                move();
                return true;
            }
            else
            {
                return false;
            }
        }

        private static bool Expect(params char[] tags)
        {
            if (look != null && tags.Contains((char)look.TokenTag))
            {
                move();
                return true;
            }
            else
            {
                return false;
            }
        }

        private static bool ExpectGrammar(params Tag[] tags)
        {
            if (look != null && tags.Contains(look.TokenTag))
            {
                move();
                return true;
            }
            else
            {
                throw new TokenWrongException(look, tags);
            }
        }

        private static bool ExpectGrammar(params char[] tags)
        {
            if (look != null && tags.Contains((char)look.TokenTag))
            {
                move();
                return true;
            }
            else
            {
                throw new TokenWrongException(look, (Tag)tags[0]);
            }
        }

        private static UISInstance ParseInstance()
        {
            Reader = new TokenReader(Loader.TokenList);
            instance = new UISInstance();
        }



        private static UISPredefineElement predefineElement()
        {
            Func<string, UISPredefineElement> generator = v => {
                if (!Enum.TryParse(v, out PredefineElementType type)) throw new ParseException(look as Word, typeof(PredefineElementType).ToString());
                return new UISPredefineElement(type);
            };
            return ReadElementT(Tag.IDENTITY, generator, props, false);
        }

        private static UISCustomElement customElement()
        {
            return ReadElementT(Tag.UserDef, v => new UISCustomElement(v), props);
        }
        

        private static UISAnimationElement animationElement()
        {
            return ReadElementT(Tag.Animation, v => new UISAnimationElement(v), aniCollect);

            //if (Expect(Tag.Animation))
            //{
            //    Word name = look as Word;
            //    UISAnimationElement aniE = new UISAnimationElement(name.Lexeme);
            //    if(Expect(Tag.Index))
            //    {
            //        aniE.IsMultiSelect = true;
            //        aniE.Indexs = indexs();
            //    }
            //    Expect(Tag.LINE_END);
            //    aniE.AddAllProperty(aniCollect());
            //    return aniE;
            //}
            //throw new ParseException(look as Word, "Animation element");
        }

        private static T ReadElementT<T, TR>
            (Tag expectTag, 
            Func<string, T> generator, 
            Func<LinkedList<TR>> readFunc, bool passTag = true) 
            where T : UISElement<TR> 
            where TR : UISObject
        {
            Func<Tag[], bool> passFunc;
            if (passTag) passFunc = Expect;
            else passFunc = Test;

            if(passFunc(new[] { expectTag }))
            {
                Word name = look as Word;
                T result = generator(name.Lexeme);
                if(Expect(Tag.Index))
                {
                    result.IsMultiSelect = true;
                    result.Indexs = indexs();
                }
                Expect(Tag.LINE_END);
                result.AddAllProperty(readFunc());
                return result;
            }
            throw new ParseException(look as Word, typeof(T).ToString());
        }

        private static LinkedList<UISNumber> indexs()
        {
            ExpectGrammar(Tag.LeftBar);
            LinkedList<UISNumber> lists = new LinkedList<UISNumber>();
            do
            {
                lists.AddLast(expr() as UISNumber);
            } while (Expect(Tag.Split));
            ExpectGrammar(Tag.RightBar);
            return lists;
        }

        private static LinkedList<UISProperty> props()
        {
            LinkedList<UISProperty> list = new LinkedList<UISProperty>();

            while (Expect(Tag.TAB))
            {
                list.AddLast(prop());
                ExpectGrammar(Tag.LINE_END);
            }

            return list;
        }

        private static UISProperty prop()
        {
            TestGrammar(Tag.IDENTITY);
            Word prop = look as Word;

            if (!Enum.TryParse(prop.Lexeme, out Property result))
                throw new ParseException(prop, typeof(Property).ToString());

            ExpectGrammar(Tag.IDENTITY);
            ExpectGrammar(Tag.Equal);

            UISValue val = value();
            ExpectGrammar(Tag.LINE_END);

            return new UISProperty(result, val);
        }

        private static void InitConstraint()
        {
            PropertyConstraint.AddPropertyConstraint<Func<UISValue>>(Property.TEX, filename);

            PropertyConstraint.AddPropertyConstraint<Func<UISValue>>(Property.FRAME, framefile);
            PropertyConstraint.AddPropertyConstraint<Func<UISValue>>(Property.COLOR, hexcolor);

            PropertyConstraint.AddPropertyConstraint<Func<UISValue>>(Property.SIZE, vector);
            PropertyConstraint.AddPropertyConstraint<Func<UISValue>>(Property.POS, vector);
            PropertyConstraint.AddPropertyConstraint<Func<UISValue>>(Property.PART, vector);

            PropertyConstraint.AddPropertyConstraint<Func<UISValue>>(Property.ANCHOR, term);
            PropertyConstraint.AddPropertyConstraint<Func<UISValue>>(Property.ROTATE, term);
            PropertyConstraint.AddPropertyConstraint<Func<UISValue>>(Property.FLIP, term);
            PropertyConstraint.AddPropertyConstraint<Func<UISValue>>(Property.OPACITY, term);
            PropertyConstraint.AddPropertyConstraint<Func<UISValue>>(Property.ZINDEX, term);
            PropertyConstraint.AddPropertyConstraint<Func<UISValue>>(Property.FSIZE, term);
            PropertyConstraint.AddPropertyConstraint<Func<UISValue>>(Property.BLEND, term);
            PropertyConstraint.AddPropertyConstraint<Func<UISValue>>(Property.TYPE, term);

            PropertyConstraint.AddPropertyConstraint<Func<UISValue>>(Property.TEXT, word);

            PropertyConstraint.AddPropertyConstraint<Func<UISValue>>(AnimationName.MOVE, vector);
            PropertyConstraint.AddPropertyConstraint<Func<UISValue>>(AnimationName.SCALE, vector);
            PropertyConstraint.AddPropertyConstraint<Func<UISValue>>(AnimationName.SIZE, vector);
            PropertyConstraint.AddPropertyConstraint<Func<UISValue>>(AnimationName.SKEW, vector);


            PropertyConstraint.AddPropertyConstraint<Func<UISValue>>(AnimationName.WIDTH, value);
            PropertyConstraint.AddPropertyConstraint<Func<UISValue>>(AnimationName.HEIGHT, value);
            PropertyConstraint.AddPropertyConstraint<Func<UISValue>>(AnimationName.MOVEX, value);
            PropertyConstraint.AddPropertyConstraint<Func<UISValue>>(AnimationName.MOVEY, value);
            PropertyConstraint.AddPropertyConstraint<Func<UISValue>>(AnimationName.SCALEX, value);
            PropertyConstraint.AddPropertyConstraint<Func<UISValue>>(AnimationName.SCALEY, value);
            PropertyConstraint.AddPropertyConstraint<Func<UISValue>>(AnimationName.SKEWX, value);
            PropertyConstraint.AddPropertyConstraint<Func<UISValue>>(AnimationName.SKEWY, value);
            PropertyConstraint.AddPropertyConstraint<Func<UISValue>>(AnimationName.ROTATE, value);
            PropertyConstraint.AddPropertyConstraint<Func<UISValue>>(AnimationName.FADE, value);

            PropertyConstraint.AddPropertyConstraint<Func<UISValue>>(AnimationName.TINY, hexcolor);

            PropertyConstraint.AddPropertyConstraint<Func<UISValue>>(AnimationName.SHOW, nul);
            PropertyConstraint.AddPropertyConstraint<Func<UISValue>>(AnimationName.HIDE, nul);
        }

        private static UISNull nul()
        {
            return new UISNull();
        }

        private static LinkedList<UISAnimation> aniCollect()
        {
            LinkedList<UISAnimation> list = new LinkedList<UISAnimation>();
            while(Expect(Tag.TAB))
            {
                list.AddLast(animation());
                ExpectGrammar(Tag.LINE_END);
            }
            return list;
        }

        private static UISAnimation animation()
        {
            Word aniName;
            ExpectGrammar(Tag.STRING);
            aniName = look as Word;
            if(aniName.Lexeme != "name")
            {
                throw new ParseException(aniName, "Animation target name");
            }
            ExpectGrammar(Tag.Equal);
            ExpectGrammar(Tag.STRING);
            aniName = look as Word;

            Func<UISValue> readFunc;
            if (Enum.TryParse(aniName.Lexeme, true, out AnimationName TargetAnimate))
            {
                readFunc = PropertyConstraint.GetPropertyConstraint<Func<UISValue>>(TargetAnimate);
            }
            else
            {
                throw new ParseException(aniName, ObjectTag.PROP.ToString());
            }

            ExpectGrammar(Tag.Split);

            Word kw;
            UISAnimation ani = new UISAnimation(TargetAnimate);

            do
            {
                kw = look as Word;
                switch (kw.Lexeme)
                {
                    case "from":
                        ExpectGrammar(Tag.Equal);
                        ani.AddAnimationProperty(new UISAnimationProperty(AnimationType.FORM, readFunc() as UISLiteralValue));
                        ExpectGrammar(Tag.Split);

                        TestGrammar(Tag.STRING);
                        kw = look as Word;
                        if (kw.Lexeme != "to") throw new ParseException(kw, "to");

                        ExpectGrammar(Tag.STRING);
                        ExpectGrammar(Tag.Equal);

                        ani.AddAnimationProperty(new UISAnimationProperty(AnimationType.TO, readFunc() as UISLiteralValue));

                        break;
                    case "time":
                        ExpectGrammar(Tag.Equal);
                        ani.AddAnimationProperty(new UISAnimationProperty(AnimationType.TIME, animationTime()));
                        break;
                    case "atime":
                        ExpectGrammar(Tag.Equal);
                        ani.AddAnimationProperty(new UISAnimationProperty(AnimationType.ATIME, animationTime()));
                        break;
                    case "repeat":
                        ExpectGrammar(Tag.Equal);
                        ani.AddAnimationProperty(new UISAnimationProperty(AnimationType.REPEAT, animationRepeat()));
                        break;
                    case "trans":
                        ExpectGrammar(Tag.Equal);
                        ani.AddAnimationProperty(new UISAnimationProperty(AnimationType.TRANS, new UISAnimationCurve(readCurve())));
                        break;
                    default:
                        throw new ParseException(kw, ObjectTag.ANI_PROP_DEF.ToString());
                }
            } while (!(Reader.ReadNext() is EndOfLine));

            return ani;
        }

        private static UISValue value()
        {
            Word propName;
            ExpectGrammar(Tag.STRING);
            propName = look as Word;
            if (Enum.TryParse(propName.Lexeme, true, out Property TargetProperty))
            {

                return PropertyConstraint.GetPropertyConstraint<Func<UISValue>>(TargetProperty)();
            }
            else
            {
                throw new ParseException(propName, ObjectTag.PROP.ToString());
            }
        }

        private static UISCurve readCurve()
        {
            List<UISNumber> lists = new List<UISNumber>();
            ExpectGrammar(Tag.LeftPar);
            lists.Add(expr() as UISNumber);
            while (Test(Tag.Split))
            {
                ExpectGrammar(Tag.Split);
                lists.Add(expr() as UISNumber);
            }
            ExpectGrammar(Tag.RightPar);
            return new UISCurve(lists);
        }

        private static UISAnimationRepeat animationRepeat()
        {
            bool isLoop = false;
            UISNumber repeatCount = null;
            UISNumber repeatTime = null;
            if(Test(Tag.IDENTITY))
            {
                string repeatCountLiteral = (look as Word).Lexeme;
                if(repeatCountLiteral.StartsWith("r"))
                {
                    isLoop = true;
                    repeatCountLiteral = repeatCountLiteral.Substring(1);
                    if (int.TryParse(repeatCountLiteral, out int result))
                    {
                        repeatCount = new UISNumber(result);
                        ExpectGrammar(Tag.IDENTITY);
                    }
                }
                else throw new ParseException(look as Word, "r+Number");
            }
            else if(Test(Tag.NUMBER))
            {
                repeatCount = expr() as UISNumber;
                ExpectGrammar(Tag.NUMBER);
            }

            if(Test(Tag.Split))
            {
                ExpectGrammar(Tag.Split);
                repeatTime = expr() as UISNumber;
            }
            else repeatTime = new UISNumber(0); 

            return new UISAnimationRepeat(repeatCount, repeatTime, isLoop);
        }

        private static UISAnimationTime animationTime()
        {
            UISNumber start = expr() as UISNumber;
            if (Test(Tag.Split)) ExpectGrammar(Tag.Split);
            else return new UISAnimationTime(start, new UISNumber(0), false);

            if (Test(Tag.Add)) ExpectGrammar(Tag.Add);
            else return new UISAnimationTime(start, expr() as UISNumber);

            return new UISAnimationTime(start, expr() as UISNumber, true);

        }

        /// <summary>
        /// vector -> term, term
        /// </summary>
        /// <returns></returns>
        private static UISVector vector()
        {
            UISLiteralValue first, second;
            bool include = Test(Tag.LeftPar);
            if (include) Expect(Tag.LeftPar);
            first = term();
            ExpectGrammar(Tag.Split);
            second = term();
            if (include) ExpectGrammar(Tag.RightPar);
            return new UISVector(first, second, include);
        }

        private static UISHexColor hexcolor()
        {
            TestGrammar(Tag.HEX_STRING);
            HexString hex = look as HexString;
            ExpectGrammar(Tag.HEX_STRING);
            return new UISHexColor(hex.Lexeme);
        }

        private static UISLiteralValue term()
        {
            UISLiteralValue expr1, expr2;

            expr1 = expr();
            if(Expect(Tag.Add))
            {
                expr2 = expr();
                return new UISSimpleExpr(expr1, expr2);
            }
            else if (Expect(Tag.Index))
            {
                expr2 = expr();
                return new UISSimpleExpr(expr1, expr2, false);
            }
            else
            {
                return expr1;
            }

        }

        /// <summary>
        /// expr -> number | px | percent
        /// <para>percent -> number%</para>
        /// <para>px -> number"px"</para>
        /// <para>number</para>
        /// </summary>
        /// <returns></returns>
        private static UISLiteralValue expr()
        {
            double value = 0;
            Number n;
            RealNumber r;
            if(Test(Tag.NUMBER))
            {
                n = look as Number;
                value = n.Value;
                ExpectGrammar(Tag.NUMBER);
            }
            else if(TestGrammar(Tag.REAL))
            {
                r = look as RealNumber;
                value = r.Value;
                ExpectGrammar(Tag.NUMBER);
            }

            //px % or literal value
            if(Test(Tag.IDENTITY))
            {
                Word t = look as Word;
                if (t.Lexeme == "px")
                {
                    ExpectGrammar(Tag.IDENTITY);
                    return new UISPixel(value);
                }
            }
            else if(Expect(Tag.Percent))
            {
                return new UISPercent(value);
            }

            return new UISNumber(value);

        }

        /// <summary>
        /// filename -> string.string
        /// </summary>
        /// <returns></returns>
        private static UISFileName filename()
        {
            Word value;
            string id1 = string.Empty, id2 = string.Empty;

            TestGrammar(Tag.IDENTITY);
            value = look as Word;
            ExpectGrammar(Tag.IDENTITY);
            id1 = value.Lexeme;

            ExpectGrammar(Tag.Dot);
            TestGrammar(Tag.IDENTITY);
            value = look as Word;
            ExpectGrammar(Tag.IDENTITY);
            id2 = value.Lexeme;

            return new UISFileName(string.Join(".", id1, id2));
        }

        private static UISText word()
        {
            Word result;
            TestGrammar(Tag.STRING);
            result = look as Word;
            ExpectGrammar(Tag.STRING);
            return new UISText(result.Lexeme);
        }

        /// <summary>
        /// framefile -> string / number - number
        /// </summary>
        /// <returns></returns>
        private static UISFrameFile framefile()
        {
            // frame/0-10
            // ID DIV NUMBER INDEX NUMBER
            Word p;
            Number n1, n2;

            TestGrammar(Tag.IDENTITY);
            p = look as Word;
            ExpectGrammar(Tag.IDENTITY);

            ExpectGrammar(Tag.FrameRangeDiv);

            TestGrammar(Tag.NUMBER);
            n1 = look as Number;
            ExpectGrammar(Tag.NUMBER);

            ExpectGrammar(Tag.Index);

            TestGrammar(Tag.NUMBER);
            n2 = look as Number;
            ExpectGrammar(Tag.NUMBER);

            return new UISFrameFile(p.Lexeme, n1.Value, n2.Value);
        }

    }
}
