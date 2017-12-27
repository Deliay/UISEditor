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
using UISEditor.Render;

namespace UISEditor.View
{
    /// <summary>
    /// ResourceViewer.xaml 的交互逻辑
    /// </summary>
    public partial class ResourceViewer : Page, IViewSwitch
    {
        
        public ResourceViewer()
        {
            InitializeComponent();
        }

        public void onSwitch()
        {
            this.ResourceList.ItemsSource = ResourceManager.GetResourcesList().Select(p => System.IO.Path.GetFileName(p));
        }

        private void ResourceList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            this.ImageLayer.Source = ResourceManager.Instance.FetchResource<ImageSource>(this.ResourceList.SelectedItem.ToString(), false);
        }
    }
}
