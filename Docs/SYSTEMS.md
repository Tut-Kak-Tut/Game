# Systems Reference

> **Also available in Russian:** [SYSTEMS_RU.md](SYSTEMS_RU.md)

Detailed specifications for each system component. For architecture overview, see [ARCHITECTURE.md](ARCHITECTURE.md).

## CoreRuntime

**Location:** `Assets/Game/Scripts/CoreRuntime/`

### GameSession
**File:** `GameSession.cs`

The root singleton managing all core services and game lifecycle.

**Key Members:**
```csharp
public static GameSession Instance { get; private set; }
public RuntimeEventBus EventBus { get; private set; }
public SaveService SaveService { get; private set; }
```

**Usage:**
```csharp
// Access event bus
GameSession.Instance.EventBus.Subscribe<LevelUpEvent>(OnLevelUp);

// Save/Load
GameSession.Instance.SaveService.SaveAll();
```

### RuntimeEventBus
**File:** `RuntimeEventBus.cs`

Pub/Sub event system with type-safe subscriptions and event replay.

**Key Methods:**
```csharp
public IDisposable Subscribe<T>(Action<T> handler) where T : class;
public void Publish<T>(T @event) where T : class;
public T[] GetEventHistory<T>() where T : class;  // Last 256 events
```

**All Events:**
- `XpGrantedEvent` — Triggered when XP awarded
- `LevelUpEvent` — Character level increased
- `SpellCastStartEvent` — Spell cast began
- `SpellResolvedEvent` — Spell resolved
- `SkillUnlockedEvent` — Skill node unlocked
- `EffectAppliedEvent` — Buff/debuff applied

### SaveService & SaveContracts
**Files:** `SaveContracts.cs`

**ISaveDataProvider Interface:**
```csharp
public interface ISaveDataProvider
{
    SaveData Serialize();
    void Deserialize(SaveData data);
}
```

Register with SaveService:
```csharp
SaveService.Register(this);  // Calls Serialize/Deserialize
```

---

## Combat System

**Location:** `Assets/Game/Scripts/Combat/`

### CombatantBehaviour
**File:** `CombatantBehaviour.cs`

Implements the `ICombatant` interface on GameObjects.

**Key Properties:**
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

**Implementing on Player:**
- Linked to `CharacterStats` for resource management
- TeamId = "player"

**Implementing on Enemy:**
- Linked to `EnemyAI` for pathfinding
- TeamId = "enemy"

### RuntimeStatBlock
**File:** `RuntimeStatBlock.cs`

Manages stat modifiers with additive and multiplicative layers.

**Architecture:**
```csharp
public float Calculate(float baseStat, RuntimeStatId statId)
{
    float additive = GetAdditiveBonus(statId);
    float multiplicative = GetMultiplicativeBonus(statId);
    return (baseStat + additive) * multiplicative;
}
```

**Adding Modifiers:**
```csharp
// From effects, skills, etc.
stats.AddModifier(
    RuntimeStatId.Strength, 
    10f,  // amount
    ModificationType.Flat,
    sourceId: "buff_warrior_stance"
);

stats.AddModifier(
    RuntimeStatId.Damage,
    1.15f,  // 15% boost
    ModificationType.Percent,
    sourceId: "skill_critical_strike",
    duration: 5f  // seconds, optional
);
```

### CombatTargeting
**File:** `CombatTargeting.cs`

Utilities for target resolution in AoE and single-target spells.

**Methods:**
```csharp
public static List<ICombatant> ResolveInRadius(
    Vector3 position, 
    float radius,
    string excludeTeam = null,
    int maxTargets = int.MaxValue
);
```

---

## Character System

**Location:** `Assets/Game/Scripts/Character/`

### CharacterStats
**File:** `Character/CharacterStats.cs`

Central stat hub for players and enemies.

**Base Attributes:**
```csharp
public float Strength;         // Physical damage, carry capacity
public float Agility;          // Dodge, critical chance, attack speed
public float Constitution;     // Max HP, poison resistance
public float Intelligence;     // Spell power, max mana
public float Wisdom;           // Mana regen, status resistance
public float Charisma;         // Vendor discounts, companion effectiveness
```

**Derived Stats (calculated):**
- `MaxHealth = 10 + Constitution * 5`
- `MaxMana = 10 + Intelligence * 3`
- `MaxStamina = 10 + Constitution * 2`
- `CritChance = Agility * 0.5%`
- `DodgeChance = Agility * 0.3%`

**Key Methods:**
```csharp
public void TakeDamage(float amount, DamageType type = DamageType.Physical);
public void RestoreHealth(float amount);
public bool TryUseStamina(float amount);
public void RegenerateResources(float deltaTime);
```

**Events:**
```csharp
public event Action OnResourceChanged;      // HP/Mana/Stamina
public event Action OnStatsRecalculated;    // Attributes or modifiers changed
public event Action OnDeath;
```

### PlayerController
**File:** `Character/Player/PlayerController.cs`

Reads input and applies movement.

**Actions:**
| Input | Effect | Cost |
|-------|--------|------|
| WASD | Move (6 units/sec) | None |
| Shift + Move | Sprint (9.6 units/sec) | 1 Stamina/sec |
| Space | Dash (3x, 0.2sec) | 20 Stamina, 0.8s cooldown |
| 1-9 | Cast spell | Mana |

**Implementation:**
```csharp
public void OnMove(InputValue value)
{
    Vector2 direction = value.Get<Vector2>();
    _rigidbody.velocity = direction * moveSpeed;
}

public void OnSprint(InputValue value)
{
    if (value.isPressed && _stats.TryUseStamina(Time.deltaTime))
        _rigidbody.velocity *= sprintMultiplier;  // 1.6x
}
```

### EnemyAI
**File:** `Character/Enemy/EnemyAI.cs`

NavMeshAgent-based pathfinding and behavior.

**States:**
1. **Patrol** — Walk between waypoints
2. **Chase** — Move toward player (detection range: 10 units)
3. **Attack** — Attack when in range (2.5 units)

**Configuration:**
```csharp
public float detectionRange = 10f;
public float attackRange = 2.5f;
public List<Vector3> patrolPoints;
public float patrolSpeed = 3.5f;
public float chaseSpeed = 5.5f;
```

---

## Spells System

**Location:** `Assets/Game/Scripts/Spells/`

### SpellDefinition
**File:** `Spells/SpellDefinition.cs`

ScriptableObject defining spell mechanics and effects.

**Fields:**
```csharp
public string id;                              // Unique: "fireball", "heal"
public string displayName;                     // UI: "Fireball Spell"
public Sprite icon;
public List<string> tags;                      // "aoe", "fire", "damaging"

public float castTime = 1f;                    // Seconds to cast
public float recoverTime = 0.5f;               // Recovery animation
public float cooldown = 3f;                    // Reuse time
public float range = 20f;
public float manaCost = 30f;
public float staminaCost = 0f;

public TargetingMode targetingMode;            // Self, Unit, Position, Directional
public int maxTargets = 1;                     // For AoE
public bool requiresLineOfSight = true;

public List<SpellEffectDefinition> effects;    // Modular effects
```

**Targeting Modes:**
- `Self` — No targeting, affects caster
- `Unit` — Click on enemy, single target
- `Position` — Click on ground, AoE area
- `Directional` — Line cast in direction

### SpellCasterRuntime
**File:** `Spells/SpellCasterRuntime.cs`

Manages spell state, cooldowns, and casting.

**State Machine:**
```
Idle
  ├─ AttemptCast() → validate → Priming
  │
  ├─ Priming (casting)
  │   └─ Wait castTime → Casting → effects execute
  │
  ├─ Casting (effects resolving)
  │   └─ Complete effects → Recover
  │
  ├─ Recover (animation)
  │   └─ Wait recoverTime → Cooldown
  │
  └─ Cooldown (reuse blocked)
      └─ Wait cooldown → Idle
```

**Validation:**
```csharp
public bool CanCast(SpellDefinition spell)
{
    if (!HasMana(spell.manaCost)) return false;
    if (!HasStamina(spell.staminaCost)) return false;
    if (IsOnCooldown(spell.id)) return false;
    return true;
}
```

### SpellEffectDefinition
**File:** `Spells/SpellEffects.cs`

Base class for modular spell effects.

**Abstract Method:**
```csharp
public abstract void Execute(SpellContext context);
```

**SpellContext:**
```csharp
public struct SpellContext
{
    public SpellDefinition Spell;
    public ICombatant Caster;
    public List<ICombatant> Targets;
    public Vector3 TargetPosition;
    public float LevelBonus;  // For scaling
}
```

**Built-in Effects:**

**DirectDamageEffect**
```csharp
public override void Execute(SpellContext context)
{
    foreach (var target in context.Targets)
    {
        float damage = damageAmount * (1 + context.LevelBonus * 0.1f);
        target.ApplyDamage(damage, damageType, context.Caster);
    }
}
```

**HealEffect**
```csharp
public override void Execute(SpellContext context)
{
    var target = context.Targets.First();
    target.ApplyHealing(healAmount, context.Caster);
}
```

**ApplyStatModifierEffect**
```csharp
public override void Execute(SpellContext context)
{
    foreach (var target in context.Targets)
    {
        target.Stats.AddModifier(
            statId: statToModify,
            amount: modifierAmount,
            type: modificationType,
            duration: effectDuration
        );
    }
}
```

### PlayerSpellInput
**File:** `Spells/PlayerSpellInput.cs`

Keyboard input for spell casting.

**Input Mapping:**
```
Keys 1-9 → Cast spell index 0-8
Mouse Click → Aim position target
Hold for directional → Direction cast
```

---

## Skills System

**Location:** `Assets/Game/Scripts/Skills/`

### SkillProgressionService
**File:** `Skills/SkillProgressionService.cs`

Manages XP, leveling, and skill unlocks.

**Progression:**
```
XP Formula:
XpRequired = baseXpPerLevel + (level - 1) * 25
Example: Level 1 → 100 XP
         Level 2 → 125 XP
         Level 3 → 150 XP
```

**Unlock System:**
```csharp
public class SkillNodeDefinition : ScriptableObject
{
    public string nodeId;
    public int tier;
    public int skillPointCost = 1;
    
    // Prerequisites
    public int requiredLevel;
    public string parentNodeId;         // Must unlock parent first
    public string requiredQuestId;
    public List<string> requiredTags;   // Any tag required
    
    // Passive bonuses
    public List<PassiveBonusData> passiveBonuses;
}

public struct PassiveBonusData
{
    public RuntimeStatId statId;
    public float amount;
    public ModificationType type;
}
```

**Unlock Logic:**
```csharp
public bool CanUnlock(SkillNodeDefinition node)
{
    if (currentLevel < node.requiredLevel) return false;
    if (!IsUnlocked(node.parentNodeId)) return false;
    if (currentSkillPoints < node.skillPointCost) return false;
    return true;
}

public void UnlockSkill(string nodeId)
{
    // Verify prerequisites
    // Deduct skill points
    // Apply passive bonuses to RuntimeStatBlock
    // Emit SkillUnlockedEvent
}
```

---

## Effects System

**Location:** `Assets/Game/Scripts/Effects/`

### EffectHandler
**File:** `Effects/EffectHandler.cs`

Applies and manages active effects on characters.

**Effect Types:**
| Type | Applies | Duration |
|------|---------|----------|
| DamageOverTime | Damage ticks | Until expire |
| HealingOverTime | Heal ticks | Until expire |
| BuffStats | +Stat modifiers | Until expire |
| DebuffStats | -Stat modifiers | Until expire |

**Stacking Rules:**
```csharp
public void ApplyEffect(EffectData effect, ICombatant source)
{
    var active = _activeEffects.FirstOrDefault(e => e.Id == effect.id);
    
    if (effect.isStackable)
    {
        if (active?.stackCount >= effect.maxStacks)
            RemoveOldest();  // FIFO
        AddStack();
    }
    else
    {
        if (active != null)
            active.duration = effect.duration;  // Refresh
        else
            AddNewEffect();
    }
}
```

**Tick System:**
```csharp
public void Update(float deltaTime)
{
    foreach (var effect in _activeEffects)
    {
        effect.duration -= deltaTime;
        
        if (effect.nextTickTime <= 0)
        {
            effect.nextTickTime = effect.tickInterval;
            ApplyTick(effect);  // DoT/HoT
        }
        
        if (effect.duration <= 0)
            RemoveEffect(effect);
    }
}
```

### EffectData
**File:** `Effects/EffectData.cs`

ScriptableObject configuration for effects.

```csharp
public class EffectData : ScriptableObject
{
    public string id;                    // "bleed_wound", "blessing"
    public string displayName;
    public Sprite icon;
    
    public EffectType effectType;
    public float duration = 10f;
    public float tickInterval = 1f;
    
    public bool isStackable = false;
    public int maxStacks = 1;
    
    // Damage/Healing
    public float dotDamage;              // Per tick
    public DamageType damageType;
    
    // Stat modifications
    public List<StatModifierData> statModifiers;
    
    public Color effectColor = Color.white;  // Visual indicator
}
```

---

## Inventory System

**Location:** `Assets/Game/Scripts/Inventory/`

### Inventory
**File:** `Inventory/Inventory.cs`

Slot-based item storage.

**Structure:**
```csharp
public class InventorySlot
{
    public ItemData Item;
    public int Quantity;
}

private InventorySlot[] _slots;  // 20 slots default
```

**Methods:**
```csharp
public bool AddItem(ItemData item, int quantity = 1)
{
    // Find existing stack if stackable
    // Or find empty slot
    // Update quantity
    // Emit OnInventoryChanged
}

public bool RemoveItem(ItemData item, int quantity = 1)
{
    // Find and reduce quantity
    // Remove slot if empty
}

public ItemData[] GetAllItems() => _slots
    .Where(s => s.Item != null)
    .Select(s => s.Item)
    .ToArray();
```

### ItemData
**File:** `Inventory/ItemData.cs`

ScriptableObject item definition.

```csharp
public class ItemData : ScriptableObject
{
    public string itemName;
    public ItemType itemType;           // Generic, Weapon, Armor, Consumable
    public Sprite icon;
    public string description;
    
    public bool isStackable;
    public int maxStackSize = 99;
    
    // Effects on use
    public float healthEffect;
    public float manaEffect;
    public float staminaEffect;
    
    // Combat values
    public float damageValue;
    public float defenseValue;
}
```

---

## UI System

**Location:** `Assets/Game/Scripts/UI/`

### UIController
**File:** `UI/UIController.cs`

Main UI update hub.

**Subscriptions:**
```csharp
_stats.OnResourceChanged += UpdateResourceBars;
_stats.OnStatsRecalculated += UpdateAttributeDisplay;
EventBus.Subscribe<LevelUpEvent>(OnLevelUp);
```

**Updates:**
- Health/Mana/Stamina bars with smooth lerp
- Attribute displays
- Level and XP bar

### DamageTextManager
**File:** `UI/DamageTextManager.cs`

Floating damage numbers.

```csharp
public void ShowDamage(Vector3 position, float amount, DamageType type)
{
    var text = Instantiate(damageTextPrefab);
    text.transform.position = position;
    text.SetText(amount.ToString());
    text.SetColor(GetColorForType(type));
    
    // Auto-destroy after animation
    Destroy(text.gameObject, 1.5f);
}
```

---

# Справка по системам

> **Read in English:** [SYSTEMS.md](SYSTEMS.md)

Полные спецификации каждого компонента системы. Для обзора архитектуры см. [ARCHITECTURE.md](ARCHITECTURE.md).

## CoreRuntime

**Местоположение:** `Assets/Game/Scripts/CoreRuntime/`

### GameSession

Корневой singleton, управляющий всеми основными сервисами и жизненным циклом игры.

**Использование:**
```csharp
// Доступ к event bus
GameSession.Instance.EventBus.Subscribe<LevelUpEvent>(OnLevelUp);

// Сохранение/загрузка
GameSession.Instance.SaveService.SaveAll();
```

### RuntimeEventBus

Pub/Sub система событий с типобезопасными подписками и повтором событий.

**Все события:**
- `XpGrantedEvent` — Опыт выдан
- `LevelUpEvent` — Уровень повышен
- `SpellCastStartEvent` — Каст спелла начался
- `SpellResolvedEvent` — Спелл разрешен
- `SkillUnlockedEvent` — Умение разблокировано
- `EffectAppliedEvent` — Эффект применен

## Combat System

**Местоположение:** `Assets/Game/Scripts/Combat/`

### CombatantBehaviour

Реализует интерфейс `ICombatant` на GameObjects.

**Для игрока:**
- TeamId = "player"

**Для врага:**
- TeamId = "enemy"

### RuntimeStatBlock

Управляет модификаторами статов с аддитивным и мультипликативным слоями.

```
FinalStat = (BaseStat + AdditивныеМодификаторы) × МультипликативныеМодификаторы
```

### CombatTargeting

Утилиты для разрешения целей в AoE и однотаргетных спеллах.

## Character System

**Местоположение:** `Assets/Game/Scripts/Character/`

### CharacterStats

Центральный хаб статов для игроков и врагов.

**Основные атрибуты:**
- Strength — Физический урон
- Agility — Уклонение, критический шанс
- Constitution — Макс HP
- Intelligence — Сила магии, макс мана
- Wisdom — Регенерация маны
- Charisma — Скидки у торговцев

### PlayerController

Читает ввод и применяет движение.

| Ввод | Эффект | Стоимость |
|------|--------|----------|
| WASD | Движение | None |
| Shift | Спринт (1.6x) | 1 Stamina/сек |
| Space | Рывок (3x, 0.2сек) | 20 Stamina |
| 1-9 | Каст спелла | Mana |

### EnemyAI

Навигация на основе NavMeshAgent.

**Состояния:**
1. Patrol — Ходит между точками (дальность обнаружения 10)
2. Chase — Преследует игрока
3. Attack — Атакует при приближении (2.5 единиц)

## Spells System

**Местоположение:** `Assets/Game/Scripts/Spells/`

### SpellDefinition

ScriptableObject определение спелла с механикой и эффектами.

**Режимы таргетирования:**
- Self — Без таргетирования, воздействует на кастера
- Unit — Клик по врагу, однотаргетный
- Position — Клик по земле, AoE область
- Directional — Направленный каст

### SpellCasterRuntime

Управление состоянием спелла и кулдаунами.

**Машина состояний:**
```
Idle → Priming → Casting → Recover → Cooldown → Idle
```

### SpellEffectDefinition

Базовый класс для модульных эффектов спелла.

**Встроенные эффекты:**
- DirectDamageEffect — Прямой урон
- HealEffect — Исцеление
- ApplyStatModifierEffect — Баффы/дебаффы

## Skills System

**Местоположение:** `Assets/Game/Scripts/Skills/`

### SkillProgressionService

Управляет опытом, уровнями и разблокировкой умений.

```
Требуемый опыт = baseXpPerLevel + (уровень - 1) × 25
Пример: Уровень 1 → 100 XP
        Уровень 2 → 125 XP
```

**Система разблокировки:**
- Проверка уровня
- Проверка родительского узла
- Проверка квестов
- Применение пассивных бонусов

## Effects System

**Местоположение:** `Assets/Game/Scripts/Effects/`

### EffectHandler

Применение и управление активными эффектами.

**Типы эффектов:**
- DamageOverTime — Урон с течением времени
- HealingOverTime — Исцеление с течением времени
- BuffStats — Баффы статов
- DebuffStats — Дебаффы статов

**Правила стакирования:**
- Нестакируемый: обновить длительность
- Стакируемый: добавить до maxStacks

## Inventory System

**Местоположение:** `Assets/Game/Scripts/Inventory/`

### Inventory

Слот-based хранилище предметов (20 слотов).

**Методы:**
- AddItem() — Добавить предмет
- RemoveItem() — Удалить предмет

### ItemData

ScriptableObject определение предмета.

**Типы:** Generic, Weapon, Armor, Consumable, Quest

## UI System

**Местоположение:** `Assets/Game/Scripts/UI/`

### UIController

Главный хаб обновления UI.

**Подписки:**
- OnResourceChanged → Обновить полоски
- OnStatsRecalculated → Обновить статы

### DamageTextManager

Всплывающие номера урона на позиции персонажа.

---
