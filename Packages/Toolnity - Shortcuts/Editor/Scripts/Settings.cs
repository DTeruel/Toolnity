#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace Toolnity.Shortcuts
{
    internal static class Settings
    {
        private const string SHORTCUTS_SETTINGS_NAME = "Project/Toolnity/Shortcuts";
        
        [MenuItem("Tools/Toolnity/Shortcuts Settings", priority = 5002)]
        public static void OpenProjectSettings()
        {
            SettingsService.OpenProjectSettings(SHORTCUTS_SETTINGS_NAME);
        }
        
        [SettingsProvider]
        public static SettingsProvider CreateSettingsProvider()
        {
            var provider = new SettingsProvider(SHORTCUTS_SETTINGS_NAME, SettingsScope.Project)
            {
                label = "Shortcuts", guiHandler = (_) =>
                {
                    EditorGUILayout.Space();

                    DrawLabel("Camera Top-Bottom: F1");
                    DrawLabel("Camera Left-Right: F2");
                    DrawLabel("Camera Front-Back: F3");
                    DrawLabel("Camera Perspective-Orthographic: F4");
                    DrawLabel("Camera Play-Stop: F5");
                    DrawLabel("Camera Pause-Resume: F6");
                    DrawLabel("Save All: F12");
                    DrawLabel("Teleport: Shift+T");
                    
                    
                    EditorGUILayout.Space();
                    DrawLabel("TELEPORT");
                    DrawLabel("    Axis to copy:");
                    
                    var originalValue = EditorGUIUtility.labelWidth;
                    EditorGUIUtility.labelWidth = 20;
                    
                    EditorGUILayout.BeginHorizontal();
                    GUILayout.Space(20f);
                    Teleport.Config.CopyAxisX = EditorGUILayout.Toggle("X:", Teleport.Config.CopyAxisX);
                    GUILayout.FlexibleSpace();
                    EditorGUILayout.EndHorizontal();
                    
                    EditorGUILayout.BeginHorizontal();
                    GUILayout.Space(20f);
                    Teleport.Config.CopyAxisY = EditorGUILayout.Toggle("Y:", Teleport.Config.CopyAxisY);
                    GUILayout.FlexibleSpace();
                    EditorGUILayout.EndHorizontal();
                    
                    EditorGUILayout.BeginHorizontal();
                    GUILayout.Space(20f);
                    Teleport.Config.CopyAxisZ = EditorGUILayout.Toggle("Z:", Teleport.Config.CopyAxisZ);
                    GUILayout.FlexibleSpace();
                    EditorGUILayout.EndHorizontal();
                    
                    EditorGUIUtility.labelWidth = originalValue;
                }
            };

            return provider;
        }
    
        private static void DrawLabel(string text)
        {
            EditorGUILayout.BeginHorizontal();
            GUILayout.Label(text);
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();
        }
    }
}
#endif