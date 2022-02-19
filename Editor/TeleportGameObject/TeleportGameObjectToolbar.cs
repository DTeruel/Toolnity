#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Toolbars;
using UnityEngine;

namespace Toolnity
{
	[EditorToolbarElement(ID, typeof(SceneView))]
	public class TeleportGameObjectToolbar : EditorToolbarDropdown
	{
		public const string ID = "Toolnity/Teleport GameObject";

		private static readonly List<string> OptionsList = new List<string>
		{
			"Copy Coordinate X",
			"Copy Coordinate Y",
			"Copy Coordinate Z"
		};
		private static string dropChoice;

		public TeleportGameObjectToolbar()
		{
			text = "Teleport GameObject";
			tooltip = "Teleport GameObject (Shift+T)";
			icon = EditorGUIUtility.IconContent("MoveTool on").image as Texture2D;
			
			clicked += ShowDropdown;
		}

		private static void ShowDropdown()
		{
			var menu = new GenericMenu();
			menu.AddItem(
				new GUIContent(OptionsList[0]), 
				TeleportGameObject.PluginData.CopyCoordinateX, 
				TeleportGameObject.ToggleCopyX);
			menu.AddItem(
				new GUIContent(OptionsList[1]), 
				TeleportGameObject.PluginData.CopyCoordinateY, 
				TeleportGameObject.ToggleCopyY);
			menu.AddItem(
				new GUIContent(OptionsList[2]), 
				TeleportGameObject.PluginData.CopyCoordinateZ, 
				TeleportGameObject.ToggleCopyZ);
			menu.ShowAsContext();
		}
	}
}
#endif
