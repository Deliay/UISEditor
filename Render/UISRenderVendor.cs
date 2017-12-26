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

    public abstract class UISRenderableElement
    {
        protected UISFileName TEX { get; set; }
        protected UISVector SIZE { get; set; }
        protected UISVector POS { get; set; }
        protected UISNumber ANCHOR { get; set; }
        protected UISHexColor COLOR { get; set; }
        protected UISNumber ROTATE { get; set; }
        protected UISNumber OPACITY { get; set; }
        protected UISNumber ZINDEX { get; set; }

        protected TC FindPropertyDefine<TC>(Property prop) where TC : UISValue => RenderProperty.FindProperty(prop)?.Value as TC;

        protected UISElement<UISProperty> RenderProperty { get; }
        public Canvas RenderedObject { get; } = new Canvas();

        protected void ApplyBaseRenderableProperty()
        {
            //var list = RenderProperty.FindType<UISProperty>();
            //foreach (var item in list)
            //{
            //    ConstriantPropertyLoader(item.Property)(this.RenderedObject, item.Value);
            //}
            this.RenderedObject.RenderTransform = new TransformGroup();
            if (TEX != null) ConstriantPropertyLoader(Property.TEX)(this.RenderedObject, TEX);
            if (SIZE != null) ConstriantPropertyLoader(Property.SIZE)(this.RenderedObject, SIZE);
            if (POS != null) ConstriantPropertyLoader(Property.POS)(this.RenderedObject, POS);
            if (ANCHOR != null) ConstriantPropertyLoader(Property.ANCHOR)(this.RenderedObject, ANCHOR);
            if (COLOR != null) ConstriantPropertyLoader(Property.COLOR)(this.RenderedObject, COLOR);
            if (ROTATE != null) ConstriantPropertyLoader(Property.ROTATE)(this.RenderedObject, ROTATE);
            if (OPACITY != null) ConstriantPropertyLoader(Property.OPACITY)(this.RenderedObject, OPACITY);
            if (ZINDEX != null) ConstriantPropertyLoader(Property.ZINDEX)(this.RenderedObject, ZINDEX);
        }

        public UISRenderableElement(UISElement<UISProperty> contianer)
        {
            RenderProperty = contianer;
            RefreshProperties();
            ApplyProperties();
            this.RenderedObject.IsEnabled = true;
            this.RenderedObject.Visibility = Visibility.Visible;
            RenderManager.RenderLayer.Children.Add(RenderedObject);
        }

        public void RefreshProperties()
        {
            RefreshBaseProperties();
            Refresh();
        }

        public void ApplyProperties()
        {
            ApplyBaseRenderableProperty();
            ApplyProperty();
        }

        protected void RefreshBaseProperties()
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

        protected abstract void ApplyProperty();
        protected abstract void Refresh();

    }

    /// <summary>
    /// A Renderable object collection
    /// <para>UISObjectTree Create renderable object via <see cref="CreateRenderObject(UISObjectTree)"/></para>
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class UISCustomRenderable : UISRenderableElement
    {
        public UISCustomRenderable(UISElement<UISProperty> contianer) : base(contianer)
        {
        }

        protected UISNumber Type { get => FindPropertyDefine<UISNumber>(Property.TYPE); }

    }

    public abstract class UISPredefineRenderable : UISRenderableElement
    {
        public UISPredefineRenderable(UISElement<UISProperty> contianer) : base(contianer)
        {
        }
    }

    public class UISCustomImageElement : UISCustomRenderable
    {
        protected UISNumber FLIP { get; set; }
        protected UISNumber BLEND { get; set; }
        public UISCustomImageElement(UISCustomElement contianer) : base(contianer) { }

        protected override void Refresh()
        {
            FLIP = FindPropertyDefine<UISNumber>(Property.FLIP);
            BLEND = FindPropertyDefine<UISNumber>(Property.BLEND);

        }

        protected override void ApplyProperty()
        {
            if (this.TEX?.FileName?.Length != 0)
            {
                FLIP?.ApplyTo(RenderedObject, Property.FLIP);
            }
            else throw new MissingTEXPropertyException(this.RenderProperty.ElementName);

            BLEND?.ApplyTo(RenderedObject, Property.BLEND);
        }
    }

    public class UISCustomTextElement : UISCustomRenderable
    {
        public UISCustomTextElement(UISElement<UISProperty> contianer) : base(contianer)
        {
        }

        protected UISNumber FSIZE { get; set; }
        protected UISText TEXT { get; set; }

        protected override void ApplyProperty()
        {
            TEXT?.ApplyTo(RenderedObject, Property.TEXT);
            FSIZE?.ApplyTo(RenderedObject, Property.FSIZE);
        }

        protected override void Refresh()
        {
            FSIZE = FindPropertyDefine<UISNumber>(Property.FSIZE);
            TEXT = FindPropertyDefine<UISText>(Property.TEXT);
        }
    }

    public class UISCustomSoildColorElement : UISCustomRenderable
    {
        public UISCustomSoildColorElement(UISElement<UISProperty> contianer) : base(contianer)
        {
        }

        protected override void ApplyProperty()
        {
        }

        protected override void Refresh()
        {
        }
    }

    public class UISCustomAnimationElement : UISCustomRenderable
    {
        protected UISFrameFile FRAME { get; set; }
        protected UISNumber INTERVAL { get; set; }
        protected UISNumber BLEND { get; set; }

        public UISCustomAnimationElement(UISElement<UISProperty> contianer) : base(contianer)
        {
        }

        protected override void ApplyProperty()
        {
            FRAME?.ApplyTo(RenderedObject, Property.FRAME);
            INTERVAL?.ApplyTo(RenderedObject, Property.INTERVAL);
            BLEND?.ApplyTo(RenderedObject, Property.BLEND);
        }

        protected override void Refresh()
        {
            FRAME = FindPropertyDefine<UISFrameFile>(Property.FRAME);
            INTERVAL = FindPropertyDefine<UISNumber>(Property.INTERVAL);
            BLEND = FindPropertyDefine<UISNumber>(Property.BLEND);
        }
    }

    public class UISCustomScale3Element : UISCustomRenderable
    {
        public UISCustomScale3Element(UISElement<UISProperty> contianer) : base(contianer)
        {
        }

        protected override void ApplyProperty()
        {
            throw new NotImplementedException();
        }

        protected override void Refresh()
        {
            throw new NotImplementedException();
        }
    }

}
