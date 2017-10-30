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

namespace UISEditor
{
    /// <summary>
    /// IndexNavgation.xaml 的交互逻辑
    /// </summary>
    public partial class IndexNavgation : Page, IViewSwitch
    {
        public IndexNavgation()
        {
            InitializeComponent();
            this.Version.Content = "Version: NaN Perview";
        }

        public void onSwitch()
        {
        }

        private void FileOpen(object sender, MouseButtonEventArgs e)
        {
            if(e.LeftButton == MouseButtonState.Pressed)
            {
                if(!MainController.CreateUISObjectTreeByFile())
                {
                    MainController.ToIndex();
                }
                else
                {
                    MainController.ToEdit();
                }

            }
        }
    }
}
