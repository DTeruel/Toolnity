#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Toolnity.ToDoList
{
	[InitializeOnLoad]
	public static class ToDoListSelector
	{
		public static readonly List<string> NamesList = new ();

		private static readonly List<string> PathsList = new ();

		static ToDoListSelector()
		{
			UpdateToDoLists();
		}

		public static void SelectItem(string selection)
		{
			for (var i = 0; i < NamesList.Count; i++)
			{
				var option = NamesList[i];
				if (selection == option)
				{
					if (i == 0)
					{
						CreateNewTodoList();
					}
					else
					{
						var asset = AssetDatabase.LoadAssetAtPath<ToDoList>(PathsList[i]);
						Selection.activeObject = asset;
					}
					return;
				}
			}
		}

		private static void CreateNewTodoList()
		{  
			var asset = ScriptableObject.CreateInstance<ToDoList>();
			AssetDatabase.CreateAsset(asset, "Assets/[ToDo] List " + Random.Range(0, 1000).ToString("000") + ".asset");
			AssetDatabase.SaveAssets();
			Selection.activeObject = asset;
		}

		private static void UpdateToDoLists()
		{
			PathsList.Clear();
			PathsList.Add("");
			
			NamesList.Clear();
			NamesList.Add(" - Create New ToDo List -");
			GetToDoListsFromProject();
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