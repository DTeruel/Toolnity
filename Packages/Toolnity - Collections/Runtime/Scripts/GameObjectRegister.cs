using UnityEngine;

namespace Toolnity
{
	public class GameObjectRegister : MonoBehaviour
	{
		[SerializeField] private GameObjectsCollection collection;

		private void OnEnable()
		{
			if (!collection)
			{
				return;
			}
			
			collection.Add(gameObject);
		}

		private void OnDisable()
		{
			if (!collection)
			{
				return;
			}

			collection.Remove(gameObject);
		}
	}
}