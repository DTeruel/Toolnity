#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.Toolbars;
using UnityEngine;

namespace Toolnity
{
	[EditorToolbarElement(ID, typeof(SceneView))]
	public class FindMissingScriptsToolbar : EditorToolbarButton
	{
		public const string ID = "Toolnity/Find Missing Scripts";

		public FindMissingScriptsToolbar()
		{
			text = "Find Missing Scripts";
			tooltip = "Find Missing Scripts in Selection";
			icon = EditorGUIUtility.IconContent("d_boo Script Icon").image as Texture2D;
			
			clicked += OnClick;
		}

		private static void OnClick()
		{
			FindMissingScripts.FindInSelected();
		}
	}
}
#endif
