using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UISEditor.View;

namespace UISEditor.Controller
{
    public interface IViewSwitch
    {
        void onSwitch();
    }

    public class MainController
    {
        private static MainController ourInstance = null;
        public static MainController Instance { get => ourInstance; }

        public static void Allocate(MainWindow main)
        {
            ourInstance = new MainController(main);
        }

        private MainWindow mainInstance;
        public MainController(MainWindow main)
        {
            mainInstance = main;
        }
        
        public static void ToIndex()
        {
            NavgationTo("View/IndexNavgation.xaml");
        }

        public static void ToEdit()
        {
            NavgationTo("View/FileEditor.xaml");
        }

        public static void NavgationTo(string Uri)
        {

            Instance.mainInstance.Navgation.Source = new Uri(Uri, UriKind.Relative);
            Instance.mainInstance.Navgation.ContentRendered += (sender, evt) =>
            {
                if (Instance.mainInstance.Navgation.Content is IViewSwitch s)
                {
                    s.onSwitch();
                }
            };

        }

        public static void TestOpenfile()
        {
            if (!MainController.CreateUISObjectTreeByFile())
            {
                MainController.ToIndex();
            }
            else
            {
                MainController.ToEdit();
                if (Instance.mainInstance.Navgation.Content is IViewSwitch s)
                {
                    s.onSwitch();
                }
            }
        }

        public static bool CreateUISObjectTreeByFile()
        {
            Microsoft.Win32.OpenFileDialog ofd = new Microsoft.Win32.OpenFileDialog();
            ofd.Filter = "Supported UIS(*.uis;*.mui)|*.uis;*.mui";
            ofd.DefaultExt = ".mui";
            if (ofd.ShowDialog() == true)
            {
                UISObjectTree.CreateInstance(ofd.FileName);
                return true;
            }
            return false;
        }

    }
}
