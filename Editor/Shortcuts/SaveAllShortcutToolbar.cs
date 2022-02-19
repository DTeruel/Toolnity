#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.Toolbars;
using UnityEngine;

namespace Toolnity
{
	[EditorToolbarElement(ID, typeof(SceneView))]
	public class SaveAllShortcutToolbar : EditorToolbarButton
	{
		public const string ID = "Toolnity/Save All Shortcut";

		public SaveAllShortcutToolbar()
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
