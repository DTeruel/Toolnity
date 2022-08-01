#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Toolnity
{
    [InitializeOnLoad]
    internal static class ToolnitySettingsRegister
    {
        private const string TOOLNITY_SETTINGS_NAME = "Project/ToolnitySettings";
        private const string SHORTCUTS_ENABLED = "Toolnity/Basic Shortcuts/Enabled";

        public static bool BasicShortcutsEnabled;

        static ToolnitySettingsRegister()
        {
            BasicShortcutsEnabled = EditorPrefs.GetBool(Application.dataPath + SHORTCUTS_ENABLED, true);
        }
        
        [MenuItem("Tools/Toolnity/Open Settings", priority = 2000)]
        public static void OpenProjectSettings()
        {
            SettingsService.OpenProjectSettings(TOOLNITY_SETTINGS_NAME);
        }
        
        [SettingsProvider]
        public static SettingsProvider CreateToolnitySettingsProvider()
        {
            var provider = new SettingsProvider(TOOLNITY_SETTINGS_NAME, SettingsScope.Project)
            {
                label = "Toolnity", guiHandler = (_) =>
                {
                    EditorGUILayout.Space();

                    ShowToggleOption("AutoSave On Play", AutoSaveOnPlay.AUTO_SAVE_SETTINGS_ENABLED);
        
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
                    
                    ShowToggleOption("Hierarchy Object Active", HierarchyObjectActive.HIERARCHY_OBJECT_SETTINGS_ENABLED);

                    EditorGUILayout.BeginHorizontal();
                    BasicShortcutsEnabled = ShowToggleOption("Basic Shortcuts", SHORTCUTS_ENABLED);
                    GUILayout.Space(20f);
                    GUILayout.Label("(F1-F4: Camera Views, F5: Play, F6: Pause, F7: Step, F12: Save all, Left Shift + T: Teleport Selected Game Object)");
                    GUILayout.FlexibleSpace();
                    EditorGUILayout.EndHorizontal();

                    EditorGUILayout.BeginHorizontal();
                    ShowToggleOption("Scene Object Selector", SceneObjectSelector.SCENE_OBJECT_SELECTOR_ENABLED);
                    GUILayout.Space(20f);
                    GUILayout.Label("(Left Ctrl + Left Shift + Left Mouse Click)");
                    GUILayout.FlexibleSpace();
                    EditorGUILayout.EndHorizontal();
                    
                    EditorGUILayout.BeginHorizontal();
                    ShowToggleOption("Hierarchy GO Creator", CreateGameObjectShortcut.CREATE_GAME_OBJECT_SETTINGS_ENABLED);
                    GUILayout.Space(20f);
                    GUILayout.Label("(Left Ctrl + Double Click inside Hierarchy Window to create a new Game Object)");
                    GUILayout.FlexibleSpace();
                    EditorGUILayout.EndHorizontal();
                    
                    EditorGUILayout.BeginHorizontal();
                    CustomButtonsMenu.Config.enabled = EditorGUILayout.Toggle("Custom Buttons Menu", CustomButtonsMenu.Config.enabled);
                    GUILayout.Space(20f);
                    GUILayout.Label("(Show Custom Buttons Menu in runtime)");
                    GUILayout.FlexibleSpace();
                    EditorGUILayout.EndHorizontal();
                    
                    if (CustomButtonsMenu.Config.enabled)
                    {
                        EditorGUILayout.BeginHorizontal();
                        GUILayout.Space(20f);
                        GUILayout.Label("Main Button Visible:    ");
                        CustomButtonsMenu.Config.mainButtonVisible = EditorGUILayout.Toggle("", CustomButtonsMenu.Config.mainButtonVisible);
                        GUILayout.FlexibleSpace();
                        EditorGUILayout.EndHorizontal();
                            
                        EditorGUILayout.BeginHorizontal();
                        GUILayout.Space(20f);
                        GUILayout.Label("Position:                  ");
                        GUILayout.Space(20f);
                        var index = CustomButtonsMenu.Config.position.GetHashCode();
                        var newIndex = EditorGUILayout.Popup(
                            index, 
                            Enum.GetNames(typeof(CustomButtonsMenu.CustomButtonPositionNames)));
                        if (newIndex != index)
                        {
                            CustomButtonsMenu.Config.position = (CustomButtonsMenu.CustomButtonPositionNames)newIndex;
                        }
                        GUILayout.FlexibleSpace();
                        EditorGUILayout.EndHorizontal();
                    }
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