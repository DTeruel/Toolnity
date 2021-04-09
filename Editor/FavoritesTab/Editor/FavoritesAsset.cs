#if UNITY_EDITOR
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Toolnity
{
    public class FavoritesAsset : ScriptableObject
    {
        public int CurrentListIndex;
        public List<FavoritesList> FavoriteLists = new List<FavoritesList>();
        public FavoritesList CurrentList
        {
            get
            {
                if (FavoriteLists.Count == 0)
                {
                    AddList();
                }
                
                return FavoriteLists[CurrentListIndex];
            }
        }

        public void AddList()
        {
            var title = "Favorites List";
            var names = NameList().ToList();
            for (var i = 1; i < 100; i++)
            {
                title = "Favorites " + i;
                if (!names.Contains(title))
                {
                    break;
                }
            }

            FavoriteLists.Add(new FavoritesList(title));
            CurrentListIndex = FavoriteLists.Count - 1;
            EditorUtility.SetDirty(this);
        }

        public void RemoveList(int index)
        {
            if (FavoriteLists.Count > 1)
            {
                FavoriteLists.RemoveAt(index);
                if (CurrentListIndex >= FavoriteLists.Count)
                {
                    CurrentListIndex--;
                }
            }
            EditorUtility.SetDirty(this);
        }

        public string[] NameList()
        {
            var nameList = new string[FavoriteLists.Count];
            for (var i = 0; i < FavoriteLists.Count; i++)
            {
                nameList[i] = FavoriteLists[i].Name;
            }
            return nameList;
        }

        public void AddToCurrentList(IEnumerable<Object> favoriteObjects)
        {
            CurrentList.Add(favoriteObjects);
            EditorUtility.SetDirty(this);
        }

        public void AddToCurrentList(Object favoriteObject)
        {
            CurrentList.Add(favoriteObject);
            EditorUtility.SetDirty(this);
        }

        public void RemoveFromCurrentList(IEnumerable<Object> favoriteObjects)
        {
            CurrentList.Remove(favoriteObjects);
            EditorUtility.SetDirty(this);
        }

        public void RemoveFromCurrentList(Object favoriteObject)
        {
            CurrentList.Remove(favoriteObject);
            EditorUtility.SetDirty(this);
        }

        public void ClearCurrentList()
        {
            CurrentList.Clear();
            EditorUtility.SetDirty(this);
        }

        public void SetCurrentListName(string newName)
        {
            CurrentList.Name = newName;
        }
    }
}
#endif