using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

namespace Toolnity.RuntimeScripts
{
	public class SceneChangeEvent : MonoBehaviour
	{
		[SerializeField] private UnityEvent onSceneChangeEvent;

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
			onSceneChangeEvent.Invoke();
		}
	}
}