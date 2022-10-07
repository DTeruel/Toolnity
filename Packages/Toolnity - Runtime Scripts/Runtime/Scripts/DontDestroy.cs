using UnityEngine;

namespace Toolnity.RuntimeScripts
{
	public class DontDestroy : MonoBehaviour
	{
		private void Awake()
		{
			DontDestroyOnLoad(this);
			enabled = false;
		}
	}
}