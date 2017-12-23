using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;
using UISEditor.Bridge;
using UISEditor.Data.Lexical;
using UISEditor.Render;
using UISEditor.View;

namespace UISEditor.Data
{
    public static partial class PropertyConstraint
    {
        static PropertyConstraint()
        {
            AddPropertyConstraint(Property.TEX, typeof(UISFileName));
            for (int i = (int)Property.TEX2; i < (int)Property.TEX10; i++)
            {
                AddPropertyConstraint((Property)i, typeof(UISFileName));
            }
            AddPropertyConstraint(Property.FRAME, typeof(UISFrameFile));
            for (int i = (int)Property.FRAME2; i < (int)Property.FRAME20; i++)
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
            AddPropertyConstraint(Property.TYPE2, typeof(UISNumber));
            AddPropertyConstraint(Property.INTERVAL, typeof(UISNumber));


            AddPropertyConstraint(Property.MOTION, typeof(UISMotion));
            AddPropertyConstraint(Property.ACTION1, typeof(UISMotion));
            AddPropertyConstraint(Property.ACTION2, typeof(UISMotion));
            AddPropertyConstraint(Property.HOVER, typeof(UISMotion));
            AddPropertyConstraint(Property.LEAVE, typeof(UISMotion));

            AddPropertyConstraint(Property.TEXT, typeof(UISText));
            AddPropertyConstraint(Property.PARENT, typeof(UISText));

            AddPropertyConstraint(Property.TAG, typeof(UISNumber));
            AddPropertyConstraint(Property.ETAG, typeof(UISNumber));

            AddPropertyConstraint(Property.LANG, typeof(UISText));
            AddPropertyConstraint(Property.UNSUPPOORT, typeof(UISUnknownNode));
            AddPropertyConstraint(Property.TIP, typeof(UISText));
            AddPropertyConstraint(Property.SHOW, typeof(UISMotion));

            //AddPropertyConstraint(Property.TEXT, typeof(UISWord));

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

            AddPropertyConstraint(typeof(UISPredefineElement), ObjectTag.Predefined);
            AddPropertyConstraint(typeof(UISFunctionalElement), ObjectTag.Functional);
            AddPropertyConstraint(typeof(UISComment), ObjectTag.Comment);
            AddPropertyConstraint(typeof(UISCustomElement), ObjectTag.Custom);
            AddPropertyConstraint(typeof(UISAnimationElement), ObjectTag.AnimationDefine);

            for (int i = (int)Property.RECT; i < (int)Property.RECT10; i++)
            {
                AddPropertyConstraint((Property)i, typeof(UISRect));
            }
            AddPropertyConstraint(Property.PADDING, typeof(UISRect));

            AddPropertyConstraint(ObjectTag.AnimationDefine, ":");
            AddPropertyConstraint(ObjectTag.Predefined, "");
            AddPropertyConstraint(ObjectTag.Custom, "_");
            AddPropertyConstraint(ObjectTag.AnimationProeprty, "!");

            AddConstraint();
        }

        private static Dictionary<object, LinkedList<object>> Constraint = new Dictionary<object, LinkedList<object>>();

        /// <summary>
        /// <para>!!!WARNING!!! This method will remove all your constriant in type <see cref="<typeparamref name="T"/>"/></para>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="prop"></param>
        /// <param name="value"></param>
        public static void ResetAllPropertyConstraint<T>(object prop, T value)
        {
            if (!Constraint.ContainsKey(prop)) Constraint.Add(prop, new LinkedList<object>());
            if (!Constraint[prop].Contains(value)) return;
            else
            {
                Constraint[prop].Clear();
                Constraint[prop].AddLast(value);
            }
        }

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

        public static bool ExistPropertyConstraint<T>(object prop, T value)
        {
            if (!Constraint.ContainsKey(prop)) return false;
            return Constraint[prop].Contains(value);
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
            return default;
        }
    }

    public enum ObjectTag
    {
        Functional, Predefined, Custom, AnimationDefine, AnimationProeprty,
        Proeprty, Value, Identity, Text, List, Comment
    }

    public enum PredefineElementType
    {
        NOTE, KEY, HIT, PRESS, JUDGE,
        SCORE_COMBO, SCORE_SCORE, SCORE_ACC, SCORE_HP,
        BGA, TOUCH, PROGRESS, HP, MUSICBAR,
        HIT_FAST, HIT_SLOW, PAUSE,

        ICON, DEFTEXT, MODE, SHADOW, TITLE, TRI, SIGN,
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
        FRAME10, FRAME11, FRAME12, FRAME13, FRAME14, FRAME15, FRAME16, FRAME17, FRAME18, FRAME19, FRAME20,
        TYPE2, TEX2, TEX3, TEX4, TEX5, TEX6, TEX7, TEX8, TEX9, TEX10,
        RECT, RECT2, RECT3, RECT4, RECT5, RECT6, RECT7, RECT8, RECT9, RECT10,
        PARENT, TAG, ETAG, LANG,

        ACTION1, ACTION2, 

        UNSUPPOORT, PADDING, HOVER, LEAVE, SHOW,
        [UISDeprecated]
        TIP,
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
        RECT
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
        FROM, TO,
        TIME, ATIME,
        TRANS,
        REPEAT,
    }

    public enum PerfabCurve
    {
        EASEIN,
        EASEOUT,
    }

    [TypeConverter(typeof(UISPixelConverter))]
    public class UISPixel : UISLiteralValue
    {
        public double Pixel { get; set; }
        public UISPixel(double px) : base(ValueType.PX)
        {
            Pixel = px;
        }

        public override string CombineValue()
        {
            return $"{Pixel.ToString()}px";
        }

        public override string ObjectTreeName() => CombineValue();
        
    }

    [TypeConverter(typeof(UISPercentConverter))]
    public class UISPercent : UISLiteralValue
    {
        public double Percent { get; set; }
        public UISPercent(double percent ) : base(ValueType.PERCENT)
        {
            Percent = percent;
        }

        public override string CombineValue()
        {
            return $"{Percent.ToString()}%";
        }
        public override string ObjectTreeName() => CombineValue();
    }

    [TypeConverter(typeof(UISNumberConverter))]
    public class UISNumber : UISLiteralValue
    {
        public double Number { get; set; }
        public UISNumber(double number) : base(ValueType.NUMBER)
        {
            Number = number;
            IndexIncreasable = false;
            IndexIncrease = null;
        }

        public override string CombineValue()
        {
            return $"{Number.ToString()}";
        }
        public override string ObjectTreeName() => CombineValue();

        public static implicit operator double(UISNumber a)
        {
            return a.Number;
        }
    }

    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class UISRect : UISLiteralValue
    {
        [TypeConverter(typeof(ExpandableObjectConverter))]
        public UISLiteralValue Width { get; set; }
        [TypeConverter(typeof(ExpandableObjectConverter))]
        public UISLiteralValue Height { get; set; }
        [TypeConverter(typeof(ExpandableObjectConverter))]
        public UISLiteralValue X { get; set; }
        [TypeConverter(typeof(ExpandableObjectConverter))]
        public UISLiteralValue Y { get; set; }
        public UISRect(UISLiteralValue w, UISLiteralValue h, UISLiteralValue x, UISLiteralValue y) : base(ValueType.RECT)
        {
            Width = w; Height = h; X = x; Y = y;
        }
         
        public override string CombineValue()
        {
            return $"{Width.CombineValue()}, {Height.CombineValue()}, {X.CombineValue()}, {Y.CombineValue()}";
        }
        public override string ObjectTreeName() => CombineValue();
    }

    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class UISCurve : UISLiteralValue
    {
        [TypeConverter(typeof(ExpandableObjectConverter))]
        public List<UISNumber> Points { get; set; }
        public bool IsPerfabCurve { get; set; }
        public PerfabCurve Perfab { get; set; }
        public UISCurve(IEnumerable<UISNumber> points) : base(ValueType.CURVE)
        {
            Points = new List<UISNumber>(points);
        }

        public UISCurve(PerfabCurve perfab) : base(ValueType.CURVE)
        {
            this.Perfab = perfab;
            IsPerfabCurve = true;

            switch (Perfab)
            {
                case PerfabCurve.EASEIN:
                    Points = new List<UISNumber>() { };
                    break;
                case PerfabCurve.EASEOUT:
                    Points = new List<UISNumber>() { };
                    break;
            }
        }

        public override string CombineValue()
        {
            return $"({string.Join(",", Points.Select(p => p.CombineValue()))})";
        }
        public override string ObjectTreeName() => CombineValue();
    }

    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class UISText : UISLiteralValue
    {
        public string Text { get; set; }
        public UISText(string text) : base(ValueType.TEXT)
        {
            Text = text;
        }

        public override string CombineValue()
        {
            return Text;
        }
        public override string ObjectTreeName() => CombineValue();
    }

    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class UISSimpleExpr : UISLiteralValue
    {
        [TypeConverter(typeof(ExpandableObjectConverter))]
        public UISLiteralValue First { get; set; }
        [TypeConverter(typeof(ExpandableObjectConverter))]
        public UISLiteralValue Second { get; set; }
        public bool IsAdd { get; set; }
        public UISSimpleExpr(UISLiteralValue firstOph, UISLiteralValue secondOph, bool isPlus = true) : base(ValueType.SIMPLE_EXPR)
        {
            First = firstOph; Second = secondOph;
            IsAdd = isPlus;
        }

        public override string CombineValue()
        {
            string op = IsAdd ? "+" : "-";
            return $"{First.CombineValue()}{op}{Second.CombineValue()}";
        }
        public override string ObjectTreeName() => CombineValue();
    }

    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class UISAnimationTime : UISLiteralValue
    {
        [TypeConverter(typeof(UISNumberConverter))]
        public UISNumber StartTime { get; set; }
        [TypeConverter(typeof(UISNumberConverter))]
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
                return $"({StartTime.Number},{perfix}{EndTime.Number})";

            }
        }
        public override string ObjectTreeName() => CombineValue();
    }

    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class UISAnimationRepeat : UISLiteralValue
    {
        [TypeConverter(typeof(UISNumberConverter))]
        public UISNumber RepeatCount { get; set; }
        [TypeConverter(typeof(UISNumberConverter))]
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
            string repperfix = RepeatTime > 0 ? "(" : "";
            string repsuffix = RepeatTime > 0 ? ")" : "";
            string count = (RepeatTime.Number > 0 ? $",{RepeatTime}" : string.Empty);

            return $"{loop}{repperfix}{RepeatCount.CombineValue()}{count}{repsuffix}";
        }
        public override string ObjectTreeName() => CombineValue();
    }

    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class UISAnimationCurve : UISLiteralValue
    {
        [TypeConverter(typeof(ExpandableObjectConverter))]
        public UISCurve AnimationCurve { get; set; }
        public UISAnimationCurve(UISCurve curve) : base(ValueType.ANIMATE_TRANS)
        {
            AnimationCurve = curve;
        }

        public override string CombineValue()
        {
            return AnimationCurve.CombineValue();
        }
        public override string ObjectTreeName() => CombineValue();
    }

    public abstract class UISLiteralValue : UISValue
    {
        public bool IndexIncreasable { get; set; }
        [TypeConverter(typeof(ExpandableObjectConverter))]
        public UISLiteralValue IndexIncrease { get; set; }
        public UISLiteralValue(ValueType type) : base(type)
        {
        }
        public override string ObjectTreeName() => CombineValue();
    }

    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class UISMotion : UISValue
    {
        [TypeConverter(typeof(UISAnimationNameConverter))]
        public UISAnimationElement TargetAnimation { get; set; }
        [TypeConverter(typeof(UISNumberConverter))]
        public UISNumber Delay { get; set; }
        public UISMotion(UISAnimationElement element) : base(ValueType.MOTION)
        {
            this.TargetAnimation = element;
        }

        public UISMotion(UISAnimationElement element, UISNumber delay) : base(ValueType.MOTION)
        {
            this.TargetAnimation = element;
            this.Delay = delay;
        }

        public override string CombineValue()
        {
            return TargetAnimation.ElementName;
        }

        public override string ObjectTreeName() => TargetAnimation.ElementName;
    }

    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class UISHexColor : UISValue
    {
        public byte Red { get; set; }
        public byte Blue { get; set; }
        public byte Green { get; set; }

        [TypeConverter(typeof(ColorConverter))]
        public Color MixedColor { get; set; } = Color.FromRgb(0, 0, 0);
        
        public UISHexColor(string value) : base(ValueType.HEX_COLOR) => FormString(value);

        public void FormString(string value)
        {
            Red = byte.Parse(value.Substring(1, 2), System.Globalization.NumberStyles.AllowHexSpecifier);
            Green = byte.Parse(value.Substring(3, 2), System.Globalization.NumberStyles.AllowHexSpecifier);
            Blue = byte.Parse(value.Substring(5, 2), System.Globalization.NumberStyles.AllowHexSpecifier);

            MixedColor = Color.FromRgb(Red, Green, Blue);
        }

        public override string CombineValue() =>  $"#{Red.ToString("x").PadLeft(2, '0')}{Green.ToString("x").PadLeft(2, '0')}{Blue.ToString("x").PadLeft(2, '0')}";
        public override string ObjectTreeName() => CombineValue();
    }

    public class UISRelativeVector : UISVector
    {
        public string Relative { get; set; }
        public UISRelativeVector(UISLiteralValue first, UISLiteralValue second, string relative, bool havePar) : base(first, second, havePar)
        {
            this.Relative = relative;
        }
    }

    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class UISVector : UISValue
    {
        [TypeConverter(typeof(ExpandableObjectConverter))]
        public UISLiteralValue First { get; set; }
        [TypeConverter(typeof(ExpandableObjectConverter))]
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
        public override string ObjectTreeName() => CombineValue();
    }

    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class UISFrameFile : UISValue
    {
        public int StartFrame { get; set; }
        public int EndFrame { get; set; }
        public string FileName { get; set; }
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
        public override string ObjectTreeName() => CombineValue();
    }

    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class UISFileName : UISValue
    {
        public string FileName { get; set; }
        public UISFileName(string fileName) : base(ValueType.FILENAME)
        {
            FileName = fileName;
        }

        public override string CombineValue()
        {
            return FileName;
        }
        public override string ObjectTreeName() => CombineValue();
    }


    /// <summary>
    /// UIS Animation Property
    /// <para>The animation property</para>
    /// </summary>
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class UISAnimationProperty : UISValue
    {
        [Category("Element")]
        public AnimationType AnimationType { get; set; }
        [Category("Element")]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        public UISLiteralValue Value { get; set; }
        public UISAnimationProperty(AnimationType prop, UISLiteralValue value) : base(ValueType.ANIMATE_PROP)
        {
            AnimationType = prop;
            Value = value;
        }

        public override string CombineValue()
        {
            return string.Join("=", AnimationType.ToString(), Value.CombineValue());
        }
        public override string ObjectTreeName() => $"{this.AnimationType.ToString()}";
    }

    public abstract class UISValue : UISObject
    {
        [Category("Element"), ReadOnly(true)]
        public ValueType ValueType { get; set; }
        public UISValue(ValueType type) : base(ObjectTag.Value)
        {
            this.ValueType = type;
        }
        public override string ObjectTreeName() => CombineValue();
        //public abstract void SetValue(object value);
    }

    /// <summary>
    /// UIS Animation
    /// <para>Manager the animation property of element property</para>
    /// </summary>
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class UISAnimation : UISList<UISAnimationProperty>
    {
        [Category("Element")]
        public AnimationName Name { get; set; }
        [Category("Animation")]
        public UISAnimation(AnimationName name) : base()
        {
            base.TokenTag = ObjectTag.AnimationProeprty;
            this.Name = name;
        }

        public void AddAnimationProperty(UISAnimationProperty prop)
        {
            this.Add(prop);
        }

        public override string CombineValue()
        {
            return string.Join(",", $"name={Name.ToString()}", string.Join(",", this.Select(p => p.CombineValue())));
        }
        public override string ObjectTreeName() => $"{Name.ToString()}";
    }

    /// <summary>
    /// UIS Property
    /// <para>The normalize property implement for elements</para>
    /// </summary>
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class UISProperty : UISObject
    {
        [Category("Element")]
        public Property Property { get; set; }
        [Category("Element")]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        public UISValue Value { get; set; }
        public UISProperty(Property t, UISValue value) : base(ObjectTag.Proeprty)
        {
            this.Value = value;
            this.Property = t;
        }

        public override string CombineValue()
        {
            return string.Join("=", Property.ToString(), Value.CombineValue());
        }
        public override string ObjectTreeName() => $"{Property.ToString()}";
    }

    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class UISUnknownNode : UISProperty
    {
        [Category("Element")]
        public string NodeName { get; set; }
        public UISUnknownNode(string name, UISValue value) : base(Property.UNSUPPOORT, value)
        {
            this.Value = value;
            this.NodeName = name;
        }

        public override string CombineValue()
        {
            return string.Join("=", NodeName, Value.CombineValue());
        }
        public override string ObjectTreeName() => $"{NodeName}";
    }

    /// <summary>
    /// UIS Custom Element
    /// <para>Manager user custom element</para>
    /// </summary>
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class UISCustomElement : UISElement<UISProperty>
    {
        public UISCustomElement(string ElementName) : base(ObjectTag.Custom, ElementName)
        {
        }

        public override string ElementCombineValue()
        {
            return $"{base.ElementCombineValue()}";
        }
        public override string ObjectTreeName() => $"{ElementName}";
    }

    /// <summary>
    /// UIS PreDefined Element 
    /// <para>Manager per-defined element</para>
    /// </summary>
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class UISPredefineElement : UISElement<UISProperty>
    {
        [Category("Index")]
        public int Index { get; set; } = 0;
        [Category("Element")]
        public PredefineElementType ElemType { get; set; }
        public UISPredefineElement(PredefineElementType t) : base(ObjectTag.Predefined, t.ToString().ToLower())
        {
            ElemType = t;
        }

        public override string ElementCombineValue()
        {
            return base.ElementCombineValue();
        }
        public override string ObjectTreeName() => $"{ElemType.ToString()}";
    }


    /// <summary>
    /// UIS Animation Element
    /// <para><see cref="UISAnimation"/> Manger</para>
    /// <para>For standalone animation define(motion=!animtion), use <see cref="UISAnimation"/> directly </para>
    /// </summary>
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class UISAnimationElement : UISElement<UISAnimation>
    {
        [Category("Animation")]
        public bool IsInlineAnimationDef { get; set; }
        public UISAnimationElement(string ElementName, bool isInline, bool isMultiSelect = false) : base(isInline ? ObjectTag.AnimationProeprty : ObjectTag.AnimationDefine, ElementName, isMultiSelect)
        {
            this.IsInlineAnimationDef = isInline;
        }

        public override string ElementCombineValue()
        {
            if(IsInlineAnimationDef)
            {
                return this.ElementCombineValue();
            }
            return $"{base.ElementCombineValue()}";
        }

        public override string ObjectTreeName() => $"{ElementName}";
    }

    /// <summary>
    /// A generic collection for top-level element
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public abstract class UISElement<T> : UISList<T> where T : UISObject
    {

        [Category("Element")]
        public ObjectTag ObjectType { get; set; }
        /// <summary>
        /// Target element name
        /// </summary>
        [Category("Element")]
        public string ElementName { get; set; }
        /// <summary>
        /// Tag this element is a multi-select element
        /// </summary>
        [Category("Multi")]
        public bool IsMultiSelect { get; set; }
        /// <summary>
        /// Selected indexs
        /// </summary>
        [Category("Multi")]
        [TypeConverter(typeof(CollectionConverter))]
        public UISList<UISNumber> Indexs { get; set; } = new UISList<UISNumber>();
        
        public IEnumerable<TC> FindType<TC>() where TC : UISObject => this.Select(p => p is TC f ? f : null);

        public TC FindType<TC>(Predicate<TC> pred) where TC : UISObject => FindType<TC>().FirstOrDefault(p => pred(p));

        public UISProperty FindProperty(Property prop) => FindType<UISProperty>(p => p.Property == prop);

        public UISAnimation FindAnimation(AnimationName animation) => FindType<UISAnimation>(p => p is UISAnimation f && f.Name == animation);

        /// <summary>
        /// Base element ctor
        /// </summary>
        /// <param name="tokenTag">Which element</param>
        /// <param name="ElementName">element name</param>
        /// <param name="isMultiSelect">is or not a multiselect element</param>j
        public UISElement(ObjectTag tokenTag, string ElementName, bool isMultiSelect = false)
        {
            this.ObjectType = tokenTag;
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
            this.Add(prop);
        }

        public void AddAllProperty(IEnumerable<T> list)
        {
            foreach (var item in list)
            {
                this.Add(item);
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
            if (ObjectType == ObjectTag.AnimationProeprty)
            {
                line = ",";
            }
            return $"{PropertyConstraint.GetPropertyConstraint<string>(ObjectType)}{ElementName}{multi}\n\t{string.Join(line, this.Select(p => p.CombineValue()))}";
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
        [Category("Element")]
        public string Argument { get; set; }
        [Category("Element")]
        public FunctionElementType ElemType { get; set; }

        public UISFunctionalElement(FunctionElementType t, string arg) : base(ObjectTag.Functional)
        {
            ElemType = t;
            Argument = arg;
        }

        public override string CombineValue()
        {
            return $"@{ElemType.ToString()} {Argument}";
        }
        public override string ObjectTreeName() => $"{ElemType.ToString()}";
    }
    
    //public class UISWord : UISObject
    //{
    //    public string Literal { get; private set; }
    //    public UISWord(string literal) : base(ObjectTag.TEXT)
    //    {
    //        Literal = literal;
    //    }

    //    public override string CombineValue()
    //    {
    //        return Literal;
    //    }
    //}

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

    public class UISGroup : UISList<UISObject>
    {
        public UISText GroupName { get; set; }
        public UISGroup(UISText name)
        {
            this.GroupName = name;
        }

        public override string ObjectTreeName() => $"+{GroupName}";

        public override string CombineValue() => $"+{GroupName}\n{base.CombineValue()}";
    }

    public abstract class UISList : UISObject
    {
        public UISList() : base(ObjectTag.List)
        {

        }

        public abstract Type GetListType();
    }

    public class UISList<T> : UISList, IList<T>, ICollection<T>, IReadOnlyCollection<T> where T : UISObject
    {
        List<T> list = new List<T>();

        public override Type GetListType() => typeof(T);

        public void AddAll(IEnumerable<T> lists)
        {
            foreach (var item in lists)
            {
                this.Add(item);
            }
        }

        public void Add(T item)
        {
            var last = list.LastOrDefault();
            if (last != null)
            {
                item.PrevObject = last;
                last.NextObject = item;
            }
            item.Parent = this;
            this.list.Add(item);
        }

        [Category("Properties")]
        [TypeConverter(typeof(ArrayConverter))]
        public T[] List { get => list.ToArray(); set => list = new List<T>(value); }

        [Category("Properties")]
        public int Count => list.Count;

        [Category("Properties")]
        public bool IsReadOnly => false;

        public T this[int index] { get => list[index]; set => list[index] = value; }

        public override string CombineValue() => string.Join("\n", this.Select(p => p.CombineValue()));

        public IEnumerator<T> GetEnumerator() => list.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => list.GetEnumerator();

        public override string ObjectTreeName() => $"List[{typeof(T).Name}]";

        public void Clear() => list.Clear();

        public bool Contains(T item) => list.Contains(item);

        public void CopyTo(T[] array, int arrayIndex) => list.CopyTo(array, arrayIndex);

        public bool Remove(T item) => list.Remove(item);

        public int IndexOf(T item) => list.IndexOf(item);

        public void Insert(int index, T item) => list.Insert(index, item);

        public void RemoveAt(int index) => list.RemoveAt(index);
    }

    public class UISComment : UISObject
    {
        public string Comment { get; set; }
        public UISComment(Comment value) : base(ObjectTag.Comment)
        {
            Comment = value.Value;
        }

        public override string CombineValue() => $"#{Comment}";

        public override string ObjectTreeName() => "#Comment";
    }

    /// <summary>
    /// Base UIS Object for AST Tree
    /// </summary>
    public abstract class UISObject
    {
        [Category("Element")]
        public ObjectTag TokenTag { get; protected set; }
        public abstract string ObjectTreeName();
        public UISObject PrevObject { get; set; }
        public UISObject NextObject { get; set; }
        public UISObject Parent { get; set; }
        public UISObject(ObjectTag tokenTag)
        {
            TokenTag = tokenTag;
        }
        public abstract string CombineValue();
        public override string ToString()
        {
            return ObjectTreeName();
        }
    }
}
