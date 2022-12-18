#if UNITY_EDITOR
using Toolnity.ProjectInfo;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(ProjectInfoConfig))]
public class ProjectInfoConfigInspector : Editor
{
    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        DrawDefaultInspector();
        
        GUILayout.Space(25);
        if (GUILayout.Button("Regenerate Menu"))
        {
            ProjectInfo.RegenerateMenu();
        }
        
        serializedObject.ApplyModifiedProperties();
    }
}
#endif