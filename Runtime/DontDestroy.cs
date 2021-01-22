using UnityEngine;

namespace Toolnity
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