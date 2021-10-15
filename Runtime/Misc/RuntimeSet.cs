using System.Collections.Generic;
using UnityEngine;

namespace Toolnity
{
    public abstract class RuntimeSet<T> : ScriptableObject
    {
        public List<T> items = new List<T>();

        public void OnEnable()
        {
            items.Clear();
        }

        public void OnDisable()
        {
            items.Clear();
        }
        
        public virtual bool Add(T item)
        {
            if (items.Contains(item))
            {
                return false;
            }

            items.Add(item);
            return true;
        }

        public virtual bool Remove(T item)
        {
            if (!items.Contains(item))
            {
                return false;
            }

            items.Remove(item);
            return true;
        }
    }
}