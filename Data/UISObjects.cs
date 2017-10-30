using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UISEditor.Data.Lexical;

namespace UISEditor.Data
{
    public static class PropertyConstraint
    {
        static PropertyConstraint()
        {
            AddPropertyConstraint(Property.TEX, typeof(UISFileName));

            AddPropertyConstraint(Property.FRAME, typeof(UISFrameFile));
            for (int i = 19; i < 38; i++)
            {
                AddPropertyConstraint((Property)i, typeof(UISFrameFile));
            }
            AddPropertyConstraint(Property.COLOR, typeof(UISHexColor));

            AddPropertyConstraint(Property.SIZE, typeof(UISVector));
            AddPropertyConstraint(Property.POS, typeof(UISVector));
            AddPropertyConstraint(Property.SIZE2, typeof(UISVector));
            AddPropertyConstraint(Property.POS2, typeof(UISVector));
            AddPropertyConstraint(Property.PART, typeof(UISVector));

            AddPropertyConstraint(Property.ANCHOR, typeof(UISNumber));
            AddPropertyConstraint(Property.ROTATE, typeof(UISNumber));
            AddPropertyConstraint(Property.FLIP, typeof(UISNumber));
            AddPropertyConstraint(Property.OPACITY, typeof(UISNumber));
            AddPropertyConstraint(Property.ZINDEX, typeof(UISNumber));
            AddPropertyConstraint(Property.FSIZE, typeof(UISNumber));
            AddPropertyConstraint(Property.BLEND, typeof(UISNumber));
            AddPropertyConstraint(Property.TYPE, typeof(UISNumber));

            AddPropertyConstraint(Property.TEXT, typeof(UISWord));

            AddPropertyConstraint(AnimationName.MOVE, typeof(UISVector));
            AddPropertyConstraint(AnimationName.SCALE, typeof(UISVector));
            AddPropertyConstraint(AnimationName.SIZE, typeof(UISVector));
            AddPropertyConstraint(AnimationName.SKEW, typeof(UISVector));


            AddPropertyConstraint(AnimationName.WIDTH, typeof(UISNumber));
            AddPropertyConstraint(AnimationName.HEIGHT, typeof(UISNumber));
            AddPropertyConstraint(AnimationName.MOVEX, typeof(UISNumber));
            AddPropertyConstraint(AnimationName.MOVEY, typeof(UISNumber));
            AddPropertyConstraint(AnimationName.SCALEX, typeof(UISNumber));
            AddPropertyConstraint(AnimationName.SCALEY, typeof(UISNumber));
            AddPropertyConstraint(AnimationName.SKEWX, typeof(UISNumber));
            AddPropertyConstraint(AnimationName.SKEWY, typeof(UISNumber));
            AddPropertyConstraint(AnimationName.ROTATE, typeof(UISNumber));
            AddPropertyConstraint(AnimationName.FADE, typeof(UISNumber));

            AddPropertyConstraint(AnimationName.TINY, typeof(UISHexColor));

            AddPropertyConstraint(AnimationName.SHOW, typeof(UISNull));
            AddPropertyConstraint(AnimationName.HIDE, typeof(UISNull));

            AddPropertyConstraint(ObjectTag.ANI_DEF, ":");
            AddPropertyConstraint(ObjectTag.PER_DEF, "");
            AddPropertyConstraint(ObjectTag.USER_DEF, "_");
            AddPropertyConstraint(ObjectTag.ANI_PROP_DEF, "!");
        }

        private static Dictionary<object, LinkedList<object>> Constraint = new Dictionary<object, LinkedList<object>>();

        public static bool AddPropertyConstraint<T>(object prop, T value)
        {
            if (!Constraint.ContainsKey(prop)) Constraint.Add(prop, new LinkedList<object>());
            if (Constraint[prop].Contains(value)) return false;
            else
            {
                Constraint[prop].AddLast(value);
                return true;
            }
        }

        public static T GetPropertyConstraint<T>(object prop) 
        {
            foreach (var item in Constraint[prop])
            {
                if(item is T val)
                {
                    return val;
                }
            }
            return default(T);
        }
    }

    public enum ObjectTag
    {
        FUNC_DEF, PER_DEF, USER_DEF, ANI_DEF, ANI_PROP_DEF,
        PROP, VALUE, IDENTITY, TEXT, _SYS_LIST_
    }

    public enum PredefineElementType
    {
        NOTE, KEY, HIT, PRESS, JUDGE,
        SCORE_COMBO, SCORE_SCORE, SCORE_ACC, SCORE_HP,
        BGA, TOUCH, PROGRESS, HP, MUSICBAR,
        HIT_FAST, HIT_SLOW, PAUSE,
    }

    public enum FunctionElementType
    {
        APPLY, VERSION, APPLY_SOUNDEND,
        ANGLE, INCLUDE, TEXPACK,
    }
    
    public enum Property
    {
        TEX, SIZE, POS, ANCHOR, FRAME, INTERVAL,
        COLOR, ROTATE, FLIP, OPACITY, ZINDEX, FSIZE,
        PART, BLEND, TEXT, TYPE, MOTION,

        POS2, SIZE2,

        FRAME2, FRAME3, FRAME4, FRAME5, FRAME6, FRAME7, FRAME8, FRAME9,
        FRAME10, FRAME11, FRAME12, FRAME13, FRAME14, FRAME15, FRAME16, FRAME17, FRAME18, FRAME19, FRAME20
    }

    public enum ValueType
    {
        VECTOR,
        NUMBER,
        PX,
        PERCENT,
        SIMPLE_EXPR,
        FRAMEFILE,
        FILENAME,
        HEX_COLOR,
        TEXT, NULL, TIME,
        ANIMATE_PROP, CURVE,
        ANIMATE_REPEAT, ANIMATE_TRANS,
        MOTION,
    }

    public enum AnimationName
    {
        MOVE, SCALE, SIZE,
        SKEW, WIDTH, HEIGHT,
        MOVEX, MOVEY, SCALEX, SCALEY,
        SKEWX, SKEWY, ROTATE, FADE, TINY,
        SHOW, HIDE,
    }

    public enum AnimationType
    {
        FORM, TO,
        TIME, ATIME,
        TRANS,
        REPEAT,
    }

    public class UISPixel : UISLiteralValue
    {
        public double Pixel { get; private set; }
        public UISPixel(double px) : base(ValueType.PX)
        {
            Pixel = px;
        }

        public override string CombineValue()
        {
            return $"{Pixel.ToString()}px";
        }
    }

    public class UISPercent : UISLiteralValue
    {
        public double Percent { get; private set; }
        public UISPercent(double percent ) : base(ValueType.PERCENT)
        {
            Percent = percent;
        }

        public override string CombineValue()
        {
            return $"{Percent.ToString()}%";
        }
    }
    
    public class UISNumber : UISLiteralValue
    {
        public double Number { get; private set; }
        public UISNumber(double number) : base(ValueType.NUMBER)
        {
            Number = number;
        }

        public override string CombineValue()
        {
            return $"{Number.ToString()}";
        }
    }

    public class UISCurve : UISLiteralValue
    {
        public List<UISNumber> Points { get; private set; }
        public UISCurve(IEnumerable<UISNumber> points) : base(ValueType.CURVE)
        {
            Points = new List<UISNumber>(points);
        }

        public override string CombineValue()
        {
            return $"({string.Join(",", Points.Select(p => p.CombineValue()))})";
        }
    }

    public class UISText : UISLiteralValue
    {
        public string Text { get; private set; }
        public UISText(string text) : base(ValueType.TEXT)
        {
            Text = text;
        }

        public override string CombineValue()
        {
            return Text;
        }
    }

    public class UISSimpleExpr : UISLiteralValue
    {
        public UISLiteralValue First { get; set; }
        public UISLiteralValue Second { get; set; }
        public bool IsAdd { get; set; }
        public UISSimpleExpr(UISLiteralValue firstOph, UISLiteralValue secondOph, bool isPlus = true) : base(ValueType.SIMPLE_EXPR)
        {
            First = firstOph; Second = secondOph;

        }

        public override string CombineValue()
        {
            string op = IsAdd ? "+" : "-";
            return $"{First.CombineValue()}{op}{Second.CombineValue()}";
        }
    }

    public class UISAnimationTime : UISLiteralValue
    {
        public UISNumber StartTime { get; set; }
        public UISNumber EndTime { get; set; }
        public bool IsAdd { get; set; }
        public UISAnimationTime(UISNumber start, UISNumber end, bool add = false) : base(ValueType.TIME)
        {
            StartTime = start; EndTime = end;
            IsAdd = add;
        }

        public override string CombineValue()
        {
            if (StartTime.Number == 0)
            {
                return EndTime.Number.ToString();
            }
            else
            {
                string perfix = IsAdd ? "+" : string.Empty;
                return $"({StartTime.Number}, {perfix}{EndTime.Number})";

            }
        }
    }

    public class UISAnimationRepeat : UISLiteralValue
    {
        public UISNumber RepeatCount { get; set; }
        public UISNumber RepeatTime { get; set; }
        public bool Repeat { get; set; }
        public UISAnimationRepeat(UISNumber repeatCount, UISNumber repeatTime, bool isLoop) : base(ValueType.ANIMATE_REPEAT)
        {
            this.RepeatCount = repeatCount;
            this.RepeatTime = repeatTime;
            this.Repeat = isLoop;
        }

        public override string CombineValue()
        {
            string loop = Repeat ? "r" : string.Empty;
            string count = RepeatTime.Number > 0 ? $",{RepeatTime}" : string.Empty;

            return $"{loop}{RepeatCount.CombineValue()}{count}";
        }
    }

    public class UISAnimationCurve : UISLiteralValue
    {
        public UISCurve AnimationCurve { get; set; }
        public UISAnimationCurve(UISCurve curve) : base(ValueType.ANIMATE_TRANS)
        {
            AnimationCurve = curve;
        }

        public override string CombineValue()
        {
            return AnimationCurve.CombineValue();
        }
    }

    public abstract class UISLiteralValue : UISValue
    {
        public UISLiteralValue(ValueType type) : base(type)
        {
        }
    }

    public class UISMotion : UISValue
    {
        public UISAnimationElement TargetAnimation { get; set; }
        public UISMotion(UISAnimationElement element) : base(ValueType.MOTION)
        {
            this.TargetAnimation = element;
        }

        public override string CombineValue()
        {
            return TargetAnimation.ElementName;
        }
    }

    public class UISHexColor : UISValue
    {
        public byte Red { get; set; }
        public byte Blue { get; set; }
        public byte Green { get; set; }
        
        public UISHexColor(string value) : base(ValueType.HEX_COLOR) => FormString(value);

        public void FormString(string value)
        {
            Red = byte.Parse(value.Substring(1, 2), System.Globalization.NumberStyles.AllowHexSpecifier);
            Green = byte.Parse(value.Substring(3, 2), System.Globalization.NumberStyles.AllowHexSpecifier);
            Blue = byte.Parse(value.Substring(5, 2), System.Globalization.NumberStyles.AllowHexSpecifier);
        }

        public override string CombineValue() =>  $"#{Red.ToString("{0:X}")}{Green.ToString("{0:X}")}{Blue.ToString("{0:X}")}";
        
    }
    
    public class UISVector : UISValue
    {
        public UISLiteralValue First { get; set; }
        public UISLiteralValue Second { get; set; }
        public bool IncludeByPar { get; set; }
        public UISVector(UISLiteralValue first, UISLiteralValue second, bool havePar) : base(ValueType.VECTOR)
        {
            First = first;
            Second = second;
            IncludeByPar = havePar;
        }

        public override string CombineValue()
        {
            string value = string.Join(",", First.CombineValue(), Second.CombineValue());

            if (IncludeByPar) return $"({value})";
            return value;
        }
    }
    
    public class UISFrameFile : UISValue
    {
        public int StartFrame { get; private set; }
        public int EndFrame { get; private set; }
        public string FileName { get; private set; }
        public UISFrameFile(string filename, int start, int end) : base(ValueType.FRAMEFILE)
        {
            FileName = filename;
            StartFrame = start;
            EndFrame = end;
        }

        public override string CombineValue()
        {
            return $"{FileName}/{StartFrame}-{EndFrame}";
        }
    }
    
    public class UISFileName : UISValue
    {
        public string FileName { get; private set; }
        public UISFileName(string fileName) : base(ValueType.FILENAME)
        {
            FileName = fileName;
        }

        public override string CombineValue()
        {
            return FileName;
        }
    }


    /// <summary>
    /// UIS Animation Property
    /// <para>The animation property</para>
    /// </summary>
    public class UISAnimationProperty : UISValue
    {
        public AnimationType AnimationType { get; private set; }
        public UISLiteralValue Value { get; private set; }
        public UISAnimationProperty(AnimationType prop, UISLiteralValue value) : base(ValueType.ANIMATE_PROP)
        {
            AnimationType = prop;
            Value = value;
        }

        public override string CombineValue()
        {
            return string.Join("=", AnimationType.ToString(), Value.CombineValue());
        }
    }

    public abstract class UISValue : UISObject
    {
        public ValueType ValueType { get; private set; }
        public UISValue(ValueType type) : base(ObjectTag.VALUE)
        {

        }
    }

    /// <summary>
    /// UIS Animation
    /// <para>Manager the animation property of element property</para>
    /// </summary>
    public class UISAnimation : UISObject
    {
        public AnimationName Name { get; set; }
        public LinkedList<UISAnimationProperty> AnimationProperties { get; private set; } = new LinkedList<UISAnimationProperty>();
        public UISAnimation(AnimationName name) : base(ObjectTag.ANI_PROP_DEF)
        {
            this.Name = name;
        }

        public void AddAnimationProperty(UISAnimationProperty prop)
        {
            AnimationProperties.AddLast(prop);
        }

        public override string CombineValue()
        {
            return string.Join(",", $"name={Name.ToString()}", string.Join(",", AnimationProperties.Select(p => p.CombineValue())));
        }
    }

    /// <summary>
    /// UIS Property
    /// <para>The normalize property implement for elements</para>
    /// </summary>
    public class UISProperty : UISObject
    {
        public Property Property { get; private set; }
        public UISValue Value { get; private set; }
        public UISProperty(Property t, UISValue value) : base(ObjectTag.PROP)
        {
            this.Value = value;
            this.Property = t;
        }

        public override string CombineValue()
        {
            return string.Join("=", Property.ToString(), Value.CombineValue());
        }
    }

    /// <summary>
    /// UIS Custom Element
    /// <para>Manager user custom element</para>
    /// </summary>
    public class UISCustomElement : UISElement<UISProperty>
    {
        public UISCustomElement(string ElementName) : base(ObjectTag.USER_DEF, ElementName)
        {
        }

        public override string ElementCombineValue()
        {
            return $"{base.ElementCombineValue()}";
        }
    }

    /// <summary>
    /// UIS JudgePredefineElement
    /// </summary>
    public class UISPredefine_JUDGE_N_Element : UISPredefineElement
    {
        int Value { get; set; }
        public UISPredefine_JUDGE_N_Element(Number value) : base(PredefineElementType.JUDGE)
        {
            this.Value = value.Value;
        }

        public override string ElementCombineValue()
        {
            return base.ElementCombineValue();
        }

    }

    /// <summary>
    /// UIS PreDefined Element 
    /// <para>Manager per-defined element</para>
    /// </summary>
    public class UISPredefineElement : UISElement<UISProperty>
    {
        public PredefineElementType ElemType { get; set; }
        public UISPredefineElement(PredefineElementType t) : base(ObjectTag.PER_DEF, t.ToString().ToLower())
        {
            ElemType = t;
        }

        public override string ElementCombineValue()
        {
            return base.ElementCombineValue();
        }

    }


    /// <summary>
    /// UIS Animation Element
    /// <para><see cref="UISAnimation"/> Manger</para>
    /// <para>For standalone animation define(motion=!animtion), use <see cref="UISAnimation"/> directly </para>
    /// </summary>
    public class UISAnimationElement : UISElement<UISAnimation>
    {
        public bool IsInlineAnimationDef { get; set; }
        public UISAnimationElement(string ElementName, bool isInline, bool isMultiSelect = false) : base(isInline ? ObjectTag.ANI_PROP_DEF : ObjectTag.ANI_DEF, ElementName, isMultiSelect)
        {
            this.IsInlineAnimationDef = isInline;
        }

        public override string ElementCombineValue()
        {
            if(IsInlineAnimationDef)
            {
                return this.ElementCombineValue();
            }
            return $":{base.ElementCombineValue()}";
        }
    }

    /// <summary>
    /// A generic collection for top-level element
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class UISElement<T> : UISObject where T : UISObject
    {

        public ObjectTag ObjectType { get; set; }
        /// <summary>
        /// Target element name
        /// </summary>
        public string ElementName { get; set; }
        /// <summary>
        /// Generic element properties
        /// </summary>
        public UISList<T> Properties { get; private set; } = new UISList<T>();
        /// <summary>
        /// Tag this element is a multi-select element
        /// </summary>
        public bool IsMultiSelect { get; set; }
        /// <summary>
        /// Selected indexs
        /// </summary>
        public UISList<UISNumber> Indexs { get; set; }

        /// <summary>
        /// Base element ctor
        /// </summary>
        /// <param name="tokenTag">Which element</param>
        /// <param name="ElementName">element name</param>
        /// <param name="isMultiSelect">is or not a multiselect element</param>
        public UISElement(ObjectTag tokenTag, string ElementName, bool isMultiSelect = false) : base(tokenTag)
        {
            IsMultiSelect = isMultiSelect;
            ObjectType = tokenTag;
            this.ElementName = ElementName;
            if (isMultiSelect) Indexs = new UISList<UISNumber>();

        }

        /// <summary>
        /// Add property to property container
        /// </summary>
        /// <param name="prop"></param>
        public void AddProperty(T prop)
        {
            Properties.Add(prop);
        }

        public void AddAllProperty(IEnumerable<T> list)
        {
            foreach (var item in list)
            {
                Properties.Add(item);
            }
        }

        /// <summary>
        /// Combine all sub element with ","
        /// </summary>
        /// <returns>Combined result for CombineValue() return</returns>
        public virtual string ElementCombineValue()
        {
            string multi = IsMultiSelect ? $"-[{string.Join(",", Indexs)}]" : string.Empty;
            string line = "\n\t";
            if (ObjectType == ObjectTag.ANI_PROP_DEF)
            {
                line = ",";
            }
            return $"{PropertyConstraint.GetPropertyConstraint<string>(ObjectType)}{ElementName}{multi}\n\t{string.Join(line, Properties.Select(p => p.CombineValue()))}";
        }

        public override string CombineValue()
        {
            return ElementCombineValue();
        }

    }

    //start with @
    /// <summary>
    /// A special class for @function
    /// </summary>
    public class UISFunctionalElement : UISObject
    {
        public string Argument { get; set; }
        public FunctionElementType ElemType { get; set; }

        public UISFunctionalElement(FunctionElementType t, string arg) : base(ObjectTag.FUNC_DEF)
        {
            ElemType = t;
            Argument = arg;
        }

        public override string CombineValue()
        {
            return $"@{ElemType.ToString()} {Argument}";
        }
    }
    
    public class UISWord : UISObject
    {
        public string Literal { get; private set; }
        public UISWord(string literal) : base(ObjectTag.TEXT)
        {
            Literal = literal;
        }

        public override string CombineValue()
        {
            return Literal;
        }
    }

    public class UISNull : UISValue
    {
        public UISNull() : base(ValueType.NULL)
        {
        }

        public override string CombineValue()
        {
            return string.Empty;
        }
    }

    public class UISList<T> : UISObject, IEnumerable<T>, IReadOnlyCollection<T> where T : UISObject
    {
        LinkedList<T> list = new LinkedList<T>();

        public UISList() : base(ObjectTag._SYS_LIST_)
        {

        }

        public void AddAll(IEnumerable<T> lists)
        {
            foreach (var item in lists)
            {
                this.list.AddLast(item);
            }
        }

        public void Add(T item)
        {
            this.list.AddLast(item);
        }

        public LinkedList<T> List { get => list; }

        public int Count => List.Count;

        public override string CombineValue() => string.Join("\n", this.Select(p => p.CombineValue()));

        public IEnumerator<T> GetEnumerator() => List.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => List.GetEnumerator();

    }

    /// <summary>
    /// Base UIS Object for AST Tree
    /// </summary>
    public abstract class UISObject
    {
        public ObjectTag TokenTag { get; private set; }
        public UISObject(ObjectTag tokenTag)
        {
            TokenTag = tokenTag;
        }
        public abstract string CombineValue();
        public override string ToString()
        {
            return CombineValue();
        }
    }
}
