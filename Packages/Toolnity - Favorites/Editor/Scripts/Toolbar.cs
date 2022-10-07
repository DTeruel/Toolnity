#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.Overlays;
using UnityEditor.Toolbars;
using UnityEngine;

namespace Toolnity.Favorites
{
	[Overlay(typeof(SceneView), ID, ID, true)]
	public class Toolbar : ToolbarOverlay
	{
		private const string ID = "Toolnity Favorites Toolbar";
        
		public Toolbar() : base (FavoritesToolbarButton.ID)
		{
		}
	}
	
	[EditorToolbarElement(ID, typeof(SceneView))]
	public class FavoritesToolbarButton : EditorToolbarButton
	{
		public const string ID = "Toolnity/Favorites";

		public FavoritesToolbarButton()
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