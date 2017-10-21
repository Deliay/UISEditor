using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UISEditor.Data
{
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

    public class EndOfLine : Token
    {
        public EndOfLine(int line) : base(Tag.LINE_END, line)
        {
        }
    }

    public class IndexArrayLeft : Operator
    {
        public IndexArrayLeft(int line) : base(Tag.LeftBar, "[", line)
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
        public Token(char tag, int line) : this((Tag)tag, line)
        {
        }
    }

}
