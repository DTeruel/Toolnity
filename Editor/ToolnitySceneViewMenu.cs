using UnityEditor;
using UnityEngine;

namespace Toolnity
{
    [InitializeOnLoad]
    public static class ToolnitySceneViewMenu
    {
        static ToolnitySceneViewMenu()
        {
            SceneView.duringSceneGui += OnSceneGUI;
        }

        private static void OnSceneGUI(SceneView sceneView)
        {
            Handles.BeginGUI();
            if (ToolnitySettingsRegister.MenuPositionSelection == ToolnitySettingsRegister.MENU_POSITION_TOP)
            {
                GUILayout.Space(-15);
            }
            else
            {
                GUILayout.FlexibleSpace();
            }
            GUILayout.BeginVertical();
            
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            
            ToDoListSelector.DrawGUI();
            SceneSelector.DrawGUI();
            LightsActivator.DrawGUI();

            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            
            if (ToolnitySettingsRegister.MenuPositionSelection == ToolnitySettingsRegister.MENU_POSITION_TOP)
            {
                GUILayout.FlexibleSpace();
            }
            else
            {
                GUILayout.Space(25);
            }
            GUILayout.EndVertical();
            
            Handles.EndGUI();
        }
    }
}