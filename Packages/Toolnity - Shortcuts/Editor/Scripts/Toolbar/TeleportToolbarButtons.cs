#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Toolbars;
using UnityEngine;

namespace Toolnity.Shortcuts
{
	[EditorToolbarElement(ID, typeof(SceneView))]
	public class TeleportToolbarButtons : EditorToolbarDropdown
	{
		public const string ID = "Toolnity/Teleport";

		private static readonly List<string> OptionsList = new()
		{
			"Copy Coordinate X",
			"Copy Coordinate Y",
			"Copy Coordinate Z"
		};
		private static string dropChoice;

		public TeleportToolbarButtons()
		{
			text = "Teleport";
			tooltip = "Teleport (Shift+T)";
			icon = EditorGUIUtility.IconContent("MoveTool on").image as Texture2D;
			
			clicked += ShowDropdown;
		}

		private static void ShowDropdown()
		{
			var menu = new GenericMenu();
			menu.AddItem(
				new GUIContent(OptionsList[0]), 
				Teleport.Config.CopyAxisX,
				() =>
				{
					Teleport.Config.CopyAxisX = !Teleport.Config.CopyAxisX;
				});
			menu.AddItem(
				new GUIContent(OptionsList[1]), 
				Teleport.Config.CopyAxisY, 
				() =>
				{
					Teleport.Config.CopyAxisY = !Teleport.Config.CopyAxisY;
				});
			menu.AddItem(
				new GUIContent(OptionsList[2]), 
				Teleport.Config.CopyAxisZ, 
				() =>
				{
					Teleport.Config.CopyAxisZ = !Teleport.Config.CopyAxisZ;
				});
			menu.ShowAsContext();
		}
	}
}
#endif
