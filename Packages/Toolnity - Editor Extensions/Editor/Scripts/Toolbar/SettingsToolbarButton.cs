#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.Toolbars;
using UnityEngine;

namespace Toolnity.EditorExtensions
{
	[EditorToolbarElement(ID, typeof(SceneView))]
	public class SettingsToolbarButton : EditorToolbarButton
	{
		public const string ID = "Toolnity/Editor Extensions Settings";

		public SettingsToolbarButton()
		{
			text = "Settings";
			tooltip = "Settings";
			icon = EditorGUIUtility.IconContent("EditorSettings Icon").image as Texture2D;
			
			clicked += OnClick;
		}

		private static void OnClick()
		{
			Settings.OpenProjectSettings();
		}
	}
}
#endif
