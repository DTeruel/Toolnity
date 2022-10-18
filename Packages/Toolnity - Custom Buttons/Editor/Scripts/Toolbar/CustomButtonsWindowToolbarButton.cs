#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.Toolbars;
using UnityEngine;

namespace Toolnity.CustomButtons
{
	[EditorToolbarElement(ID, typeof(SceneView))]
	public class CustomButtonsWindowToolbarButton : EditorToolbarButton
	{
		public const string ID = "Toolnity/Custom Buttons Window";

		public CustomButtonsWindowToolbarButton()
		{
			text = "Custom Buttons Window";
			tooltip = "Custom Buttons Window";
			icon = EditorGUIUtility.IconContent("Selectable Icon").image as Texture2D;
			
			clicked += OnClick;
		}

		private static void OnClick()
		{
			CustomButtonsWindow.ShowWindow();
		}
	}
}
#endif
