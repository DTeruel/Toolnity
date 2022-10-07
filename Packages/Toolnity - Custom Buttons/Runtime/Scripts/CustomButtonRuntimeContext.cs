using UnityEngine;

namespace Toolnity.CustomButtons
{
    public class CustomButtonRuntimeContext : MonoBehaviour
    {
        private void Start()
        {
            CustomButtonsMenu.AddCustomButtonsFromGameObject(gameObject);
        }

        private void OnDestroy()
        {
            CustomButtonsMenu.RemoveCustomButtonsFromGameObject(gameObject);
        }
    }
}