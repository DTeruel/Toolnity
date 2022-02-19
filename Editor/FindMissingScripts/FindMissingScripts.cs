#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

namespace Toolnity
{
	public static class FindMissingScripts
	{
		private static int goCount;
		private static int componentsCount;
		private static int missingCount;
		
		[MenuItem("Tools/Toolnity/Find Missing Scripts in Selection", false, -100)]
		public static void FindInSelected()
		{
			var go = Selection.gameObjects;
			goCount = 0;
			componentsCount = 0;
			missingCount = 0;
			foreach(var g in go)
			{
				FindInGameObject(g);
			}
			Debug.Log($"Searched in {goCount} GameObjects: {componentsCount} components, found {missingCount} missing scripts.");
		}

		private static void FindInGameObject(GameObject g)
		{
			goCount++;
			var components = g.GetComponents<Component>();
			for (int i = 0; i < components.Length; i++)
			{
				componentsCount++;
				if (components[i] == null)
				{
					missingCount++;
					var s = g.name;
					var t = g.transform;
					while(t.parent != null)
					{
						var parent = t.parent;
						s = parent.name + "/" + s;
						t = parent;
					}
					Debug.Log(s + " has an empty script attached in position: " + i, g);
				}
			}

			foreach(Transform childT in g.transform)
			{
				FindInGameObject(childT.gameObject);
			}
		}
	}
}
#endif