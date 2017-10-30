using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UISEditor.Data
{
    public class UISInstance : IEnumerable<UISObject>
    {
        LinkedList<UISObject> ObjectTree = new LinkedList<UISObject>();

        public T FindObject<T>(Func<UISObject, bool> selector) where T : UISObject
        {
            foreach (var item in ObjectTree)
            {
                if (item is T && selector(item)) return (T)item;
            }
            return null;
        }

        public T FindObject<T>() where T : UISObject
        {
            foreach (var item in ObjectTree)
            {
                if (item is T) return (T)item;
            }
            return null;
        }

        public void AddObject(UISObject obj)
        {
            if (!ObjectTree.Contains(obj)) ObjectTree.AddLast(obj);
        }

        public IEnumerator<UISObject> GetEnumerator() => ObjectTree.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => ObjectTree.GetEnumerator();
    }
}
