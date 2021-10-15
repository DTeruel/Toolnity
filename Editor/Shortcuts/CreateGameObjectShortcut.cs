#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Toolnity
{
    [InitializeOnLoad]
    public static class CreateGameObjectShortcut
    {
        public const string CREATE_GAME_OBJECT_SETTINGS_ENABLED = "Toolnity/Create Game Object/Enabled";
        
        static CreateGameObjectShortcut()
        {
            EditorApplication.hierarchyWindowItemOnGUI += HierarchyWindowItemOnGUI;
        }

        private static void HierarchyWindowItemOnGUI(int instanceid, Rect selectionrect)
        {
            if (Event.current.type != EventType.MouseDown 
                || Event.current.button != 0
                || Event.current.clickCount < 2
                || Selection.objects.Length > 0)
            {
                return;
            }

            var enabledOption = EditorPrefs.GetBool(Application.dataPath + CREATE_GAME_OBJECT_SETTINGS_ENABLED, true);
            if (!enabledOption)
            {
                return;
            }

            var gameObject = EditorUtility.InstanceIDToObject(instanceid) as GameObject;
            if (gameObject == null)
            {
                var newObject = new GameObject();
                Debug.Log("[Toolnity] Creating new GameObject...", newObject);
                Undo.RegisterCreatedObjectUndo(newObject, "Creating new GameObject");
                Selection.SetActiveObjectWithContext(newObject, newObject);
                EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
            }
        }
    }
}
#endif