using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using UISEditor.Data;

namespace UISEditor.Render
{
    public abstract class UISRenderTypeFactory
    {
        public abstract FrameworkElement GenerateDrawable();
    }

    public class UISImageElementFactory : UISRenderTypeFactory
    {
        private UISImageElementFactory()
        {

        }

        public static UISImageElementFactory GenerateByUISPredefineElement(UISPredefineElement element)
        {

        }

        public override FrameworkElement GenerateDrawable()
        {
            
        }
    }
}
