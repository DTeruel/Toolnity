#if UNITY_EDITOR
using System.IO;
using UnityEngine;
using UnityEditor;

namespace Toolnity.Shortcuts
{
	[InitializeOnLoad]
	public static class Teleport
	{
		private const string UNDO_APPLY_MOVE_GAME_OBJECT_HERE = "Teleport";

		private static TeleportConfig config;
		public static TeleportConfig Config
		{
			get
			{
				if (config == null)
				{
					LoadOrCreateConfig();
				}

				return config;
			}
		}

		private static void LoadOrCreateConfig()
		{
			var allAssets = Resources.LoadAll<TeleportConfig>("");
			if (allAssets.Length > 0)
			{
				config = allAssets[0];
				return;
			}

			Debug.Log("[Teleport] No 'Teleport Config' file found in the Resources folders. Creating a new one in \"\\Assets\\Editor\\Resources\"");
            
			config = ScriptableObject.CreateInstance<TeleportConfig>();
			const string pathFolder = "Assets/Editor/Resources/";
			const string assetName = "Teleport Config.asset";
			if (!Directory.Exists("Assets/Editor/Resources"))
			{
				Directory.CreateDirectory("Assets/Editor/Resources");
			}
			AssetDatabase.CreateAsset(config, pathFolder + assetName);
			AssetDatabase.SaveAssets();
			AssetDatabase.Refresh();
		}

		private static GameObject[] selectedObjects;
		private static Vector3 positionToTeleport;

		static Teleport()
		{
			SceneView.duringSceneGui += _ => OnSceneGUI();
		}

		private static void OnSceneGUI()
		{
			if (!Event.current.shift || Event.current.keyCode != KeyCode.T)
			{
				return;
			}

			selectedObjects = Selection.gameObjects;
			if (selectedObjects.Length > 0)
			{
				Vector3 mousePosition = Event.current.mousePosition;
				mousePosition.y = SceneView.currentDrawingSceneView.camera.pixelHeight - mousePosition.y;
				mousePosition = SceneView.currentDrawingSceneView.camera.ScreenToWorldPoint(mousePosition);
				mousePosition.y = -mousePosition.y;

				var mouseRay = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
				var raycastHits = Physics.RaycastAll(mouseRay, 1000);

				if (raycastHits.Length > 0)
				{
					var currentDistance = float.MaxValue;
					var cameraPosition = SceneView.currentDrawingSceneView.camera.transform.position;

					for(var i = 0; i < raycastHits.Length; i++)
					{
						if (IsThisObjectSelected(raycastHits[i].collider.gameObject))
						{
							continue;
						}
						var checkDistance = Vector3.Distance(cameraPosition, raycastHits[i].point);
						if (checkDistance < currentDistance)
						{
							positionToTeleport = raycastHits[i].point;
							currentDistance = checkDistance;
						}
					}
				}
				else
				{
					positionToTeleport = HandleUtility.GUIPointToScreenPixelCoordinate(Event.current.mousePosition);
					positionToTeleport = SceneView.currentDrawingSceneView.camera.ScreenToWorldPoint(positionToTeleport);
				}

				TeleportObjects();
			}
		}

		private static bool IsThisObjectSelected(GameObject gameObjectToCheck)
		{
			for (var i = 0; i < selectedObjects.Length; i++)
			{
				if (selectedObjects[i] == gameObjectToCheck)
				{
					return true;
				}
			}

			return false;
		}

		private static void TeleportObjects()
		{
			for (var i = 0; i < selectedObjects.Length; i++)
			{
				var objectPosition = selectedObjects[i].transform.position;
				if (Config.CopyAxisX)
				{
					objectPosition.x = positionToTeleport.x;
				}
				if (Config.CopyAxisY)
				{
					objectPosition.y = positionToTeleport.y;
				}
				if (Config.CopyAxisZ)
				{
					objectPosition.z = positionToTeleport.z;
				}

				Undo.RecordObject(selectedObjects[i].transform, UNDO_APPLY_MOVE_GAME_OBJECT_HERE);
				selectedObjects[i].transform.position = objectPosition;
			}

		}
	}
}
#endif