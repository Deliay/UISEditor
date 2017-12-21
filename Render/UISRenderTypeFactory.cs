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
        protected UISFileName TEX { get => FindPropertyDefine<UISFileName>(Property.TEX); }
        protected UISVector SIZE { get => FindPropertyDefine<UISVector>(Property.SIZE); }
        protected UISVector POS { get => FindPropertyDefine<UISVector>(Property.POS); }
        protected UISNumber ANCHOR { get => FindPropertyDefine<UISNumber>(Property.ANCHOR); }
        protected UISHexColor COLOR { get => FindPropertyDefine<UISHexColor>(Property.COLOR); }
        protected UISNumber ROTATE { get => FindPropertyDefine<UISNumber>(Property.ROTATE); }
        protected UISNumber OPACITY { get => FindPropertyDefine<UISNumber>(Property.OPACITY); }
        protected UISNumber ZINDEX { get => FindPropertyDefine<UISNumber>(Property.ZINDEX); }

        protected TC FindPropertyDefine<TC>(Property prop) where TC : UISValue => RenderProperty.FindProperty(prop) as TC;

        protected UISElement<T> RenderProperty { get; }
        public FrameworkElement RenderedObject { get; set; } = null;

        public UISRenderableElement(UISElement<T> contianer)
        {
            RenderProperty = contianer;
        }

        public abstract FrameworkElement CreateRenderObject();
        public abstract void Refresh();
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

    public static class UISPropertyApplies
    {
        public static 
    }

    public class UISCustomImageElementFactory : UISCustomRenderable<UISProperty>
    {
        protected UISNumber FLIP { get => FindPropertyDefine<UISNumber>(Property.FLIP); }
        private UISCustomImageElementFactory(UISCustomElement contianer) : base(contianer) {}

        public override FrameworkElement CreateRenderObject()
        {
            if (this.TEX?.FileName?.Length != 0)
            {
                //存在指定纹理
                if (UISObjectTree.Instance.ExistFile(this.TEX.FileName))
                {

                }
                else throw new MissingRenderImageException(this.TEX.FileName);
            }
            else throw new MissingTEXPropertyException(this.RenderProperty.ElementName);
        }

        public override void Refresh()
        {
            throw new NotImplementedException();
        }
    }
   
}
