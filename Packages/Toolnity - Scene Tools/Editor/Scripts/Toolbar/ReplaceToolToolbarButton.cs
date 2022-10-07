#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.Toolbars;
using UnityEngine;

namespace Toolnity.SceneTools
{
	[EditorToolbarElement(ID, typeof(SceneView))]
	public class ReplaceToolToolbarButton : EditorToolbarButton
	{
		public const string ID = "Toolnity/Replace Tool";

		public ReplaceToolToolbarButton()
		{
			text = "Replace Tool";
			tooltip = "Replace Tool";
			icon = EditorGUIUtility.IconContent("d_RotateTool On").image as Texture2D;
			
			clicked += OnClick;
		}

		private static void OnClick()
		{
			ReplaceTool.ShowWindow();
		}
	}
}
#endif
