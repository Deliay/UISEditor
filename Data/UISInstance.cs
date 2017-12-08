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
        LinkedList<UISError> Errors = new LinkedList<UISError>();
        IReadOnlyCollection<UISAnimationElement> Animations;

        public IReadOnlyCollection<UISError> ScriptErrors { get => Errors; }
        public IReadOnlyCollection<UISAnimationElement> AnimationList { get => Animations; }

        public void AddError(UISError e)
        {
            Errors.AddLast(e);
        }

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

        public void SetAnimationList(IReadOnlyCollection<UISAnimationElement> Animations)
        {
            this.Animations = Animations;
        }

        public IEnumerator<UISObject> GetEnumerator() => ObjectTree.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => ObjectTree.GetEnumerator();
    }
}
