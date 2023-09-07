#if UNITY_EDITOR
using System;
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.IO;
using System.Linq;

public class StateMachineGenerator : EditorWindow
{
    private string className = "";
    private string folderPath = "";
    private List<string> states = new();

    [MenuItem("Tools/State Machine Generator")]
    public static void ShowWindow()
    {
        GetWindow<StateMachineGenerator>("State Machine Generator");
    }

    private void OnGUI()
    {
        GUILayout.Space(10);

        GUILayout.Label("Class Name:", EditorStyles.boldLabel);
        className = EditorGUILayout.TextField(className);

        GUILayout.Space(10);

        GUILayout.Label("States:", EditorStyles.boldLabel);
        for (var i = 0; i < states.Count; i++)
        {
            states[i] = EditorGUILayout.TextField(states[i]);
        }

        GUILayout.Space(10);

        if (GUILayout.Button("Add State"))
        {
            states.Add("");
        }

        GUILayout.FlexibleSpace();

        if (GUILayout.Button("Generate"))
        {
            folderPath = EditorUtility.OpenFolderPanel("Select Folder to Generate Script", "", "");
            if (!string.IsNullOrEmpty(folderPath) && HasValidData())
            {
                GenerateScript();
            }
        }
    }

    private bool HasValidData()
    {
        FixName(className, out className);
        
        states.RemoveAll(string.IsNullOrEmpty);
        for (var index = 0; index < states.Count; index++)
        {
            FixName(states[index], out var finalName);
            states[index] = finalName;
        }
        if (states.Count == 0)
        {
            Debug.LogError("Enum Names list cannot be empty.");
            return false;
        }

        return true;
    }
    
    private bool FixName(string nameToFix, out string finalName)
    {
        finalName = nameToFix;
        if (string.IsNullOrEmpty(finalName))
        {
            Debug.LogError("Name cannot be empty.");
            return false;
        }

        if (!char.IsLetter(finalName[0]))
        {
            Debug.LogWarning("Removing invalid characters from the beginning.");
            finalName = finalName.Remove(0, 1);

            return FixName(finalName, out finalName);
        }
    
        finalName = new string(finalName.Where(char.IsLetterOrDigit).ToArray());

        if (string.IsNullOrEmpty(finalName))
        {
            Debug.LogError("Name cannot be empty after removing invalid characters.");
            return false;
        }

        return true;
    }

    private void GenerateScript()
    {
        var filePath = Path.Combine(folderPath, className + ".cs");

        using (var writer = new StreamWriter(filePath))
        {
            writer.WriteLine("using System;");
            writer.WriteLine("\npublic class " + className);
            writer.WriteLine("{");

            writer.WriteLine("\tpublic enum " + className + "State");
            writer.WriteLine("\t{");
            for (var i = 0; i < states.Count; i++)
            {
                writer.WriteLine("\t\t" + states[i] + " = " + i + ",");
            }
            writer.WriteLine("\t}");

            writer.WriteLine("\n\t#region Main");

            writer.WriteLine("\tprivate " + className + "State currentState = " + className + "State." + states[0] + ";");

            writer.WriteLine("\n\tpublic void SetState(" + className + "State newState)");
            writer.WriteLine("\t{");
            writer.WriteLine("\t\tOnExitState(currentState);");
            writer.WriteLine("\t\tcurrentState = newState;");
            writer.WriteLine("\t\tOnEnterState(currentState);");
            writer.WriteLine("\t}");

            writer.WriteLine("\n\tprivate void OnExitState(" + className + "State state)");
            writer.WriteLine("\t{");
            writer.WriteLine("\t\tswitch (state)");
            writer.WriteLine("\t\t{");
            for (var i = 0; i < states.Count; i++)
            {
                var enumName = states[i];
                writer.WriteLine("\t\t\tcase " + className + "State." + enumName + ":");
                writer.WriteLine("\t\t\t\tOnExitState" + enumName + "();");
                writer.WriteLine("\t\t\t\tbreak;");
            }
            writer.WriteLine("\t\t\tdefault:");
            writer.WriteLine("\t\t\t\tthrow new ArgumentOutOfRangeException();");

            writer.WriteLine("\t\t}");
            writer.WriteLine("\t}");

            writer.WriteLine("\n\tprivate void OnEnterState(" + className + "State state)");
            writer.WriteLine("\t{");
            writer.WriteLine("\t\tswitch (state)");
            writer.WriteLine("\t\t{");
            for (var i = 0; i < states.Count; i++)
            {
                var enumName = states[i];
                writer.WriteLine("\t\t\tcase " + className + "State." + enumName + ":");
                writer.WriteLine("\t\t\t\tOnEnterState" + enumName + "();");
                writer.WriteLine("\t\t\t\tbreak;");
            }
            writer.WriteLine("\t\t\tdefault:");
            writer.WriteLine("\t\t\t\tthrow new ArgumentOutOfRangeException();");
            writer.WriteLine("\t\t}");
            writer.WriteLine("\t}");

            writer.WriteLine("\n\tpublic void Update()");
            writer.WriteLine("\t{");
            writer.WriteLine("\t\tswitch (currentState)");
            writer.WriteLine("\t\t{");
            for (var i = 0; i < states.Count; i++)
            {
                var enumName = states[i];
                writer.WriteLine("\t\t\tcase " + className + "State." + enumName + ":");
                writer.WriteLine("\t\t\t\tUpdateState" + enumName + "();");
                writer.WriteLine("\t\t\t\tbreak;");
            }
            writer.WriteLine("\t\t\tdefault:");
            writer.WriteLine("\t\t\t\tthrow new ArgumentOutOfRangeException();");
            writer.WriteLine("\t\t}");
            writer.WriteLine("\t}");
            writer.WriteLine("\t#endregion");

            for (int i = 0; i < states.Count; i++)
            {
                var enumName = states[i];
                writer.WriteLine("\n\t#region " + enumName);
                writer.WriteLine("\tprivate void OnEnterState" + enumName + "() {}");
                writer.WriteLine("\tprivate void UpdateState" + enumName + "() {}");
                writer.WriteLine("\tprivate void OnExitState" + enumName + "() {}");
                writer.WriteLine("\t#endregion");
            }

            writer.WriteLine("}");
        }

        AssetDatabase.Refresh();
        Debug.Log("Script generated at: " + filePath);
    }
}
#endif
