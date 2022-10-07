#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

namespace Toolnity.EditorExtensions
{
	public static class FindMissingScripts
	{
		private const int PRIORITY = 1;

		private static int goCount;
		private static int componentsCount;
		private static int missingCount;
		
		[MenuItem("GameObject/Toolnity/Find Missing Scripts in Selection", false, PRIORITY)]
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
			Debug.Log($"[Missing Scripts] Searched in {goCount} GameObjects and {componentsCount} components: Found {missingCount} missing scripts.");
		}

		private static void FindInGameObject(GameObject g)
		{
			goCount++;
			var components = g.GetComponents<Component>();
			for (var i = 0; i < components.Length; i++)
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
					Debug.Log("[Missing Scripts] Empty script attached in the object: '" + s + "'.", g);
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