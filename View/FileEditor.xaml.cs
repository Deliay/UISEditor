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
using UISEditor.Data;

namespace UISEditor.View
{
    /// <summary>
    /// FileEditor.xaml 的交互逻辑
    /// </summary>
    public partial class FileEditor : Page, IViewSwitch
    {
        public FileEditor()
        {
            InitializeComponent();
        }

        public void onSwitch()
        {
            tvUISTree.Items.Add(PutListToTreeView(new TreeViewItem() { Header = "root" }, UISObjectTree.Instance));
            if (tvUISTree.Items.Count >= 2) tvUISTree.Items.RemoveAt(1);
            textEditor.Text = string.Join("", UISObjectTree.Instance.Select(p => p.CombineValue()));
        }

        public TreeViewItem PutListToTreeView(TreeViewItem parent, IEnumerable<UISObject> list)
        {
            foreach (var item in list)
            {
                if (item.TokenTag == ObjectTag._SYS_LIST_)
                {
                    PutListToTreeView(parent, item as IEnumerable<UISObject>);
                }
                else
                {
                    parent.Items.Add(item.TokenTag.ToString());
                }
            }
            return parent;
        }
    }
}
