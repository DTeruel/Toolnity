#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.Toolbars;
using UnityEngine;

namespace Toolnity
{
	[EditorToolbarElement(ID, typeof(SceneView))]
	public class FavoritesToolbar : EditorToolbarButton
	{
		public const string ID = "Toolnity/Favorites";

		public FavoritesToolbar()
		{
			text = "Favorites";
			tooltip = "Favorites";
			icon = EditorGUIUtility.IconContent("d_Favorite").image as Texture2D;
			
			clicked += OnClick;
		}

		private static void OnClick()
		{
			FavoritesPanel.ShowWindow();
		}
	}
}
#endif
