using System;
using System.Collections.Generic;
using UnityEngine;

namespace Toolnity.Collection
{
    public abstract class Collection<T> : ScriptableObject
    {
        public Action ItemAdded;
        public Action ItemRemoved;
        public Action ItemsChanged;
        
        public List<T> items = new();

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
            
            ItemAdded?.Invoke();
            ItemsChanged?.Invoke();
            
            return true;
        }

        public virtual bool Remove(T item)
        {
            if (!items.Contains(item))
            {
                return false;
            }

            items.Remove(item);
            
            ItemRemoved?.Invoke();
            ItemsChanged?.Invoke();
            
            return true;
        }
    }
}