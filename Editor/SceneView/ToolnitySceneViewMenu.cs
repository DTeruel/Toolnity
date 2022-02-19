#if UNITY_EDITOR
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
            if (ToolnitySettingsRegister.MenuPositionSelection == ToolnitySettingsRegister.MENU_POSITION_TOP
                || ToolnitySettingsRegister.MenuPositionSelection == ToolnitySettingsRegister.MENU_POSITION_BOTTOM)
            {
                DrawTopBottomDirection();
            }
            else
            {
                DrawLeftRightDirection();
            }
            Handles.EndGUI();
        }

        private static void DrawTopBottomDirection()
        {
            if (ToolnitySettingsRegister.MenuPositionSelection == ToolnitySettingsRegister.MENU_POSITION_TOP)
            {
                GUILayout.Space(-15 + ToolnitySettingsRegister.MenuOffset);
            }
            else
            {
                GUILayout.FlexibleSpace();
            }
                
            GUILayout.BeginVertical();

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();

            DrawAllButtons();

            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            if (ToolnitySettingsRegister.MenuPositionSelection == ToolnitySettingsRegister.MENU_POSITION_TOP)
            {
                GUILayout.FlexibleSpace();
            }
            else
            {
                GUILayout.Space(25 + ToolnitySettingsRegister.MenuOffset);
            }
            GUILayout.EndVertical();
        }

        private static void DrawLeftRightDirection()
        {
            GUILayout.BeginHorizontal();
            if (ToolnitySettingsRegister.MenuPositionSelection == ToolnitySettingsRegister.MENU_POSITION_LEFT)
            {
                GUILayout.Space(10 + ToolnitySettingsRegister.MenuOffset);
            }
            else
            {
                GUILayout.FlexibleSpace();
            }
            
            GUILayout.BeginVertical();
            GUILayout.FlexibleSpace();
            
            DrawAllButtons();

            GUILayout.FlexibleSpace();
            GUILayout.EndVertical();

            if (ToolnitySettingsRegister.MenuPositionSelection == ToolnitySettingsRegister.MENU_POSITION_LEFT)
            {
                GUILayout.FlexibleSpace();
            }
            else
            {
                GUILayout.Space(10 + ToolnitySettingsRegister.MenuOffset);
            }
            GUILayout.EndHorizontal();
        }

        private static void DrawAllButtons()
        {
            ToDoListSelector.DrawGUI();
            SceneSelector.DrawGUI();
            LightsActivator.DrawGUI();
        }
    }
}
#endif