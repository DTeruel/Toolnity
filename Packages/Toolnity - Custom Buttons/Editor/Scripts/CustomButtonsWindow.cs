#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Toolnity.CustomButtons
{
    public class CustomButtonsWindow : EditorWindow
    {
	    private struct CustomButtonInfo
	    {
		    public string Path;
		    public string FunctionName;
		    public MethodInfo Method;
		    public object Object;
		    public KeyCode[] Shortcut;
	    }

	    private static CustomButtonsWindow window;
	    private static bool showStaticButtons = true;
	    private static bool showNonStaticButtons = true;
	    private static bool showOnlyShortcuts;
	    private static readonly List<CustomButtonInfo> StaticCustomButtons = new();
	    private static readonly List<CustomButtonInfo> NonStaticCustomButtons = new();

        [MenuItem("Tools/Toolnity/Custom Buttons Panel", priority = 3000)]
        public static void ShowWindow()
        {
            window = GetWindow<CustomButtonsWindow>("Custom Buttons");
            window.titleContent = new GUIContent("Custom Buttons", 
                EditorGUIUtility.IconContent("Selectable Icon").image);
            window.Show();
            UpdateFunctions();
        }
        
        private void OnGUI()
        {
	        DrawStaticButtons();
	        DrawNonStaticButtons();
            DrawHint();
            DrawRefresh();
        }

        private void OnEnable()
        {
	        UpdateFunctions();
        }

        private void OnFocus()
        {
	        UpdateFunctions();
        }

        private static void DrawStaticButtons()
        {
            EditorGUILayout.Separator();
            
            EditorGUILayout.BeginHorizontal();
            showStaticButtons = EditorGUILayout.Foldout(showStaticButtons , "STATIC CUSTOM BUTTONS", EditorStyles.foldoutHeader);
            GUILayout.FlexibleSpace();
            showOnlyShortcuts = GUILayout.Toggle(showOnlyShortcuts, " Only Shortcuts ");
            EditorGUILayout.EndHorizontal();

            if (showStaticButtons)
            {
	            DrawButtonsInfo(StaticCustomButtons);
            }
        }

        private static void DrawNonStaticButtons()
        {
	        EditorGUILayout.Separator();
            EditorGUILayout.Separator();
            showNonStaticButtons = EditorGUILayout.Foldout(showNonStaticButtons , "NON-STATIC CUSTOM BUTTONS", EditorStyles.foldoutHeader);
            
            if (showNonStaticButtons)
            {
	            DrawButtonsInfo(NonStaticCustomButtons);
            }
        }

        private static void DrawButtonsInfo(List<CustomButtonInfo> customButtonInfos)
        {
	        EditorGUI.indentLevel++;
	        var numberOfInfoDisplayed = 0;
	        foreach(var info in customButtonInfos)
	        {
		        if (showOnlyShortcuts && info.Shortcut == null)
		        {
			        continue;
		        }
		        EditorGUILayout.Separator();

		        EditorGUILayout.BeginHorizontal();
		        
		        EditorGUILayout.Space();
		        if (GUILayout.Button("►", GUILayout.Width(20f)))
		        {
			        info.Method.Invoke(info.Object, null);
		        }

		        numberOfInfoDisplayed++;
		        GUILayout.Label(info.FunctionName, EditorStyles.boldLabel);
		        
		        GUILayout.FlexibleSpace();
		        EditorGUILayout.EndHorizontal();
		        
		        if (info.Path != string.Empty && !showOnlyShortcuts)
		        {
			        DrawAttribute("Path:", info.Path);
		        }
		        
		        if (info.Shortcut != null)
		        {
			        var shortcuts = string.Empty;
			        foreach(var key in info.Shortcut)
			        {
				        if (shortcuts != string.Empty)
				        {
					        shortcuts += " + ";
				        }
				        shortcuts += key;
			        }
			        DrawAttribute("Shortcut:", shortcuts);
		        }
	        }

	        if (numberOfInfoDisplayed == 0)
	        {
		        EditorGUILayout.Separator();
		        EditorGUILayout.LabelField("- None -", EditorStyles.centeredGreyMiniLabel);
	        }
	        
	        EditorGUI.indentLevel--;
        }

        private static void DrawAttribute(string attribute, string info)
        {
	        EditorGUI.indentLevel++;
	        EditorGUILayout.BeginHorizontal();
	        EditorGUILayout.LabelField(attribute, EditorStyles.whiteLabel, GUILayout.Width(90f));
	        EditorGUILayout.LabelField(info);
	        EditorGUILayout.EndHorizontal();
	        EditorGUI.indentLevel--;
        }

        private static void DrawHint()
        {
	        GUILayout.FlexibleSpace();
	        EditorGUI.indentLevel++;
	        EditorGUILayout.LabelField("*** Shortcuts only will work on Runtime ***", EditorStyles.centeredGreyMiniLabel);
	        EditorGUI.indentLevel--;
        }

        private static void DrawRefresh()
        {
            if (GUILayout.Button("Refresh"))
            {
	            UpdateFunctions();
            }
            EditorGUILayout.Separator();
        }

        private static void UpdateFunctions()
        {
	        AddNonStaticCustomButtonsInScene();
	        AddStaticCustomButtonsInProject();
        }
	
		private static void AddNonStaticCustomButtonsInScene()
		{
			NonStaticCustomButtons.Clear();
			var allMonoBehaviours = Object.FindObjectsOfType<MonoBehaviour>();
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
						CustomButtonsMenu.GetNonStaticButtonName(monoType, method, customButton, mono, out var path, out var functionName);
						CustomButtonInfo info;
						info.Path = path;
						info.FunctionName = functionName;
						info.Method = method;
						info.Object = mono;
						info.Shortcut = customButton.Shortcut;
						NonStaticCustomButtons.Add(info);
					}
				}
			}
		}

		private static void AddStaticCustomButtonsInProject()
		{
			StaticCustomButtons.Clear();
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
							var method = methods[k];
							CustomButtonsMenu.GetStaticButtonPathAndName(allTypes[j], method, customButton, out var path, out var functionName);
							CustomButtonInfo info;
							info.Path = path;
							info.FunctionName = functionName;
							info.Method = method;
							info.Object = null;
							info.Shortcut = customButton.Shortcut;
							StaticCustomButtons.Add(info);
						}
					}
				}
			}
		}
    }
}
#endif