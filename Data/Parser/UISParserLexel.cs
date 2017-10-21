using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UISEditor.Data
{
    /// <summary>
    /// Token Parser
    /// </summary>
    public static partial class UISParser
    {

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

            if (passFunc(new[] { expectTag }))
            {
                Word name = look as Word;
                T result = generator(name.Lexeme);
                if (Expect(Tag.Index))
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

        private static UISNull nul()
        {
            return new UISNull();
        }

        private static LinkedList<UISAnimation> aniCollect()
        {
            LinkedList<UISAnimation> list = new LinkedList<UISAnimation>();
            while (Expect(Tag.TAB))
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
            if (aniName.Lexeme != "name")
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
            if (Test(Tag.IDENTITY))
            {
                string repeatCountLiteral = (look as Word).Lexeme;
                if (repeatCountLiteral.StartsWith("r"))
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
            else if (Test(Tag.NUMBER))
            {
                repeatCount = expr() as UISNumber;
                ExpectGrammar(Tag.NUMBER);
            }

            if (Test(Tag.Split))
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
            if (Expect(Tag.Add))
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
            if (Test(Tag.NUMBER))
            {
                n = look as Number;
                value = n.Value;
                ExpectGrammar(Tag.NUMBER);
            }
            else if (TestGrammar(Tag.REAL))
            {
                r = look as RealNumber;
                value = r.Value;
                ExpectGrammar(Tag.NUMBER);
            }

            //px % or literal value
            if (Test(Tag.IDENTITY))
            {
                Word t = look as Word;
                if (t.Lexeme == "px")
                {
                    ExpectGrammar(Tag.IDENTITY);
                    return new UISPixel(value);
                }
            }
            else if (Expect(Tag.Percent))
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
