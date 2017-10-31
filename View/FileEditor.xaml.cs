﻿using System;
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

        public class ItemWrapper
        {
            public string Name { get; private set; }
            public UISObject Item { get; private set; }
            public ItemWrapper(UISObject item)
            {
                Item = item;
                Name = Item.ObjectTreeName();
            }

            public override string ToString() => Name;
        }

        public class PropertyWrapper
        {
            public string Name { get; set; }
            public object Value { get; set; }
        }

        public TreeViewItem PutListToTreeView(TreeViewItem parent, IEnumerable<UISObject> list)
        {
            foreach (var item in list)
            {
                switch (item.TokenTag)
                {
                    case ObjectTag.ANI_PROP_DEF:
                    case ObjectTag.ANI_DEF:
                    case ObjectTag.PER_DEF:
                    case ObjectTag.USER_DEF:
                    case ObjectTag._SYS_LIST_:
                        parent.Items.Add(PutListToTreeView(new TreeViewItem() { Header = new ItemWrapper(item) }, item as IEnumerable<UISObject>));
                        break;
                    default:
                        parent.Items.Add(new ItemWrapper(item));
                        break;
                }
            }
            return parent;
        }

        public void LoadProperty(object sender)
        {
            tvElementProperty.Items.Clear();
            ItemWrapper wrapper = (sender as ItemWrapper);
            if (wrapper == null)
            {
                wrapper = ((TreeViewItem)sender)?.Header as ItemWrapper;
            }
            if (wrapper == null)
            {
                return;
            }
                UISObject target = wrapper.Item;
            Type t = target.GetType();
            var list = t.GetProperties();
            foreach (var item in list)
            {
                object value = item.GetValue(target);
                if(value != null)
                    tvElementProperty.Items.Add(new PropertyWrapper { Name = item.Name, Value = value.ToString() });
            }
        }

        private void ChangeSelection(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            LoadProperty(e.NewValue);
        }
    }
}
