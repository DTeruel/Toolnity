#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Toolnity.Shortcuts
{
    [InitializeOnLoad]
    public class SceneObjectSelector
    {
        private const int NUM_MAX_ITEMS_TO_ADD = 100;
        
        private static readonly List<GameObject> GameObjectsPicked = new();
        private static readonly SceneObjectSelectorPopup Popup = new();

        static SceneObjectSelector () 
        {
            SceneView.beforeSceneGui -= BeforeSceneGui;
            SceneView.beforeSceneGui += BeforeSceneGui;
        }
        
        private static void BeforeSceneGui(SceneView sceneView)
        {
            if (!Event.current.shift || !Event.current.control || Event.current.button != 0 || Event.current.type != EventType.MouseDown)
            {
                return;
            }
            
            Event.current.Use();
            
            PickGameObjects();
            ShowPopup();
        }

        private static void PickGameObjects()
        {
            Selection.objects = null;
            GameObjectsPicked.Clear();

            AddGameObjectsInMousePosition(true);
            AddGameObjectsInMousePosition(false);
        }

        private static void AddGameObjectsInMousePosition(bool selectPrefabRoot)
        {
            for(var i = 0; i < NUM_MAX_ITEMS_TO_ADD; i++)
            {
                var gameObject = HandleUtility.PickGameObject(Event.current.mousePosition, selectPrefabRoot, GameObjectsPicked.ToArray());
                if (gameObject != null)
                {
                    if(!GameObjectsPicked.Contains(gameObject))
                    {
                        GameObjectsPicked.Add(gameObject);
                    }
                }
                else
                {
                    break;
                }
            }
        }

        private static void ShowPopup()
        {
            if (GameObjectsPicked.Count == 0)
            {
                return;
            }

            Popup.Init(GameObjectsPicked);
            var rect = new Rect(Event.current.mousePosition, Vector2.zero);
            PopupWindow.Show(rect, Popup);
        }
    }
}
#endif
