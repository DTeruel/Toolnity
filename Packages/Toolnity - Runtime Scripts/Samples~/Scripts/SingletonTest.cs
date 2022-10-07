using UnityEngine;

namespace Toolnity.Test
{
    public class SingletonTest : MonoBehaviour
    {
        private void Start()
        {
            SingletonMonoBehaviourClass.Instance.PublicMethod();
        }
    }
}