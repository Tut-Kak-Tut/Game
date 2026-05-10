using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Game.CoreRuntime
{
    public sealed class DeathModeService : IDisposable
    {
        private readonly IEventBus _bus;
        private readonly SaveService _save;
        private readonly GraveyardService _graveyard;
        private readonly MonoBehaviour _coroutineHost;

        private IDisposable _diedSubscription;
        private DeathMode _activeMode;
        private DeathModeConfig _config;
        private Transform _hubSpawn;
        private CharacterStats _playerStats;
        private string _playerCombatantId;
        private Func<int, int> _xpAtNextLevelLookup;
        private bool _initialized;
        private bool _respawning;

        public DeathMode ActiveMode => _activeMode;
        public bool IsInitialized => _initialized;

        public DeathModeService(IEventBus bus, SaveService save, GraveyardService graveyard, MonoBehaviour coroutineHost)
        {
            _bus = bus;
            _save = save;
            _graveyard = graveyard;
            _coroutineHost = coroutineHost;
        }

        public void Initialize(
            DeathMode mode,
            DeathModeConfig config,
            Transform hubSpawn,
            CharacterStats playerStats,
            string playerCombatantId,
            Func<int, int> xpAtNextLevelLookup = null)
        {
            _activeMode = mode;
            _config = config;
            _hubSpawn = hubSpawn;
            _playerStats = playerStats;
            _playerCombatantId = playerCombatantId;
            _xpAtNextLevelLookup = xpAtNextLevelLookup;

            _diedSubscription?.Dispose();
            _diedSubscription = _bus.Subscribe<CombatantDiedEvent>(OnCombatantDied);

            _initialized = true;
        }

        private void OnCombatantDied(CombatantDiedEvent evt)
        {
            if (!_initialized || string.IsNullOrEmpty(_playerCombatantId)) return;
            if (evt.VictimId != _playerCombatantId) return;
            if (_respawning) return;

            if (_activeMode == DeathMode.Hardcore)
            {
                HandleHardcore();
            }
            else
            {
                if (_coroutineHost == null || _playerStats == null) return;
                _coroutineHost.StartCoroutine(StandardRespawnRoutine());
            }
        }

        private IEnumerator StandardRespawnRoutine()
        {
            _respawning = true;

            float fadeOut = _config != null ? _config.respawnFadeOutDuration : 0.5f;
            float fadeIn = _config != null ? _config.respawnFadeInDuration : 0.5f;

            if (fadeOut > 0f) yield return new WaitForSecondsRealtime(fadeOut);

            if (_hubSpawn != null && _playerStats != null)
                _playerStats.transform.position = _hubSpawn.position;

            int xpLost = 0;
            if (_xpAtNextLevelLookup != null && _config != null)
            {
                xpLost = Mathf.RoundToInt(_xpAtNextLevelLookup(0) * _config.xpPenaltyFraction);
                if (xpLost > 0)
                    _bus.Publish(new XpDeductedEvent(xpLost, "death-penalty"));
            }

            // TODO: gold penalty (config.goldPenaltyFraction) — wire when economy/currency system lands.

            if (_playerStats != null)
            {
                _playerStats.Revive();
                _playerStats.RestoreAllResources();
            }

            if (fadeIn > 0f) yield return new WaitForSecondsRealtime(fadeIn);

            Vector3 location = _hubSpawn != null ? _hubSpawn.position : Vector3.zero;
            _bus.Publish(new PlayerRespawnedEvent(location, xpLost, 0f));

            _respawning = false;
        }

        private void HandleHardcore()
        {
            string saveJson = _save != null ? _save.BuildSaveJson() : string.Empty;
            string archived = _graveyard != null ? _graveyard.ArchiveSave(_playerCombatantId, saveJson) : string.Empty;

            _bus.Publish(new PlayerHardcoreDeathEvent(_playerCombatantId, archived));

            Debug.Log($"[Hardcore] You died. Save archived: {archived}");

            string activeScene = SceneManager.GetActiveScene().name;
            SceneManager.LoadScene(activeScene);
        }

        public void Dispose()
        {
            _diedSubscription?.Dispose();
            _diedSubscription = null;
            _initialized = false;
        }
    }
}
