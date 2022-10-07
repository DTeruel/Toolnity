#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.Toolbars;
using UnityEngine;

namespace Toolnity.Shortcuts
{
	[EditorToolbarElement(ID, typeof(SceneView))]
	public class SaveAllToolbarButton : EditorToolbarButton
	{
		public const string ID = "Toolnity/Shortcuts Save All";

		public SaveAllToolbarButton()
		{
			text = "Save All";
			tooltip = "Save All";
			icon = EditorGUIUtility.IconContent("d_SaveAs").image as Texture2D;
			
			clicked += OnClick;
		}

		private static void OnClick()
		{
			BasicShortcuts.SaveAll();
		}
	}
}
#endif
