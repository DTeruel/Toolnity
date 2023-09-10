#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.Toolbars;
using UnityEngine;

namespace Toolnity.EditorExtensions
{
	[EditorToolbarElement(ID, typeof(SceneView))]
	public class ScriptableObjectViewerToolbarButton : EditorToolbarButton
	{
		public const string ID = "Toolnity/Scriptable Object Viewer";

		public ScriptableObjectViewerToolbarButton()
		{
			text = "SO Viewer";
			tooltip = "Scriptable Object Viewer";
			icon = EditorGUIUtility.IconContent("d_ScriptableObject Icon").image as Texture2D;
			
			clicked += OnClick;
		}

		private static void OnClick()
		{
			ScriptableObjectsViewer.ShowWindow();
		}
	}
}
#endif
