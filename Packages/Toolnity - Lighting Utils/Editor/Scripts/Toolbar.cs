#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using UnityEditor.Overlays;
using UnityEditor.Toolbars;

namespace Toolnity.Lighting
{
	[Overlay(typeof(SceneView), ID, ID, true)]
	public class Toolbar : ToolbarOverlay
	{
		private const string ID = "Toolnity Lighting Toolbar";
        
		public Toolbar() : base (LightingToolbarButton.ID)
		{
		}
	}
	
	[EditorToolbarElement(ID, typeof(SceneView))]
	public class LightingToolbarButton : EditorToolbarDropdown
	{
		public const string ID = "Toolnity/Lightning";
		private static string dropChoice;

		public LightingToolbarButton()
		{
			text = "Lighting";
			tooltip = "Lighting Utils";
			icon = EditorGUIUtility.IconContent("d_DirectionalLight Icon").image as Texture2D;
			
			clicked += ShowDropdown;
		}

		private static void ShowDropdown()
		{
			var menu = new GenericMenu();
			for (var i = 0; i < LightingUtils.OptionsSelectionList.Count; i++)
			{
				var selection = LightingUtils.OptionsSelectionList[i];
				menu.AddItem(
					new GUIContent(selection), 
					false, 
					() => { LightingUtils.ApplyOption(selection); });
			}
			menu.ShowAsContext();
		}
	}
}
#endif