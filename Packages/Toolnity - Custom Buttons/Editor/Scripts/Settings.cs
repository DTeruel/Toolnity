#if UNITY_EDITOR
using System;
using UnityEditor;
using UnityEngine;

namespace Toolnity.CustomButtons
{
    internal static class Settings
    {
        private const string CUSTOM_BUTTONS_SETTINGS_NAME = "Project/Toolnity/Custom Buttons";
        
        [MenuItem("Tools/Toolnity/Custom Buttons Settings", priority = 5001)]
        public static void OpenProjectSettings()
        {
            SettingsService.OpenProjectSettings(CUSTOM_BUTTONS_SETTINGS_NAME);
        }
        
        [SettingsProvider]
        public static SettingsProvider CreateCustomButtonsSettingsProvider()
        {
            var provider = new SettingsProvider(CUSTOM_BUTTONS_SETTINGS_NAME, SettingsScope.Project)
            {
                label = "Custom Buttons", guiHandler = (_) =>
                {
                    EditorGUILayout.Space();

                    EditorGUILayout.BeginHorizontal();
                    CustomButtonsMenu.Config.enabled = EditorGUILayout.Toggle("Enabled in Runtime:", CustomButtonsMenu.Config.enabled);
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
                }
            };

            return provider;
        }
    }
}
#endif