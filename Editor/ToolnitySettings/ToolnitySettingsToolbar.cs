#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.Toolbars;
using UnityEngine;

namespace Toolnity
{
	[EditorToolbarElement(ID, typeof(SceneView))]
	public class SettingsToolbar : EditorToolbarButton
	{
		public const string ID = "Toolnity/Settings";

		public SettingsToolbar()
		{
			text = "Settings";
			tooltip = "Settings";
			icon = EditorGUIUtility.IconContent("EditorSettings Icon").image as Texture2D;
			
			clicked += OnClick;
		}

		private static void OnClick()
		{
			ToolnitySettingsRegister.OpenProjectSettings();
		}
	}
}
#endif
