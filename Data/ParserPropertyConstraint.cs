using System.Collections.Generic;
using System.Linq;

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

        public static bool ExistConstraint<T>(object prop, T value)
        {
            return Constraint.ContainsKey(prop);
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
}
