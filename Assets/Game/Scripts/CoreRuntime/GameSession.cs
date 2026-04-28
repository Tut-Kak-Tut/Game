using UnityEngine;

namespace Game.CoreRuntime
{
    public class GameSession : MonoBehaviour
    {
        public static GameSession Instance { get; private set; }

        public IEventBus EventBus { get; private set; }
        public SaveService SaveService { get; private set; }

        [SerializeField] private bool dontDestroyOnLoad = true;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            EventBus = new RuntimeEventBus();
            SaveService = new SaveService();

            if (dontDestroyOnLoad)
            {
                DontDestroyOnLoad(gameObject);
            }
        }
    }
}
