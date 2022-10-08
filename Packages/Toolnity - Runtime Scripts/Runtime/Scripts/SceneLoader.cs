using UnityEngine;
using UnityEngine.SceneManagement;

namespace Toolnity.RuntimeScripts
{
	public class SceneLoader : MonoBehaviour
	{
		[SerializeField] private string scene;
		[SerializeField] private LoadSceneMode loadSceneMode = LoadSceneMode.Single;

		public void Load()
		{
			SceneManager.LoadScene(scene, loadSceneMode);
		}
	}
}