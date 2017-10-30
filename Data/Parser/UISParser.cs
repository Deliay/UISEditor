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
        private static UISLoader Loader;
        private static TokenReader Reader { get; set; }
        private static Token look;
        private static UISInstance instance;

        static UISParser()
        {
            InitConstraint();
        }



        private static void InitConstraint()
        {
            PropertyConstraint.AddPropertyConstraint<Func<UISValue>>(Property.TEX, filename);

            PropertyConstraint.AddPropertyConstraint<Func<UISValue>>(Property.FRAME, framefile);
            for (int i = 19; i < 38; i++)
            {
                PropertyConstraint.AddPropertyConstraint<Func<UISValue>>((Property)i, framefile);
            }
            PropertyConstraint.AddPropertyConstraint<Func<UISValue>>(Property.COLOR, hexcolor);

            PropertyConstraint.AddPropertyConstraint<Func<UISValue>>(Property.SIZE, vector);
            PropertyConstraint.AddPropertyConstraint<Func<UISValue>>(Property.POS, vector);
            PropertyConstraint.AddPropertyConstraint<Func<UISValue>>(Property.SIZE2, vector);
            PropertyConstraint.AddPropertyConstraint<Func<UISValue>>(Property.POS2, vector);
            PropertyConstraint.AddPropertyConstraint<Func<UISValue>>(Property.PART, vector);

            PropertyConstraint.AddPropertyConstraint<Func<UISValue>>(Property.ANCHOR, term);
            PropertyConstraint.AddPropertyConstraint<Func<UISValue>>(Property.ROTATE, term);
            PropertyConstraint.AddPropertyConstraint<Func<UISValue>>(Property.FLIP, term);
            PropertyConstraint.AddPropertyConstraint<Func<UISValue>>(Property.OPACITY, term);
            PropertyConstraint.AddPropertyConstraint<Func<UISValue>>(Property.ZINDEX, term);
            PropertyConstraint.AddPropertyConstraint<Func<UISValue>>(Property.FSIZE, term);
            PropertyConstraint.AddPropertyConstraint<Func<UISValue>>(Property.BLEND, term);
            PropertyConstraint.AddPropertyConstraint<Func<UISValue>>(Property.TYPE, term);
            PropertyConstraint.AddPropertyConstraint<Func<UISValue>>(Property.INTERVAL, term);
            PropertyConstraint.AddPropertyConstraint<Func<UISValue>>(Property.MOTION, motion);
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
            //while (look.TokenTag == Tag.LINE_END) look = Reader.Read();
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

        public static UISInstance ParseInstance()
        {
            Reader = new TokenReader(Loader.TokenList);
            look = Reader.Read();
            instance = new UISInstance();
            instance.AddObject(uis());
            return instance;
        }

    }
}
