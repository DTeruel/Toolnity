#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Toolbars;
using UnityEngine;

namespace Toolnity.EditorExtensions
{
	[EditorToolbarElement(ID, typeof(SceneView))]
	public class OnPlayToolbarButton : EditorToolbarDropdown
	{
		public const string ID = "Toolnity/On Play";
		
		private static readonly List<string> OptionsList = new()
		{
			"Auto Save On Play",
			"Load Scene On Play"
		};
		
		private static string dropChoice;

		public OnPlayToolbarButton()
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
				EditorExtensions.Config.SaveOnPlay,
				() =>
				{
					EditorExtensions.Config.SaveOnPlay = !EditorExtensions.Config.SaveOnPlay;
				});
			menu.AddItem(
				new GUIContent(OptionsList[1]), 
				EditorExtensions.Config.SceneOnPlay,
				LoadSceneOnPlay.SelectMasterScene);
			menu.ShowAsContext();
		}
	}
}
#endif
