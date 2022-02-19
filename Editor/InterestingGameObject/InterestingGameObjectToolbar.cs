#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.Toolbars;
using UnityEngine;

namespace Toolnity
{
	[EditorToolbarElement(ID, typeof(SceneView))]
	public class InterestingGameObjectToolbar : EditorToolbarButton
	{
		public const string ID = "Toolnity/Interesting Game Object";

		public InterestingGameObjectToolbar()
		{
			text = "Select Interesting Game Object";
			tooltip = "Select Interesting Game Object (Shift+N)";
			icon = EditorGUIUtility.IconContent("d_Selectable Icon").image as Texture2D;
			
			clicked += OnClick;
		}

		private static void OnClick()
		{
			FindInterestingGameObject.SearchNextInterestingGameObject();
		}
	}
}
#endif
