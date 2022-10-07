using UnityEngine;

namespace Toolnity.RuntimeScripts
{
	public class SingletonMonoBehaviour<T> : MonoBehaviour where T : MonoBehaviour
	{
		private static bool shuttingDown;
		private static readonly object Lock = new ();
		private static T myInstance;

		public static T Instance
		{
			get
			{
				if (shuttingDown)
				{
					return null;
				}

				if (myInstance != null)
				{
					return myInstance;
				}

				lock (Lock)
				{
					myInstance = (T) FindObjectOfType(typeof(T));
					if (myInstance != null)
					{
						return myInstance;
					}

					var singletonObject = new GameObject();
					var tmpInstance = singletonObject.AddComponent<T>();
					singletonObject.name = "[Singleton] " + typeof(T);

					myInstance = tmpInstance;

					return myInstance;
				}
			}
		}

		private void OnApplicationQuit()
		{
			shuttingDown = true;
		}

		private void OnDestroy()
		{
			shuttingDown = true;
		}

		public static void DontDestroyOnLoad()
		{
			DontDestroyOnLoad(Instance.gameObject);
		}
	}
}