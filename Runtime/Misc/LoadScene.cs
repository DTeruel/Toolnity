using UnityEngine;
using UnityEngine.SceneManagement;

namespace Toolnity
{
	public class LoadScene : MonoBehaviour
	{
		[SerializeField] private string scene;

		public void Load()
		{
			SceneManager.LoadScene(scene);
		}
	}
}