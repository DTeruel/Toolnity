#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace Toolnity.EditorExtensions
{
    internal static class Settings
    {
        private const string EDITOR_EXTENSIONS_SETTINGS_NAME = "Project/Toolnity/Editor Extensions";
        
        [MenuItem("Tools/Toolnity/Editor Extensions Settings", priority = 5000)]
        public static void OpenProjectSettings()
        {
            SettingsService.OpenProjectSettings(EDITOR_EXTENSIONS_SETTINGS_NAME);
        }
        
        [SettingsProvider]
        public static SettingsProvider CreateSettingsProvider()
        {
            var provider = new SettingsProvider(EDITOR_EXTENSIONS_SETTINGS_NAME, SettingsScope.Project)
            {
                label = "Editor Extensions", guiHandler = (_) =>
                {
                    EditorGUILayout.Space();

                    EditorGUILayout.BeginHorizontal();
                    var saveOnPlay= EditorGUILayout.Toggle("Auto-Save on play:", EditorExtensions.SaveOnPlay);
                    if (!EditorExtensions.SaveOnPlay.Equals(saveOnPlay))
                    {
                        EditorExtensions.SaveOnPlay = saveOnPlay;
                    }
                    GUILayout.FlexibleSpace();
                    EditorGUILayout.EndHorizontal();

                    EditorGUILayout.BeginHorizontal();
                    var loadSceneOnPlay = EditorGUILayout.Toggle("Load scene on play:", EditorExtensions.LoadSceneOnPlay);
                    if (!EditorExtensions.LoadSceneOnPlay.Equals(loadSceneOnPlay))
                    {
                        EditorExtensions.LoadSceneOnPlay = loadSceneOnPlay;
                        
                        if (loadSceneOnPlay)
                        {
                            LoadSceneOnPlay.SelectMasterScene();
                        }
                    }

                    if (loadSceneOnPlay)
                    {
                        GUILayout.Space(20f);
                        GUILayout.Label($"({EditorExtensions.MasterScene})");
                    }
                    GUILayout.FlexibleSpace();
                    EditorGUILayout.EndHorizontal();

                    if (loadSceneOnPlay)
                    {
                        EditorGUILayout.BeginHorizontal();
                        GUILayout.Space(50f);
                        GUILayout.FlexibleSpace();
                        EditorGUILayout.EndHorizontal();
                        
                    }
                }
            };

            return provider;
        }
    }
}
#endif