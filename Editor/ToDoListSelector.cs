#if UNITY_EDITOR
using System.Collections.Generic;
using Toolnity;
using UnityEditor;
using UnityEngine;

namespace EditorUtils
{
	[InitializeOnLoad]
	public class ToDoListSelector : EditorWindow
	{
		private const string ACTIVE_OPTION_NAME = "Tools/Toolnity/To Do List/Active";

		private static bool active;
		private static bool justScenesInBuild;
		private static bool showSceneLauncher;
		private static readonly List<string> NamesList = new List<string>();
		private static readonly List<string> PathsList = new List<string>();
		private static string buttonText;

		private static GUIStyle popupMiddleAlignment;

		static ToDoListSelector()
		{
			EditorApplication.delayCall += DelayCall;
			SceneView.duringSceneGui += OnSceneGUI;

			UpdateToDoLists();
		}

		private static void DelayCall()
		{
			active = EditorPrefs.GetBool(ACTIVE_OPTION_NAME, true);
			Menu.SetChecked(ACTIVE_OPTION_NAME, active);
		}

		[MenuItem(ACTIVE_OPTION_NAME)]
		public static void ToggleActive()
		{
			active = !active;
			Menu.SetChecked(ACTIVE_OPTION_NAME, active);
			EditorPrefs.SetBool(ACTIVE_OPTION_NAME, active);

			UpdateToDoLists();
		}

		private static void OnSceneGUI(SceneView sceneView)
		{
			if (!active)
			{
				return;
			}
			
			if (popupMiddleAlignment == null)
			{
				popupMiddleAlignment = GUI.skin.GetStyle("Popup");
				popupMiddleAlignment.alignment = TextAnchor.MiddleCenter;
				popupMiddleAlignment.fontSize = 12;
			}
			
			Handles.BeginGUI();
			GUILayout.BeginHorizontal();
			
			if (GUILayout.Button(buttonText))
			{
				showSceneLauncher = !showSceneLauncher;
				UpdateToDoLists();
			}
			
			if (showSceneLauncher)
			{
				buttonText = "<";

				var newSelection = EditorGUILayout.Popup(0, NamesList.ToArray(), popupMiddleAlignment);
				if (newSelection > 0)
				{
					Selection.objects = new Object[] { AssetDatabase.LoadAssetAtPath<ToDoList>(PathsList[newSelection]) };
				}
			}
			else
			{
				buttonText = ">";
			}

			GUILayout.FlexibleSpace();
			GUILayout.EndHorizontal();
			Handles.EndGUI();
		}

		public static void UpdateToDoLists()
		{
			NamesList.Clear();
			NamesList.Add(" - Select Asset -");
			
			PathsList.Clear();
			PathsList.Add("");
			
			GetToDoListsFromProject();

			if (NamesList.Count == 1)
			{
				NamesList[0] = " - No Assets Found -";
			}
		}
		
		private static void GetToDoListsFromProject()
		{
			var guids = AssetDatabase.FindAssets("t:ToDoList");
			for (var i = 0; i < guids.Length; i++)
			{
				var path = AssetDatabase.GUIDToAssetPath(guids[i]);
				var toDoList = AssetDatabase.LoadAssetAtPath<ToDoList>(path);
				if (toDoList)
				{
					NamesList.Add(toDoList.name);
					PathsList.Add(path);
				}
			}
		}
	}
}
#endif