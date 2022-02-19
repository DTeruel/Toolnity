#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Toolbars;
using UnityEngine;

namespace Toolnity
{
	[EditorToolbarElement(ID, typeof(SceneView))]
	public class GameViewResolutionsToolbar : EditorToolbarDropdown
	{
		public const string ID = "Toolnity/Game View Resolutions";

		private static readonly List<string> OptionsList = new List<string>
		{
			"Add Portrait Aspect Ratios",
			"Add Portrait Resolutions",
			"Add Landscape Aspect Ratios",
			"Add Landscape Resolutions",
			"Clean Custom Resolutions"
		};
		private static string dropChoice;

		public GameViewResolutionsToolbar()
		{
			text = "Game View Resolutions";
			tooltip = "Game View Resolutions";
			icon = EditorGUIUtility.IconContent("d_BuildSettings.Standalone").image as Texture2D;
			
			clicked += ShowDropdown;
		}

		private static void ShowDropdown()
		{
			var menu = new GenericMenu();
			menu.AddItem(
				new GUIContent(OptionsList[0]), 
				false, 
				GameViewResolutions.AddPortraitAspectRatios);
			menu.AddItem(
				new GUIContent(OptionsList[1]), 
				false, 
				GameViewResolutions.AddPortraitResolutions);
			menu.AddItem(
				new GUIContent(OptionsList[2]), 
				false, 
				GameViewResolutions.AddLandscapeAspectRatios);
			menu.AddItem(
				new GUIContent(OptionsList[3]), 
				false, 
				GameViewResolutions.AddLandscapeResolutions);
			menu.AddItem(
				new GUIContent(OptionsList[4]), 
				false, 
				GameViewResolutions.CleanCustomResolutions);
			menu.ShowAsContext();
		}
	}
}
#endif
