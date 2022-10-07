#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Toolnity.Shortcuts
{
	internal sealed class SceneObjectSelectorPopup : PopupWindowContent
	{
		private const float WINDOW_WIDTH = 300f;
		private const float BUTTON_HEIGHT = 20f;
		
		private List<GameObject> objectsToShow;
		private readonly List<Texture2D> thumbnails = new();
		private GUIStyle buttonStyle;
		private GUIStyle boxStyle;
		private Vector2 windowSize;

		public void Init(List<GameObject> gameObjectsPicked)
		{
			objectsToShow = gameObjectsPicked;

			GenerateThumbnails();
			SetWindowSizeAndPosition();
		}

		private void CheckStyles()
		{
			if (buttonStyle == null)
			{
				buttonStyle = new GUIStyle(GUI.skin.GetStyle("Button"))
				{
					fixedHeight = BUTTON_HEIGHT
				};
			}

			if (boxStyle == null)
			{
				boxStyle = new GUIStyle(GUI.skin.GetStyle("Box"))
				{
					fixedWidth = BUTTON_HEIGHT,
					fixedHeight = BUTTON_HEIGHT
				};
			}
		}

		private void GenerateThumbnails()
		{
			thumbnails.Clear();
			for (var i = 0; i < objectsToShow.Count; i++)
			{
				thumbnails.Add(AssetPreview.GetMiniThumbnail(objectsToShow[i]));
			}
		}

		private void SetWindowSizeAndPosition()
		{
			windowSize = new Vector2(WINDOW_WIDTH, 2 + objectsToShow.Count * 2 + objectsToShow.Count * BUTTON_HEIGHT);
		}

		public override void OnGUI(Rect rect)
		{
			CheckStyles();
			
			for (var i = 0; i < objectsToShow.Count; i++)
			{
				GUILayout.BeginHorizontal();
				GUILayout.Box(thumbnails[i], boxStyle);
				if (GUILayout.Button(objectsToShow[i].name, buttonStyle))
				{
					Selection.activeGameObject = objectsToShow[i];
					ClosePopup();
				}
				GUILayout.EndHorizontal();
			}
			
			editorWindow.minSize = windowSize;
			editorWindow.maxSize = windowSize;
			editorWindow.Repaint();
		}

		private void ClosePopup()
		{
			if (editorWindow)
			{
				editorWindow.Close();
			}

			GUIUtility.ExitGUI();
		}
	}
}
#endif
