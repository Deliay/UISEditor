using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Xml;
using UISEditor.View;

namespace UISEditor.Render
{
    public struct PImage
    {
        public string Name;
        public int Width;
        public int Height;
        public int StartX;
        public int StartY;
        public int OffsetX;
        public int OffsetY;
        public int OriginalWidth;
        public int OriginalHeight;
        public int SourceColorStartX;
        public int SourceColorStartY;
        public bool Rotate;
    }

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

        public bool ExistResource(string FileName, bool IsAbsoulte = true) => resourceTab.ContainsKey(IsAbsoulte ? FileName : Path.Combine(UISObjectTree.Instance.BasePath, FileName));

        public Guid GetGUID(string FileName) => resourceTab[FileName];

        public T FetchResource<T>(Guid guid) where T : class
        {
            return resourceDict[guid] as T;
        }

        public T FetchResource<T>(string FileName, bool IsAbsoulte = true) where T : class
        {
            return FetchResource<T>(resourceTab[IsAbsoulte ? FileName : Path.Combine(UISObjectTree.Instance.BasePath, FileName)]);
        }

        public static BitmapImage LoadBitmapResource(string FilePath)
        {
            return Instance.FetchOrLoadResource(Path.Combine(UISObjectTree.Instance.BasePath, FilePath), (fn) => new BitmapImage(new Uri(fn))).Item2;
        }

        public static IReadOnlyList<BitmapSource> LoadFrameImageResource(string perfix, int start, int end)
        {
            string fixedPerfix = $"{perfix}-";
            string tempFile = Directory.EnumerateFiles(UISObjectTree.Instance.BasePath)
                                       .ToList()
                                       .FirstOrDefault(p => Path.GetFileNameWithoutExtension(p).StartsWith(fixedPerfix));
            if (tempFile != null)
            {
                string ext = Path.GetExtension(tempFile);

                List<BitmapSource> list = new List<BitmapSource>();
                Enumerable.Range(start, end - start + 1)
                            .ToList()
                            .ForEach(x => list.Add(LoadBitmapResource(Path.Combine(UISObjectTree.Instance.BasePath, $"{fixedPerfix}{x}{ext}"))));
                return list;
            }
            else
            {
                tempFile = Instance.resourceTab.Keys.FirstOrDefault(p => Path.GetFileNameWithoutExtension(p).StartsWith(fixedPerfix));
                if (tempFile == null) throw new FileNotFoundException($"{perfix}/{start}-{end}");
                else
                {
                    string ext = Path.GetExtension(tempFile);
                    List<BitmapSource> list = new List<BitmapSource>();
                    Enumerable.Range(start, end - start + 1)
                                .ToList()
                                .ForEach(x => list.Add(Instance.FetchResource<BitmapSource>(Path.Combine(UISObjectTree.Instance.BasePath, $"{fixedPerfix}{x}{ext}"))));
                    return list;
                }
            }
        }

        public void LoadResourceFormPackage(string name)
        {
            XmlDocument xml = new XmlDocument();
            xml.Load(Path.Combine(UISObjectTree.Instance.BasePath, $"{name.Trim()}.plist"));
            XmlNodeList list = xml.SelectNodes("/plist/dict")[0].ChildNodes;

            IEnumerable<PImage> fileList = null;
            string srcImage = null;
             foreach (XmlNode node in list)
            {
                if (node.InnerXml == "frames") fileList = PackageFramesLoader(node.NextSibling.ChildNodes);
                if (node.InnerXml == "metadata") srcImage = PackageTextureName(node.NextSibling.ChildNodes);
            }
            if (srcImage == null) throw new MissingRenderImageException(name);

            BitmapImage bitmap = new BitmapImage(new Uri(Path.Combine(UISObjectTree.Instance.BasePath, srcImage)));

            foreach (var item in fileList)
            {
                string path = Path.Combine(UISObjectTree.Instance.BasePath, item.Name);
                if (ExistResource(path)) continue;    //exist a local image

                Int32Rect cut;
                int srcW, srcH;
                if (item.Rotate)
                {
                    srcW = item.OriginalWidth; srcH = item.OriginalHeight;
                    cut = new Int32Rect(item.StartX, item.StartY, item.Height, item.Width);
                }
                else
                {
                    srcH = item.OriginalWidth; srcW = item.OriginalHeight;
                    cut = new Int32Rect(item.StartX, item.StartY, item.Width, item.Height);
                }
                var stride = bitmap.Format.BitsPerPixel * cut.Width / 8;
                byte[] data = new byte[cut.Height * stride];

                bitmap.CopyPixels(cut, data, stride, 0);

                Guid uid = Guid.NewGuid();
                var src = BitmapSource.Create(cut.Width, cut.Height, bitmap.DpiX, bitmap.DpiY, bitmap.Format, bitmap.Palette, data, stride);
                if (item.Rotate) src = new TransformedBitmap(src, new RotateTransform(270d));
                //src = new TransformedBitmap(src, new ScaleTransform(srcW / src.Width, srcH / src.Height));
                resourceTab.Add(path, uid);
                resourceDict.Add(uid, src);
            }
        }

        public static IEnumerable<string> GetResourcesList() => Instance.resourceTab.Keys;

        private IEnumerable<PImage> PackageFramesLoader(XmlNodeList nodes)
        {
            foreach (XmlNode node in nodes)
            {
                if(node.Name == "key")
                {
                    PImage img = LoadFrameImage(node.NextSibling.ChildNodes);
                    img.Name = node.InnerXml;
                    yield return img;
                }
            }
        }

        private PImage LoadFrameImage(XmlNodeList nodes)
        {
            var img = new PImage();
            Regex matchFrame = new Regex(@"\d+");
            MatchCollection result;
            foreach (XmlNode item in nodes)
            {
                switch (item.InnerXml)
                {
                    case "frame":
                        result = matchFrame.Matches(item.NextSibling.InnerText);
                        img.StartX = int.Parse(result[0].Value);
                        img.StartY = int.Parse(result[1].Value);
                        img.Width = int.Parse(result[2].Value);
                        img.Height = int.Parse(result[3].Value);
                        break;
                    case "offset":
                        result = matchFrame.Matches(item.NextSibling.InnerText);
                        img.OffsetX = int.Parse(result[0].Value);
                        img.OffsetY = int.Parse(result[1].Value);
                        break;
                    case "rotated":
                        if (item.NextSibling.Name == "false") img.Rotate = false;
                        else img.Rotate = true;
                        break;
                    case "sourceColorRect":
                        result = matchFrame.Matches(item.NextSibling.InnerText);
                        img.SourceColorStartX = int.Parse(result[0].Value);
                        img.SourceColorStartY = int.Parse(result[1].Value);
                        break;
                    case "sourceSize":
                        result = matchFrame.Matches(item.NextSibling.InnerText);
                        img.OriginalWidth = int.Parse(result[0].Value); 
                        img.OriginalHeight = int.Parse(result[1].Value);
                        break;
                    default:
                        break;
                }
            }
            return img;
        }

        private string PackageTextureName(XmlNodeList nodes)
        {
            foreach (XmlNode node in nodes)
            {
                if (node.InnerXml == "realTextureFileName" || node.InnerXml == "textureFileName") return node.NextSibling.InnerText;
            }
            return null;
        }
    }
}
