#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;
using System.Linq;
using Object = UnityEngine.Object;

public class ScriptableObjectsViewer : EditorWindow
{
    private class FolderInfo
    {
        public string Name;
        public bool Folded;
        public List<FolderInfo> SubFolders;
        public List<ScriptableObject> Objects;
    }
    
    private bool showAttachedInspector;
    private bool searchInPackages;
    private bool showClassType;
    private int selectedMask;
    
    private bool[] selectedTypeVisible;
    private Vector2 leftScrollPos;
    private Vector2 rightScrollPos;
    private string[] typeOptions;
    private Object objectSelected;
    private GUIStyle defaultAssetButton;
    private GUIStyle selectedAssetButton;
    private readonly List<FolderInfo> assetFolders = new();

    [MenuItem("Tools/Toolnity/Scriptable Objects Viewer", priority = 3000)]
    public static void ShowWindow()
    {
        var scriptableObjectsEditorWindow = GetWindow<ScriptableObjectsViewer>("Scriptable Objects Viewer");
        scriptableObjectsEditorWindow.titleContent = new GUIContent(
            "Scriptable Objects Viewer", 
            EditorGUIUtility.IconContent("d_ScriptableObject Icon").image);
        scriptableObjectsEditorWindow.Show();
    }

    private void GetParameters()
    {
        showAttachedInspector = EditorPrefs.GetBool(Application.productName + "_showAttachedInspector", false);
        searchInPackages = EditorPrefs.GetBool(Application.productName + "_searchInPackages", false);
        showClassType = EditorPrefs.GetBool(Application.productName + "_showClassType", false);
        selectedMask = EditorPrefs.GetInt(Application.productName + "_selectedMask", 0);
    }

    private void OnEnable()
    {
        RefreshAllData();
    }
    
    private void OnFocus()
    {
        GetParameters();
    }

    private void OnGUI()
    {
        CheckGUIStyles();

        DrawOptions();
        DrawAssets();
    }

    private void CheckGUIStyles()
    {
        if (defaultAssetButton == null)
        {
            defaultAssetButton = GUI.skin.label;
        }

        if (selectedAssetButton == null)
        {
            selectedAssetButton = new GUIStyle(GUI.skin.label)
            {
                fontStyle = FontStyle.Bold,
                fontSize = 14
            };
        }
    }

    #region DRAW OPTIONS
    private void DrawOptions()
    {
        EditorGUILayout.Space(15f);
        
        // ATTACKED INSPECTOR
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.Space();
        GUILayout.Label("Attached Inspector: ", GUILayout.Width(125f));
        var newShowAttachedInspector = EditorGUILayout.Toggle(showAttachedInspector, GUILayout.Width(20f));
        if (showAttachedInspector != newShowAttachedInspector)
        {
            showAttachedInspector = newShowAttachedInspector;
            EditorPrefs.SetBool(Application.productName + "_showAttachedInspector", showAttachedInspector);
        }
        GUILayout.FlexibleSpace();
        if (GUILayout.Button("Refresh"))
        {
            RefreshAllData();
        }
        EditorGUILayout.Space();
        EditorGUILayout.EndHorizontal();
        
        EditorGUILayout.Space();
        
        // SEARCH IN PACKAGES
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.Space();
        GUILayout.Label("Search In Packages: ", GUILayout.Width(125f));
        var newSearchInPackages = EditorGUILayout.Toggle(searchInPackages, GUILayout.Width(20f));
        if (searchInPackages != newSearchInPackages)
        {
            searchInPackages = newSearchInPackages;
            EditorPrefs.SetBool(Application.productName + "_searchInPackages", searchInPackages);
            RefreshScriptableObjects();
        }
        GUILayout.FlexibleSpace();
        EditorGUILayout.EndHorizontal();
        
        EditorGUILayout.Space();
        
        // SHOW CLASS TYPE
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.Space();
        GUILayout.Label("Show Class Type: ", GUILayout.Width(125f));
        var newShowClassType = EditorGUILayout.Toggle(showClassType, GUILayout.Width(20f));
        if (showClassType != newShowClassType)
        {
            showClassType = newShowClassType;
            EditorPrefs.SetBool(Application.productName + "_showClassType", showClassType);
        }
        GUILayout.FlexibleSpace();
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.Space();

        // TYPES
        EditorGUILayout.BeginHorizontal();

        EditorGUILayout.Space();

        if (typeOptions != null && typeOptions.Length > 0)
        {
            GUILayout.Label("Types: ");
            var newSelectedMask = EditorGUILayout.MaskField(selectedMask, typeOptions, GUILayout.Width(250f));
            if (selectedMask != newSelectedMask)
            {
                selectedMask = newSelectedMask;
                EditorPrefs.SetInt(Application.productName + "_selectedMask", selectedMask);

                for (var i = 0; i < typeOptions.Length; i++)
                {
                    selectedTypeVisible[i] = (selectedMask & (1 << i)) != 0;
                }

                RefreshScriptableObjects();
            }
        }

        GUILayout.FlexibleSpace();
        EditorGUILayout.EndHorizontal();
    }
    #endregion
    
    #region ASSETS

    private void RefreshAllData()
    {
        GetParameters();
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
        for (var i = 0; i < typeOptions.Length; i++)
        {
            selectedTypeVisible[i] = (selectedMask & (1 << i)) != 0;
        }
    }

    private void RefreshScriptableObjects()
    {
        objectSelected = null;
        assetFolders.Clear();

        var selectedTypes = new List<string>();
        for (var i = 0; i < selectedTypeVisible.Length; i++)
        {
            if (selectedTypeVisible[i])
            {
                selectedTypes.Add(typeOptions[i]);
            }
        }

        foreach(var selectedType in selectedTypes)
        {
            var guids = AssetDatabase.FindAssets("t:" + selectedType);
            foreach(var guid in guids)
            {
                var path = AssetDatabase.GUIDToAssetPath(guid);
                var asset = AssetDatabase.LoadAssetAtPath<ScriptableObject>(path);
                if (asset != null
                    && (searchInPackages || (!searchInPackages && path.StartsWith("Assets/"))))
                {
                    var assetPath = path.Split('/').ToList();
                    //var folderPath = string.Join("/", folders.Take(folders.Count - 1));
                    assetPath.RemoveAt(assetPath.Count - 1);
                    AddAsset(assetFolders, assetPath, asset);
                }
            }
        }

        Repaint();
    }
    
    private void AddAsset(List<FolderInfo> foldersInfo, List<string> assetPath, ScriptableObject asset)
    {
        FolderInfo currentFolder = null;
        var folderFound = false;
        foreach(var folder in foldersInfo)
        {
            if (folder.Name == assetPath[0])
            {
                folderFound = true;
                currentFolder = folder;
                break;
            }
        }

        if (!folderFound)
        {
            currentFolder = new FolderInfo
            {
                Name = assetPath[0], 
                Folded = true, 
                Objects = new List<ScriptableObject>(), 
                SubFolders = new List<FolderInfo>()
            };
            
            foldersInfo.Add(currentFolder);
        }

        if (assetPath.Count == 1)
        {
            currentFolder.Objects.Add(asset);
        }
        else
        {
            assetPath.RemoveAt(0);
            AddAsset(currentFolder.SubFolders, assetPath, asset);
        }
    }
    #endregion

    #region DRAW ASSETS
    private void DrawAssets()
    {
        EditorGUILayout.Space(15f);
        
        GUILayout.BeginHorizontal();

        const float margins = 5f;
        float panelSize;
        if (showAttachedInspector)
        {
            panelSize = (position.width / 2f) - (margins * 3f);
        }
        else
        {
            panelSize = position.width - (margins * 2f);
        }
        
        EditorGUILayout.Space(margins);

        DisplayScriptableObjects(panelSize);

        if (showAttachedInspector)
        {
            EditorGUILayout.Space(margins);

            DisplayInspector(panelSize);
        }

        EditorGUILayout.Space(margins);

        GUILayout.EndHorizontal();
    }

    private void DisplayScriptableObjects(float panelSize)
    { 
        leftScrollPos = GUILayout.BeginScrollView(leftScrollPos, GUI.skin.box, GUILayout.Width(panelSize), GUILayout.ExpandHeight(true));
        RenderFolders(assetFolders);
        GUILayout.EndScrollView();
    }

    private void RenderFolders(List<FolderInfo> foldersToRender)
    {
        const float indentationSpace = 15f;

        foreach(var folder in foldersToRender)
        {
            folder.Folded = EditorGUILayout.Foldout(folder.Folded, folder.Name, true);
            if (folder.Folded)
            {
                GUILayout.BeginHorizontal();
                GUILayout.Space(indentationSpace);
                
                GUILayout.BeginVertical();
                RenderFolders(folder.SubFolders);
                foreach(var asset in folder.Objects)
                {
                    RenderAssetButton(asset);
                }
                GUILayout.EndVertical();
                
                GUILayout.EndHorizontal();
            }
        }
    }
    
    private void RenderAssetButton(ScriptableObject obj)
    {
        if (obj == null)
        {
            RefreshScriptableObjects();
            return;
        }
        
        var buttonName = obj.name;
        var style = defaultAssetButton;
        if (objectSelected == obj)
        {
            style = selectedAssetButton;
            buttonName = "> " + buttonName;
        }

        var buttonPressed = false;
        GUILayout.BeginHorizontal();
        if (GUILayout.Button(buttonName, style))
        {
            buttonPressed = true;
        }
        
        if (showClassType)
        {
            GUILayout.FlexibleSpace();
            
            var type = obj.GetType();
            var typeName = type.Name;
            if (objectSelected == obj)
            {
                typeName += " <";
            }
            if (GUILayout.Button(typeName, style))
            {
                buttonPressed = true;
            }
        }

        if (buttonPressed)
        {
            objectSelected = obj;
            Selection.activeObject = obj;
            EditorGUIUtility.PingObject(objectSelected);
        }
        GUILayout.EndHorizontal();
    }

    private void DisplayInspector(float panelSize)
    {
        rightScrollPos = GUILayout.BeginScrollView(rightScrollPos, GUI.skin.box, GUILayout.Width(panelSize), GUILayout.ExpandHeight(true));

        if (objectSelected is ScriptableObject scriptableObject)
        {
            if (GUILayout.Button("Select Asset", GUILayout.ExpandWidth(true)))
            {
                Selection.activeObject = objectSelected;
                EditorGUIUtility.PingObject(objectSelected);
            }

            EditorGUILayout.Space();
            
            EditorGUI.BeginChangeCheck();
            var serializedObject = new SerializedObject(scriptableObject);
            serializedObject.Update();

            var iterator = serializedObject.GetIterator();
            var enterChildren = true;
            while(iterator.NextVisible(enterChildren))
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
        
        GUILayout.EndScrollView();
    }
    #endregion
}
#endif