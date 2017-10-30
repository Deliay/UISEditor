using System;
using System.Collections;
using System.Collections.Generic;
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

        public IEnumerator<UISObject> GetEnumerator() => uisOriginTree.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => uisOriginTree.GetEnumerator();

        UISInstance uisOriginTree;

        public UISObjectTree(string FilePath)
        {
            UISParser.ReadFile(FilePath);
            uisOriginTree = UISParser.ParseInstance();
        }

    }
}
