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

        public static event EventHandler onParseStart;
        public static event EventHandler<UISInstance> onParseComplete;

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
            PropertyConstraint.AddPropertyConstraint<Func<UISValue>>(Property.TEX, filename);
            for (int i = (int)Property.TEX2; i < (int)Property.TEX10; i++)
            {
                PropertyConstraint.AddPropertyConstraint<Func<UISValue>>((Property)i, filename);
            }
            PropertyConstraint.AddPropertyConstraint<Func<UISValue>>(Property.FRAME, framefile);
            for (int i = (int)Property.FRAME2; i < (int)Property.FRAME20; i++)
            {
                PropertyConstraint.AddPropertyConstraint<Func<UISValue>>((Property)i, framefile);
            }
            PropertyConstraint.AddPropertyConstraint<Func<UISValue>>(Property.COLOR, hexcolor);

            PropertyConstraint.AddPropertyConstraint<Func<UISValue>>(Property.SIZE, relativeVector);
            PropertyConstraint.AddPropertyConstraint<Func<UISValue>>(Property.POS, relativeVector);
            PropertyConstraint.AddPropertyConstraint<Func<UISValue>>(Property.SIZE2, relativeVector);
            PropertyConstraint.AddPropertyConstraint<Func<UISValue>>(Property.POS2, relativeVector);
            PropertyConstraint.AddPropertyConstraint<Func<UISValue>>(Property.PART, vector);

            PropertyConstraint.AddPropertyConstraint<Func<UISValue>>(Property.ANCHOR, term);
            PropertyConstraint.AddPropertyConstraint<Func<UISValue>>(Property.ROTATE, term);
            PropertyConstraint.AddPropertyConstraint<Func<UISValue>>(Property.FLIP, term);
            PropertyConstraint.AddPropertyConstraint<Func<UISValue>>(Property.OPACITY, term);
            PropertyConstraint.AddPropertyConstraint<Func<UISValue>>(Property.ZINDEX, term);
            PropertyConstraint.AddPropertyConstraint<Func<UISValue>>(Property.FSIZE, term);
            PropertyConstraint.AddPropertyConstraint<Func<UISValue>>(Property.BLEND, term);
            PropertyConstraint.AddPropertyConstraint<Func<UISValue>>(Property.TYPE, term);
            PropertyConstraint.AddPropertyConstraint<Func<UISValue>>(Property.TYPE2, term);
            PropertyConstraint.AddPropertyConstraint<Func<UISValue>>(Property.INTERVAL, term);
            PropertyConstraint.AddPropertyConstraint<Func<UISValue>>(Property.MOTION, motion);
            PropertyConstraint.AddPropertyConstraint<Func<UISValue>>(Property.ACTION1, motion);
            PropertyConstraint.AddPropertyConstraint<Func<UISValue>>(Property.ACTION2, motion);
            PropertyConstraint.AddPropertyConstraint<Func<UISValue>>(Property.HOVER, motion);
            PropertyConstraint.AddPropertyConstraint<Func<UISValue>>(Property.LEAVE, motion);
            PropertyConstraint.AddPropertyConstraint<Func<UISValue>>(Property.TEXT, word);
            PropertyConstraint.AddPropertyConstraint<Func<UISValue>>(Property.PARENT, word);
            PropertyConstraint.AddPropertyConstraint<Func<UISValue>>(Property.TAG, term);
            PropertyConstraint.AddPropertyConstraint<Func<UISValue>>(Property.ETAG, term);
            PropertyConstraint.AddPropertyConstraint<Func<UISValue>>(Property.LANG, word);
            PropertyConstraint.AddPropertyConstraint<Func<UISValue>>(Property.UNSUPPOORT, word);
            PropertyConstraint.AddPropertyConstraint<Func<UISValue>>(Property.TIP, word);
            PropertyConstraint.AddPropertyConstraint<Func<UISValue>>(Property.SHOW, term);

            PropertyConstraint.AddPropertyConstraint<Func<UISValue>>(AnimationName.MOVE, vector);
            PropertyConstraint.AddPropertyConstraint<Func<UISValue>>(AnimationName.SCALE, vector);
            PropertyConstraint.AddPropertyConstraint<Func<UISValue>>(AnimationName.SIZE, vector);
            PropertyConstraint.AddPropertyConstraint<Func<UISValue>>(AnimationName.SKEW, vector);

            PropertyConstraint.AddPropertyConstraint<Func<UISValue>>(AnimationName.WIDTH, term);
            PropertyConstraint.AddPropertyConstraint<Func<UISValue>>(AnimationName.HEIGHT, term);
            PropertyConstraint.AddPropertyConstraint<Func<UISValue>>(AnimationName.MOVEX, term);
            PropertyConstraint.AddPropertyConstraint<Func<UISValue>>(AnimationName.MOVEY, term);
            PropertyConstraint.AddPropertyConstraint<Func<UISValue>>(AnimationName.SCALEX, term);
            PropertyConstraint.AddPropertyConstraint<Func<UISValue>>(AnimationName.SCALEY, term);
            PropertyConstraint.AddPropertyConstraint<Func<UISValue>>(AnimationName.SKEWX, term);
            PropertyConstraint.AddPropertyConstraint<Func<UISValue>>(AnimationName.SKEWY, term);
            PropertyConstraint.AddPropertyConstraint<Func<UISValue>>(AnimationName.ROTATE, term);
            PropertyConstraint.AddPropertyConstraint<Func<UISValue>>(AnimationName.FADE, term);

            PropertyConstraint.AddPropertyConstraint<Func<UISValue>>(AnimationName.TINY, hexcolor);

            PropertyConstraint.AddPropertyConstraint<Func<UISValue>>(AnimationName.SHOW, nul);
            PropertyConstraint.AddPropertyConstraint<Func<UISValue>>(AnimationName.HIDE, nul);

            for (int i = 48; i < 58; i++)
            {
                PropertyConstraint.AddPropertyConstraint<Func<UISRect>>((Property)i, rect);
            }
            PropertyConstraint.AddPropertyConstraint<Func<UISRect>>(Property.PADDING, rect);

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

        private static void move()
        {
            look = Reader.Read();
            while (look.TokenTag == Tag.SPACE) look = Reader.Read();
        }

        private static void move_line()
        {
            look = Reader.Read();
            while (Test(Tag.LINE_END)) look = Reader.Read();
        }

        private static void move_force()
        {
            look = Reader.Read();
        }

        private static void space()
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
            else { ThrowError(new TokenWrongException(look, (Tag)tags[0])); move(); return false; }
        }

        private static bool TestGrammar(params Tag[] tags)
        {
            if (tags.Contains(look.TokenTag)) return true;
            else { ThrowError(new TokenWrongException(look, (Tag)tags[0])); move(); return false; }
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
                ThrowError(new TokenWrongException(look, tags));
                move();
                return false;
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
                ThrowError(new TokenWrongException(look, (Tag)tags[0]));
                move();
                return false;
            }
        }

        private static UISNull ThrowError(UISRuntimeException e)
        {
            instance.AddError(new UISError(e, Reader.RealLine));
            return nul();
        }

        public static UISInstance ParseInstance()
        {
            Reader = new TokenReader(Loader.TokenList);
            instance = new UISInstance();
            onParseStart?.Invoke(instance, new EventArgs());
            instance.AddObject(uis());
            instance.SetAnimationList(Animation_Table);
            onParseComplete?.Invoke(instance, instance);
            return instance;
        }

    }
}
