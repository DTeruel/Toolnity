#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Toolnity
{
	[InitializeOnLoad]
	public static class ToDoListSelector
	{
		public const string TODO_LIST_ENABLED = "Toolnity/To Do List/Enabled";

		private static bool showSceneLauncher;
		private static readonly List<string> NamesList = new List<string>();
		private static readonly List<string> PathsList = new List<string>();
		private static string buttonText;

		private static GUIStyle popupMiddleAlignment;

		static ToDoListSelector()
		{
			UpdateToDoLists();
		}

		public static void DrawGUI()
		{
			var enabledOption = EditorPrefs.GetBool(Application.dataPath + TODO_LIST_ENABLED, true);
			if (!enabledOption)
			{
				return;
			}
			
			GUILayout.BeginHorizontal();
			if (GUILayout.Button(buttonText, GUILayout.Width(25)))
			{
				showSceneLauncher = !showSceneLauncher;
				UpdateToDoLists();
			}
			
			if (showSceneLauncher)
			{
				buttonText = "X";

				CheckStyles();
				var newSelection = EditorGUILayout.Popup(0, NamesList.ToArray(), popupMiddleAlignment);
				if (newSelection > 0)
				{
					Selection.objects = new Object[] { AssetDatabase.LoadAssetAtPath<ToDoList>(PathsList[newSelection]) };
				}
			}
			else
			{
				buttonText = "T";
			}
			GUILayout.EndHorizontal();
		}

		private static void CheckStyles()
		{
			if (popupMiddleAlignment == null)
			{
				popupMiddleAlignment = GUI.skin.GetStyle("Popup");
				popupMiddleAlignment.alignment = TextAnchor.MiddleCenter;
				popupMiddleAlignment.fontSize = 12;
			}
		}

		public static void UpdateToDoLists()
		{
			NamesList.Clear();
			NamesList.Add(" - Select ToDo List -");
			
			PathsList.Clear();
			PathsList.Add("");
			
			GetToDoListsFromProject();

			if (NamesList.Count == 1)
			{
				NamesList[0] = " - No ToDo Lists Found -";
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

		internal class ToDoListDropdownPostprocessor : AssetPostprocessor
		{
			private static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets,
				string[] movedAssets, string[] movedFromAssetPaths)
			{
				UpdateToDoLists();
			}
		}
	}
}
#endif