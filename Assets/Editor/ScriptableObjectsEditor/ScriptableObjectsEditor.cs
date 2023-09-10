using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;
using System.Linq;
using Object = UnityEngine.Object;

public class ScriptableObjectsEditor : EditorWindow
{
    private class FolderInfo
    {
        public string Name;
        public bool Folded;
        public List<FolderInfo> SubFolders;
        public List<ScriptableObject> Objects;
    }
    
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
    private readonly List<FolderInfo> assetFolders = new();

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
        CheckGUIStyles();

        DrawOptions();

        EditorGUILayout.Space();

        DrawAssets();
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

    #region DRAW OPTIONS
    private void DrawOptions()
    {
        EditorGUILayout.Space(25f);

        EditorGUILayout.BeginHorizontal();


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

            GUILayout.Label("Search In Packages: ");
            var newSearchInPackages = EditorGUILayout.Toggle(searchInPackages, GUILayout.Width(70f));
            if (searchInPackages != newSearchInPackages)
            {
                searchInPackages = newSearchInPackages;
                RefreshScriptableObjects();
            }
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
                    scriptableObjects.Add(asset);
                    
                    var folders = path.Split('/');
                    var folderPath = string.Join("/", folders.Take(folders.Length - 1));
                    var folderExist = false;
                    foreach(var folder in assetFolders)
                    {
                        if (folder.Name == folderPath)
                        {
                            folderExist = true;
                            folder.Objects.Add(asset);
                        }
                    }
                    if (!folderExist)
                    {
                        var newFolder = new FolderInfo
                        {
                            Name = folderPath,
                            Folded = true,
                            Objects = new List<ScriptableObject>
                            {
                                asset
                            }
                        };

                        assetFolders.Add(newFolder);
                    }
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

        DisplayScriptableObjects(panelSize);

        EditorGUILayout.Space(margins);

        DisplayInspector(panelSize);

        EditorGUILayout.Space(margins);

        GUILayout.EndHorizontal();
    }

    /*private void DisplayScriptableObjects()
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
    }*/
        
    private void DisplayScriptableObjects(float panelSize)
    { 
        leftScrollPos = GUILayout.BeginScrollView(leftScrollPos, GUI.skin.box, GUILayout.Width(panelSize), GUILayout.ExpandHeight(true));
        RenderFolders();
        GUILayout.EndScrollView();
    }

    private void RenderFolders()
    {
        const float indentationSpace = 15f;

        foreach(var folder in assetFolders)
        {
            folder.Folded = EditorGUILayout.Foldout(folder.Folded, folder.Name, true);
            if (folder.Folded)
            {
                //RenderAssetFolder(subFolder);
                foreach(var asset in folder.Objects)
                {
                    GUILayout.BeginHorizontal();
                    GUILayout.Space(indentationSpace);
                    RenderAssetButton(asset);
                    GUILayout.EndHorizontal();
                }
            }
        }
    }
    
   /* private void RenderAssetFolder(string folderPath)
    {
        if (folderPath == null || !assetFolders.ContainsKey(folderPath))
        {
            return;
        }

        var assetsInFolder = assetFolders[folderPath];

       * foreach(var asset in assetsInFolder)
        {
            RenderAssetButton(asset);
        }*

        foreach(var subFolder in assetFolders.Keys)
        {
            if (subFolder.StartsWith(folderPath) && subFolder != folderPath)
            {
                var subFolderPath = subFolder.Substring(folderPath.Length);
                if (subFolderPath.StartsWith("/"))
                {
                    subFolderPath = subFolderPath.Substring(1);
                }

                if (subFolderPath.Contains("/"))
                {
                    var subFolderName = subFolderPath.Split('/')[0];
                    if (EditorGUILayout.Foldout(true, subFolderName, true))
                    {
                        EditorGUI.indentLevel++;
                        RenderAssetFolder(subFolder);
                        EditorGUI.indentLevel--;
                    }
                }
            }
        }
    }*/

    private void RenderAssetButton(ScriptableObject obj)
    {
        var buttonName = obj.name;
        var style = defaultAssetButton;

        if (objectSelected == obj)
        {
            style = selectedAssetButton;
            buttonName = "> " + buttonName + " <";
        }

        if (GUILayout.Button(buttonName, style))
        {
            objectSelected = obj;
            EditorGUIUtility.PingObject(objectSelected);
        }
    }

    private void DisplayInspector(float panelSize)
    {
        rightScrollPos = GUILayout.BeginScrollView(rightScrollPos, GUI.skin.box, GUILayout.Width(panelSize), GUILayout.ExpandHeight(true));

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
