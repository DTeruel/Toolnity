#if UNITY_EDITOR
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Toolnity
{
    public class FavoritesAsset : ScriptableObject
    {
        public int CurrentListIndex = 0;
        public List<FavoritesList> FavoriteLists = new List<FavoritesList>();
        public FavoritesList CurrentList => FavoriteLists[CurrentListIndex];

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
    }
}
#endif