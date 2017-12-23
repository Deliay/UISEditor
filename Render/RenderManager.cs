using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using UISEditor.Data;
using UISEditor.View;

namespace UISEditor.Render
{
    public static class RenderManager
    {
        public static Canvas RenderLayer;

        private static Dictionary<UISList, UISRenderableElement> renderData = new Dictionary<UISList, UISRenderableElement>();

        public static void Render()
        {
            foreach (var item in UISObjectTree.Instance)
            {
                if(item is UISList)
                {
                    Render(item as UISList<UISObject>);
                }
            }
        }

        public static void Render<T>(IEnumerable<T> list) where T : UISObject
        {
            foreach (var item in list)
            {
                if(item is UISCustomElement e)
                {
                    if(renderData.ContainsKey(e))
                    {
                        renderData[e].RefreshProperties();
                        renderData[e].ApplyProperties();
                    }
                    else
                    {
                        renderData.Add(e, e.ConstriantToRenderableType());
                    }
                }
                else if(item is UISList)
                {
                    Render(item as IEnumerable<UISObject>);
                }
            }
        }
    }

}
