using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

namespace Toolnity.RuntimeScripts
{
	public class OnSceneChangedEvent : MonoBehaviour
	{
		[SerializeField] private UnityEvent onSceneChangedEvent;

		private void OnEnable()
		{
			SceneManager.activeSceneChanged += SceneManagerOnActiveSceneChanged;
		}

		private void OnDisable()
		{
			SceneManager.activeSceneChanged -= SceneManagerOnActiveSceneChanged;
		}

		private void SceneManagerOnActiveSceneChanged(Scene _, Scene __)
		{
			onSceneChangedEvent.Invoke();
		}
	}
}