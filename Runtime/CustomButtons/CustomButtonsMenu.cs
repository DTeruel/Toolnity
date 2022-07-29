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
    public class CustomButtonsMenu : MonoBehaviour
    {
        private const string StaticFolderName = "- Statics -";
        
        public enum CustomButtonPositionNames
        {
            TopRight = 0,
            TopLeft = 1,
            BottomRight = 2,
            BottomLeft = 3
        }
        
        private class CustomButtonFolder
        {
            public string Name;
            public Button ButtonInstance;
            public CustomButtonFolder ParentFolder;
            public List<CustomButtonFolder> Subfolders = new ();
            public List<Button> Functions = new ();

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
            Debug.Log("AutoLoader");
            
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
            Debug.Log("Instantiating CustomButtonsMenu prefab");
            var customButtonsInstance = Resources.Load<CustomButtonsMenu>(nameof(CustomButtonsMenu));
            Instantiate(customButtonsInstance.gameObject);
        }

        private void Start()
        {
            CheckEventSystem();
            InitMenu();
        }

        private void CheckEventSystem()
        {
            Debug.Log("CheckEventSystem");
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
            Debug.Log("InitMenu");
            
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
        
        private void CreateSubfoldersAndAddFunction(CustomButtonFolder folder, string pathAndFunctionName, MethodBase method)
        {
            var subfolders = pathAndFunctionName.Split("/");
            var finalFolder = GetOrCreateSubfolders(subfolders, folder);
            var buttonName = subfolders[subfolders.Length - 1];
            
            AddFunction(finalFolder, buttonName, method);
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

        private void AddFunction(CustomButtonFolder folder, string buttonName, MethodBase method)
        {
            var buttonInstance = Instantiate(functionTemplateButton, functionsContent);
            var buttonText = buttonInstance.GetComponentInChildren<Text>();
            buttonText.text = buttonName;
            
            buttonInstance.onClick.AddListener(() =>
            {
                method.Invoke(null, null);
            });
            
            folder.Functions.Add(buttonInstance);
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
            Debug.Log("OpenMenu");
            menuButton.gameObject.SetActive(false);
            backButton.gameObject.SetActive(true);
            functionsPanel.gameObject.SetActive(true);
            
            OpenFolder(mainFolder);
        }

        private void OpenFolder(CustomButtonFolder folder)
        {
            Debug.Log("OpenFolder: " + folder.Name);
            
            DisableAllButtons();
            
            currentFolder = folder;
            
            var numButtons = ShowButtons(currentFolder);
            
            UpdateContentSize(numButtons);
        }

        private int ShowButtons(CustomButtonFolder folder)
        {
            var numButtons = 0;
            
            for (var i = 0; i < folder.Subfolders.Count; i++)
            {
                folder.Subfolders[i].ButtonInstance.gameObject.SetActive(true);
                numButtons++;
            }
            for (var i = 0; i < folder.Functions.Count; i++)
            {
                folder.Functions[i].gameObject.SetActive(true);
                numButtons++;
            }

            return numButtons;
        }

        private void OnBackPressed()
        {
            Debug.Log("OnBackPressed");
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
            Debug.Log("CloseMenu");
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
			
            buttonName = currentClass.Name + "/" + buttonName;
			
            return buttonName;
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
			
            buttonName = mono.name + "/" + currentClass.Name + "/" + buttonName;

            return buttonName;
        }
    }
}