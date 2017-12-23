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
    public class RenderManager
    {
        public static Canvas RenderLayer;

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
                    PropertyConstraint.ConstriantCustomElementGenerator((e.FindProperty(Property.TYPE).Value as UISNumber).Number)(e);
                }
                else if(item is UISList)
                {
                    Render(item as IEnumerable<UISObject>);
                }
            }
        }
    }

}
