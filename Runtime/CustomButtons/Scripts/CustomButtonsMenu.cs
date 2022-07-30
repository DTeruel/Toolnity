using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Toolnity
{
    public class CustomButtonsMenu : Singleton<CustomButtonsMenu>
    {
        private const string StaticFolderName = "- Statics -";
        
        public enum CustomButtonPositionNames
        {
            TopRight = 0,
            TopLeft = 1,
            BottomRight = 2,
            BottomLeft = 3
        }

        private class CustomButtonInstance
        {
            public Button ButtonInstance;
            public GameObject GameObject;
            
            public CustomButtonInstance(Button buttonInstance, GameObject monoBehaviourGameObject)
            {
                ButtonInstance = buttonInstance;
                GameObject = monoBehaviourGameObject;
            }
        }

        private class CustomButtonFolder
        {
            public string Name;
            public Button ButtonInstance;
            public CustomButtonFolder ParentFolder;
            public List<CustomButtonFolder> Subfolders = new ();
            public List<CustomButtonInstance> Functions = new ();

            public CustomButtonFolder() { }

            public CustomButtonFolder(
                string name, 
                Button buttonInstance, 
                CustomButtonFolder parentFolder)
            {
                Name = name;
                ButtonInstance = buttonInstance;
                ParentFolder = parentFolder;
            }
        }

        [SerializeField] private EventSystem eventSystem;
        [SerializeField] private RectTransform mainPanel;
        [SerializeField] private VerticalLayoutGroup mainPanelVerticalLayoutGroup;
        [SerializeField] private RectTransform functionsPanel;
        [SerializeField] private RectTransform functionsContent;
        [SerializeField] private VerticalLayoutGroup functionsVerticalLayoutGroup;
        [SerializeField] private Button menuButton;
        [SerializeField] private Button backButton;
        [SerializeField] private Button folderTemplateButton;
        [SerializeField] private Button functionTemplateButton;

        private static CustomButtonsMenuConfig config;
        private readonly CustomButtonFolder mainFolder = new ();
        private CustomButtonFolder currentFolder;
        private List<Transform> folderButtons = new();
        private List<Transform> functionButtons = new();
        
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

        protected override void OnSingletonAwake()
        {
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
                    anchorsMin.y = 1;
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
                    anchorsMin.y = 1;
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
                    anchorsMax.y = 0;
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
                    anchorsMax.y = 0;
                    mainPanelVerticalLayoutGroup.childAlignment = TextAnchor.LowerCenter;
                    mainPanelVerticalLayoutGroup.reverseArrangement = true;
                    functionsVerticalLayoutGroup.childAlignment = TextAnchor.LowerCenter;
                    functionsVerticalLayoutGroup.reverseArrangement = true;
                    break;
            }
            mainPanel.pivot = pivot;
            mainPanel.anchorMin = anchorsMin;
            mainPanel.anchorMax = anchorsMax;
        }

        private void InitMainButtons()
        {
            menuButton.onClick.RemoveAllListeners();
            menuButton.onClick.AddListener(OpenMenu);
            backButton.onClick.RemoveAllListeners();
            backButton.onClick.AddListener(OnBackPressed);
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
            mainFolder.Name = "Main Menu";
            currentFolder = mainFolder;
            
            InitStaticButtons();
        }
        
        private void InitStaticButtons()
        {
            var staticFolder = AddFolder(StaticFolderName, mainFolder);
            
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
                            var pathAndFunctionName = GetStaticButtonName(allTypes[j], method, customButton);
                            CreateSubfoldersAndAddFunction(staticFolder, pathAndFunctionName, method);
                        }
                    }
                }
            }
        }
        
        private void CreateSubfoldersAndAddFunction(
            CustomButtonFolder folder, 
            string pathAndFunctionName, 
            MethodBase method, 
            MonoBehaviour monoBehaviour = null)
        {
            var subfolders = pathAndFunctionName.Split("/");
            var finalFolder = GetOrCreateSubfolders(subfolders, folder);
            var buttonName = subfolders[subfolders.Length - 1];
            
            AddFunction(finalFolder, buttonName, method, monoBehaviour);
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

        private void AddFunction(
            CustomButtonFolder folder, 
            string buttonName, 
            MethodBase method, 
            MonoBehaviour monoBehaviour)
        {
            var buttonInstance = Instantiate(functionTemplateButton, functionsContent);
            var buttonText = buttonInstance.GetComponentInChildren<Text>();
            buttonText.text = buttonName;
            
            buttonInstance.onClick.AddListener(() =>
            {
                method.Invoke(monoBehaviour, null);
            });

            GameObject monoGameObject = null;
            if (monoBehaviour != null)
            {
                monoGameObject = monoBehaviour.gameObject;
            }
            folder.Functions.Add(new CustomButtonInstance(buttonInstance, monoGameObject));
            
            functionButtons.Add(buttonInstance.transform);
            ReorderButtons();
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
            sizeDelta.y = Mathf.Max(totalSize, functionsPanel.rect.height);
            functionsContent.sizeDelta = sizeDelta;
        }

        private void OpenMenu()
        {
            menuButton.gameObject.SetActive(false);
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
            backButton.gameObject.SetActive(false);
            functionsPanel.gameObject.SetActive(false);
        }
        
        public static string GetStaticButtonName(Type currentClass, MethodBase method, CustomButton customButton)
        {
            var buttonName = "";
            if (!string.IsNullOrEmpty(customButton.NameFunction))
            {
                var methods = currentClass.GetMethods(
                    BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);

                for (var i = 0; i < methods.Length; i++)
                {
                    if (methods[i].Name == customButton.NameFunction && methods[i].ReturnType == typeof(string))
                    {
                        var nameFunction = methods[i].Invoke(null, null);
                        buttonName = nameFunction.ToString();
                    }
                }
				
                if (string.IsNullOrEmpty(buttonName))
                {
                    Debug.LogWarning("Static Method not found " + currentClass.Name + "::" + customButton.NameFunction);
                }
            }

            if (string.IsNullOrEmpty(buttonName) && !string.IsNullOrEmpty(customButton.Name))
            {
                buttonName = customButton.Name;
            }

            if (string.IsNullOrEmpty(buttonName))
            {
                buttonName = method.Name;
            }
			
            var finalName = "";
            if (customButton.UseClassNameAsPath)
            {
                finalName = currentClass.Name + "/";
            }
            finalName += buttonName;

            return finalName;
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
                        var pathAndFunctionName = GetNonStaticButtonName(monoType, method, customButton, mono);
                        
                        Instance.CreateSubfoldersAndAddFunction(Instance.mainFolder, pathAndFunctionName, method, mono);
                    }
                }
            }
            
            CloseMenu();
        }

        public static void RemoveCustomButtonsFromGameObject(GameObject mono)
        {
            if (Instance == null)
            {
                return;
            }
            Instance.RemoveCustomButtonsFromGameObjectInternal(mono);
        }

        private void RemoveCustomButtonsFromGameObjectInternal(GameObject mono)
        {
            SearchFunctionByGameObjectAndDestroy(mainFolder, mono);
            RemoveEmptyFolders(mainFolder);
            RemoveNullButtonReferences();
            CloseMenu();
        }

        private void SearchFunctionByGameObjectAndDestroy(CustomButtonFolder folder, GameObject mono)
        {
            for (var i = 0; i < folder.Subfolders.Count ; i++)
            {
                SearchFunctionByGameObjectAndDestroy(folder.Subfolders[i], mono);
            }
            
            for (var i = folder.Functions.Count - 1; i >= 0 ; i--)
            {
                if (folder.Functions[i].GameObject != mono)
                {
                    continue;
                }
                DestroyImmediate(folder.Functions[i].ButtonInstance.gameObject);
                folder.Functions.RemoveAt(i);
            }
        }

        private void RemoveEmptyFolders(CustomButtonFolder folder)
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
        }

        public static string GetNonStaticButtonName(Type currentClass, MethodBase method, CustomButton customButton, MonoBehaviour mono)
        {
            var buttonName = "";
            if (!string.IsNullOrEmpty(customButton.NameFunction))
            {
                var methods = currentClass.GetMethods(
                    BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

                for (var i = 0; i < methods.Length; i++)
                {
                    if (methods[i].Name == customButton.NameFunction && methods[i].ReturnType == typeof(string))
                    {
                        var nameFunction = methods[i].Invoke(mono, null);
                        buttonName = nameFunction.ToString();
                    }
                }
				
                if (string.IsNullOrEmpty(buttonName))
                {
                    Debug.LogWarning("Non Static Method not found " + currentClass.Name + "/" + customButton.NameFunction);
                }
            }

            if (string.IsNullOrEmpty(buttonName) && !string.IsNullOrEmpty(customButton.Name))
            {
                buttonName = customButton.Name;
            }

            if (string.IsNullOrEmpty(buttonName))
            {
                buttonName = method.Name;
            }

            var finalName = "";
            if (customButton.UseGameObjectNameAsPath)
            {
                finalName += mono.name + "/";
            }
            if (customButton.UseClassNameAsPath)
            {
                finalName += currentClass.Name + "/";
            }
            finalName += buttonName;

            return finalName;
        }
        
        private void ReorderButtons()
        {
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
    }
}