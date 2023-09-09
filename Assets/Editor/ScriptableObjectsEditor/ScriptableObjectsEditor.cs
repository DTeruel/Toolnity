using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;
using System.Linq;

public class ScriptableObjectsEditor : EditorWindow
{
    private bool showPath;
    private bool searchInPackages;
    private readonly List<ScriptableObject> scriptableObjects = new();
    private Vector2 leftScrollPos;
    private Vector2 rightScrollPos;
    private string[] typeOptions;
    private bool[] selectedTypeVisible;

    [MenuItem("Tools/Toolnity/Scriptable Objects Editor")]
    private static void Init()
    {
        GetWindow<ScriptableObjectsEditor>("Scriptable Objects Editor");
    }

    private void OnEnable()
    {
        RefreshAllData();
    }
    
    
    private void OnGUI()
    {
        DrawOptions();

        EditorGUILayout.Space();
        
        DrawAssets();
    }

    #region DRAW OPTIONS
    private void DrawOptions()
    {
        EditorGUILayout.Space(25f);

        EditorGUILayout.BeginHorizontal();
        
        EditorGUILayout.Space();
        
        GUILayout.Label("Show Path: ");
        showPath = EditorGUILayout.Toggle(showPath, GUILayout.Width(25f));

        EditorGUILayout.Space();

        if (typeOptions != null && typeOptions.Length > 0)
        {
            var selectedMask = 0;
            for (var i = 0; i < typeOptions.Length; i++)
            {
                if (selectedTypeVisible[i])
                {
                    selectedMask |= 1 << i;
                }
            }
        
            GUILayout.Label("Search In Packages: ");
            var newSearchInPackages = EditorGUILayout.Toggle(searchInPackages, GUILayout.Width(70f));
            if (searchInPackages != newSearchInPackages)
            {
                searchInPackages = newSearchInPackages;
                RefreshScriptableObjects();
            }
            
            GUILayout.Label("Types: ");
            var newSelectedMask = EditorGUILayout.MaskField(selectedMask, typeOptions, GUILayout.Width(250f));
            if (selectedMask != newSelectedMask)
            {
                selectedMask = newSelectedMask;
                
                for (var i = 0; i < typeOptions.Length; i++)
                {
                    selectedTypeVisible[i] = (selectedMask & (1 << i)) != 0;
                }

                RefreshScriptableObjects();
            }

            EditorGUILayout.Space();
        }
        
        GUILayout.FlexibleSpace();
        
        if (GUILayout.Button("Refresh"))
        {
            RefreshAllData();
        }
        
        EditorGUILayout.Space();
        EditorGUILayout.EndHorizontal();
    }

    private void RefreshAllData()
    {
        RefreshScriptableTypesWithAssets();
        RefreshScriptableObjects();
    }
    
    private void RefreshScriptableTypesWithAssets()
    {
        var typesWithAssets = AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(assembly => assembly.GetTypes())
            .Where(type => type.BaseType == typeof(ScriptableObject) && !type.IsAbstract)
            .ToList();
            // Replace type.BaseType by .IsSubclassOf(typeof(ScriptableObject)) for more depth search
            
        for (var i = typesWithAssets.Count - 1; i >= 0; i--)
        {
            var assets = AssetDatabase.FindAssets("t:" + typesWithAssets[i].Name);
            var removeType = true;

            for (var j = 0; j < assets.Length; j++)
            {
                var path = AssetDatabase.GUIDToAssetPath(assets[j]);
                var asset = AssetDatabase.LoadAssetAtPath<ScriptableObject>(path);
                if (asset != null 
                    && (searchInPackages || (!searchInPackages && path.StartsWith("Assets/"))))
                {
                    removeType = false;
                }
            }
            
            if(removeType)
            {
                typesWithAssets.RemoveAt(i);
            }
        }

        typeOptions = typesWithAssets.Select(type => type.Name).ToArray();
        selectedTypeVisible = new bool[typeOptions.Length];
    }


    private void RefreshScriptableObjects()
    {
        Selection.activeObject = null;
        
        scriptableObjects.Clear();

        var selectedTypes = new List<string>();
        for (var i = 0; i < selectedTypeVisible.Length; i++)
        {
            if (selectedTypeVisible[i])
            {
                selectedTypes.Add(typeOptions[i]);
            }
        }

        foreach (var selectedType in selectedTypes)
        {
            var guids = AssetDatabase.FindAssets("t:" + selectedType);
            foreach (var guid in guids)
            {
                var path = AssetDatabase.GUIDToAssetPath(guid);
                var asset = AssetDatabase.LoadAssetAtPath<ScriptableObject>(path);
                if (asset != null 
                    && (searchInPackages || (!searchInPackages && path.StartsWith("Assets/"))))
                {
                    scriptableObjects.Add(asset);
                }
            }
        }

        Repaint();
    }
    #endregion
    
    #region DRAW ASSETS
    private void DrawAssets()
    {
        GUILayout.BeginHorizontal();
        
        const float margins = 5f;
        var panelSize = (position.width / 2f) - (margins * 3f);
        
        EditorGUILayout.Space(margins);
        leftScrollPos = GUILayout.BeginScrollView(leftScrollPos, GUILayout.Width(panelSize), GUILayout.ExpandHeight(true));
        DisplayScriptableObjects();
        GUILayout.EndScrollView();

        EditorGUILayout.Space(margins);
        
        rightScrollPos = GUILayout.BeginScrollView(rightScrollPos, GUILayout.Width(panelSize), GUILayout.ExpandHeight(true));
        DisplayInspector();
        GUILayout.EndScrollView();

        EditorGUILayout.Space(margins);

        GUILayout.EndHorizontal();
    }

    private void DisplayScriptableObjects()
    {
        foreach (var obj in scriptableObjects)
        {
            var buttonName = obj.name;
            if (showPath)
            {
                buttonName = AssetDatabase.GetAssetPath(obj);
            }
            
            if (GUILayout.Button(buttonName, GUILayout.ExpandWidth(true)))
            {
                Selection.activeObject = obj;
            }
        }
    }

    private static void DisplayInspector()
    {
        if (Selection.activeObject is ScriptableObject scriptableObject)
        {
            EditorGUI.BeginChangeCheck();
            var serializedObject = new SerializedObject(scriptableObject);
            serializedObject.Update();

            var iterator = serializedObject.GetIterator();
            var enterChildren = true;
            while (iterator.NextVisible(enterChildren))
            {
                EditorGUILayout.PropertyField(iterator, true);
                enterChildren = false;
            }

            serializedObject.ApplyModifiedProperties();
            if (EditorGUI.EndChangeCheck())
            {
                EditorUtility.SetDirty(scriptableObject);
            }
        }
    }
    #endregion
}
