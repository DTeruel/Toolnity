#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Toolbars;
using UnityEngine;

namespace Toolnity
{
	[EditorToolbarElement(ID, typeof(SceneView))]
	public class CameraShortcutsToolbar : EditorToolbarDropdown
	{
		public const string ID = "Toolnity/Camera Shortcuts";

		private static readonly List<string> OptionsList = new List<string>
		{
			"Camera Top-Bottom (F1)",
			"Camera Left-Right (F2)",
			"Camera Front-Back (F3)",
			"Camera Perspective-Orthographic (F4)"
		};
		private static string dropChoice;

		public CameraShortcutsToolbar()
		{
			text = "Camera Shortcuts";
			tooltip = "Camera Shortcuts";
			icon = EditorGUIUtility.IconContent("Camera Gizmo").image as Texture2D;
			
			clicked += ShowDropdown;
		}

		private static void ShowDropdown()
		{
			var menu = new GenericMenu();
			menu.AddItem(
				new GUIContent(OptionsList[0]), 
				false, 
				CameraShortcuts.SwitchTopBottomCamera);
			menu.AddItem(
				new GUIContent(OptionsList[1]), 
				false, 
				CameraShortcuts.SwitchLeftRightCamera);
			menu.AddItem(
				new GUIContent(OptionsList[2]), 
				false, 
				CameraShortcuts.SwitchFrontBackCamera);
			menu.AddItem(
				new GUIContent(OptionsList[3]), 
				false, 
				CameraShortcuts.PerspectiveOrthographicCamera);
			menu.ShowAsContext();
		}
	}
}
#endif
