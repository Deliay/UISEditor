﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UISEditor.Data;
using UISEditor.View;

namespace UISEditor.Bridge
{
    public abstract class UISValueConverter<T> : ExpandableObjectConverter where T : UISValue
    {
        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            if(destinationType == typeof(T))
            {
                return true;
            }
            return base.CanConvertTo(context, destinationType);
        }

        public abstract object ConvertTo(T instance, Type target);

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            return ConvertTo(value as T, destinationType);
        }

        public abstract bool CanCovertFrom(Type target);

        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return CanCovertFrom(sourceType);
        }

        public new abstract T ConvertFrom(object value);

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            return ConvertFrom(value);
        }
    }

    public class UISPixelConverter : UISValueConverter<UISPixel>
    {
        public override bool CanCovertFrom(Type target)
        {
            return target == typeof(double);
        }

        public override UISPixel ConvertFrom(object value)
        {
            if (value is double v)
            {
                return new UISPixel(v);
            }
            else return null;
        }

        public override object ConvertTo(UISPixel instance, Type target)
        {
            return instance.Pixel;
        }
    }
    
    public class UISPercentConverter : UISValueConverter<UISPixel>
    {
        public override bool CanCovertFrom(Type target)
        {
            return target == typeof(double);
        }

        public override UISPixel ConvertFrom(object value)
        {
            if (value is double v)
            {
                return new UISPixel(v);
            }
            else return null;
        }

        public override object ConvertTo(UISPixel instance, Type target)
        {
            return instance.Pixel;
        }
    }

    public class UISNumberConverter : UISValueConverter<UISNumber>
    {
        public override bool CanCovertFrom(Type target)
        {
            return target == typeof(string) || target == typeof(double);
        }

        public override UISNumber ConvertFrom(object value)
        {
            if (value is double v)
            {
                return new UISNumber(v);
            }
            else if(double.TryParse(value.ToString(), out double val))
            {
                return new UISNumber(val);
            }
            else return null;
        }

        public override object ConvertTo(UISNumber instance, Type target)
        {
            return instance?.Number ?? 0;
        }
    }

    public class UISAnimationNameConverter : CollectionConverter
    {
        public override bool GetStandardValuesSupported(ITypeDescriptorContext context) => true;

        public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
         => new StandardValuesCollection(UISObjectTree.Instance.GetAnimations()
                                        .ToArray());

        public override bool GetStandardValuesExclusive(ITypeDescriptorContext context)
        {
            return false;
        }

        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return sourceType == typeof(string);
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            if (value is string v)
            {
                return UISObjectTree.Instance.GetAnimations().FirstOrDefault(p => p.ElementName == v);
            }
            else return null;
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if (value is UISAnimationElement v)
            {
                return v.ElementName;
            }
            else return null;
        }
    }
}
