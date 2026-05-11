using UnityEngine;

namespace Game.CoreRuntime
{
    public class GameSession : MonoBehaviour
    {
        public static GameSession Instance { get; private set; }

        public IEventBus EventBus { get; private set; }
        public SaveService SaveService { get; private set; }
        public GraveyardService Graveyard { get; private set; }
        public DeathModeService DeathModeService { get; private set; }

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
            Graveyard = new GraveyardService();
            DeathModeService = new DeathModeService(EventBus, SaveService, Graveyard, this);

            if (dontDestroyOnLoad)
            {
                DontDestroyOnLoad(gameObject);
            }
        }

        private void OnDestroy()
        {
            if (Instance == this)
            {
                DeathModeService?.Dispose();
                Instance = null;
            }
        }
    }
}
