using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UISEditor.Data;
using UISEditor.Data.Parser;

namespace UISEditor.View
{
    public class UISObjectTree : IEnumerable<UISObject>
    {
        private static UISObjectTree ourInstance = null;
        public static UISObjectTree Instance { get => ourInstance; }
        public static void CreateInstance(string FilePath)
        {
            ourInstance = new UISObjectTree(FilePath);
        }

        public static void UpdateInstanceByCode(string Code)
        {
            UISParser.ReadCode(Code);
            Instance.uisOriginTree = UISParser.ParseInstance();
        }

        public string FilePath { get; private set; }
        public string FileName { get => Path.GetFileName(FilePath); }

        public IEnumerator<UISObject> GetEnumerator() => uisOriginTree.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => uisOriginTree.GetEnumerator();
        public IReadOnlyCollection<UISError> GetErrors() => uisOriginTree.ScriptErrors;
        public IReadOnlyCollection<UISAnimationElement> GetAnimations() => uisOriginTree.AnimationList;

        UISInstance uisOriginTree;

        public UISObjectTree(string FilePath)
        {
            this.FilePath = FilePath;
            UISParser.ReadFile(FilePath);
            uisOriginTree = UISParser.ParseInstance();
        }

    }
}
