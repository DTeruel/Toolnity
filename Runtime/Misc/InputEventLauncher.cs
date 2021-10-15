using System;
using UnityEngine;
using UnityEngine.Events;

namespace Toolnity
{
	public class InputEventLauncher : MonoBehaviour
	{
		[Serializable]
		public struct InputEvent
		{
			public KeyCode keyCode;
			public UnityEvent unityEvent;
		}

		[SerializeField] private InputEvent[] inputEventsList;

		private void Update()
		{
			for (var i = 0; i < inputEventsList.Length; i++)
			{
				if (Input.GetKeyDown(inputEventsList[i].keyCode))
				{
					inputEventsList[i].unityEvent?.Invoke();
				}
			}
		}
	}
}