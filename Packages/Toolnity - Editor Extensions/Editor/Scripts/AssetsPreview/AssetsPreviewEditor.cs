#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace Toolnity.EditorExtensions
{
	[CustomEditor(typeof(AssetsPreview))]
	public class AssetsPreviewEditor : Editor
	{
		private GUIStyle numberStyle;
		
		private int currentFileType = -1;
		private int currentFileIndex = -1;

		public override void OnInspectorGUI()
		{
			var assetsPreview = (AssetsPreview)target;

			DrawSelectPathMenu(assetsPreview);
			DrawLoadedAssetsMenu(assetsPreview);
			DrawAssetMenu(assetsPreview);
		}

		private void DrawSelectPathMenu(AssetsPreview assetsPreview)
		{
			GUILayout.Space(10f);
			
			var recursive = GUILayout.Toggle(assetsPreview.Recursive, " Recursive search");
			if (assetsPreview.Recursive != recursive)
			{
				assetsPreview.Recursive = recursive;
				EditorUtility.SetDirty(target);
			}
			
			GUILayout.Space(5f);
			if (GUILayout.Button("Select Path"))
			{
				assetsPreview.SelectNewPath();
				EditorUtility.SetDirty(target);
				ResetVarsAfterRefreshingAssets(assetsPreview);
			}
		}
		private void OnValidate()
		{
			var assetsPreview = (AssetsPreview)target;
			assetsPreview.RefreshFiles();
			ResetVarsAfterRefreshingAssets(assetsPreview);
		}

		private void ResetVarsAfterRefreshingAssets(AssetsPreview assetsPreview)
		{
			currentFileType = -1;
			currentFileIndex = -1;

			for (var i = 0; i < (int)AssetsPreview.FileType.Count; i++)
			{
				var filesCount = assetsPreview.GetFilesCount(i);
				if (filesCount > 0)
				{
					currentFileType = i;
					Repaint();
					break;
				}
			}
		}

		private void DrawLoadedAssetsMenu(AssetsPreview assetsPreview)
		{
			if (string.IsNullOrEmpty(assetsPreview.Path))
			{
				return;
			}

			GUILayout.Space(5f);
			if (GUILayout.Button($"Refresh '{assetsPreview.Path}'"))
			{
				assetsPreview.RefreshFiles();
				ResetVarsAfterRefreshingAssets(assetsPreview);
			}
			
			GUILayout.Space(10f);
			UpdateShortcutToChangeFileType(assetsPreview);
			for (var i = 0; i < (int)AssetsPreview.FileType.Count; i++)
			{
				DrawFilesTypeMenu(assetsPreview, i);
			}
		}

		private void UpdateShortcutToChangeFileType(AssetsPreview assetsPreview)
		{
			if (Event.current.type == EventType.KeyDown)
			{
				if (Event.current.keyCode == KeyCode.DownArrow)
				{
					for (var i = 0; i < (int)AssetsPreview.FileType.Count; i++)
					{
						currentFileType++;
						currentFileType %= (int)AssetsPreview.FileType.Count;
						var filesCount = assetsPreview.GetFilesCount(currentFileType);
						if (filesCount > 0)
						{
							currentFileIndex = 0;
							assetsPreview.ShowFile(currentFileType, currentFileIndex);
							Repaint();
							break;
						}
					}
				}
				else if (Event.current.keyCode == KeyCode.UpArrow)
				{
					for (var i = 0; i < (int)AssetsPreview.FileType.Count; i++)
					{
						currentFileType--;
						if (currentFileType < 0)
						{
							currentFileType = (int)AssetsPreview.FileType.Count - 1;
						}
						var filesCount = assetsPreview.GetFilesCount(currentFileType);
						if (filesCount > 0)
						{
							currentFileIndex = 0;
							assetsPreview.ShowFile(currentFileType, currentFileIndex);
							Repaint();
							break;
						}
					}
				}
			}
		}

		private void DrawFilesTypeMenu(AssetsPreview assetsPreview, int fileType)
		{
			var leftKeyPressed = false;
			var rightKeyPressed = false;
			
			if (numberStyle == null)
			{
				numberStyle = new GUIStyle(EditorStyles.label)
				{
					alignment = TextAnchor.MiddleCenter, fixedWidth = 20f
				};
			}
			
			var filesCount = assetsPreview.GetFilesCount(fileType);
			var fileTypeSelected = currentFileType == fileType;
			EditorGUI.BeginDisabledGroup(filesCount <= 0);
			
			GUILayout.BeginHorizontal();

			if (currentFileType == fileType && Event.current.type == EventType.KeyDown)
			{
				leftKeyPressed = Event.current.keyCode == KeyCode.LeftArrow;
				rightKeyPressed = Event.current.keyCode == KeyCode.RightArrow;
			}
			
			if (GUILayout.Button("←", GUILayout.Width(20f)) || leftKeyPressed)
			{
				ShowPreviousFile(assetsPreview, fileType, filesCount);
			}
			if (fileTypeSelected && currentFileIndex >= 0)
			{
				GUILayout.Label((currentFileIndex + 1).ToString(), numberStyle);
			}
			else
			{
				GUILayout.Label("-", numberStyle);
			}
			if (GUILayout.Button("→", GUILayout.Width(20f)) || rightKeyPressed)
			{
				ShowNextFile(assetsPreview, fileType, filesCount);
			}

			GUILayout.Label(
				(fileTypeSelected ? "* " : "") +
				AssetsPreview.GetFileTypeName(fileType) + ": " +
				filesCount);
			
			GUILayout.EndHorizontal();
			EditorGUI.EndDisabledGroup();
		}

		private void ShowPreviousFile(AssetsPreview assetsPreview, int fileType, int filesCount)
		{
			if (currentFileType != fileType)
			{
				currentFileType = fileType;
				currentFileIndex = 0;
			}
			else
			{
				currentFileIndex--;
				if (currentFileIndex == -1)
				{
					currentFileIndex = filesCount - 1;
				}
			}

			assetsPreview.ShowFile(currentFileType, currentFileIndex);
			Repaint();
		}

		private void ShowNextFile(AssetsPreview assetsPreview, int fileType, int filesCount)
		{
			if (currentFileType != fileType)
			{
				currentFileType = fileType;
				currentFileIndex = 0;
			}
			else
			{
				currentFileIndex++;
				if (currentFileIndex == filesCount)
				{
					currentFileIndex = 0;
				}
			}

			assetsPreview.ShowFile(currentFileType, currentFileIndex);
			Repaint();
		}

		private void DrawAssetMenu(AssetsPreview assetsPreview)
		{
			if (string.IsNullOrEmpty(assetsPreview.Path) || !assetsPreview.HasAssetLoaded())
			{
				return;
			}
			
			GUILayout.Space(10f);
			if (GUILayout.Button(assetsPreview.GetCurrentAssetName()))
			{
				assetsPreview.SelectPathInProjectWindow();
			}
			GUILayout.Space(5f);
			
			assetsPreview.Position = EditorGUILayout.Slider("Position",
				assetsPreview.Position, -100f, 100f);
			
			assetsPreview.AutoRotateSpeed = EditorGUILayout.Slider("Autorotate",
				assetsPreview.AutoRotateSpeed, -10f, 10f);
			
			assetsPreview.Scale = EditorGUILayout.Slider("Scale",
				assetsPreview.Scale, 0.001f, 50f);
			
			GUILayout.BeginHorizontal();
			GUILayout.Label("Parent", GUILayout.Width(75f));
			if (GUILayout.Button("This Object"))
			{
				assetsPreview.SetThisAsParent();
			}
			if (GUILayout.Button("Main Camera"))
			{
				assetsPreview.SetMainCameraAsParent();
			}
			GUILayout.EndHorizontal();

			if (currentFileType is 
			    (int)AssetsPreview.FileType.Textures or (int)AssetsPreview.FileType.Models)
			{
				GUILayout.BeginHorizontal();
				GUILayout.Label("Material", GUILayout.Width(75f));
				if (GUILayout.Button("Use Lit"))
				{
					assetsPreview.UseLitMaterial(true);
				}
				if (GUILayout.Button("Use Unlit"))
				{
					assetsPreview.UseLitMaterial(false);
				}
				GUILayout.EndHorizontal();
			}
		}
	}
}
#endif