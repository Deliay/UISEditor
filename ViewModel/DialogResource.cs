using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using UISEditor.Controller;
using UISEditor.Data;

namespace UISEditor.ViewModel
{

    public enum Dialog
    {
        CreateNode,
        ResourceView,
    }

    public static class DialogResource
    {
        static DialogResource()
        {
            PropertyConstraint.AddPropertyConstraint(Dialog.CreateNode, $"View/Dialog/CreateNode.xaml");
            PropertyConstraint.AddPropertyConstraint(Dialog.ResourceView, $"View/Dialog/ResourceViewer.xaml");
        }

        public static void LoadDialog(this Frame frame, Dialog dialog)
        {
            frame.Source = new Uri(GetRelativeUri(dialog), UriKind.RelativeOrAbsolute);
            if (frame.Content is IViewSwitch sw)
            {
                sw.onSwitch();
            }
        }

        public static string GetRelativeUri(Dialog dialog)
        {
            return $"pack://application:,,,/UISEditor;component/{PropertyConstraint.GetPropertyConstraint<string>(dialog)}";
        }
    }
}
