using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Toolnity.CustomButtons
{
    public partial class CustomButtonsMenu : MonoBehaviour
    {
        [SerializeField] private EventSystem eventSystem;
        [SerializeField] private RectTransform mainPanel;
        [SerializeField] private VerticalLayoutGroup mainPanelVerticalLayoutGroup;
        [SerializeField] private RectTransform functionsPanel;
        [SerializeField] private RectTransform functionsContent;
        [SerializeField] private VerticalLayoutGroup functionsVerticalLayoutGroup;
        [SerializeField] private Button menuButton;
        [SerializeField] private GameObject emptyPanel;
        [SerializeField] private Button backButton;
        [SerializeField] private Button folderTemplateButton;
        [SerializeField] private Button functionTemplateButton;

        private static CustomButtonsMenuConfig config;
        private readonly CustomButtonFolder mainFolder = new ();
        private CustomButtonFolder currentFolder;
        private readonly List<Transform> folderButtons = new();
        private readonly List<Transform> functionButtons = new();
        
        public static CustomButtonsMenuConfig Config
        {
            get
            {
                if (config == null)
                {
                    LoadOrCreateConfig();
                }

                return config;
            }
        }

        #region SINGLETON
        private static bool shuttingDown;
        private static readonly object Lock = new ();
        private static CustomButtonsMenu myInstance;

        public static CustomButtonsMenu Instance
        {
            get
            {
                if (shuttingDown)
                {
                    return null;
                }

                if (myInstance != null)
                {
                    return myInstance;
                }

                lock (Lock)
                {
                    myInstance = (CustomButtonsMenu) FindObjectOfType(typeof(CustomButtonsMenu));
                    if (myInstance != null)
                    {
                        return myInstance;
                    }

                    var singletonObject = new GameObject();
                    var tmpInstance = singletonObject.AddComponent<CustomButtonsMenu>();
                    singletonObject.name = "[Singleton] " + typeof(CustomButtonsMenu);

                    myInstance = tmpInstance;

                    return myInstance;
                }
            }
        }

        private void OnApplicationQuit()
        {
            shuttingDown = true;
        }

        private void OnDestroy()
        {
            shuttingDown = true;
        }
        #endregion
        
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        public static void AutoLoader()
        {
            var customButtonsInstance = FindObjectOfType<CustomButtonsMenu>();
            var enabledOption = Config.enabled;
            if (!enabledOption)
            {
                if (customButtonsInstance != null)
                {
                    customButtonsInstance.gameObject.SetActive(false);
                }
                return;
            }
            
            if (customButtonsInstance != null)
            {
                return;
            }

            InstantiatePrefab();
        }

        private static void LoadOrCreateConfig()
        {
            var allAssets = Resources.LoadAll<CustomButtonsMenuConfig>("");
            if (allAssets.Length > 0)
            {
                config = allAssets[0];
                return;
            }

            #if UNITY_EDITOR
            Debug.Log("[CustomButtonsMenu] No 'Custom Buttons Menu Config' file found in the Resources folders. Creating a new one in \"\\Assets\\Resources\"");
            
            config = ScriptableObject.CreateInstance<CustomButtonsMenuConfig>();
            const string pathFolder = "Assets/Resources/";
            const string assetName = "Custom Buttons Menu Config.asset";
            if (!Directory.Exists("Assets/Resources"))
            {
                Directory.CreateDirectory("Assets/Resources");
            }
            AssetDatabase.CreateAsset(config, pathFolder + assetName);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            #else
            Debug.LogError("[CustomButtonsMenu] No 'Custom Buttons Menu Config' file found in the Resources folders. Create one in the editor. ");
            #endif
        }

        private static void InstantiatePrefab()
        {
            var customButtonsInstance = Resources.Load<CustomButtonsMenu>(nameof(CustomButtonsMenu));
            Instantiate(customButtonsInstance.gameObject);
        }

        protected void Awake()
        {
            DontDestroyOnLoad(gameObject);
            LoadOrCreateConfig();
            CheckEventSystem();
            InitMenu();
        }

        private void CheckEventSystem()
        {
            if (EventSystem.current != null)
            {
                return;
            }

            var eventSystemInScene = FindObjectOfType<EventSystem>();
            if (eventSystemInScene != null)
            {
                return;
            }

            eventSystem.gameObject.SetActive(true);
        }

        private void InitMenu()
        {
            InitMenuPosition();
            InitMainButtons();
            InitMainFolder();
            CloseMenu();
            CheckCustomButtonsVisibility();
        }

        private void InitMenuPosition()
        {
            var pivot = mainPanel.pivot;
            var anchorsMin = mainPanel.anchorMin;
            var anchorsMax = mainPanel.anchorMax;
            
            switch (config.position)
            {
                case CustomButtonPositionNames.TopRight:
                    pivot.x = 1;
                    anchorsMin.x = 1;
                    anchorsMax.x = 1;
                    pivot.y = 1;
                    anchorsMin.y = 0;
                    anchorsMax.y = 1;
                    mainPanelVerticalLayoutGroup.childAlignment = TextAnchor.UpperCenter;
                    mainPanelVerticalLayoutGroup.reverseArrangement = false;
                    functionsVerticalLayoutGroup.childAlignment = TextAnchor.UpperCenter;
                    functionsVerticalLayoutGroup.reverseArrangement = false;
                    break;
                case CustomButtonPositionNames.TopLeft:
                    pivot.x = 0;
                    anchorsMin.x = 0;
                    anchorsMax.x = 0;
                    pivot.y = 1;
                    anchorsMin.y = 0;
                    anchorsMax.y = 1;
                    mainPanelVerticalLayoutGroup.childAlignment = TextAnchor.UpperCenter;
                    mainPanelVerticalLayoutGroup.reverseArrangement = false;
                    functionsVerticalLayoutGroup.childAlignment = TextAnchor.UpperCenter;
                    functionsVerticalLayoutGroup.reverseArrangement = false;
                    break;
                case CustomButtonPositionNames.BottomRight:
                    pivot.x = 1;
                    anchorsMin.x = 1;
                    anchorsMax.x = 1;
                    pivot.y = 0;
                    anchorsMin.y = 0;
                    anchorsMax.y = 1;
                    mainPanelVerticalLayoutGroup.childAlignment = TextAnchor.LowerCenter;
                    mainPanelVerticalLayoutGroup.reverseArrangement = true;
                    functionsVerticalLayoutGroup.childAlignment = TextAnchor.LowerCenter;
                    functionsVerticalLayoutGroup.reverseArrangement = true;
                    break;
                case CustomButtonPositionNames.BottomLeft:
                    pivot.x = 0;
                    anchorsMin.x = 0;
                    anchorsMax.x = 0;
                    pivot.y = 0;
                    anchorsMin.y = 0;
                    anchorsMax.y = 1;
                    mainPanelVerticalLayoutGroup.childAlignment = TextAnchor.LowerCenter;
                    mainPanelVerticalLayoutGroup.reverseArrangement = true;
                    functionsVerticalLayoutGroup.childAlignment = TextAnchor.LowerCenter;
                    functionsVerticalLayoutGroup.reverseArrangement = true;
                    break;
            }
            mainPanel.pivot = pivot;
            functionsContent.pivot = pivot;
            mainPanel.anchorMin = anchorsMin;
            mainPanel.anchorMax = anchorsMax;
        }

        private void InitMainButtons()
        {
            CheckMainButtonVisibility();
            menuButton.onClick.RemoveAllListeners();
            menuButton.onClick.AddListener(OpenMenu);
            backButton.onClick.RemoveAllListeners();
            backButton.onClick.AddListener(OnBackPressed);
        }

        private void CheckMainButtonVisibility()
        {
            if (config.mainButtonVisible)
            {
                return;
            }

            var image = menuButton.GetComponent<Image>();
            image.color = new Color(0, 0, 0, 0);
            var text = menuButton.GetComponentInChildren<Text>();
            text.gameObject.SetActive(false);
        }

        private void DisableAllButtons()
        {
            var childrenButtons = functionsContent.GetComponentsInChildren<Button>();
            for (var i = 0; i < childrenButtons.Length; i++)
            {
                childrenButtons[i].gameObject.SetActive(false);
            }
        }

        private void InitMainFolder()
        {
            mainFolder.Name = MAIN_FOLDER_NAME;
            currentFolder = mainFolder;
            
            InitStaticButtons();
        }

        private void CheckCustomButtonsVisibility()
        {
            var visible = functionButtons.Count > 0;
            mainPanel.gameObject.SetActive(visible);
        }
        
        private void InitStaticButtons()
        {
            var staticFolder = AddFolder(STATIC_FOLDER_NAME, mainFolder);
            
            var allAssemblies = AppDomain.CurrentDomain.GetAssemblies();
            for (var i = 0; i < allAssemblies.Length; i++)
            {
                var allTypes = allAssemblies[i].GetTypes();
                for (var j = 0; j < allTypes.Length; j++)
                {
                    var methods = allTypes[j].GetMethods(
                        BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);

                    for (var k = 0; k < methods.Length; k++)
                    {
                        if (Attribute.GetCustomAttribute(methods[k], typeof(CustomButton)) is CustomButton customButton)
                        {
                            if (!customButton.ShowInRuntime)
                            {
                                continue;
                            }
                            
                            var method = methods[k];
                            GetStaticButtonPathAndName(
                                allTypes[j], 
                                method, 
                                customButton, 
                                out var methodGetName,
                                out var path,
                                out var functionName);
                            CreateSubfoldersAndAddFunction(
                                staticFolder, 
                                path,
                                functionName,
                                method, 
                                methodGetName, 
                                customButton.Shortcut,
                                customButton.CloseMenuOnPressed,
                                customButton.NameFunctionExecType);
                        }
                    }
                }
            }
            
            RemoveEmptyFolders(mainFolder);
        }
        
        private void CreateSubfoldersAndAddFunction(
            CustomButtonFolder folder,
            string path,
            string functionName,
            MethodBase method,
            MethodInfo methodGetName,
            KeyCode[] shortcut,
            bool closeMenuOnPressed,
            NameFunctionExecType nameFunctionExecType,
            MonoBehaviour monoBehaviour = null)
        {
            var pathAndFunctionName = path + functionName;
            var subfolders = pathAndFunctionName.Split("/");
            var finalFolder = GetOrCreateSubfolders(subfolders, folder);
            var buttonName = subfolders[subfolders.Length - 1];
            
            AddFunction(
                finalFolder, 
                buttonName, 
                method, 
                monoBehaviour, 
                methodGetName, 
                shortcut,
                closeMenuOnPressed, 
                nameFunctionExecType);
        }
        
        private CustomButtonFolder GetOrCreateSubfolders(string[] subfolders, CustomButtonFolder finalFolder)
        {
            for (var i = 0; i < subfolders.Length - 1; i++)
            {
                var folderAlreadyCreated = false;
                for (var j = 0; j < finalFolder.Subfolders.Count; j++)
                {
                    if (finalFolder.Subfolders[j].Name != subfolders[i])
                    {
                        continue;
                    }

                    folderAlreadyCreated = true;
                    finalFolder = finalFolder.Subfolders[j];
                    break;
                }

                if (!folderAlreadyCreated)
                {
                    finalFolder = AddFolder(subfolders[i], finalFolder);
                }
            }
            
            return finalFolder;
        }

        private void AddFunction(CustomButtonFolder folder,
            string buttonName,
            MethodBase method,
            MonoBehaviour monoBehaviour,
            MethodInfo methodGetName,
            KeyCode[] shortcut,
            bool closeMenuOnPressed, 
            NameFunctionExecType nameFunctionExecType)
        {
            var buttonInstance = Instantiate(functionTemplateButton, functionsContent);
            var buttonText = buttonInstance.GetComponentInChildren<Text>();
            buttonText.text = buttonName;
            
            buttonInstance.onClick.AddListener(() =>
            {
                method.Invoke(monoBehaviour, null);
                if (closeMenuOnPressed)
                {
                    CloseMenu();
                }
            });

            var customButtonInstance = new CustomButtonInstance(
                buttonInstance, 
                monoBehaviour, 
                methodGetName,
                shortcut);
            if (nameFunctionExecType == NameFunctionExecType.OnPressed)
            {
                customButtonInstance.ButtonInstance.onClick.AddListener(() =>
                {
                    customButtonInstance.UpdateName();
                });
            }
            folder.Functions.Add(customButtonInstance);
            
            functionButtons.Add(buttonInstance.transform);
            ReorderButtons();
            CheckCustomButtonsVisibility();
        }

        private CustomButtonFolder AddFolder(string buttonName, CustomButtonFolder parentFolder)
        {
            var buttonInstance = Instantiate(folderTemplateButton, functionsContent);
            var buttonText = buttonInstance.GetComponentInChildren<Text>();
            buttonText.text = buttonName;
            var newButton = new CustomButtonFolder(
                buttonName,
                buttonInstance,
                parentFolder);
            
            buttonInstance.onClick.RemoveAllListeners();
            buttonInstance.onClick.AddListener(
                () =>
                {
                    OpenFolder(newButton);
                });
            
            parentFolder.Subfolders.Add(newButton);
            folderButtons.Add(buttonInstance.transform);
            ReorderButtons();
            CheckCustomButtonsVisibility();
            
            return newButton;
        }

        private void UpdateContentSize(int numButtons)
        {
            var buttonTransform = folderTemplateButton.GetComponent<RectTransform>();
            var totalButtonsHeight = numButtons * buttonTransform.rect.height;
            var totalSpacing = (numButtons - 1) * functionsVerticalLayoutGroup.spacing;
            var totalSize = totalButtonsHeight + totalSpacing + 
                functionsVerticalLayoutGroup.padding.top +
            functionsVerticalLayoutGroup.padding.bottom;
            
            var sizeDelta = functionsContent.sizeDelta;
            sizeDelta.y = totalSize;
            functionsContent.sizeDelta = sizeDelta;
        }

        private void OpenMenu()
        {
            menuButton.gameObject.SetActive(false);
            emptyPanel.SetActive(false);
            backButton.gameObject.SetActive(true);
            functionsPanel.gameObject.SetActive(true);
            
            OpenFolder(mainFolder);
        }

        private void OpenFolder(CustomButtonFolder folder)
        {
            DisableAllButtons();
            
            currentFolder = folder;
            
            var numButtons = ShowButtons(currentFolder);
            
            UpdateContentSize(numButtons);
        }

        private static int ShowButtons(CustomButtonFolder folder)
        {
            var numButtons = 0;
            
            for (var i = 0; i < folder.Subfolders.Count; i++)
            {
                folder.Subfolders[i].ButtonInstance.gameObject.SetActive(true);
                numButtons++;
            }
            for (var i = 0; i < folder.Functions.Count; i++)
            {
                folder.Functions[i].UpdateName();
                folder.Functions[i].ButtonInstance.gameObject.SetActive(true);
                numButtons++;
            }

            return numButtons;
        }

        private void OnBackPressed()
        {
            if (currentFolder.ParentFolder == null)
            {
                CloseMenu();
            }
            else
            {
                OpenFolder(currentFolder.ParentFolder);
            }
        }
        
        private void CloseMenu()
        {
            menuButton.gameObject.SetActive(true);
            emptyPanel.SetActive(true);
            backButton.gameObject.SetActive(false);
            functionsPanel.gameObject.SetActive(false);
        }
        
        public static void GetStaticButtonPathAndName(Type currentClass, MethodBase method, CustomButton customButton, out string path, out string name)
        {
            GetStaticButtonPathAndName(currentClass, method, customButton, out _, out path, out name);
        }

        private static void GetStaticButtonPathAndName(
            Type currentClass, 
            MethodBase method, 
            CustomButton customButton, 
            out MethodInfo methodGetName, 
            out string path, 
            out string name)
        {
            name = "";
            methodGetName = null;
            if (!string.IsNullOrEmpty(customButton.NameFunction))
            {
                var methods = currentClass.GetMethods(
                    BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);

                for (var i = 0; i < methods.Length; i++)
                {
                    if (methods[i].Name == customButton.NameFunction && methods[i].ReturnType == typeof(string))
                    {
                        methodGetName = methods[i];
                        var nameFunction = methods[i].Invoke(null, null);
                        name = nameFunction.ToString();
                    }
                }
				
                if (string.IsNullOrEmpty(name))
                {
                    Debug.LogWarning("Static Method not found " + currentClass.Name + "::" + customButton.NameFunction);
                }
            }

            if (string.IsNullOrEmpty(name) && !string.IsNullOrEmpty(customButton.Name))
            {
                name = customButton.Name;
            }

            if (string.IsNullOrEmpty(name))
            {
                name = method.Name;
            }
			
            path = "";
            if (customButton.PathAddClassName)
            {
                path += currentClass.Name + "/";
            }
            if (!string.IsNullOrEmpty(customButton.Path))
            {
                path += customButton.Path;
                if (customButton.Path[customButton.Path.Length - 1] != '/')
                {
                    path += "/";
                }
            }
        }

        public static void AddCustomButtonsFromGameObject(GameObject monoGameObject)
        {
            if (Instance == null)
            {
                return;
            }
            
            Instance.AddCustomButtonsFromGameObjectInternal(monoGameObject);
        }

        private void AddCustomButtonsFromGameObjectInternal(GameObject monoGameObject)
        {
            RemoveCustomButtonsFromGameObjectInternal(monoGameObject);
            
            var allMonoBehaviours = monoGameObject.GetComponentsInChildren<MonoBehaviour>(true);
            foreach (var mono in allMonoBehaviours)
            {
                var monoType = mono.GetType();
                var methods = monoType.GetMethods(
                    BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                for (var i = 0; i < methods.Length; i++) 
                {
                    if (Attribute.GetCustomAttribute(methods[i], typeof(CustomButton)) is CustomButton customButton)
                    {
                        var method = methods[i];
                        GetNonStaticButtonName(
                            monoType, 
                            method, 
                            customButton, 
                            mono, 
                            out var methodGetName, 
                            out var path, 
                            out var functionName);
                        CreateSubfoldersAndAddFunction(
                            mainFolder, 
                            path,
                            functionName,
                            method, 
                            methodGetName, 
                            customButton.Shortcut,
                            customButton.CloseMenuOnPressed, 
                            customButton.NameFunctionExecType,
                            mono);
                    }
                }
            }
            
            CloseMenu();
        }

        public static void RemoveCustomButtonsFromGameObject(GameObject monoGameObject)
        {
            if (Instance == null)
            {
                return;
            }
            Instance.RemoveCustomButtonsFromGameObjectInternal(monoGameObject);
        }

        private void RemoveCustomButtonsFromGameObjectInternal(GameObject monoGameObject)
        {
            SearchFunctionByGameObjectAndDestroy(mainFolder, monoGameObject);
            RemoveEmptyFolders(mainFolder);
            RemoveNullButtonReferences();
            CloseMenu();
        }

        private static void SearchFunctionByGameObjectAndDestroy(CustomButtonFolder folder, GameObject monoGameObject)
        {
            for (var i = 0; i < folder.Subfolders.Count ; i++)
            {
                SearchFunctionByGameObjectAndDestroy(folder.Subfolders[i], monoGameObject);
            }
            
            for (var i = folder.Functions.Count - 1; i >= 0 ; i--)
            {
                if (folder.Functions[i].StaticFunction ||
                    (folder.Functions[i].Mono != null &&
                    folder.Functions[i].Mono.gameObject != monoGameObject))
                {
                    continue;
                }
                DestroyImmediate(folder.Functions[i].ButtonInstance.gameObject);
                folder.Functions.RemoveAt(i);
            }
        }

        private static void RemoveEmptyFolders(CustomButtonFolder folder)
        {
            for (var i = 0; i < folder.Subfolders.Count; i++)
            {
                RemoveEmptyFolders(folder.Subfolders[i]);
            }
            for (var i = folder.Subfolders.Count - 1; i >= 0; i--)
            {
                if(folder.Subfolders[i].Functions.Count > 0 || folder.Subfolders[i].Subfolders.Count > 0)
                {
                    continue;
                }
                
                DestroyImmediate(folder.Subfolders[i].ButtonInstance.gameObject);
                folder.Subfolders.RemoveAt(i);
            }
        }

        private void RemoveNullButtonReferences()
        {
            for (var i = folderButtons.Count - 1; i >= 0 ; i--)
            {
                if (folderButtons[i] == null)
                {
                    folderButtons.RemoveAt(i);
                }
            }
            
            for (var i = functionButtons.Count - 1; i >= 0 ; i--)
            {
                if (functionButtons[i] == null)
                {
                    functionButtons.RemoveAt(i);
                }
            }
            CheckCustomButtonsVisibility();
        }

        public static void GetNonStaticButtonName(
            Type currentClass, 
            MethodBase method, 
            CustomButton customButton, 
            MonoBehaviour mono, 
            out string path, 
            out string functionName)
        {
            GetNonStaticButtonName(currentClass, method, customButton, mono, out _, out path, out functionName);
        }

        private static void GetNonStaticButtonName(
            Type currentClass, 
            MethodBase method, 
            CustomButton customButton, 
            MonoBehaviour mono, 
            out MethodInfo methodGetName,
            out string path,
            out string functionName)
        {
            functionName = "";
            methodGetName = null;
            if (!string.IsNullOrEmpty(customButton.NameFunction))
            {
                var methods = currentClass.GetMethods(
                    BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

                for (var i = 0; i < methods.Length; i++)
                {
                    if (methods[i].Name == customButton.NameFunction && methods[i].ReturnType == typeof(string))
                    {
                        if (customButton.NameFunctionExecType != NameFunctionExecType.OnCreation)
                        {
                            methodGetName = methods[i];
                        }
                        var nameFunction = methods[i].Invoke(mono, null);
                        functionName = nameFunction.ToString();
                    }
                }
				
                if (string.IsNullOrEmpty(functionName))
                {
                    Debug.LogWarning("Non Static Method not found " + currentClass.Name + "/" + customButton.NameFunction);
                }
            }

            if (string.IsNullOrEmpty(functionName) && !string.IsNullOrEmpty(customButton.Name))
            {
                functionName = customButton.Name;
            }

            if (string.IsNullOrEmpty(functionName))
            {
                functionName = method.Name;
            }

            path = "";
            if (customButton.PathAddClassName)
            {
                path += currentClass.Name + "/";
            }
            if (customButton.PathAddGameObjectName)
            {
                path += mono.name + "/";
            }
            if (!string.IsNullOrEmpty(customButton.Path))
            {
                path += customButton.Path;
                if (customButton.Path[customButton.Path.Length - 1] != '/')
                {
                    path += "/";
                }
            }
        }
        
        private void ReorderButtons()
        {
            RemoveNullButtonReferences();
            
            var index = 0;
            for (var i = 0; i < folderButtons.Count; i++)
            {
                folderButtons[i].SetSiblingIndex(index);
                index++;
            }
            
            for (var i = 0; i < functionButtons.Count; i++)
            {
                functionButtons[i].SetSiblingIndex(index);
                index++;
            }
        }

        private void Update()
        {
            UpdateShortcutsInFolder(mainFolder);
        }

        private void UpdateShortcutsInFolder(CustomButtonFolder folder)
        {
            foreach(var function in folder.Functions)
            {
                UpdateFunctionShortcut(function);
            }

            foreach(var subfolder in folder.Subfolders)
            {
                UpdateShortcutsInFolder(subfolder);
            }
        }

        private void UpdateFunctionShortcut(CustomButtonInstance function)
        {
            if (function.Shortcut == null || function.Shortcut.Length == 0)
            {
                return;
            }

            foreach(var key in function.Shortcut)
            {
                var checkJustPressed = true;

                switch (key)
                {
                    case KeyCode.RightShift:
                    case KeyCode.LeftShift:
                    case KeyCode.RightControl:
                    case KeyCode.LeftControl:
                    case KeyCode.RightAlt:
                    case KeyCode.LeftAlt:
                    case KeyCode.LeftMeta:
                    case KeyCode.LeftWindows:
                    case KeyCode.RightMeta:
                    case KeyCode.RightWindows:
                    case KeyCode.AltGr:
                        checkJustPressed = false;
                        break;
                }

                if (checkJustPressed)
                {
                    if (!Input.GetKeyDown(key))
                    {
                        return;
                    }
                }
                else
                {
                    if (!Input.GetKey(key))
                    {
                        return;
                    }
                }
            }

            function.ButtonInstance.onClick?.Invoke();
        }
    }
}