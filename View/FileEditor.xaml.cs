using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Highlighting;
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
using UISEditor.ViewModel;

namespace UISEditor.View
{
    /// <summary>
    /// FileEditor.xaml 的交互逻辑
    /// </summary>
    public partial class FileEditor : Page, IViewSwitch
    {
        public static ICSharpCode.AvalonEdit.TextEditor Editor = null;
        public static Array AllowTag;

        public FileEditor()
        {
            InitializeComponent();
            tvProperty.ToolbarVisible = false;
            PopupContent.ContentRendered += (p, v) => {
                if (PopupContent.Content is IViewSwitch sw)
                {
                    sw.onSwitch();
                }
            };
        }

        public void onSwitch()
        {
            tvUISTree.Items.Clear();
            tvUISTree.Items.Add(PutListToTreeView(new TreeViewItem() { Header = UISObjectTree.Instance.FileName, DisplayMemberPath = UISObjectTree.Instance.FileName }, UISObjectTree.Instance));
            if (tvUISTree.Items.Count >= 2) tvUISTree.Items.RemoveAt(1);
            PutErrorListToTreeView(UISObjectTree.Instance.GetErrors());
            textEditor.Text = string.Join("", UISObjectTree.Instance.Select(p => p.CombineValue()));
            Editor = this.textEditor;
        }

        //public class ItemWrapper
        //{
        //    public string Name { get; private set; }
        //    public UISObject Item { get; private set; }
        //    public ItemWrapper(UISObject item)
        //    {
        //        Item = item;
        //        Name = Item.ObjectTreeName();
        //    }

        //    public override string ToString() => Name;
        //}

        public class PropertyWrapper
        {
            public string Name { get; set; }
            public object Value { get; set; }
        }

        public class ErrorWrapper
        {
            public int Line { get; set; }
            public string Error { get; set; }
            public string Value { get; set; }
        }

        public void PutErrorListToTreeView(IEnumerable<UISError> list)
        {
            tvErrors.Items.Clear();
            foreach (var item in list)
            {
                tvErrors.Items.Add(new ErrorWrapper() { Line = item.ErrorLine , Error = item.InnerException.GetType().ToString(),  Value = item.InnerException.Message });
            }
        }

        public TreeViewItem PutListToTreeView(TreeViewItem parent, IEnumerable<UISObject> list)
        {
            foreach (var item in list)
            {
                switch (item.TokenTag)
                {
                    case ObjectTag.AnimationProeprty:
                    case ObjectTag.AnimationDefine:
                    case ObjectTag.Predefined:
                    case ObjectTag.Custom:
                    case ObjectTag.List:
                        parent.Items.Add(PutListToTreeView(new TreeViewItem() { Header = item, DisplayMemberPath = $"{parent.DisplayMemberPath}/{item.ObjectTreeName()}" }, item as IEnumerable<UISObject>));
                        break;
                    default:
                        parent.Items.Add(new TreeViewItem() { Header = item, DisplayMemberPath = $"{parent.DisplayMemberPath}/{item.ObjectTreeName()}" });
                        break;
                }
            }
            return parent;
        }

        public void LoadProperty(object sender)
        {
            //ItemWrapper wrapper = (sender as ItemWrapper);
            //if (wrapper == null)
            //{
            //    wrapper = ((TreeViewItem)sender)?.Header as ItemWrapper;
            //}
            //if (wrapper == null)
            //{
            //    return;
            //}
            if (sender == null) return;
            UISObject target = (sender as TreeViewItem).Header as UISObject;
            if(target is UISProperty prop) 
            {
                tvProperty.SelectedObject = prop.Value;
            }
            else
            {
                tvProperty.SelectedObject = target;
            }
            //Type t = target.GetType();
            //var list = t.GetProperties();
            //foreach (var item in list)
            //{
            //    object value = item.GetValue(target);
            //    if(value != null)
            //        tvElementProperty.Items.Add(new PropertyWrapper { Name = item.Name, Value = value.ToString() });
            //}
        }

        private void ChangeSelection(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            LoadProperty(e.NewValue);
        }

        private void TreeRightClickElement(object sender, MouseButtonEventArgs e)
        {
            DisplayContextMenu(tvUISTree.SelectedItem);
        }

        private void DisplayContextMenu(object item)
        {
            tvUISTree.ContextMenu.IsOpen = true;
        }

        private void CreateNodeClick(object sender, RoutedEventArgs e)
        {
            if(tvUISTree.SelectedItem is TreeViewItem tvi 
                && tvi.Header is UISObject obj
                && obj is UISList lst)
            {
                Type listType = lst.GetListType();
                if (listType == typeof(UISObject))
                {

                    AllowTag = new ObjectTag[] { ObjectTag.AnimationDefine, ObjectTag.Comment, ObjectTag.Predefined, ObjectTag.Functional, ObjectTag.Custom, ObjectTag.List };
                }
                else if (listType == typeof(UISProperty))
                {
                    AllowTag = Enum.GetValues(typeof(Property));
                }
                else if (listType == typeof(UISAnimation))
                {
                    AllowTag = Enum.GetValues(typeof(AnimationName));
                }
                else if(listType == typeof(UISFunctionalElement))
                {
                    AllowTag = Enum.GetValues(typeof(FunctionElementType));
                }
                else
                {
                    ObjectTag tag = PropertyConstraint.GetPropertyConstraint<ObjectTag>(lst.GetListType());
                    AllowTag = new ObjectTag[] { tag };
                }
                    
                ShowModalDialog(true);
                this.PopupContent.LoadDialog(Dialog.CreateNode);
            }
        }

        private void PropertyValueChange(object s, System.Windows.Forms.PropertyValueChangedEventArgs e)
        {
            textEditor.Text = string.Join("", UISObjectTree.Instance.Select(p => p.CombineValue()));
        }

        private void UpdateDOM()
        {
            string current = string.Empty;
            if (tvUISTree.SelectedItem != null)
            {
                //storage current path value
                current = (tvUISTree.SelectedItem as TreeViewItem).DisplayMemberPath;
            }
            UISObjectTree.UpdateInstanceByCode(textEditor.Text);
            tvUISTree.Items.Clear();
            tvUISTree.Items.Add(PutListToTreeView(new TreeViewItem() { Header = UISObjectTree.Instance.FileName, DisplayMemberPath = UISObjectTree.Instance.FileName }, UISObjectTree.Instance));
            if (tvUISTree.Items.Count >= 2) tvUISTree.Items.RemoveAt(1);
            PutErrorListToTreeView(UISObjectTree.Instance.GetErrors());
            //restore
            RestoreSelectByPath(tvUISTree.Items, current);
            tvProperty.Refresh();
        }

        private void RestoreSelectByPath(ItemCollection root, string path)
        {
            foreach (var item in root)
            {
                if (item is TreeViewItem i)
                {
                    if (i.DisplayMemberPath == path)
                    {
                        var p = i.Parent;
                        do
                        {
                            if(p is TreeViewItem f)
                            {
                                f.IsExpanded = true;
                                p = f.Parent;
                            }
                        } while (p != null);

                        i.IsExpanded = true;
                        i.IsSelected = true;

                        return;
                    }
                    else
                    {
                        if (i.Items.Count > 0) RestoreSelectByPath(i.Items, path);
                    }
                }
            }
        }

        public void ShowModalDialog(bool bShow)
        {
            this.PopupDialog.IsOpen = bShow;
            this.EditorGrid.IsEnabled = !bShow;
        }

        private void ClosePopup(object sender, RoutedEventArgs e)
        {
            ShowModalDialog(false);
        }
        
    }
}
