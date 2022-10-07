#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.Toolbars;
using UnityEngine;

namespace Toolnity.ToDoList
{
	[EditorToolbarElement(ID, typeof(SceneView))]
	public class ToDoListToolbarButton : EditorToolbarDropdown
	{
		public const string ID = "Toolnity/ToDoList";
		private static string dropChoice;

		public ToDoListToolbarButton()
		{
			text = "ToDo";
			tooltip = "ToDo Lists";
			icon = EditorGUIUtility.IconContent("UnityEditor.SceneHierarchyWindow").image as Texture2D;
			
			clicked += ShowDropdown;
		}

		private static void ShowDropdown()
		{
			var menu = new GenericMenu();
			for (var i = 0; i < ToDoListSelector.NamesList.Count; i++)
			{
				var selection = ToDoListSelector.NamesList[i];
				menu.AddItem(
					new GUIContent(selection), 
					false, 
					() => { ToDoListSelector.SelectItem(selection); });
			}
			menu.ShowAsContext();
		}
	}
}
#endif