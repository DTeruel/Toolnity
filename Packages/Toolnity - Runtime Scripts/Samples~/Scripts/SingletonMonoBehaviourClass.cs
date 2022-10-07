using Toolnity.RuntimeScripts;
using UnityEngine;

namespace Toolnity.Test
{
    public class SingletonMonoBehaviourClass : SingletonMonoBehaviour<SingletonMonoBehaviourClass>
    {
        private void Awake()
        {
            Debug.Log("[Singleton Class] Awake");
        }

        public void PublicMethod()
        {
            Debug.Log("[Singleton Class] Public Method called from instance: " + gameObject.name);
        }
    }
}