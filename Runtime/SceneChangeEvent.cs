using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

namespace Toolnity
{
    public class SceneChangeEvent : MonoBehaviour
    {
        [SerializeField] private UnityEvent onSceneChangeEvent;

        private void OnEnable()
        {
            SceneManager.activeSceneChanged += SceneManagerOnActiveSceneChanged;
        }

        private void OnDisable()
        {
            SceneManager.activeSceneChanged -= SceneManagerOnActiveSceneChanged;
        }

        private void SceneManagerOnActiveSceneChanged(Scene arg0, Scene arg1)
        {
            onSceneChangeEvent.Invoke();
        }
    }
}