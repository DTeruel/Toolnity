#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.Toolbars;
using UnityEngine;

namespace Toolnity
{
	[EditorToolbarElement(ID, typeof(SceneView))]
	public class LightingToolbar : EditorToolbarDropdown
	{
		public const string ID = "Toolnity/Lightning";
		private static string dropChoice;

		public LightingToolbar()
		{
			text = "Lighting";
			tooltip = "Lighting Utils";
			icon = EditorGUIUtility.IconContent("d_DirectionalLight Icon").image as Texture2D;
			
			clicked += ShowDropdown;
		}

		private static void ShowDropdown()
		{
			var menu = new GenericMenu();
			for (var i = 0; i < LightsActivator.OptionsSelectionList.Count; i++)
			{
				var selection = LightsActivator.OptionsSelectionList[i];
				menu.AddItem(
					new GUIContent(selection), 
					false, 
					() => { LightsActivator.ApplyOption(selection); });
			}
			menu.ShowAsContext();
		}
	}
}
#endif