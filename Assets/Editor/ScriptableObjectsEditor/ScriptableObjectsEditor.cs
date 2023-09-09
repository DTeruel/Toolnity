using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;
using System.Linq;
using Object = UnityEngine.Object;

public class ScriptableObjectsEditor : EditorWindow
{
    private bool showPath;
    private bool searchInPackages;
    private readonly List<ScriptableObject> scriptableObjects = new();
    private Vector2 leftScrollPos;
    private Vector2 rightScrollPos;
    private string[] typeOptions;
    private bool[] selectedTypeVisible;
    private Object objectSelected;
    private int objectSelectedIndex;
    private GUIStyle defaultAssetButton;
    private GUIStyle selectedAssetButton;

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
            .Where(type => type.IsSubclassOf(typeof(ScriptableObject)) && !type.IsAbstract)
            .ToList();
        // Replace type.BaseType by .IsSubclassOf(typeof(ScriptableObject)) for more depth search

        var typeNameToAssetFolder = new Dictionary<string, string>();
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
                    typeNameToAssetFolder[typesWithAssets[i].Name] = path;
                }
            }

            if (removeType)
            {
                typesWithAssets.RemoveAt(i);
            }
        }

        // Natural Order: typeOptions = typesWithAssets.Select(type => type.Name).ToArray();
        typeOptions = typeNameToAssetFolder.OrderBy(pair => pair.Value)
            .Select(pair => pair.Key)
            .ToArray();
        
        selectedTypeVisible = new bool[typeOptions.Length];
    }



    private void RefreshScriptableObjects()
    {
        objectSelected = null;
        objectSelectedIndex = -1;
        
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
        
        leftScrollPos = GUILayout.BeginScrollView(leftScrollPos, GUI.skin.box, GUILayout.Width(panelSize), GUILayout.ExpandHeight(true));
        DisplayScriptableObjects();
        GUILayout.EndScrollView();

        EditorGUILayout.Space(margins);
        
        rightScrollPos = GUILayout.BeginScrollView(rightScrollPos, GUI.skin.box, GUILayout.Width(panelSize), GUILayout.ExpandHeight(true));
        DisplayInspector();
        GUILayout.EndScrollView();

        EditorGUILayout.Space(margins);

        GUILayout.EndHorizontal();
    }
    
    private void DisplayScriptableObjects()
    {
        CheckGUIStyles();
        
        for (var i = 0; i < scriptableObjects.Count; i++)
        {
            var obj = scriptableObjects[i];
            var buttonName = obj.name;
            if (showPath)
            {
                buttonName = AssetDatabase.GetAssetPath(obj);
            }
            
            var style = defaultAssetButton;
            if (objectSelectedIndex == i)
            {
                style = selectedAssetButton;
                buttonName = "> " + buttonName + " <";
            }
            if (GUILayout.Button(buttonName, style, GUILayout.ExpandWidth(true)))
            {
                objectSelected = obj;
                objectSelectedIndex = i;
                EditorGUIUtility.PingObject(objectSelected);
            }
        }
    }

    private void CheckGUIStyles()
    {
        if (defaultAssetButton == null)
        {
            defaultAssetButton = GUI.skin.button;
        }
        
        if (selectedAssetButton == null)
        {
            selectedAssetButton = new GUIStyle(GUI.skin.button)
            {
                fontStyle = FontStyle.Bold
            };
        }
    }

    private void DisplayInspector()
    {
        if (objectSelected is ScriptableObject scriptableObject)
        {
            if (GUILayout.Button("Select Asset", GUILayout.ExpandWidth(true)))
            {
                EditorGUIUtility.PingObject(objectSelected);
            }
            
            EditorGUILayout.Space();
            
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
