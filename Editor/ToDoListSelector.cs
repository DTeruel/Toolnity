﻿#if UNITY_EDITOR
using System.Collections.Generic;
using Toolnity;
using UnityEditor;
using UnityEngine;

namespace Toolnity
{
	[InitializeOnLoad]
	public class ToDoListSelector : EditorWindow
	{
		public const string TODO_LIST_ENABLED = "Toolnity/To Do List/Enabled";

		private static bool showSceneLauncher;
		private static readonly List<string> NamesList = new List<string>();
		private static readonly List<string> PathsList = new List<string>();
		private static string buttonText;

		private static GUIStyle popupMiddleAlignment;

		static ToDoListSelector()
		{
			SceneView.duringSceneGui += OnSceneGUI;

			UpdateToDoLists();
		}

		private static void OnSceneGUI(SceneView sceneView)
		{
			var enabledOption = EditorPrefs.GetBool(Application.dataPath + TODO_LIST_ENABLED, true);
			if (!enabledOption)
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
			GUILayout.BeginVertical();
			GUILayout.FlexibleSpace();
			GUILayout.BeginHorizontal();
			GUILayout.FlexibleSpace();
			
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
					showSceneLauncher = false;
				}
			}
			else
			{
				buttonText = "T";
			}

			GUILayout.FlexibleSpace();
			GUILayout.EndHorizontal();
			GUILayout.Space(25);
			GUILayout.EndVertical();
			Handles.EndGUI();
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