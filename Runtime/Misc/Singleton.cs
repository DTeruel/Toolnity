using UnityEngine;

namespace Toolnity
{
	public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
	{
		private static bool shuttingDown;
		private static readonly object Lock = new object();
		private static T myInstance;

		public static T Instance
		{
			get
			{
				if (shuttingDown)
				{
					Debug.LogWarning("[Singleton] Instance '" + typeof(T) +
					                 "' already destroyed. Returning null.");
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

					DontDestroyOnLoad(singletonObject);

					myInstance = tmpInstance; // ensure myInstance is never partially initiated

					return myInstance;
				}
			}
		}

		private void Awake()
		{
			DontDestroyOnLoad(gameObject);
			OnSingletonAwake();
		}

		protected virtual void OnSingletonAwake()
		{
		}

		private void OnApplicationQuit()
		{
			shuttingDown = true;
		}

		private void OnDestroy()
		{
			shuttingDown = true;
			OnSingletonDestroy();
		}

		protected virtual void OnSingletonDestroy()
		{
		}
	}
}