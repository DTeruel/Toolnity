#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.Toolbars;
using UnityEngine;

namespace Toolnity.ProjectInfo
{
	[EditorToolbarElement(ID, typeof(SceneView))]
	public sealed class ProjectLinksToolbarButton : EditorToolbarDropdown, IAccessContainerWindow
	{
		public const string ID = "Toolnity/ProjectLinks";
		
		public EditorWindow containerWindow { get; set; }
		private static string dropChoice;

		public ProjectLinksToolbarButton()
		{
			text = "Project Links";
			icon = EditorGUIUtility.IconContent("BuildSettings.Web").image as Texture2D;
			clicked += ShowDropdown;
		}

		private static void ShowDropdown()
		{
			var menu = new GenericMenu();
			for (var i = 0; i < ProjectInfo.Config.Links.Count; i++)
			{
				var linkInfo = ProjectInfo.Config.Links[i];
				menu.AddItem(
					new GUIContent(linkInfo.Name), 
					false, 
					() => { Application.OpenURL(linkInfo.URL); });
			}
			menu.ShowAsContext();
		}
	}
}
#endif