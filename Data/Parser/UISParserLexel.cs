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
        private static UISList<UISObject> tempTree;

        private static UISList<UISObject> Uis()
        {
            Move_line();
            tempTree = new UISList<UISObject>();

            var cm = Comments();
            if (cm.Count > 0)
                tempTree.Add(cm);

            tempTree.Add(Cmds());
            tempTree.Add(Elements());

            return tempTree;
        }
        
        private static UISList<UISComment> Comments()
        {
            UISList<UISComment> currentList = new UISList<UISComment>();

            while(Test(Tag.Comment))
            {
                currentList.Add(Comment());
            }
            return currentList;
        }

        private static UISComment Comment()
        {
            var obj = look as Comment;
            Expect(Tag.Comment);
            return new UISComment(obj);
        }

        private static UISList<UISFunctionalElement> Cmds()
        {
            var cm = Comments();
            if(cm.Count > 0)
                tempTree.Add(cm);
            UISList<UISFunctionalElement> currentList = new UISList<UISFunctionalElement>();
            while (Test(Tag.AtProp))
            {
                currentList.Add(Cmd());
            }
            return currentList;
        }

        private static UISFunctionalElement Cmd()
        {
            AtProperty prop = look as AtProperty;
            if (Enum.TryParse(prop.Id, true, out FunctionElementType result) == false) ThrowError(new UISUnsupportFunctionalElemenetException(prop.Lexeme));
            ExpectGrammar(Tag.AtProp);
            ExpectGrammar(Tag.LINE_END);
            return new UISFunctionalElement(result, prop.Value);

        }

        private static UISList<UISObject> Elements()
        {
            var cm = Comments();
            if (cm.Count > 0)
                tempTree.Add(cm);
            UISList<UISObject> currentList = new UISList<UISObject>();
            while (true)
            {
                while (Test(Tag.LINE_END)) Expect(Tag.LINE_END);
                var item = Element();
                if (item != null) currentList.Add(item);
                else break;
            }
            return currentList;
        }

        private static UISObject Element()
        {
            if (Test(Tag.IDENTITY)) return PredefineElement();
            else if (Test(Tag.UserDef)) return CustomElement();
            else if (Test(Tag.Animation)) return AnimationElement();
            else if (Test(Tag.Comment)) return Comment();
            else if (Test(Tag.LINE_END)) return new UISComment(new Comment(Reader.RealLine, ""));
            else if (Test(Tag.Add))
            {
                ExpectGrammar(Tag.Add);
                var group = new UISGroup(Word());
                ExpectGrammar(Tag.LINE_END);
                group.AddAll(Elements());
                return group;
            }
            else return null;
        }

        private static UISPredefineElement PredefineElement()
        {
            UISPredefineElement generator(string v)
            {
                string tag = (look as Word).Lexeme;
                int index = 0;
                // A-B or judge-N
                if (Reader.ReadNext().TokenTag == (Tag.Index))
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
                if (!Enum.TryParse(tag, true, out PredefineElementType type)) ThrowError(new UISUnsupportPerdefineElemenetException(tag));
                return new UISPredefineElement(type) { Index = index };
            }
            return ReadElementT(Tag.IDENTITY, generator, Props);
        }

        private static UISCustomElement CustomElement()
        {
            ExpectGrammar(Tag.UserDef);
            return ReadElementT(Tag.IDENTITY, v => new UISCustomElement(v), Props);
        }


        private static UISAnimationElement AnimationElement()
        {
            ExpectGrammar(Tag.Animation);
            UISAnimationElement e = ReadElementT(Tag.IDENTITY, v => new UISAnimationElement(v, false), AniCollect);
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

        private static UISValue Motion()
        {
            UISAnimationElement result = null;
            if (Test(Tag.IDENTITY))
            {
                UISText ani = Word(typeof(Space));
                result = Animation_Table.FirstOrDefault(p => p.ElementName == ani.Text);
                if (result == null) ThrowError( new UISTargetAnimationNotExistException(ani.Text));                //读取Delay
                if (' ' == (int)look.TokenTag)
                {
                    Move();
                    Word id = look as Word;
                    if (id.Lexeme != "delay") ThrowError(new UISUnsupportPropertyException(id.Lexeme));
                    ExpectGrammar(Tag.IDENTITY);
                    ExpectGrammar(Tag.Equal);
                    TestGrammar(Tag.NUMBER);
                    return new UISMotion(result, Expr() as UISNumber);
                }
                return new UISMotion(result);
            }
            else if (ExpectGrammar(Tag.AnimationInline, Tag.Animation))
            {
                result = new UISAnimationElement($"INLINE_ANIMATION_{TEMP_ANIMATION_FLAG++}", true);
                result.AddProperty(Animation());
                return new UISMotion(result);
            }

            return ThrowError(new UISInlineAnimationException((look as Word).Lexeme));
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
                    if (Test(Tag.LeftBar))
                    {
                        result.IsMultiSelect = true;
                        result.Indexs = Indexs();
                    }
                    else
                    {
                        result.IsMultiSelect = false;
                        result.ElementName += $"-{Word()}";
                    }
                }
                Expect(Tag.LINE_END);
                result.AddAllProperty(readFunc());
                return result;
            }
            ThrowError(new UISUnexpectTag((look as Word).Lexeme));
            return null;
        }

        private static UISList<UISNumber> Indexs()
        {
            ExpectGrammar(Tag.LeftBar);
            UISList<UISNumber> lists = new UISList<UISNumber>();
            do
            {
                lists.Add(Expr() as UISNumber);
            } while (Expect(Tag.Split));
            //[2.1更新]: 支持_sprite-[1-3]形式的写法以简化多个元素同时定义的场景
            if (Expect(Tag.Index))
            {
                UISNumber end = Expr() as UISNumber;
                lists.AddAll(Enumerable
                            .Range((int)lists.First().Number + 1,
                                   (int)end.Number)
                            .Select(p => new UISNumber(p)));
            }
            ExpectGrammar(Tag.RightBar);
            return lists;
        }

        private static UISList<UISProperty> Props()
        {
            UISList<UISProperty> list = new UISList<UISProperty>();

            while (Expect(Tag.TAB))
            {
                var p = Prop();
                if (p == null) break;
                list.Add(p);
                ExpectGrammar(Tag.LINE_END);
            }

            return list;
        }

        private static UISProperty Prop()
        {
            Test(Tag.IDENTITY);
            Word prop = look as Word;
            if (prop == null) return null;

            if (!Enum.TryParse(prop.Lexeme, true, out Property result))
            {
                result = Property.UNSUPPOORT;
                ThrowError(new UISUnsupportPropertyException(prop.Lexeme));
            }

            ExpectGrammar(Tag.IDENTITY);
            ExpectGrammar(Tag.Equal);
            
            CURRENT_PROPERTY = result;
            UISValue val = Value();
            
            if(result == Property.UNSUPPOORT)
            {
                return new UISUnknownNode(prop.Lexeme, val);
            }
            return new UISProperty(result, val);
        }

        private static UISNull Nul()
        {
            return new UISNull();
        }

        private static UISList<UISAnimation> AniCollect()
        {
            UISList<UISAnimation> list = new UISList<UISAnimation>();
            while (Expect(Tag.TAB))
            {
                list.Add(Animation());
                ExpectGrammar(Tag.LINE_END);
            }
            return list;
        }

        private static UISAnimation Animation()
        {
            Word aniName;
            aniName = look as Word;
            ExpectGrammar(Tag.IDENTITY);
            if (aniName.Lexeme != "name")
            {
                ThrowError(new UISAnimationNameMissingException(aniName.Lexeme));
            }
            ExpectGrammar(Tag.Equal);
            aniName = look as Word;
            string animation_name = string.Empty;
            if (aniName.Lexeme.Length <= 2)
            {
                animation_name = PropertyConstraint.GetPropertyConstraint<string>(aniName.Lexeme);
            }
            else
            {
                animation_name = aniName.Lexeme;
            }
            ExpectGrammar(Tag.IDENTITY);

            Func<UISValue> readFunc;
            if (Enum.TryParse(animation_name, true, out AnimationName TargetAnimate))
            {
                readFunc = PropertyConstraint.GetPropertyConstraint<Func<UISValue>>(TargetAnimate);
            }
            else
            {
                readFunc = null;
                ThrowError(new UISUnsupportAnimationException(aniName.Lexeme));
            }

            ExpectGrammar(Tag.Split);

            Word kw;
            UISAnimation ani = new UISAnimation(TargetAnimate);

            do
            {
                kw = look as Word;
                if (kw == null) break;
                ExpectGrammar(Tag.IDENTITY);
                switch (kw.Lexeme.ToLower())
                {
                    case "from":
                        ExpectGrammar(Tag.Equal);
                        ani.AddAnimationProperty(new UISAnimationProperty(AnimationType.FROM, readFunc() as UISLiteralValue));
                        Expect(Tag.Split);
                        break;
                    case "to":
                        ExpectGrammar(Tag.Equal);
                        ani.AddAnimationProperty(new UISAnimationProperty(AnimationType.TO, readFunc() as UISLiteralValue));
                        Expect(Tag.Split);
                        break;
                    case "time":
                        ExpectGrammar(Tag.Equal);
                        ani.AddAnimationProperty(new UISAnimationProperty(AnimationType.TIME, AnimationTime()));
                        Expect(Tag.Split);
                        break;
                    case "atime":
                        ExpectGrammar(Tag.Equal);
                        ani.AddAnimationProperty(new UISAnimationProperty(AnimationType.ATIME, AnimationTime()));
                        Expect(Tag.Split);
                        break;
                    case "repeat":
                        ExpectGrammar(Tag.Equal);
                        ani.AddAnimationProperty(new UISAnimationProperty(AnimationType.REPEAT, AnimationRepeat()));
                        Expect(Tag.Split);
                        break;
                    case "trans":
                        ExpectGrammar(Tag.Equal);
                        ani.AddAnimationProperty(new UISAnimationProperty(AnimationType.TRANS, new UISAnimationCurve(ReadCurve())));
                        Expect(Tag.Split);
                        break;
                    default:
                        ThrowError(new UISUnsupportAnimationControllerException(kw.Lexeme));
                        break;
                }
            } while (!Test(Tag.LINE_END));

            return ani;
        }

        /// <summary>
        /// Read value by constrainted <see cref="Func{UISValue}"/>
        /// </summary>
        /// <returns></returns>
        private static UISValue Value()
        {
            return PropertyConstraint.GetPropertyConstraint<Func<UISValue>>(CURRENT_PROPERTY)();
        }

        /// <summary>
        /// <para>Read a curve e.g: (a, b, c, .... n)</para>
        /// <see cref="ReadCurve"/> -> (<see cref="Expr(bool)"/>....) -> <see cref="UISCurve"/>
        /// </summary>
        /// <returns></returns>
        private static UISCurve ReadCurve()
        {
            //perfab curve
            if (Test(Tag.IDENTITY))
            {
                Word curve = look as Word;
                ExpectGrammar(Tag.IDENTITY);
                if (Enum.TryParse(curve.Lexeme, true, out PerfabCurve perfab))
                {
                    return new UISCurve(perfab);
                }
                else
                {
                    ThrowError(new UISUnknownPerfabCurveException(curve.Lexeme));
                }
            }

            List<UISNumber> lists = new List<UISNumber>();
            ExpectGrammar(Tag.LeftPar);
            lists.Add(Expr() as UISNumber);
            while (Test(Tag.Split))
            {
                ExpectGrammar(Tag.Split);
                lists.Add(Expr() as UISNumber);
            }
            ExpectGrammar(Tag.RightPar);
            return new UISCurve(lists);
        }

        /// <summary>
        /// <para>Read animation repeat like (rA), (A, rB)</para>
        /// <see cref="AnimationRepeat"/> -> <see cref="UISAnimationRepeat"/>
        /// </summary>
        /// <returns></returns>
        private static UISAnimationRepeat AnimationRepeat()
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
                else ThrowError(new UISUnsupportAnimationRepeatException((look as Word).Lexeme));
            }
            else if (Test(Tag.NUMBER))
            {
                repeatCount = Expr() as UISNumber;
            }

            if (allowSplit && Test(Tag.Split))
            {
                ExpectGrammar(Tag.Split);
                repeatTime = Expr() as UISNumber;
            }
            else repeatTime = new UISNumber(0);

            if (allowSplit) ExpectGrammar(Tag.RightPar);
            return new UISAnimationRepeat(repeatCount, repeatTime, isLoop);
        }

        private static UISAnimationTime AnimationTime()
        {
            UISNumber start;
            if (Test(Tag.LeftPar))
            {
                Expect(Tag.LeftPar);
                start = Expr() as UISNumber;
                ExpectGrammar(Tag.Split);
                if (Test(Tag.Add)) ExpectGrammar(Tag.Add);
                else return new UISAnimationTime(start, Expr() as UISNumber);

                UISNumber end = Expr() as UISNumber;
                ExpectGrammar(Tag.RightPar);

                return new UISAnimationTime(start, end, true);
            }
            else
            {
                start = Expr() as UISNumber;
                return new UISAnimationTime(start, new UISNumber(0), false);
            }
        }

        /// <summary>
        /// <para>Read a Relatived POS/SIZE vector</para>
        ///<see cref="UISRelativeVector"/> -> <see cref="Word(Type)"/> <see cref="Lexical.Space"/> <see cref="Vector"/>
        /// </summary>
        /// <returns></returns>
        private static UISValue RelativeVector()
        {
            //Relative with some element
            if (Test(Tag.REAL, Tag.NUMBER, Tag.LeftPar, Tag.Index))
            {
                return Vector();
            }
            else
            {
                UISText lex = Word(typeof(Space));
                ExpectGrammar(Tag.SPACE);
                UISVector vec = Vector();
                UISRelativeVector result = new UISRelativeVector(vec.First, vec.Second, lex.Text, vec.IncludeByPar);
                return result;
            }
        }

        /// <summary>
        /// <para>Read a Vector like (a, b) or a, b</para>
        /// <see cref="Vector"/> -> (<see cref="Term"/> , <see cref="Term"/>)
        /// </summary>
        /// <returns></returns>
        private static UISVector Vector()
        {
            UISLiteralValue first, second;
            bool include = Test(Tag.LeftPar);
            if (include) Expect(Tag.LeftPar);
            first = Term();
            ExpectGrammar(Tag.Split);
            second = Term();
            if (include) ExpectGrammar(Tag.RightPar);
            return new UISVector(first, second, include);
        }

        /// <summary>
        /// <para>Read a Hex 6 length color like #RRGGBB</para>
        /// <see cref="Hexcolor"/> -> <see cref="UISHexColor"/>
        /// <para>color=#ef6666</para>
        /// </summary>
        /// <returns></returns>
        private static UISHexColor Hexcolor()
        {
            TestGrammar(Tag.HEX_STRING);
            HexString hex = look as HexString;
            ExpectGrammar(Tag.HEX_STRING);
            return new UISHexColor(hex.Lexeme);
        }

        /// <summary>
        /// <see cref="Term"/> -> <see cref="Expr"/> +/- <see cref="Expr"/>
        /// </summary>
        /// <returns></returns>
        private static UISLiteralValue Term()
        {
            UISLiteralValue expr1;
            expr1 = Expr();
            switch (look.TokenTag)
            {
                case Tag.Add:
                    ExpectGrammar(Tag.Add);
                    return new UISSimpleExpr(expr1, Term());
                case Tag.Index:
                    ExpectGrammar(Tag.Index);
                    return new UISSimpleExpr(expr1, Term(), false);
                default:
                    return expr1;
            }
        }

        /// <summary>
        /// <para>Read a px, % or a literal real/integer</para>
        /// <see cref="Expr"/> -> <see cref="UISNumber"/> | <see cref="UISPixel"/> | <see cref="UISPercent"/>
        /// <para>number -> number%</para>
        /// <para>px -> number"px"</para>
        /// <para>number</para>
        /// </summary>
        /// <returns></returns>
        private static UISLiteralValue Expr(bool pass = false)
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
                    //if (nagtive != 1) throw new ParseException(look as Word, "Number request");
                    ExpectGrammar(Tag.IDENTITY);

                    return Increase(new UISPixel(value), pass);
                }
            }
            else if (Expect(Tag.Percent))
            {
                //if (nagtive != 1) throw new ParseException(look as Word, "Number request");
                return Increase(new UISPercent(value), pass);
            }

            return Increase(new UISNumber(value), pass);

        }

        /// <summary>
        /// <para>Read a $ increase value</para>
        /// <see cref="Expr(bool)"/> -> <see cref="Increase"/> -> <see cref="UISLiteralValue"/>
        /// </summary>
        /// <param name="src"></param>
        /// <param name="pass"></param>
        /// <returns></returns>
        private static UISLiteralValue Increase(UISLiteralValue src, bool pass = false)
        {
            if (pass) return src;
            if (Expect(Tag.Increase))
            {
                src.IndexIncreasable = true;
                src.IndexIncrease = Expr(true);
            }
            return src;
        }

        /// <summary>
        /// <para>Read a line for single filename</para>
        /// <see cref="Filename"/> -> string.string
        /// <para>tex=my.png</para>
        /// </summary>
        /// <returns>Return a initialized <see cref="UISFileName"/> instance.</returns>
        private static UISFileName Filename()
        {
            string id1 = string.Empty;
            while (!Test(Tag.LINE_END))
            {
                if (look is Word word)
                {
                    Expect(look.TokenTag);
                    id1 += word.Lexeme;
                }
            }

            return new UISFileName(id1);
        }

        /// <summary>
        /// A wrapper for <see cref="Word(Type)"/>
        /// </summary>
        /// <returns></returns>
        private static UISText Word() => Word(null);

        /// <summary>
        /// <para>Read a string[]</para>
        /// <see cref="Word(Type)"/> -> <see cref="UISText"/>
        /// </summary>
        /// <returns></returns>
        private static UISText Word(Type terminal = null)
        {
            string final = string.Empty;
            Word result;
            while (!Test(Tag.LINE_END))
            {
                result = look as Word;
                if (result == null) break;
                if (result.GetType() == terminal) break;
                final += result.Lexeme;
                Move_force();
            }
            return new UISText(final);
        }

        /// <summary>
        /// framefile -> string / number - number
        /// <para>frame=light/0-4</para>
        /// </summary>
        /// <returns>Return initialized <see cref="UISFrameFile"/> instance.</returns>
        private static UISFrameFile Framefile()
        {
            // frame/0-10
            // ID DIV NUMBER INDEX NUMBER
            string id = string.Empty;
            Number n1, n2;

            while (!Test(Tag.FrameRangeDiv))
            {
                if (look is Word word)
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

        /// <summary>
        /// <para>Read a rectangle h,w,x,y</para>
        /// <para><see cref="Rect"/> -> <see cref="UISRect"/></para>
        /// </summary>
        /// <returns></returns>
        private static UISRect Rect()
        {
            UISLiteralValue h = Expr();
            ExpectGrammar(Tag.Split);
            UISLiteralValue w = Expr();
            ExpectGrammar(Tag.Split);
            UISLiteralValue x = Expr();
            ExpectGrammar(Tag.Split);
            UISLiteralValue y = Expr();
            return new UISRect(h, w, x, y);
        }

    }
}
