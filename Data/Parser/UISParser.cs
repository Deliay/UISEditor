using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UISEditor.Data.Lexical;

namespace UISEditor.Data.Parser
{
    public static partial class UISParser
    {

        public static event EventHandler OnParseStart;
        public static event EventHandler OnParseComplete;

        private static UISLoader Loader;
        public static TokenReader Reader { get; set; }
        private static Token look;
        private static UISInstance instance;

        static UISParser()
        {
            InitConstraint();
        }



        private static void InitConstraint()
        {
            PropertyConstraint.AddPropertyConstraint<Func<UISValue>>(Property.TEX, Filename);
            for (int i = (int)Property.TEX2; i < (int)Property.TEX10; i++)
            {
                PropertyConstraint.AddPropertyConstraint<Func<UISValue>>((Property)i, Filename);
            }
            PropertyConstraint.AddPropertyConstraint<Func<UISValue>>(Property.FRAME, Framefile);
            for (int i = (int)Property.FRAME2; i < (int)Property.FRAME20; i++)
            {
                PropertyConstraint.AddPropertyConstraint<Func<UISValue>>((Property)i, Framefile);
            }
            PropertyConstraint.AddPropertyConstraint<Func<UISValue>>(Property.COLOR, Hexcolor);

            PropertyConstraint.AddPropertyConstraint<Func<UISValue>>(Property.SIZE, RelativeVector);
            PropertyConstraint.AddPropertyConstraint<Func<UISValue>>(Property.POS, RelativeVector);
            PropertyConstraint.AddPropertyConstraint<Func<UISValue>>(Property.SIZE2, RelativeVector);
            PropertyConstraint.AddPropertyConstraint<Func<UISValue>>(Property.POS2, RelativeVector);
            PropertyConstraint.AddPropertyConstraint<Func<UISValue>>(Property.PART, Vector);

            PropertyConstraint.AddPropertyConstraint<Func<UISValue>>(Property.ANCHOR, Term);
            PropertyConstraint.AddPropertyConstraint<Func<UISValue>>(Property.ROTATE, Term);
            PropertyConstraint.AddPropertyConstraint<Func<UISValue>>(Property.FLIP, Term);
            PropertyConstraint.AddPropertyConstraint<Func<UISValue>>(Property.OPACITY, Term);
            PropertyConstraint.AddPropertyConstraint<Func<UISValue>>(Property.ZINDEX, Term);
            PropertyConstraint.AddPropertyConstraint<Func<UISValue>>(Property.FSIZE, Term);
            PropertyConstraint.AddPropertyConstraint<Func<UISValue>>(Property.BLEND, Term);
            PropertyConstraint.AddPropertyConstraint<Func<UISValue>>(Property.TYPE, Term);
            PropertyConstraint.AddPropertyConstraint<Func<UISValue>>(Property.TYPE2, Term);
            PropertyConstraint.AddPropertyConstraint<Func<UISValue>>(Property.INTERVAL, Term);
            PropertyConstraint.AddPropertyConstraint<Func<UISValue>>(Property.MOTION, Motion);
            PropertyConstraint.AddPropertyConstraint<Func<UISValue>>(Property.ACTION1, Motion);
            PropertyConstraint.AddPropertyConstraint<Func<UISValue>>(Property.ACTION2, Motion);
            PropertyConstraint.AddPropertyConstraint<Func<UISValue>>(Property.HOVER, Motion);
            PropertyConstraint.AddPropertyConstraint<Func<UISValue>>(Property.LEAVE, Motion);
            PropertyConstraint.AddPropertyConstraint<Func<UISValue>>(Property.TEXT, Word);
            PropertyConstraint.AddPropertyConstraint<Func<UISValue>>(Property.PARENT, Word);
            PropertyConstraint.AddPropertyConstraint<Func<UISValue>>(Property.TAG, Term);
            PropertyConstraint.AddPropertyConstraint<Func<UISValue>>(Property.ETAG, Term);
            PropertyConstraint.AddPropertyConstraint<Func<UISValue>>(Property.LANG, Word);
            PropertyConstraint.AddPropertyConstraint<Func<UISValue>>(Property.UNSUPPOORT, Word);
            PropertyConstraint.AddPropertyConstraint<Func<UISValue>>(Property.TIP, Word);
            PropertyConstraint.AddPropertyConstraint<Func<UISValue>>(Property.SHOW, Term);

            PropertyConstraint.AddPropertyConstraint<Func<UISValue>>(AnimationName.MOVE, Vector);
            PropertyConstraint.AddPropertyConstraint<Func<UISValue>>(AnimationName.SCALE, Vector);
            PropertyConstraint.AddPropertyConstraint<Func<UISValue>>(AnimationName.SIZE, Vector);
            PropertyConstraint.AddPropertyConstraint<Func<UISValue>>(AnimationName.SKEW, Vector);

            PropertyConstraint.AddPropertyConstraint<Func<UISValue>>(AnimationName.WIDTH, Term);
            PropertyConstraint.AddPropertyConstraint<Func<UISValue>>(AnimationName.HEIGHT, Term);
            PropertyConstraint.AddPropertyConstraint<Func<UISValue>>(AnimationName.MOVEX, Term);
            PropertyConstraint.AddPropertyConstraint<Func<UISValue>>(AnimationName.MOVEY, Term);
            PropertyConstraint.AddPropertyConstraint<Func<UISValue>>(AnimationName.SCALEX, Term);
            PropertyConstraint.AddPropertyConstraint<Func<UISValue>>(AnimationName.SCALEY, Term);
            PropertyConstraint.AddPropertyConstraint<Func<UISValue>>(AnimationName.SKEWX, Term);
            PropertyConstraint.AddPropertyConstraint<Func<UISValue>>(AnimationName.SKEWY, Term);
            PropertyConstraint.AddPropertyConstraint<Func<UISValue>>(AnimationName.ROTATE, Term);
            PropertyConstraint.AddPropertyConstraint<Func<UISValue>>(AnimationName.FADE, Term);

            PropertyConstraint.AddPropertyConstraint<Func<UISValue>>(AnimationName.TINY, Hexcolor);

            PropertyConstraint.AddPropertyConstraint<Func<UISValue>>(AnimationName.SHOW, Nul);
            PropertyConstraint.AddPropertyConstraint<Func<UISValue>>(AnimationName.HIDE, Nul);

            for (int i = (int)Property.RECT; i < (int)Property.RECT10; i++)
            {
                PropertyConstraint.AddPropertyConstraint<Func<UISRect>>((Property)i, Rect);
            }
            PropertyConstraint.AddPropertyConstraint<Func<UISRect>>(Property.PADDING, Rect);

            PropertyConstraint.AddPropertyConstraint("m", "move");
            PropertyConstraint.AddPropertyConstraint("s", "scale");
            PropertyConstraint.AddPropertyConstraint("w", "width");
            PropertyConstraint.AddPropertyConstraint("h", "height");
            PropertyConstraint.AddPropertyConstraint("mx", "movex");
            PropertyConstraint.AddPropertyConstraint("my", "movey");
            PropertyConstraint.AddPropertyConstraint("sx", "scalex");
            PropertyConstraint.AddPropertyConstraint("sy", "scaley");
            PropertyConstraint.AddPropertyConstraint("r", "rotate");
            PropertyConstraint.AddPropertyConstraint("f", "fade");
        }

        public static void ReadFile(FileInfo UISFile)
        {
            using (StreamReader sr = UISFile.OpenText())
            {
                string code = sr.ReadToEnd();
                Loader = new UISLoader(code);
            }
        }

        public static void ReadFile(string UISFile)
        {
            using (StreamReader sr = File.OpenText(UISFile))
            {
                string code = sr.ReadToEnd();
                Loader = new UISLoader(code);
            }
        }

        public static void ReadCode(String code)
        {
            Loader = new UISLoader(code);
        }

        private static void Move()
        {
            look = Reader.Read();
            while (look.TokenTag == Tag.SPACE) look = Reader.Read();
        }

        private static void Move_line()
        {
            look = Reader.Read();
            while (Test(Tag.LINE_END)) look = Reader.Read();
        }

        private static void Move_force()
        {
            look = Reader.Read();
        }

        private static void Space()
        {
            while (Test(Tag.SPACE)) look = Reader.Read();
        }

        private static bool Test(params Tag[] tags)
        {
            return tags.Contains(look.TokenTag);
        }


        private static bool Test(params char[] tags)
        {
            return tags.Contains((char)look.TokenTag);
        }

        private static bool TestGrammar(params char[] tags)
        {
            if (tags.Contains((char)look.TokenTag)) return true;
            else { ThrowError(new TokenWrongException(look, (Tag)tags[0])); Move(); return false; }
        }

        private static bool TestGrammar(params Tag[] tags)
        {
            if (tags.Contains(look.TokenTag)) return true;
            else { ThrowError(new TokenWrongException(look, (Tag)tags[0])); Move(); return false; }
        }

        private static bool Expect(params Tag[] tags)
        {
            if (tags.Contains(look.TokenTag))
            {
                Move();
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
                Move();
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
                Move();
                return true;
            }
            else
            {
                ThrowError(new TokenWrongException(look, tags));
                Move();
                return false;
            }
        }

        private static bool ExpectGrammar(params char[] tags)
        {
            if (look != null && tags.Contains((char)look.TokenTag))
            {
                Move();
                return true;
            }
            else
            {
                ThrowError(new TokenWrongException(look, (Tag)tags[0]));
                Move();
                return false;
            }
        }

        private static UISNull ThrowError(UISRuntimeException e)
        {
            instance.AddError(new UISError(e, Reader.RealLine));
            return Nul();
        }

        public static UISInstance ParseInstance()
        {
            Reader = new TokenReader(Loader.TokenList);
            instance = new UISInstance();
            OnParseStart?.Invoke(instance, new EventArgs());
            instance.AddObject(Uis());
            instance.SetAnimationList(Animation_Table);
            OnParseComplete?.Invoke(instance, new EventArgs());
            return instance;
        }

    }
}
