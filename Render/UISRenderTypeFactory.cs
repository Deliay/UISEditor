using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using UISEditor.Data;
using UISEditor.View;
using static UISEditor.Data.PropertyConstraint;

namespace UISEditor.Render
{
    /*
     * Element property support list
     * ----------------------
     * prop     |       bind      |   support
     * 
     * TEX          UISFilename         ALL
     * SIZE         UISVector           ALL
     * POS          UISVector           ALL
     * ANCHOR       UISNumber           ALL
     * COLOR        UISHexColor         ALL
     * ROTATE       UISNumber           ALL
     * OPACITY      UISNumber           ALL
     * ZINDEX       UISNumber           ALL
     * 
     * FLIP         UISNumber           0 - Image
     * 
     * FSIZE        UISNumber           1 - Text
     * 
     * FRAME        UISFrameFile        3 - Animation 
     * INTERVAL     UISNumber           3 - Animation
     * 
     * PART         UISVector           4 - Stretchable (Scale)
     * 
     * TYPE         UISNumber           All user-custom element
     */



    public abstract class UISRenderableElement<T> where T : UISObject
    {
        protected UISFileName TEX { get; set; }
        protected UISVector SIZE { get; set; }
        protected UISVector POS { get; set; }
        protected UISNumber ANCHOR { get; set; }
        protected UISHexColor COLOR { get; set; }
        protected UISNumber ROTATE { get; set; }
        protected UISNumber OPACITY { get; set; }
        protected UISNumber ZINDEX { get; set; }

        protected TC FindPropertyDefine<TC>(Property prop) where TC : UISValue => RenderProperty.FindProperty(prop) as TC;

        protected UISElement<T> RenderProperty { get; }
        public Canvas RenderedObject { get; } = new Canvas();

        protected void ApplyBaseRenderableProperty()
        {
            //var list = RenderProperty.FindType<UISProperty>();
            //foreach (var item in list)
            //{
            //    ConstriantPropertyLoader(item.Property)(this.RenderedObject, item.Value);
            //}

            if (TEX != null) ConstriantPropertyLoader(Property.TEX)(this.RenderedObject, TEX);
            if (SIZE != null) ConstriantPropertyLoader(Property.SIZE)(this.RenderedObject, SIZE);
            if (POS != null) ConstriantPropertyLoader(Property.POS)(this.RenderedObject, POS);
            if (ANCHOR != null) ConstriantPropertyLoader(Property.ANCHOR)(this.RenderedObject, ANCHOR);
            if (COLOR != null) ConstriantPropertyLoader(Property.COLOR)(this.RenderedObject, COLOR);
            if (ROTATE != null) ConstriantPropertyLoader(Property.ROTATE)(this.RenderedObject, ROTATE);
            if (OPACITY != null) ConstriantPropertyLoader(Property.OPACITY)(this.RenderedObject, OPACITY);
            if (ZINDEX != null) ConstriantPropertyLoader(Property.ZINDEX)(this.RenderedObject, ZINDEX);
        }

        public UISRenderableElement(UISElement<T> contianer)
        {
            RenderProperty = contianer;
            _Refresh();
        }

        protected void RefreshProperties()
        {
            TEX = FindPropertyDefine<UISFileName>(Property.TEX);
            SIZE = FindPropertyDefine<UISVector>(Property.SIZE);
            POS = FindPropertyDefine<UISVector>(Property.POS);
            ANCHOR = FindPropertyDefine<UISNumber>(Property.ANCHOR);
            COLOR = FindPropertyDefine<UISHexColor>(Property.COLOR);
            ROTATE = FindPropertyDefine<UISNumber>(Property.ROTATE);
            OPACITY = FindPropertyDefine<UISNumber>(Property.OPACITY);
            ZINDEX = FindPropertyDefine<UISNumber>(Property.ZINDEX);
        }
        
        public abstract void ApplyProperty();
        public abstract void Refresh();

        private void _Refresh()
        {
            RefreshProperties();
            Refresh();
        }

    }

    /// <summary>
    /// A Renderable object collection
    /// <para>UISObjectTree Create renderable object via <see cref="CreateRenderObject(UISObjectTree)"/></para>
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class UISCustomRenderable<T> : UISRenderableElement<T> where T : UISObject
    {
        public UISCustomRenderable(UISElement<T> contianer) : base(contianer)
        {
        }

        protected UISNumber Type { get => FindPropertyDefine<UISNumber>(Property.TYPE); }

    }

    public class UISCustomImageElementFactory : UISCustomRenderable<UISProperty>
    {
        protected UISNumber FLIP { get; set; }
        private UISCustomImageElementFactory(UISCustomElement contianer) : base(contianer) { Refresh(); }
        
        public override void Refresh()
        {
            FLIP = FindPropertyDefine<UISNumber>(Property.FLIP);
        }

        public override void ApplyProperty()
        {
            if (this.TEX?.FileName?.Length != 0)
            {
                if (FLIP != null) ConstriantPropertyLoader(Property.FLIP)(this.RenderedObject, FLIP);
            }
            else throw new MissingTEXPropertyException(this.RenderProperty.ElementName);
        }
    }
   
}
