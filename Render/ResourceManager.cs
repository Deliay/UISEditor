using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using UISEditor.View;

namespace UISEditor.Render
{
    public class ResourceManager
    {
        public static readonly ResourceManager Instance = new ResourceManager();
        private ResourceManager() { }

        private Dictionary<Guid, object> resourceDict = new Dictionary<Guid, object>();
        private Dictionary<string, Guid> resourceTab = new Dictionary<string, Guid>();
        public Guid LoadResource<T>(string FileName, Func<string, T> generator) where T : class
        {
            string path = Path.Combine(UISObjectTree.Instance.BasePath, FileName);
            if (ExistResource(path)) return GetGUID(path);

            Guid uid = Guid.NewGuid();
            resourceTab.Add(path, uid);
            T obj = generator(FileName);
            resourceDict.Add(uid, obj);
            return uid;
        }

        public (Guid, T) FetchOrLoadResource<T>(string FileName, Func<string, T> generator) where T : class
        {
            if (ExistResource(FileName)) return (GetGUID(FileName), FetchResource<T>(FileName));
            else
            {
                return (LoadResource<T>(FileName, generator), FetchResource<T>(FileName));
            }
        }

        public bool ExistResource(Guid guid) => resourceDict.ContainsKey(guid);

        public bool ExistResource(string FileName) => resourceTab.ContainsKey(FileName);

        public Guid GetGUID(string FileName) => resourceTab[FileName];

        public T FetchResource<T>(Guid guid) where T : class
        {
            return resourceDict[guid] as T;
        }

        public T FetchResource<T>(string FileName) where T : class
        {
            return FetchResource<T>(resourceTab[FileName]);
        }

        public static BitmapImage LoadBitmapResource(string FilePath)
        {
            return Instance.FetchOrLoadResource(Path.Combine(UISObjectTree.Instance.BasePath, FilePath), (fn) => new BitmapImage(new Uri(fn))).Item2;
        }

        public static IEnumerable<BitmapImage> LoadFrameImageResource(string perfix, int start, int end)
        {
            string fixedPerfix = $"{perfix}-";
            string tempFile = Directory.EnumerateFiles(UISObjectTree.Instance.BasePath).ToList().FirstOrDefault(p => Path.GetFileNameWithoutExtension(p).StartsWith(fixedPerfix));
            string ext = tempFile != null ? Path.GetExtension(tempFile) : throw new FileNotFoundException($"{perfix}/{start}-{end}");

            List<BitmapImage> list = new List<BitmapImage>();
            Enumerable.Range(start, end - start + 1).ToList().ForEach(x => list.Add(LoadBitmapResource(Path.Combine(UISObjectTree.Instance.BasePath, $"{fixedPerfix}{x}{ext}"))));
            return list;
        }
    }
}
