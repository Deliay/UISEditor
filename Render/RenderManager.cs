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

            }
        }

        public static void Render<T>(UISList<T> list) where T : UISObject
        {

        }
    }

}
