#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Toolnity.Shortcuts
{
    [InitializeOnLoad]
    public static class CreateGameObjectShortcut
    {
        static CreateGameObjectShortcut()
        {
            EditorApplication.hierarchyWindowItemOnGUI += HierarchyWindowItemOnGUI;
        }

        private static void HierarchyWindowItemOnGUI(int instanceID, Rect selectionRect)
        {
            if (Application.isPlaying
                || !Event.current.alt
                || Event.current.type != EventType.MouseDown 
                || Event.current.button != 0
                || Event.current.clickCount < 2
                || Selection.objects.Length > 0)
            {
                return;
            }

            var gameObject = EditorUtility.InstanceIDToObject(instanceID) as GameObject;
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