using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using UISEditor.Render;
using UISEditor.View;

namespace UISEditor.Data
{
    public static partial class PropertyConstraint
    {
        public enum PixelDirection
        {
            NoDirection,
            Height,
            Width,
        }

        public static Func<object, Func<PixelDirection, UISLiteralValue, double>> ConstriantPixelConvertor = GetPropertyConstraint<Func<PixelDirection, UISLiteralValue, double>>;

        public static Func<object, Action<Canvas, UISValue>> ConstriantPropertyLoader = GetPropertyConstraint<Action<Canvas, UISValue>>;

        public static Func<object, Func<UISCustomElement, UISCustomRenderable<UISProperty>>> ConstriantCustomElementGenerator = GetPropertyConstraint<Func<UISCustomElement, UISCustomRenderable<UISProperty>>>;

        public static void AddConstraint()
        {
            AddPropertyConstraint<Func<PixelDirection, UISLiteralValue, double>>(ValueType.NUMBER, NumberConvert);
            AddPropertyConstraint<Func<PixelDirection, UISLiteralValue, double>>(ValueType.PX, PixelConvert);
            AddPropertyConstraint<Func<PixelDirection, UISLiteralValue, double>>(ValueType.PERCENT, PercentConvert);
            AddPropertyConstraint<Func<PixelDirection, UISLiteralValue, double>>(ValueType.SIMPLE_EXPR, ExprConvert);

            AddPropertyConstraint<Action<Canvas, UISValue>>(Property.SIZE, SIZE);
            AddPropertyConstraint<Action<Canvas, UISValue>>(Property.POS, POS);
            AddPropertyConstraint<Action<Canvas, UISValue>>(Property.ANCHOR, ANCHOR);
            AddPropertyConstraint<Action<Canvas, UISValue>>(Property.COLOR, COLOR);
            AddPropertyConstraint<Action<Canvas, UISValue>>(Property.ROTATE, ROTATE);
            AddPropertyConstraint<Action<Canvas, UISValue>>(Property.OPACITY, OPACITY);
            AddPropertyConstraint<Action<Canvas, UISValue>>(Property.ZINDEX, ZINDEX);
            AddPropertyConstraint<Action<Canvas, UISValue>>(Property.FLIP, FLIP);
            AddPropertyConstraint<Action<Canvas, UISValue>>(Property.TEX, TEX);
            AddPropertyConstraint<Action<Canvas, UISValue>>(Property.TEXT, TEXT);
            AddPropertyConstraint<Action<Canvas, UISValue>>(Property.FSIZE, FSIZE);

            AddPropertyConstraint<Func<UISCustomElement, UISCustomRenderable<UISProperty>>>(0.0d, (x) => new UISCustomImageElement(x));
            AddPropertyConstraint<Func<UISCustomElement, UISCustomRenderable<UISProperty>>>(1.0d, (x) => new UISCustomTextElement(x));
        }

        private static void FSIZE(Canvas Layer, UISValue prop)
        {
            foreach (var item in Layer.Children)
            {
                if (item is Label a)
                {
                    a.FontSize = Cast<UISNumber>(prop).Number;
                    break;
                }
            }
        }

        private static void TEXT(Canvas Layer, UISValue prop)
        {
            Label text = null;
            foreach (var item in Layer.Children)
            {
                if(item is Label a)
                {
                    text = a;
                    text.Content = Cast<UISText>(prop).Text;
                    break;
                }
            }
            if (text == null)
            {
                text = new Label
                {
                    Content = Cast<UISText>(prop).Text
                };
                Layer.Children.Add(text);
            }
        }

        public static TSrc Cast<TSrc>(UISValue val) where TSrc : UISValue => val as TSrc;

        public static void TEX(Canvas Layer, UISValue prop)
        {
            var tex = Cast<UISFileName>(prop);
            if (tex.FileName?.Length != 0)
            {
                if (UISObjectTree.Instance.ExistFile(tex.FileName))
                {
                    Layer.Background = new ImageBrush(ResourceManager.LoadBitmapResource(tex.FileName));
                }
                else throw new MissingRenderImageException(tex.FileName);
            }
        }

        private static void FLIP(Canvas Layer, UISValue prop)
        {
            if (Layer.RenderTransform == null || Layer.RenderTransform.GetType() != typeof(TransformGroup)) Layer.RenderTransform = new TransformGroup();
            var mode = (int)Cast<UISNumber>(prop).Number;
            TransformGroup props = Layer.RenderTransform as TransformGroup;
            if (mode == 0)
            {
                props.Children.Add(new ScaleTransform(-1, 0));
            }
            else if(mode == 1)
            {
                props.Children.Add(new ScaleTransform(0, -1));
            }
            else
            {
                props.Children.Add(new ScaleTransform(-1, -1));
            }

        }

        public static void SIZE(Canvas Layer, UISValue prop)
        {
            var size = Cast<UISVector>(prop);
            Layer.Height = ConstriantPixelConvertor(size.First.ValueType)(PixelDirection.Height, size.First);
            Layer.Width = ConstriantPixelConvertor(size.Second.ValueType)(PixelDirection.Width, size.Second);
        }

        public static void POS(Canvas Layer, UISValue prop)
        {
            var pos = Cast<UISVector>(prop);
            Layer.Margin = new Thickness(ConstriantPixelConvertor(pos.First.ValueType)(PixelDirection.Width, pos.First),
                                ConstriantPixelConvertor(pos.Second.ValueType)(PixelDirection.Height, pos.Second), 0, 0);
        }

        public static void ANCHOR(Canvas Layer, UISValue prop)
        {
            var anchor = prop as UISNumber;
            switch (anchor.Number)
            {
                case 0:
                    Layer.VerticalAlignment = VerticalAlignment.Top;
                    Layer.HorizontalAlignment = HorizontalAlignment.Left;
                    break;
                case 1:
                    Layer.VerticalAlignment = VerticalAlignment.Top;
                    Layer.HorizontalAlignment = HorizontalAlignment.Center;
                    break;
                case 2:
                    Layer.VerticalAlignment = VerticalAlignment.Top;
                    Layer.HorizontalAlignment = HorizontalAlignment.Right;
                    break;
                case 3:
                    Layer.VerticalAlignment = VerticalAlignment.Center;
                    Layer.HorizontalAlignment = HorizontalAlignment.Left;
                    break;
                case 4:
                    Layer.VerticalAlignment = VerticalAlignment.Center;
                    Layer.HorizontalAlignment = HorizontalAlignment.Center;
                    break;
                case 5:
                    Layer.VerticalAlignment = VerticalAlignment.Center;
                    Layer.HorizontalAlignment = HorizontalAlignment.Right;
                    break;
                case 6:
                    Layer.VerticalAlignment = VerticalAlignment.Bottom;
                    Layer.HorizontalAlignment = HorizontalAlignment.Left;
                    break;
                case 7:
                    Layer.VerticalAlignment = VerticalAlignment.Bottom;
                    Layer.HorizontalAlignment = HorizontalAlignment.Center;
                    break;
                case 8:
                    Layer.VerticalAlignment = VerticalAlignment.Bottom;
                    Layer.HorizontalAlignment = HorizontalAlignment.Right;
                    break;
                default:
                    break;
            }
        }

        public static void COLOR(Canvas Layer, UISValue prop)
        {
            var color = Cast<UISHexColor>(prop);
            if (Layer.Background == null)
            {
                Layer.Background = new SolidColorBrush(color.MixedColor);
            }
        }

        public static void ROTATE(Canvas Layer, UISValue prop)
        {
            if (Layer.RenderTransform == null) Layer.RenderTransform = new TransformGroup();
            (Layer.RenderTransform as TransformGroup).Children.Add(new RotateTransform(Cast<UISNumber>(prop).Number));
        }

        public static void OPACITY(Canvas Layer, UISValue prop) => Layer.Opacity = Cast<UISNumber>(prop).Number;

        public static void ZINDEX(Canvas Layer, UISValue prop) => Panel.SetZIndex(Layer, (int)Cast<UISNumber>(prop).Number);

        public static double NumberConvert(PixelDirection dir, UISLiteralValue num) => (num as UISNumber).Number;

        public static double PixelConvert(PixelDirection dir, UISLiteralValue pixel) => (pixel as UISPixel).Pixel;

        public static double FetchConstriantDirection(PixelDirection dir) => (dir == PixelDirection.Width ? RenderManager.RenderLayer.Width : RenderManager.RenderLayer.Height);

        public static double PercentConvert(PixelDirection dir, UISLiteralValue pct) => FetchConstriantDirection(dir) * ((pct as UISPercent).Percent / 100);

        public static double ExprConvert(PixelDirection dir, UISLiteralValue val)
        {
            UISSimpleExpr expr = val as UISSimpleExpr;
            double val1 = 0, val2 = 0;
            val1 = ConstriantPixelConvertor(expr.First.ValueType)(dir, expr.First);
            if(expr.Second != null)
            {
                val2 = ConstriantPixelConvertor(expr.Second.ValueType)(dir, expr.Second);
            }
            if (expr.IsAdd) return val1 + val2;
            else return val1 - val2;
        }
    }
}
