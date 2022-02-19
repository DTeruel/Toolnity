#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.Overlays;
using UnityEditor.Toolbars;
using UnityEngine;

namespace Toolnity
{
	[EditorToolbarElement(ID, typeof(SceneView))]
	public sealed class SceneSelectorToolbar : EditorToolbarDropdownToggle, IAccessContainerWindow
	{
		public const string ID = "Toolnity/SceneSelector";
		
		public EditorWindow containerWindow { get; set; }
		private static string dropChoice;

		public SceneSelectorToolbar()
		{
			text = "Scenes";
			tooltip = "Scene Selector (Toggle ON: Just Scenes In Build)";
			icon = EditorGUIUtility.IconContent("d_FolderOpened Icon").image as Texture2D;
			value = EditorPrefs.GetBool(Application.dataPath + SceneSelector.SCENE_SELECTOR_JUST_SCENES_IN_BUILD);
			
			dropdownClicked += ShowDropdown;
		}

		private void ShowDropdown()
		{
			var menu = new GenericMenu();
			
			EditorPrefs.SetBool(Application.dataPath + SceneSelector.SCENE_SELECTOR_JUST_SCENES_IN_BUILD, value);
			SceneSelector.UpdateScenes(value);
			
			for (var i = 0; i < SceneSelector.NamesList.Count; i++)
			{
				var selection = SceneSelector.NamesList[i];
				menu.AddItem(
					new GUIContent(selection), 
					false, 
					() => { SceneSelector.OpenSceneByName(selection); });
			}
			menu.ShowAsContext();
		}
	}
}
#endif