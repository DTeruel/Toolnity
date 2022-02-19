#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.Toolbars;
using UnityEngine;

namespace Toolnity
{
	[EditorToolbarElement(ID, typeof(SceneView))]
	public class TransformRandomizerToolbar : EditorToolbarButton
	{
		public const string ID = "Toolnity/Transform Randomizer";

		public TransformRandomizerToolbar()
		{
			text = "Transform Randomizer";
			tooltip = "Transform Randomizer";
			icon = EditorGUIUtility.IconContent("d_TransformTool").image as Texture2D;
			
			clicked += OnClick;
		}

		private static void OnClick()
		{
			TransformRandomizer.ShowWindow();
		}
	}
}
#endif
