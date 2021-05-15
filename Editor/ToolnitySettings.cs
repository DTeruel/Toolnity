#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Toolnity
{
    internal static class ToolnitySettingsRegister
    {
        private const string ToolnitySettingsName = "Project/ToolnitySettings"; 
        
        [MenuItem("Tools/Toolnity/Open Settings", priority = 2000)]
        public static void OpenProjectSettings()
        {
            SettingsService.OpenProjectSettings(ToolnitySettingsName);
        }
        
        [SettingsProvider]
        public static SettingsProvider CreateToolnitySettingsProvider()
        {
            var provider = new SettingsProvider(ToolnitySettingsName, SettingsScope.Project)
            {
                label = "Toolnity", guiHandler = (searchContext) =>
                {
                    EditorGUILayout.Space();
                    EditorGUILayout.BeginHorizontal();
                    var loadSceneOnPlay = ShowToggleOption("Load Scene On Play", LoadSceneOnPlay.LOAD_SCENE_ON_PLAY_SETTINGS_ENABLED, false);
                    if (loadSceneOnPlay)
                    {
                        GUILayout.Space(20f);
                        if (GUILayout.Button("Select Master Scene"))
                        {
                            LoadSceneOnPlay.SelectMasterScene();
                        }
                        GUILayout.Space(10f);
                        GUILayout.Label("(" + LoadSceneOnPlay.MasterScene + ")");
                        GUILayout.FlexibleSpace();
                    }
                    EditorGUILayout.EndHorizontal();

                    ShowToggleOption("AutoSave On Play", AutoSave.AUTO_SAVE_SETTINGS_ENABLED);
                    ShowToggleOption("Hierarchy Object Active", HierarchyObjectActive.HIERARCHY_OBJECT_SETTINGS_ENABLED);
                    
                    EditorGUILayout.BeginHorizontal();
                    var sceneObjectSelector = ShowToggleOption("Scene Object Selector", SceneObjectSelector.SCENE_OBJECT_SELECTOR_ENABLED);
                    if (sceneObjectSelector)
                    {
                        GUILayout.Space(20f);
                        GUILayout.Label("(Left Ctrl + Left Shift + Left Mouse Click)");
                        GUILayout.FlexibleSpace();
                    }
                    EditorGUILayout.EndHorizontal();
                    
                    ShowToggleOption("Scene Selector", SceneSelector.SCENE_SELECTOR_ENABLED);
                    ShowToggleOption("ToDo List", ToDoListSelector.TODO_LIST_ENABLED);
                },

                keywords = new HashSet<string>(new[]
                {
                    "Load Scene On Play", "Select Master Scene", "Auto Save On Play", "Hierarchy Object Active", "Scene Object Selector", "Scene Selector", "ToDo List"
                })
            };

            return provider;
        }

        private static bool ShowToggleOption(string label, string settingsKey, bool defaultValue = true)
        {
            var currentOption = EditorPrefs.GetBool(Application.dataPath + settingsKey, defaultValue);
            var changedOption = EditorGUILayout.Toggle(label, currentOption);
            if (currentOption != changedOption)
            {
                EditorPrefs.SetBool(Application.dataPath + settingsKey, changedOption);
            }

            return changedOption;
        }
    }
}
#endif