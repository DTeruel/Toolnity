#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.Toolbars;
using UnityEngine;

namespace Toolnity
{
	[EditorToolbarElement(ID, typeof(SceneView))]
	public class ReplaceToolToolbar : EditorToolbarButton
	{
		public const string ID = "Toolnity/Replace Tool";

		public ReplaceToolToolbar()
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
