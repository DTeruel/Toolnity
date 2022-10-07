using UnityEngine;

namespace Toolnity.Test
{
    public class ScriptToCollectTest : MonoBehaviour
    {
        [SerializeField] private ScriptToCollectContainer container;

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