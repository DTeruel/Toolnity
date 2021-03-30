#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Toolnity
{
    [InitializeOnLoad]
    public class SceneObjectSelector
    {
        private const string ACTIVE_OPTION_NAME = "Tools/Toolnity/Scene Object Selector/Active";
        private const int NUM_MAX_ITEMS_TO_ADD = 100;
        
        private static bool active;
        private static readonly List<GameObject> GameObjectsPicked = new List<GameObject>();
        private static SceneObjectSelectorPopup popup = new SceneObjectSelectorPopup();

        static SceneObjectSelector () 
        {
            EditorApplication.delayCall += DelayCall;
            
            SceneView.beforeSceneGui -= BeforeSceneGui;
            SceneView.beforeSceneGui += BeforeSceneGui;
        }

        private static void DelayCall()
        {
            active = EditorPrefs.GetBool(ACTIVE_OPTION_NAME, true);
            Menu.SetChecked(ACTIVE_OPTION_NAME, active);
        }
		
        [MenuItem(ACTIVE_OPTION_NAME)]
        public static void ToggleActive()
        {
            active = !active;
            Menu.SetChecked(ACTIVE_OPTION_NAME, active);
            EditorPrefs.SetBool(ACTIVE_OPTION_NAME, active);
        }
        
        private static void BeforeSceneGui(SceneView sceneView)
        {
            if (!active || !Event.current.shift || Event.current.button != 0 || Event.current.type != EventType.MouseDown)
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

            popup.Init(GameObjectsPicked);
            var rect = new Rect(Event.current.mousePosition, Vector2.zero);
            PopupWindow.Show(rect, popup);
        }
    }
}
#endif
