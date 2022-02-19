#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEditor;

namespace Toolnity
{
	public class FindInterestingGameObject : Editor
	{
		private const string UNDO_SELECT_INTERESTING_GAME_OBJECT = "Select Interesting Game Object";

		private static readonly List<InterestingGameObject> InterestingGameObjects = new List<InterestingGameObject>();
		private static int currentIndex;

		[MenuItem("Tools/Toolnity/Shortcuts/Select Next Interesting Game Object #N", priority = -500)]
		public static void SearchNextInterestingGameObject()
		{
			UpdateInterestingGameObjects();
			SelectNextInterestingGameObject();
		}

		private static void UpdateInterestingGameObjects()
		{
			var previousListSize = InterestingGameObjects.Count;
			RemoveMissing();
			AddNews();

			if (previousListSize != InterestingGameObjects.Count)
			{
				currentIndex = 0;
			}
		}

		private static void RemoveMissing()
		{
			for (var i = InterestingGameObjects.Count - 1; i >= 0; i--)
			{
				if (!InterestingGameObjects[i])
				{
					InterestingGameObjects.RemoveAt(i);
				}
			}
		}

		private static void AddNews()
		{
			var objects = FindObjectsOfType<InterestingGameObject>(true);
			for (var i = 0; i < objects.Length; i++)
			{
				if (!InterestingGameObjects.Contains(objects[i]))
				{
					InterestingGameObjects.Add(objects[i]);
				}
			}
		}

		private static void SelectNextInterestingGameObject()
		{
			if (InterestingGameObjects.Count == 0)
			{
				return;
			}
			
			Selection.activeGameObject = InterestingGameObjects[currentIndex].gameObject;
			
			Undo.RecordObject(InterestingGameObjects[currentIndex], UNDO_SELECT_INTERESTING_GAME_OBJECT);
			
			currentIndex = (currentIndex + 1) % InterestingGameObjects.Count;
		}
	}
}
#endif