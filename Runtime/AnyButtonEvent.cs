using UnityEngine;
using UnityEngine.Events;

namespace Toolnity
{
	public class AnyButtonEvent : MonoBehaviour
	{
		[SerializeField] private UnityEvent _event;

		private void Update()
		{
			if (Input.anyKey)
			{
				_event.Invoke();
			}
		}
	}
}