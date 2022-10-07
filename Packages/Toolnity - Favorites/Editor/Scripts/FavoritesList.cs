#if UNITY_EDITOR
using System;
using System.Collections;
using System.Collections.Generic;
using Object = UnityEngine.Object;

namespace Toolnity.Favorites
{
    [Serializable]
    public class FavoritesList
    {
        public string Name;
        public List<Object> Objects;

        public FavoritesList(string name = "Favorites")
        {
            Name = name;
            Objects = new List<Object>();
        }

        public void Update()
        {
            Objects.RemoveAll(obj => obj == null);
            Objects.Sort((a, b) => (new CaseInsensitiveComparer()).Compare(a.name, b.name));
        }

        public Object Get(int index)
        {
            if (Objects.Count < index)
            {
                return null;
            }
            
            return Objects[index];
        }

        public bool Contains(Object favoriteObject)
        {
            return Objects.Contains(favoriteObject);
        }

        public void Add(IEnumerable<Object> favoriteObjects)
        {
            Objects.AddRange(favoriteObjects);
        }

        public void Add(Object favoriteObject)
        {
            Objects.Add(favoriteObject);
        }
        
        public void Remove(IEnumerable<Object> favoriteObjects)
        {
            foreach(var iObject in favoriteObjects)
            {
                Objects.Remove(iObject);
            }
        }

        public void Remove(Object favoriteObject)
        {
            Objects.Remove(favoriteObject);
        }

        public void RemoveAt(int favoriteIndex)
        {
            Objects.RemoveAt(favoriteIndex);
        }

        public void Clear()
        {
            Objects.Clear();
        }
    }
}
#endif