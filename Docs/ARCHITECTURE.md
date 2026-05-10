# Architecture & Design Patterns

> **Also available in Russian:** [ARCHITECTURE_RU.md](ARCHITECTURE_RU.md)

## Design Philosophy

The codebase follows **data-driven** and **event-driven** paradigms to enable:
- **Modularity**: Systems are independent and communicate via events
- **Testability**: Services depend on interfaces, not concrete implementations
- **Extensibility**: New systems integrate without modifying existing code
- **Maintainability**: Clear responsibilities and minimal coupling

## Core Architecture

```
                    ┌─────────────────────┐
                    │   Unity Scene       │
                    │  (SampleScene)      │
                    └──────────┬──────────┘
                               │
                    ┌──────────▼──────────┐
                    │  GameSession        │
                    │  (Singleton)        │
                    │  DontDestroyOnLoad  │
                    └──┬──────────┬───────┘
                       │          │
           ┌───────────▼─┐    ┌──▼──────────┐
           │ EventBus    │    │ SaveService │
           │ (Pub/Sub)   │    │             │
           └───────────┬─┘    └─────────────┘
                       │
        ┌──────────────┼──────────────────┐
        │              │                  │
        ▼              ▼                  ▼
   [Systems subscribe to events]
   Character  Combat  Spells  Skills  Effects  Inventory  UI
```

### GameSession (CoreRuntime/GameSession.cs)

**Responsibilities:**
- Create/initialize all core services
- Provide global access point via `GameSession.Instance`
- Ensure DontDestroyOnLoad persistence
- Initialize bootstrap sequence

**Lifetime:** Game start → Game end (persists across scenes)

```csharp
public class GameSession : MonoBehaviour
{
    public static GameSession Instance { get; private set; }
    public RuntimeEventBus EventBus { get; private set; }
    public SaveService SaveService { get; private set; }
}
```

### RuntimeEventBus (CoreRuntime/RuntimeEventBus.cs)

**Event-Driven Pattern Implementation**

Enables loose coupling between systems through pub/sub:

```csharp
// Subscribe to events
EventBus.Subscribe<LevelUpEvent>(OnLevelUp);

// Publish events  
EventBus.Publish(new SpellCastStartEvent { ... });

// Automatic cleanup on unsubscribe
subscription.Dispose();
```

**Features:**
- Type-safe event subscriptions
- Event replay buffer (last 256 events)
- Automatic subscription management
- Zero-allocation event publishing option

**Events in system:**
- `XpGrantedEvent` — Experience awarded
- `LevelUpEvent` — Character leveled up
- `SpellCastStartEvent` — Spell cast initiated
- `SpellResolvedEvent` — Spell effects resolved

## System Layers

### 1. Input Layer (PlayerController)

```
┌─────────────┐
│Input System │
└──────┬──────┘
       │ Move, Sprint, Dash, Spell inputs
       ▼
┌──────────────────┐
│PlayerController  │
│ • Movement       │
│ • Sprint         │
│ • Dash           │
│ • Action routing │
└──────┬───────────┘
       │ Applies to CharacterStats
       ▼
┌──────────────────┐
│CharacterStats    │
│ • Resource usage │
│ • Stat updates   │
└──────────────────┘
```

### 2. Character Layer (Character System)

**CharacterStats** — Central hub for character data:
- Base attributes (Strength, Agility, Constitution, Intelligence, Wisdom, Charisma)
- Derived stats (maxHealth, maxMana, walkSpeed, critChance, etc.)
- Resource tracking (HP, Mana, Stamina)
- Regeneration timers
- Active effects collection

**RuntimeStatBlock** — Modifier system:
```csharp
// Additive + Multiplicative formula
FinalStat = (BaseStat + AdditiveModifiers) * MultiplicativeModifiers

// Modifiers from:
// - Active effects (buffs/debuffs)
// - Skill passive bonuses
// - Equipment (future)
```

**Events emitted:**
- `OnResourceChanged` — HP/Mana/Stamina updates
- `OnStatsRecalculated` — Base or modifier stats changed

### 3. Combat Layer (Combat System)

**ICombatant Interface** — Defines combat participants:
```csharp
public interface ICombatant
{
    string CombatantId { get; }
    string TeamId { get; }
    bool IsAlive { get; }
    RuntimeStatBlock Stats { get; }
    void ApplyDamage(float amount, DamageType type, ICombatant source);
    void ApplyHealing(float amount, ICombatant source);
}
```

**DamageType Enum:**
- `Physical` — Affected by armor, dodge
- `Magic` — Affected by spell resistance
- `True` — Ignores all defenses

**Damage Flow:**
```
Spell Effect / Attack
    │
    ▼
Resolve Target (CombatTargeting)
    │ Filter by: team, position, range
    ▼
Calculate Damage
    │ Type, source stats, modifiers
    ▼
Apply to ICombatant.ApplyDamage()
    │
    ▼
Emit DamageApplied event
    │
    ▼
Visual feedback (DamageTextManager)
```

### 4. Spell Layer (Spells System)

**SpellDefinition** — ScriptableObject configuration:
```csharp
[Serializable]
public class SpellDefinition : ScriptableObject
{
    public string id;                           // Unique identifier
    public string displayName;                  // UI display
    public float castTime;                      // Preparation time
    public float recoverTime;                   // Recovery time
    public float cooldown;                      // Time until reusable
    public float manaCost;                      // Resource cost
    public TargetingMode targetingMode;        // Self/Unit/Position/Directional
    public float range;                         // Cast range
    public int maxTargets;                      // AoE target count
    public List<SpellEffectDefinition> effects; // Modular effects
}
```

**SpellCasterRuntime** — Manages spell state:
- Cooldown tracking per spell
- State machine: Idle → Priming → Casting → Recover → Cooldown
- Resource validation (mana, stamina)
- Event publishing

**SpellEffectDefinition** — Modular effect system:
```csharp
public abstract class SpellEffectDefinition : ScriptableObject
{
    public abstract void Execute(SpellContext context);
}

// Implementations:
// - DirectDamageEffect
// - HealEffect
// - ApplyStatModifierEffect
// - (extensible pattern)
```

### 5. Skill Layer (Skills System)

**SkillProgressionService** — XP & unlock management:
```
Gain XP Event
    │
    ▼
Add to pool
    │ Check level threshold
    ▼
Level Up? → Emit LevelUpEvent
    │       → Grant skill points
    ▼
Player spends points on SkillNodes
    │
    ▼
Verify prerequisites met
    │ Level, parent node, quest, tag
    ▼
Unlock + Apply passive bonuses
    │ Modifies RuntimeStatBlock
    ▼
Emit SkillUnlockedEvent
```

### 6. Effect Layer (Effects System)

**EffectHandler** — Applies buffs/debuffs:
```
Spell Effect / Ability triggers
    │
    ▼
Create ActiveEffect instance
    │ Duration, damage/stat values
    ▼
Check stacking rules
    │ Refresh or add stack?
    ▼
Apply modifiers to RuntimeStatBlock
    │
    ▼
Schedule tick callbacks (DoT, HoT)
    │
    ▼
On timeout → Remove → Restore stats
```

**Stacking Logic:**
- Non-stackable: refresh duration
- Stackable: add new stack up to maxStacks
- Remove oldest when max exceeded

## Data Flow Examples

### Example 1: Player casts Fireball

```
Player presses '1' key
    │
    ▼
PlayerSpellInput detects key
    │
    ▼
PlayerSpellInput.CastSpell(spellId)
    │
    ▼
SpellCasterRuntime.AttemptCast()
    ├─ Validate: mana, cooldown, distance
    └─ On success:
           │
           ▼
       Emit SpellCastStartEvent
           │
           ▼
       State → Casting
           │
           ▼
       Wait castTime
           │
           ▼
       Resolve effects for targets
       ├─ Find targets in range
       ├─ For each target:
       │   ├─ Damage = SpellPower * StatModifiers
       │   ├─ Apply to ICombatant
       │   └─ Emit DamageEvent
       │
       ▼
       Emit SpellResolvedEvent
           │
           ▼
       Apply cooldown
           │
           ▼
       State → Cooldown
```

### Example 2: Enemy takes damage with active DoT

```
Enemy.ApplyDamage(50, DamageType.Physical)
    │
    ▼
CharacterStats.TakeDamage(50)
    │
    ▼
Reduce HP by 50
    │
    ▼
Check for death
    │
    ▼
Emit DamageApplied event
    │
    ▼
Subscribe: EffectHandler checks for DoT
    │
    ▼
DoT ticks: damage = baseDamage * tickCount
    │
    ▼
On DoT expiration → Remove modifier
    │
    ▼
Emit EffectExpired event
```

## Patterns Used

| Pattern | Location | Purpose |
|---------|----------|---------|
| **Singleton** | GameSession | Global lifecycle |
| **Service Locator** | GameSession.Instance | Central access |
| **Pub/Sub (Observer)** | RuntimeEventBus | System communication |
| **Command** | Input → PlayerController → Stats | Input handling |
| **State Machine** | SpellCasterRuntime | Spell state |
| **ScriptableObject** | All *Definition classes | Configuration |
| **Strategy** | SpellEffectDefinition | Pluggable effects |
| **Modifier Stack** | RuntimeStatBlock | Stat calculation |
| **Save Provider** | ISaveDataProvider | Modular persistence |

## Dependency Injection Model

**Implicit via Service Locator:**
```csharp
// Direct access via singleton
var eventBus = GameSession.Instance.EventBus;

// Or through method injection
public void Setup(RuntimeEventBus eventBus)
{
    _eventBus = eventBus;
    _eventBus.Subscribe<LevelUpEvent>(OnLevelUp);
}
```

## Thread Safety

- **Not thread-safe** by design (single-threaded Unity)
- Events are processed immediately on publish
- No concurrent modifications during event handling

## Performance Considerations

1. **Event System:** O(1) publish, O(n) subscribe (n = listener count)
2. **Stat Modifiers:** Recalculated only when changed (cached)
3. **Effect Ticking:** Fixed timestep, O(1) per active effect
4. **Spell Targeting:** O(n) raycast where n = scene colliders

---

# Архитектура и паттерны проектирования

> **Read in English:** [ARCHITECTURE.md](ARCHITECTURE.md)

## Философия дизайна

Кодовая база следует **data-driven** и **event-driven** парадигмам для обеспечения:
- **Модульности**: Системы независимы и общаются через события
- **Тестируемости**: Сервисы зависят от интерфейсов, а не конкретных реализаций
- **Расширяемости**: Новые системы интегрируются без модификации существующего кода
- **Поддерживаемости**: Четкие обязанности и минимальная связанность

## Основная архитектура

```
                    ┌─────────────────────┐
                    │   Unity Scene       │
                    │  (SampleScene)      │
                    └──────────┬──────────┘
                               │
                    ┌──────────▼──────────┐
                    │  GameSession        │
                    │  (Singleton)        │
                    │  DontDestroyOnLoad  │
                    └──┬──────────┬───────┘
                       │          │
           ┌───────────▼─┐    ┌──▼──────────┐
           │ EventBus    │    │ SaveService │
           │ (Pub/Sub)   │    │             │
           └───────────┬─┘    └─────────────┘
                       │
        ┌──────────────┼──────────────────┐
        │              │                  │
        ▼              ▼                  ▼
   [Системы подписаны на события]
   Character  Combat  Spells  Skills  Effects  Inventory  UI
```

### GameSession (CoreRuntime/GameSession.cs)

**Обязанности:**
- Создание/инициализация всех основных сервисов
- Предоставление глобальной точки доступа через `GameSession.Instance`
- Обеспечение постоянства между сценами (DontDestroyOnLoad)
- Инициализация последовательности загрузки

**Время жизни:** Начало игры → Конец игры (сохраняется между сценами)

### RuntimeEventBus (CoreRuntime/RuntimeEventBus.cs)

**Реализация паттерна Event-Driven**

Обеспечивает слабую связь между системами через pub/sub:

**События в системе:**
- `XpGrantedEvent` — Опыт получен
- `LevelUpEvent` — Персонаж поднялся на уровень
- `SpellCastStartEvent` — Каст спелла инициирован
- `SpellResolvedEvent` — Эффекты спелла разрешены

## Слои систем

### 1. Слой ввода (PlayerController)

Получает ввод → применяет к CharacterStats → обновляет ресурсы

### 2. Слой персонажа (Character System)

**CharacterStats** — Центральный хаб данных персонажа:
- Базовые атрибуты (Strength, Agility, Constitution, etc.)
- Производные статы (maxHealth, maxMana, скорость, критический шанс)
- Отслеживание ресурсов (HP, Mana, Stamina)
- Таймеры регенерации
- Коллекция активных эффектов

**RuntimeStatBlock** — Система модификаторов:
```
FinalStat = (BaseStat + AdditивныеМодификаторы) × МультипликативныеМодификаторы

Источники модификаторов:
- Активные эффекты (баффы/дебаффы)
- Пассивные бонусы навыков
- Оборудование (будущее)
```

### 3. Боевой слой (Combat System)

**ICombatant Interface** — Определяет участников боя:
- CombatantId, TeamId — идентификация
- IsAlive — проверка жизни
- ApplyDamage/ApplyHealing — боевые методы

**DamageType enum:**
- `Physical` — затронут броней, уклонением
- `Magic` — затронут сопротивлением магии
- `True` — игнорирует защиту

### 4. Слой заклинаний (Spells System)

**SpellDefinition** — Конфигурация ScriptableObject:
- id, имя, иконка, теги
- castTime, recoverTime, cooldown
- manaCost, staminaCost
- TargetingMode, range, maxTargets
- Список модульных эффектов

**SpellCasterRuntime** — Управление состоянием спелла:
- Отслеживание кулдаунов
- Машина состояний: Idle → Priming → Casting → Recover → Cooldown
- Проверка ресурсов (мана, выносливость)
- Публикация событий

### 5. Слой навыков (Skills System)

**SkillProgressionService** — Управление опытом и разблокировкой:

```
Получение опыта
    │
    ▼
Проверка порога уровня
    │
    ▼
Повышение уровня? → Предоставление очков навыков
    │
    ▼
Трата очков на узлы умений
    │
    ▼
Проверка предварительных условий
    │
    ▼
Разблокировка + Применение пассивных бонусов
```

### 6. Слой эффектов (Effects System)

**EffectHandler** — Применение баффов/дебаффов:

```
Эффект спелла/способности запущен
    │
    ▼
Создание экземпляра ActiveEffect
    │
    ▼
Проверка правил стакирования
    │
    ▼
Применение модификаторов к RuntimeStatBlock
    │
    ▼
Планирование тиков (DoT, HoT)
    │
    ▼
По истечении времени → Удаление
```

## Используемые паттерны

| Паттерн | Местоположение | Назначение |
|--------|----------------|-----------|
| **Singleton** | GameSession | Глобальный жизненный цикл |
| **Service Locator** | GameSession.Instance | Централизованный доступ |
| **Pub/Sub (Observer)** | RuntimeEventBus | Коммуникация систем |
| **Modifier Stack** | RuntimeStatBlock | Расчет статов |
| **ScriptableObject** | Все *Definition классы | Конфигурация |
| **Strategy** | SpellEffectDefinition | Подключаемые эффекты |

## Производительность

1. **Event System:** O(1) публикация, O(n) подписка (n = кол-во слушателей)
2. **Модификаторы статов:** Пересчитываются только при изменении (кэшированы)
3. **Тикинг эффектов:** Фиксированный timestep, O(1) на активный эффект
4. **Таргетирование спеллов:** O(n) raycast где n = кол-во коллайдеров

---
