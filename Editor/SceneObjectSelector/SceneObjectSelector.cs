#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Toolnity
{
    [InitializeOnLoad]
    public class SceneObjectSelector
    {
        public const string SCENE_OBJECT_SELECTOR_ENABLED = "Toolnity/Scene Object Selector/Enabled";
        private const int NumMAXItemsToAdd = 100;
        
        private static readonly List<GameObject> GameObjectsPicked = new List<GameObject>();
        private static readonly SceneObjectSelectorPopup Popup = new SceneObjectSelectorPopup();

        static SceneObjectSelector () 
        {
            SceneView.beforeSceneGui -= BeforeSceneGui;
            SceneView.beforeSceneGui += BeforeSceneGui;
        }
        
        private static void BeforeSceneGui(SceneView sceneView)
        {
            var enabledOption = EditorPrefs.GetBool(Application.dataPath + SCENE_OBJECT_SELECTOR_ENABLED, true);
            if (!enabledOption)
            {
                return;
            }

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
            for(var i = 0; i < NumMAXItemsToAdd; i++)
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
