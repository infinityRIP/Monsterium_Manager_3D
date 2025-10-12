// File: Singletons.cs
using UnityEngine;

namespace Game.Patterns 
{
    public abstract class CardSingleton<J> : MonoBehaviour where J : MonoBehaviour
    {
        public static J Instance { get; private set; }

        protected virtual void Awake()
        {
            if (Instance != null && Instance != (this as J))
            {
                Destroy(gameObject);
                return;
            }
            Instance = this as J;
        }

        protected virtual void OnApplicationQuit()
        {
            Instance = null;
        }
    }

    public abstract class PersistentSingleton<J> : CardSingleton<J> where J : MonoBehaviour
    {
        protected override void Awake()
        {
            base.Awake();
            DontDestroyOnLoad(gameObject);
        }
    }
}
