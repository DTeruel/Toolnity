#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.Toolbars;
using UnityEngine;

namespace Toolnity
{
	[EditorToolbarElement(ID, typeof(SceneView))]
	public class PropPlacementToolbar : EditorToolbarButton
	{
		public const string ID = "Toolnity/Prop Placement Tool";

		public PropPlacementToolbar()
		{
			text = "Prop Placement Tool";
			tooltip = "Prop Placement Tool";
			icon = EditorGUIUtility.IconContent("d_TransformTool").image as Texture2D;
			
			clicked += OnClick;
		}

		private static void OnClick()
		{
			PropPlacementTool.ShowWindow();
		}
	}
}
#endif
