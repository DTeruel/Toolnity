#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Toolbars;
using UnityEngine;

namespace Toolnity
{
	[EditorToolbarElement(ID, typeof(SceneView))]
	public class OnPlayToolbar : EditorToolbarDropdown
	{
		public const string ID = "Toolnity/On Play";
		
		private static readonly List<string> OptionsList = new List<string>
		{
			"Auto Save On Play",
			"Load Scene On Play"
		};
		
		private static string dropChoice;

		public OnPlayToolbar()
		{
			text = "On Play Events";
			tooltip = "On Play Events";
			icon = EditorGUIUtility.IconContent("SaveFromPlay").image as Texture2D;
			
			clicked += ShowDropdown;
		}

		private static void ShowDropdown()
		{
			var menu = new GenericMenu();
			menu.AddItem(
				new GUIContent(OptionsList[0]), 
				AutoSaveOnPlay.Enabled,
				AutoSaveOnPlay.ToggleEnabled);
			menu.AddItem(
				new GUIContent(OptionsList[1]), 
				LoadSceneOnPlay.LoadMasterOnPlay,
				LoadSceneOnPlay.ToggleEnabled);
			menu.ShowAsContext();
		}
	}
}
#endif
