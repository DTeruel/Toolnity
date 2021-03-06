#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using static UnityEditor.SceneManagement.EditorSceneManager;

namespace Toolnity
{
    public static class SaveProject
    {
        [MenuItem("Tools/Toolnity/Shortcuts/Project Save All _F12", priority = 100)]
        private static void SaveAll()
        {
            if (EditorApplication.isPlaying)
            {
                return;
            }

            var scenesWithChanges = false;
            for (var i = 0; i < SceneManager.sceneCount; i++)
            {
                var scene = SceneManager.GetSceneAt(i);
                if (scene.isDirty)
                {
                    scenesWithChanges = true;
                }
            }

            if (scenesWithChanges)
            {
                if (SaveCurrentModifiedScenesIfUserWantsTo())
                {
                    Debug.Log("-------------------------------- SCENES SAVED --------------------------------");
                }
            }

            AssetDatabase.SaveAssets();
            Debug.Log("+++++++++++++++++++++++++ PROJECT SAVED +++++++++++++++++++++++++");
        }
    }
}
#endif