using System;
using UnityEngine;

namespace Game.CoreRuntime
{
    public readonly struct XpGrantedEvent : IGameEvent
    {
        public readonly int Amount;
        public readonly string SourceId;
        public DateTime TimestampUtc { get; }

        public XpGrantedEvent(int amount, string sourceId)
        {
            Amount = amount;
            SourceId = sourceId;
            TimestampUtc = DateTime.UtcNow;
        }
    }

    public readonly struct XpDeductedEvent : IGameEvent
    {
        public readonly int Amount;
        public readonly string Reason;
        public DateTime TimestampUtc { get; }

        public XpDeductedEvent(int amount, string reason)
        {
            Amount = amount;
            Reason = reason;
            TimestampUtc = DateTime.UtcNow;
        }
    }

    public readonly struct LevelUpEvent : IGameEvent
    {
        public readonly int NewLevel;
        public DateTime TimestampUtc { get; }

        public LevelUpEvent(int newLevel)
        {
            NewLevel = newLevel;
            TimestampUtc = DateTime.UtcNow;
        }
    }

    public readonly struct SpellCastStartEvent : IGameEvent
    {
        public readonly string SpellId;
        public DateTime TimestampUtc { get; }

        public SpellCastStartEvent(string spellId)
        {
            SpellId = spellId;
            TimestampUtc = DateTime.UtcNow;
        }
    }

    public readonly struct SpellResolvedEvent : IGameEvent
    {
        public readonly string SpellId;
        public readonly bool Interrupted;
        public DateTime TimestampUtc { get; }

        public SpellResolvedEvent(string spellId, bool interrupted = false)
        {
            SpellId = spellId;
            Interrupted = interrupted;
            TimestampUtc = DateTime.UtcNow;
        }
    }

    public readonly struct DamageDealtEvent : IGameEvent
    {
        public readonly string SourceId;
        public readonly string TargetId;
        public readonly float Amount;
        public readonly DamageType Type;
        public readonly bool WasCrit;
        public DateTime TimestampUtc { get; }

        public DamageDealtEvent(string sourceId, string targetId, float amount, DamageType type, bool wasCrit)
        {
            SourceId = sourceId;
            TargetId = targetId;
            Amount = amount;
            Type = type;
            WasCrit = wasCrit;
            TimestampUtc = DateTime.UtcNow;
        }
    }

    public readonly struct CombatantDiedEvent : IGameEvent
    {
        public readonly string VictimId;
        public readonly string KillerId;
        public readonly int TeamId;
        public DateTime TimestampUtc { get; }

        public CombatantDiedEvent(string victimId, string killerId, int teamId)
        {
            VictimId = victimId;
            KillerId = killerId;
            TeamId = teamId;
            TimestampUtc = DateTime.UtcNow;
        }
    }

    public readonly struct PlayerRespawnedEvent : IGameEvent
    {
        public readonly Vector3 Location;
        public readonly float XpLost;
        public readonly float GoldLost;
        public DateTime TimestampUtc { get; }

        public PlayerRespawnedEvent(Vector3 location, float xpLost, float goldLost)
        {
            Location = location;
            XpLost = xpLost;
            GoldLost = goldLost;
            TimestampUtc = DateTime.UtcNow;
        }
    }

    public readonly struct PlayerHardcoreDeathEvent : IGameEvent
    {
        public readonly string SaveId;
        public readonly string ArchivedPath;
        public DateTime TimestampUtc { get; }

        public PlayerHardcoreDeathEvent(string saveId, string archivedPath)
        {
            SaveId = saveId;
            ArchivedPath = archivedPath;
            TimestampUtc = DateTime.UtcNow;
        }
    }
}
