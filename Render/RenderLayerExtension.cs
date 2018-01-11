using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;

namespace UISEditor.Render
{
    public static class RenderLayerExtension
    {
        public static void AddTransform(this Grid view, Transform transform)
        {
            if (view.RenderTransform is TransformGroup tg) { }
            else
            {
                tg = new TransformGroup();
                view.RenderTransform = tg;
            }

            tg.Children.Add(transform);
        }
    }
}
