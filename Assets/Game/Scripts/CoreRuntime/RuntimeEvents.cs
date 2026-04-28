using System;

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
        public DateTime TimestampUtc { get; }

        public SpellResolvedEvent(string spellId)
        {
            SpellId = spellId;
            TimestampUtc = DateTime.UtcNow;
        }
    }
}
