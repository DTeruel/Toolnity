using UnityEngine;

namespace Toolnity
{
	public class GameObjectRegister : MonoBehaviour
	{
		[SerializeField] private GameObjectsContainer container;

		private void OnEnable()
		{
			if (!container)
			{
				return;
			}
			
			container.Add(gameObject);
		}

		private void OnDisable()
		{
			if (!container)
			{
				return;
			}

			container.Remove(gameObject);
		}
	}
}