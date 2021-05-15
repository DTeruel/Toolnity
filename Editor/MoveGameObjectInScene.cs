#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

namespace Toolnity
{
	[InitializeOnLoad]
	public class MoveGameObjectInScene : Editor
	{
		private const string COPY_COORDINATE_X_OPTION_NAME = "Tools/Toolnity/Move GameObject/Copy X";
		private const string COPY_COORDINATE_Y_OPTION_NAME = "Tools/Toolnity/Move GameObject/Copy Y";
		private const string COPY_COORDINATE_Z_OPTION_NAME = "Tools/Toolnity/Move GameObject/Copy Z";

		private const string UNDO_APPLY_MOVE_GAMEOBJECT_HERE = "Move GameObject Here";

		private class ObjectMovement
		{
			public GameObject[] SelectedObjects;
			public Vector3 Position;
			public bool CopyCoordinateX = true;
			public bool CopyCoordinateY = true;
			public bool CopyCoordinateZ = true;
		}

		private static readonly ObjectMovement PluginData = new ObjectMovement();

		static MoveGameObjectInScene()
		{
			EditorApplication.delayCall += DelayCall;
			SceneView.duringSceneGui += sceneView => OnSceneGUI();
		}

		private static void DelayCall()
		{
			PluginData.CopyCoordinateX = EditorPrefs.GetBool(Application.dataPath + COPY_COORDINATE_X_OPTION_NAME, true);
			Menu.SetChecked(COPY_COORDINATE_X_OPTION_NAME, PluginData.CopyCoordinateX);
			PluginData.CopyCoordinateY = EditorPrefs.GetBool(Application.dataPath + COPY_COORDINATE_Y_OPTION_NAME, true);
			Menu.SetChecked(COPY_COORDINATE_Y_OPTION_NAME, PluginData.CopyCoordinateY);
			PluginData.CopyCoordinateZ = EditorPrefs.GetBool(Application.dataPath + COPY_COORDINATE_Z_OPTION_NAME, true);
			Menu.SetChecked(COPY_COORDINATE_Z_OPTION_NAME, PluginData.CopyCoordinateZ);
		}

		[MenuItem(COPY_COORDINATE_X_OPTION_NAME)]
		private static void ToggleCopyX()
		{
			PluginData.CopyCoordinateX = !PluginData.CopyCoordinateX;
			Menu.SetChecked(COPY_COORDINATE_X_OPTION_NAME, PluginData.CopyCoordinateX);
			EditorPrefs.SetBool(Application.dataPath + COPY_COORDINATE_X_OPTION_NAME, PluginData.CopyCoordinateX);
		}

		[MenuItem(COPY_COORDINATE_Y_OPTION_NAME)]
		private static void ToggleCopyY()
		{
			PluginData.CopyCoordinateY = !PluginData.CopyCoordinateY;
			Menu.SetChecked(COPY_COORDINATE_Y_OPTION_NAME, PluginData.CopyCoordinateY);
			EditorPrefs.SetBool(Application.dataPath + COPY_COORDINATE_Y_OPTION_NAME, PluginData.CopyCoordinateY);
		}

		[MenuItem(COPY_COORDINATE_Z_OPTION_NAME)]
		private static void ToggleCopyZ()
		{
			PluginData.CopyCoordinateZ = !PluginData.CopyCoordinateZ;
			Menu.SetChecked(COPY_COORDINATE_Z_OPTION_NAME, PluginData.CopyCoordinateZ);
			EditorPrefs.SetBool(Application.dataPath + COPY_COORDINATE_Z_OPTION_NAME, PluginData.CopyCoordinateZ);
		}

		private static void OnSceneGUI()
		{
			if (!Event.current.shift || Event.current.keyCode != KeyCode.G) return;

			PluginData.SelectedObjects = Selection.gameObjects;
			if (PluginData.SelectedObjects.Length == 0)
			{
				return;
			}
			else
			{
				Vector3 mousePosition = Event.current.mousePosition;
				mousePosition.y = SceneView.currentDrawingSceneView.camera.pixelHeight - mousePosition.y;
				mousePosition = SceneView.currentDrawingSceneView.camera.ScreenToWorldPoint(mousePosition);
				mousePosition.y = -mousePosition.y;

				var mouseRay = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
				var raycastHits = Physics.RaycastAll(mouseRay, 1000);

				if (raycastHits.Length > 0)
				{
					PluginData.Position = raycastHits[0].point;
				}
				else
				{
					PluginData.Position = HandleUtility.GUIPointToScreenPixelCoordinate(Event.current.mousePosition);
					PluginData.Position = SceneView.currentDrawingSceneView.camera.ScreenToWorldPoint(PluginData.Position);
				}

				MoveObjects();
			}
		}

		private static void MoveObjects()
		{
			for (var i = 0; i < PluginData.SelectedObjects.Length; i++)
			{
				var objectPosition = PluginData.SelectedObjects[i].transform.position;
				if (PluginData.CopyCoordinateX)
				{
					objectPosition.x = PluginData.Position.x;
				}
				if (PluginData.CopyCoordinateY)
				{
					objectPosition.y = PluginData.Position.y;
				}
				if (PluginData.CopyCoordinateZ)
				{
					objectPosition.z = PluginData.Position.z;
				}

				Undo.RecordObject(PluginData.SelectedObjects[i].transform, UNDO_APPLY_MOVE_GAMEOBJECT_HERE);
				PluginData.SelectedObjects[i].transform.position = objectPosition;
			}

		}
	}
}
#endif