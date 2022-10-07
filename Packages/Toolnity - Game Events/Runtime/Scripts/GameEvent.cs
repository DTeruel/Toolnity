using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Toolnity
{
	[CreateAssetMenu(fileName = "Game Event", menuName = "Toolnity/Game Event")]
	public class GameEvent : ScriptableObject
	{
		private readonly List<UnityEvent> unityEvents = new List<UnityEvent>();

		public void Raise()
		{
			for (var i = unityEvents.Count - 1; i >= 0; i--)
			{
				unityEvents[i].Invoke();
			}
		}

		public void RegisterUnityEvent(UnityEvent unityEvent)
		{
			unityEvents.Add(unityEvent);
		}

		public void UnregisterUnityEvent(UnityEvent unityEvent)
		{
			unityEvents.Remove(unityEvent);
		}
	}
}