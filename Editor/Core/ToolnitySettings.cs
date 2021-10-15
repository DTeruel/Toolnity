#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Toolnity
{
    internal static class ToolnitySettingsRegister
    {
        private const string ToolnitySettingsName = "Project/ToolnitySettings";
        public const string MENU_POSITION = "Toolnity/Menu Position";
        public const string SHORTCUTS_ENABLED = "Toolnity/Basic Shortcuts/Enabled";
        
        private static readonly List<string> MenuPositionList = new List<string> {"Top", "Bottom", "Left", "Right"};
        public const int MENU_POSITION_TOP = 0;
        public const int MENU_POSITION_BOTTOM = 1;
        public const int MENU_POSITION_LEFT = 2;
        //public const int MENU_POSITION_RIGHT = 3;

        public static int MenuPositionSelection;
        public static bool BasicShortcutsEnabled;

        [MenuItem("Tools/Toolnity/Open Settings", priority = 2000)]
        public static void OpenProjectSettings()
        {
            SettingsService.OpenProjectSettings(ToolnitySettingsName);
            MenuPositionSelection = EditorPrefs.GetInt(Application.dataPath + MENU_POSITION, 0);
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
                    
                    EditorGUILayout.BeginHorizontal();
                    BasicShortcutsEnabled = ShowToggleOption("Basic Shortcuts", SHORTCUTS_ENABLED);
                    GUILayout.Space(20f);
                    GUILayout.Label("(F1-F4: Camera Views, F5: Play, F6: Pause, F7: Step, F12: Save all, Left Shift + T: Teleport Game Object)");
                    GUILayout.FlexibleSpace();
                    EditorGUILayout.EndHorizontal();
                    
                    EditorGUILayout.BeginHorizontal();
                    ShowToggleOption("Create GO Shortcut", CreateGameObjectShortcut.CREATE_GAME_OBJECT_SETTINGS_ENABLED);
                    GUILayout.Space(20f);
                    GUILayout.Label("(Double Click in an empty space in Hierarchy Window to create a new Game Object)");
                    GUILayout.FlexibleSpace();
                    EditorGUILayout.EndHorizontal();

                    ShowToggleOption("AutoSave On Play", AutoSave.AUTO_SAVE_SETTINGS_ENABLED);
                    ShowToggleOption("Hierarchy Object Active", HierarchyObjectActive.HIERARCHY_OBJECT_SETTINGS_ENABLED);

                    EditorGUILayout.BeginHorizontal();
                    ShowToggleOption("Scene Object Selector", SceneObjectSelector.SCENE_OBJECT_SELECTOR_ENABLED);
                    GUILayout.Space(20f);
                    GUILayout.Label("(Left Ctrl + Left Shift + Left Mouse Click)");
                    GUILayout.FlexibleSpace();
                    EditorGUILayout.EndHorizontal();
                    
                    
                    GUILayout.Label("");
                    GUILayout.Label("SCENE VIEW MENU");

                    EditorGUILayout.BeginHorizontal();
                    GUILayout.Label("Position");
                    GUILayout.Space(60f);
                    var newSelection = EditorGUILayout.Popup(MenuPositionSelection, MenuPositionList.ToArray());
                    if (newSelection != MenuPositionSelection)
                    {
                        MenuPositionSelection = newSelection;
                        EditorPrefs.SetInt(Application.dataPath + MENU_POSITION, MenuPositionSelection);
                    }
                    GUILayout.FlexibleSpace();
                    EditorGUILayout.EndHorizontal();
                    
                    ShowToggleOption("Scene Selector", SceneSelector.SCENE_SELECTOR_ENABLED);
                    ShowToggleOption("ToDo List", ToDoListSelector.TODO_LIST_ENABLED);
                    ShowToggleOption("Lights Actions", LightsActivator.LIGHTS_ACTIVATOR_ENABLED);
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