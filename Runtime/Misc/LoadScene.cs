using UnityEngine;
using UnityEngine.SceneManagement;

namespace Toolnity
{
	public class LoadScene : MonoBehaviour
	{
		[SerializeField] private string scene;
		[SerializeField] private LoadSceneMode loadSceneMode = LoadSceneMode.Single;

		public void Load()
		{
			SceneManager.LoadScene(scene, loadSceneMode);
		}
	}
}