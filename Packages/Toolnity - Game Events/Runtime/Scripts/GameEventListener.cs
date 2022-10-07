using System;
using UnityEngine;
using UnityEngine.Events;

namespace Toolnity
{
	public class GameEventListener : MonoBehaviour
	{
		[Serializable]
		private struct GameEventListenerAndResponse
		{
			public GameEvent gameEvent;
			public UnityEvent response;
		}

		[SerializeField] private GameEventListenerAndResponse[] listenerAndResponses;

		private void OnEnable()
		{
			for (var i = 0; i < listenerAndResponses.Length; i++)
			{
				listenerAndResponses[i].gameEvent.RegisterUnityEvent(listenerAndResponses[i].response);
			}
		}

		private void OnDisable()
		{
			for (var i = 0; i < listenerAndResponses.Length; i++)
			{
				listenerAndResponses[i].gameEvent.UnregisterUnityEvent(listenerAndResponses[i].response);
			}
		}
	}
}