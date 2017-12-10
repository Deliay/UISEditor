using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using UISEditor.Controller;

namespace UISEditor.View
{
    /// <summary>
    /// Interaction logic for CreateNode.xaml
    /// </summary>
    public partial class CreateNode : Page, IViewSwitch
    {
        public CreateNode()
        {
            InitializeComponent();
        }

        public void onSwitch()
        {
            this.EnumObjectTag.Items.Clear();
            foreach (var item in FileEditor.AllowTag)
            {
                this.EnumObjectTag.Items.Add(item);
            }
        }
    }
}
