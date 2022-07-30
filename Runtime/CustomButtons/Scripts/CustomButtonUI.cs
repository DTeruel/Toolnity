using Toolnity;
using UnityEngine;

public class CustomButtonUI : MonoBehaviour
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
