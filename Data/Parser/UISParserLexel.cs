using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UISEditor.Data.Lexical;

namespace UISEditor.Data.Parser
{
    /// <summary>
    /// Token Parser
    /// </summary>
    public static partial class UISParser
    {

        private static Property CURRENT_PROPERTY;
        private static LinkedList<UISAnimationElement> Animation_Table = new LinkedList<UISAnimationElement>();
        private static int TEMP_ANIMATION_FLAG = 0;

        private static UISList<UISObject> uis()
        {
            move_line();
            UISList<UISObject> list = new UISList<UISObject>
            {
                cmds(),
                elements()
            };
            return list;
        }

        private static UISList<UISFunctionalElement> cmds()
        {
            UISList<UISFunctionalElement> currentList = new UISList<UISFunctionalElement>();
            while (Test(Tag.AtProp))
            {
                currentList.Add(cmd());
            }
            return currentList;
        }

        private static UISFunctionalElement cmd()
        {
            AtProperty prop = look as AtProperty;
            if (Enum.TryParse(prop.id, true, out FunctionElementType result) == false) throw new ParseException(prop, typeof(FunctionElementType).ToString());
            ExpectGrammar(Tag.AtProp);
            ExpectGrammar(Tag.LINE_END);
            return new UISFunctionalElement(result, prop.value);

        }

        private static UISList<UISObject> elements()
        {
            UISList<UISObject> currentList = new UISList<UISObject>();
            while(true)
            {
                while (Test(Tag.LINE_END)) Expect(Tag.LINE_END);
                var item = element();
                if (item != null) currentList.Add(item);
                else break;
            }
            return currentList;
        }

        private static UISObject element()
        {
            if (Test(Tag.IDENTITY)) return predefineElement();
            else if (Test(Tag.UserDef)) return customElement();
            else if (Test(Tag.Animation)) return animationElement();
            else return null;
        }

        private static UISPredefineElement predefineElement()
        {
            Func<string, UISPredefineElement> generator = v => {
                string tag = (look as Word).Lexeme;
                int index = 0;
                // A-B or judge-N
                if(Reader.ReadNext().TokenTag == (Tag.Index))
                { 
                    // A-B
                    if (Reader.ReadNext(1).TokenTag == (Tag.IDENTITY))
                    {
                        ExpectGrammar(Tag.IDENTITY);
                        ExpectGrammar(Tag.Index);
                        // reserve a tag for generic reader func
                        tag += $"_{(look as Word).Lexeme}";
                    }
                    else if (Reader.ReadNext(1).TokenTag == Tag.NUMBER)
                    {
                        ExpectGrammar(Tag.IDENTITY);
                        ExpectGrammar(Tag.Index);
                        Number vr = look as Number;
                        ExpectGrammar(Tag.NUMBER);
                        index = vr.Value;
                    }
                    // A-[arr] pass to generic raeder
                }
                if (!Enum.TryParse(tag, true, out PredefineElementType type)) throw new ParseException(look as Word, typeof(PredefineElementType).ToString());
                return new UISPredefineElement(type) { Index = index };
            };
            return ReadElementT(Tag.IDENTITY, generator, props);
        }

        private static UISCustomElement customElement()
        {
            ExpectGrammar(Tag.UserDef);
            return ReadElementT(Tag.IDENTITY, v => new UISCustomElement(v), props);
        }


        private static UISAnimationElement animationElement()
        {
            ExpectGrammar(Tag.Animation);
            UISAnimationElement e = ReadElementT(Tag.IDENTITY, v => new UISAnimationElement(v, false), aniCollect);
            Animation_Table.AddLast(e);
            return e;

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

        private static UISMotion motion()
        {
            UISAnimationElement result = null;
            if (Test(Tag.IDENTITY))
            {
                Word name = look as Word;
                ExpectGrammar(Tag.IDENTITY);
                result = Animation_Table.FirstOrDefault(p => p.ElementName == name.Lexeme);
                if (result == null) throw new ParseException(name, "Animation not define");
                return new UISMotion(result);
            }
            else if (ExpectGrammar(Tag.AnimationInline))
            {
                result = new UISAnimationElement($"INLINE_ANIMATION_{TEMP_ANIMATION_FLAG++}", true);
                result.AddAllProperty(aniCollect());
                return new UISMotion(result);
            }

            throw new ParseException(look as Word, "Unexpection input.");
        }

        /// <summary>
        /// Generic <see cref="UISElement{TR}"/> implement for read:
        /// <para>A <see cref="UISProperty"/> or <see cref="UISAnimationProperty"/> reader implement for <see cref="UISElement{TR}"/></para>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TR"></typeparam>
        /// <param name="expectTag"></param>
        /// <param name="generator"></param>
        /// <param name="readFunc"></param>
        /// <param name="passTag"></param>
        /// <returns></returns>
        private static T ReadElementT<T, TR>
            (Tag expectTag,
            Func<string, T> generator,
            Func<UISList<TR>> readFunc, bool passTag = true)
            where T : UISElement<TR>
            where TR : UISObject
        {
            Func<Tag[], bool> passFunc;
            if (passTag) passFunc = Expect;
            else passFunc = Test;

            if (Test(expectTag))
            {
                Word name = look as Word;
                T result = generator(name.Lexeme);
                Expect(expectTag);
                if (Expect(Tag.Index))
                {
                    result.IsMultiSelect = true;
                    if (Test(Tag.LeftBar))
                    {
                        result.Indexs = indexs();
                    }
                    else
                    {
                        result.Indexs = new UISList<UISNumber>
                        {
                            expr() as UISNumber
                        };
                    }
                }
                Expect(Tag.LINE_END);
                result.AddAllProperty(readFunc());
                return result;
            }
            throw new ParseException(look as Word, typeof(T).ToString());
        }

        private static UISList<UISNumber> indexs()
        {
            ExpectGrammar(Tag.LeftBar);
            UISList<UISNumber> lists = new UISList<UISNumber>();
            do
            {
                lists.Add(expr() as UISNumber);
            } while (Expect(Tag.Split));
            ExpectGrammar(Tag.RightBar);
            return lists;
        }

        private static UISList<UISProperty> props()
        {
            UISList<UISProperty> list = new UISList<UISProperty>();

            while (Expect(Tag.TAB))
            {
                list.Add(prop());
                ExpectGrammar(Tag.LINE_END);
            }

            return list;
        }

        private static UISProperty prop()
        {
            TestGrammar(Tag.IDENTITY);
            Word prop = look as Word;

            if (!Enum.TryParse(prop.Lexeme, true, out Property result))
                throw new ParseException(prop, typeof(Property).ToString());

            ExpectGrammar(Tag.IDENTITY);
            ExpectGrammar(Tag.Equal);

            CURRENT_PROPERTY = result;
            UISValue val = value();

            return new UISProperty(result, val);
        }

        private static UISNull nul()
        {
            return new UISNull();
        }

        private static UISList<UISAnimation> aniCollect()
        {
            UISList<UISAnimation> list = new UISList<UISAnimation>();
            while (Expect(Tag.TAB))
            {
                list.Add(animation());
                ExpectGrammar(Tag.LINE_END);
            }
            return list;
        }

        private static UISAnimation animation()
        {
            Word aniName;
            aniName = look as Word;
            ExpectGrammar(Tag.IDENTITY);
            if (aniName.Lexeme != "name")
            {
                throw new ParseException(aniName, "Animation target name");
            }
            ExpectGrammar(Tag.Equal);
            aniName = look as Word;
            string animation_name = string.Empty;
            if(aniName.Lexeme.Length <= 2)
            {
                animation_name = PropertyConstraint.GetPropertyConstraint<string>(aniName.Lexeme);
            }
            ExpectGrammar(Tag.IDENTITY);

            Func<UISValue> readFunc;
            if (Enum.TryParse(animation_name, true, out AnimationName TargetAnimate))
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
                ExpectGrammar(Tag.IDENTITY);
                switch (kw.Lexeme)
                {
                    case "from":
                        ExpectGrammar(Tag.Equal);
                        ani.AddAnimationProperty(new UISAnimationProperty(AnimationType.FORM, readFunc() as UISLiteralValue));
                        ExpectGrammar(Tag.Split);

                        TestGrammar(Tag.IDENTITY);
                        kw = look as Word;
                        if (kw.Lexeme != "to") throw new ParseException(kw, "to");

                        ExpectGrammar(Tag.IDENTITY);
                        ExpectGrammar(Tag.Equal);

                        ani.AddAnimationProperty(new UISAnimationProperty(AnimationType.TO, readFunc() as UISLiteralValue));
                        Expect(Tag.Split);
                        break;
                    case "time":
                        ExpectGrammar(Tag.Equal);
                        ani.AddAnimationProperty(new UISAnimationProperty(AnimationType.TIME, animationTime()));
                        Expect(Tag.Split);
                        break;
                    case "atime":
                        ExpectGrammar(Tag.Equal);
                        ani.AddAnimationProperty(new UISAnimationProperty(AnimationType.ATIME, animationTime()));
                        Expect(Tag.Split);
                        break;
                    case "repeat":
                        ExpectGrammar(Tag.Equal);
                        ani.AddAnimationProperty(new UISAnimationProperty(AnimationType.REPEAT, animationRepeat()));
                        Expect(Tag.Split);
                        break;
                    case "trans":
                        ExpectGrammar(Tag.Equal);
                        ani.AddAnimationProperty(new UISAnimationProperty(AnimationType.TRANS, new UISAnimationCurve(readCurve())));
                        Expect(Tag.Split);
                        break;
                    default:
                        throw new ParseException(kw, ObjectTag.ANI_PROP_DEF.ToString());
                }
            } while (!Test(Tag.LINE_END));

            return ani;
        }

        private static UISValue value()
        {
            return PropertyConstraint.GetPropertyConstraint<Func<UISValue>>(CURRENT_PROPERTY)();
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
            bool allowSplit = false;
            UISNumber repeatCount = null;
            UISNumber repeatTime = null;
            if (Expect(Tag.LeftPar)) allowSplit = true;
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
            }

            if (allowSplit && Test(Tag.Split))
            {
                ExpectGrammar(Tag.Split);
                repeatTime = expr() as UISNumber;
            }
            else repeatTime = new UISNumber(0);

            if (allowSplit) ExpectGrammar(Tag.RightPar);
            return new UISAnimationRepeat(repeatCount, repeatTime, isLoop);
        }

        private static UISAnimationTime animationTime()
        {
            UISNumber start = expr() as UISNumber;
            if (Test(Tag.LeftPar))
            {
                ExpectGrammar(Tag.Split);
                if (Test(Tag.Add)) ExpectGrammar(Tag.Add);
                else return new UISAnimationTime(start, expr() as UISNumber);

                return new UISAnimationTime(start, expr() as UISNumber, true);
            }
            else
            {
                return new UISAnimationTime(start, new UISNumber(0), false);
            }
        }

        /// <summary>
        /// <see cref="vector"/> -> (<see cref="term"/> , <see cref="term"/>)
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

        /// <summary>
        /// <see cref="hexcolor"/> -> <see cref="UISHexColor"/>
        /// <para>color=#ef6666</para>
        /// </summary>
        /// <returns></returns>
        private static UISHexColor hexcolor()
        {
            TestGrammar(Tag.HEX_STRING);
            HexString hex = look as HexString;
            ExpectGrammar(Tag.HEX_STRING);
            return new UISHexColor(hex.Lexeme);
        }

        /// <summary>
        /// <see cref="term"/> -> <see cref="expr"/> +/- <see cref="expr"/>
        /// </summary>
        /// <returns></returns>
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
        /// <see cref="expr"/> -> <see cref="UISNumber"/> | <see cref="UISPixel"/> | <see cref="UISPercent"/>
        /// <para>number -> number%</para>
        /// <para>px -> number"px"</para>
        /// <para>number</para>
        /// </summary>
        /// <returns></returns>
        private static UISLiteralValue expr(bool pass = false)
        {
            double value = 0;
            int nagtive = 1;
            Number n;
            RealNumber r;
            if (Expect(Tag.Index)) nagtive = -1;

            if (Test(Tag.NUMBER))
            {
                n = look as Number;
                value = n.Value * nagtive;
                ExpectGrammar(Tag.NUMBER);
            }
            else if (TestGrammar(Tag.REAL))
            {
                r = look as RealNumber;
                value = r.Value * nagtive;
                ExpectGrammar(Tag.REAL);
            }


            //px % or other literal value
            if (Test(Tag.IDENTITY))
            {
                Word t = look as Word;
                if (t.Lexeme == "px")
                {
                    if (nagtive != 1) throw new ParseException(look as Word, "Number request");
                    ExpectGrammar(Tag.IDENTITY);
                    
                    return increase(new UISPixel(value), pass);
                }
            }
            else if (Expect(Tag.Percent))
            {
                if (nagtive != 1) throw new ParseException(look as Word, "Number request");
                return increase(new UISPercent(value), pass);
            }

            return increase(new UISNumber(value), pass);

        }

        private static UISLiteralValue increase(UISLiteralValue src, bool pass = false)
        {
            if (pass) return src;
            if (Expect(Tag.Increase))
            {
                src.IndexIncreasable = true;
                src.IndexIncrease = expr(true);
            }
            return src;
        }

        /// <summary>
        /// <see cref="filename"/> -> string.string
        /// <para>tex=my.png</para>
        /// </summary>
        /// <returns>Return a initialized <see cref="UISFileName"/> instance.</returns>
        private static UISFileName filename()
        {
            string id1 = string.Empty;
            while(!Test(Tag.LINE_END))
            {
                if(look is Word word)
                {
                    Expect(look.TokenTag);
                    id1 += word.Lexeme;
                }
            }
            
            return new UISFileName(id1);
        }

        /// <summary>
        /// Read a string[]
        /// </summary>
        /// <returns></returns>
        private static UISText word()
        {
            string final = string.Empty;
            Word result;
            while (!Test(Tag.LINE_END))
            {
                result = look as Word;
                final += result.Lexeme;
                move();
            }
            return new UISText(final);
        }

        /// <summary>
        /// framefile -> string / number - number
        /// frame=light/0-4
        /// </summary>
        /// <returns>Return initialized <see cref="UISFrameFile"/> instance.</returns>
        private static UISFrameFile framefile()
        {
            // frame/0-10
            // ID DIV NUMBER INDEX NUMBER
            string id = string.Empty;
            Number n1, n2;

            while (!Test(Tag.FrameRangeDiv))
            {
                if(look is Word word)
                {
                    Expect(look.TokenTag);
                    id += word.Lexeme;
                }
            }

            ExpectGrammar(Tag.FrameRangeDiv);

            TestGrammar(Tag.NUMBER);
            n1 = look as Number;
            ExpectGrammar(Tag.NUMBER);

            ExpectGrammar(Tag.Index);

            TestGrammar(Tag.NUMBER);
            n2 = look as Number;
            ExpectGrammar(Tag.NUMBER);

            return new UISFrameFile(id, n1.Value, n2.Value);
        }

        private static UISRect rect()
        {
            UISLiteralValue h = expr();
            ExpectGrammar(Tag.Split);
            UISLiteralValue w = expr();
            ExpectGrammar(Tag.Split);
            UISLiteralValue x = expr();
            ExpectGrammar(Tag.Split);
            UISLiteralValue y = expr();
            return new UISRect(h, w, x, y);
        }

    }
}
