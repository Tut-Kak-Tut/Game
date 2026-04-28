# Extending the Engine

> **Also available in Russian:** [EXTENDING_RU.md](EXTENDING_RU.md)

Guides for adding new systems, mechanics, and features to the engine.

## Adding a New Spell

### Step 1: Create Spell Definition (ScriptableObject)

```csharp
// 1. In editor, Right-click > Create > Game/Spell > New Spell
// 2. Fill in basic properties:
//    - ID: "lightning_strike"
//    - Display Name: "Lightning Strike"
//    - Cast Time: 0.8
//    - Cooldown: 5.0
//    - Mana Cost: 45
//    - Range: 25
//    - Targeting Mode: Unit
```

### Step 2: Create Custom Spell Effect (if needed)

```csharp
using Game.Spells;
using Game.Combat;
using UnityEngine;

[CreateAssetMenu(menuName = "Game/Spell Effects/Chain Lightning")]
public class ChainLightningEffect : SpellEffectDefinition
{
    public float jumpRange = 10f;
    public int maxChains = 3;
    
    public override void Execute(SpellContext context)
    {
        var damaged = new HashSet<ICombatant>();
        DamageChain(context.Targets[0], context, 0, damaged);
    }
    
    private void DamageChain(
        ICombatant target, 
        SpellContext context, 
        int chainCount,
        HashSet<ICombatant> damaged)
    {
        if (chainCount >= maxChains || target == null) return;
        
        // Deal damage
        target.ApplyDamage(
            context.Spell.manaCost * 2,  // Damage based on cost
            DamageType.Magic,
            context.Caster
        );
        
        damaged.Add(target);
        
        // Find next target
        var nearby = CombatTargeting.ResolveInRadius(
            target.transform.position,
            jumpRange,
            excludeTeam: context.Caster.TeamId,
            maxTargets: 1
        ).FirstOrDefault(t => !damaged.Contains(t));
        
        if (nearby != null)
            DamageChain(nearby, context, chainCount + 1, damaged);
    }
}
```

### Step 3: Add Effect to Spell Definition

1. In your spell's ScriptableObject, add the ChainLightningEffect to the effects list
2. Configure parameters (jumpRange, maxChains)
3. Test by pressing key 1-9 in game

### Step 4: Add to Player's Spell Bar (Optional)

```csharp
// In SpellCasterRuntime inspector, add spell to availableSpells list
```

---

## Adding a New Buff/Debuff Effect

### Step 1: Create Effect Data (ScriptableObject)

```csharp
// Right-click > Create > Game/Effect > New Effect
// Configure:
//   - ID: "weakness_curse"
//   - Effect Type: DebuffStats
//   - Duration: 15.0
//   - Stackable: true, Max Stacks: 3
//   - Stat Modifiers: Strength -10 (Flat)
//   - Color: Red
```

### Step 2: Create Custom Effect Handler (if needed)

```csharp
using Game.Effects;
using Game.Combat;

[CreateAssetMenu(menuName = "Game/Effects/Poison Cloud")]
public class PoisonCloudEffect : EffectData
{
    public Vector3 originPosition;
    
    public override void OnTick(ICombatant affected)
    {
        // Custom poison tick logic
        float distance = Vector3.Distance(
            affected.transform.position, 
            originPosition
        );
        
        // Damage scales with distance (closer = more damage)
        float damage = dotDamage * (1 - distance * 0.1f);
        affected.ApplyDamage(damage, damageType, null);
    }
}
```

### Step 3: Apply Effect from Spell

```csharp
[CreateAssetMenu(menuName = "Game/Spell Effects/Apply Poison")]
public class ApplyPoisonEffect : SpellEffectDefinition
{
    public EffectData poisonEffect;
    
    public override void Execute(SpellContext context)
    {
        foreach (var target in context.Targets)
        {
            var handler = target.GetComponent<EffectHandler>();
            if (handler != null)
                handler.ApplyEffect(poisonEffect, context.Caster);
        }
    }
}
```

---

## Adding a New Character Attribute (Stat)

### Step 1: Extend RuntimeStatId Enum

```csharp
// In GlobalEnums.cs
public enum RuntimeStatId
{
    // Existing...
    Strength,
    Agility,
    Constitution,
    Intelligence,
    Wisdom,
    Charisma,
    
    // New custom stat
    FireResistance,      // [NEW]
    ColdResistance,      // [NEW]
    LootBonus,           // [NEW]
}
```

### Step 2: Add to CharacterStats

```csharp
// In CharacterStats.cs
public class CharacterStats : MonoBehaviour
{
    // Existing attributes...
    
    [SerializeField] private float fireResistance;    // [NEW]
    [SerializeField] private float coldResistance;    // [NEW]
    
    public float FireResistance => 
        _runtimeStatBlock.Calculate(fireResistance, RuntimeStatId.FireResistance);
    
    public float ColdResistance => 
        _runtimeStatBlock.Calculate(coldResistance, RuntimeStatId.ColdResistance);
    
    // Use in damage calculation:
    private float ReduceByResistance(float baseDamage, DamageType type)
    {
        if (type == DamageType.Magic)
            return baseDamage * (1 - FireResistance * 0.01f);
        return baseDamage;
    }
}
```

### Step 3: Update Derived Stats

```csharp
public void RecalculateDerivedStats()
{
    // Existing calculations...
    
    // New: Fire resistance affects mana regen
    _manaRegenRate = 5f + (Intelligence * 0.5f) + FireResistance * 0.1f;
}
```

---

## Adding a New Skill Tree Node

### Step 1: Create Skill Definition

```csharp
// Right-click > Create > Game/Skill > New Skill Node
// Configure:
//   - Node ID: "skill_fireball_expert"
//   - Display Name: "Fireball Expert"
//   - Tier: 2
//   - Skill Point Cost: 2
//   - Prerequisites:
//     - Required Level: 10
//     - Parent Node: "skill_fire_magic"
//   - Passive Bonuses:
//     - Intelligence +5 (Flat)
//     - Spell Power +15% (Percent)
```

### Step 2: Update Skill Tree Definition

```csharp
// In your SkillTreeDefinition asset:
// 1. Add new node to _allNodes list
// 2. Verify prerequisites are defined
```

### Step 3: Test Unlock

```csharp
// In game or editor:
var skillService = GetComponent<SkillProgressionService>();
skillService.UnlockSkill("skill_fireball_expert");
// Check that stats increased and event fired
```

---

## Adding a New Enemy Type

### Step 1: Create Enemy Prefab

```csharp
// 1. Duplicate existing enemy prefab
// 2. Rename to "EnemyOrc" or similar
// 3. Modify components:
//    - CharacterStats: Increase base attributes
//    - EnemyAI: Adjust detection/attack ranges
//    - Renderer: Change sprite/color
```

### Step 2: Configure AI Behavior

```csharp
// In EnemyAI inspector:
[SerializeField] private float detectionRange = 12f;     // Aggressive
[SerializeField] private float attackRange = 3f;         // Close combat
[SerializeField] private float patrolSpeed = 3.5f;
[SerializeField] private float chaseSpeed = 6.5f;        // Faster
```

### Step 3: Add Custom AI State (Optional)

```csharp
using Game.Character;
using UnityEngine;

public class MiniBossAI : EnemyAI
{
    [SerializeField] private float summonCooldown = 10f;
    private float _summonTimer;
    
    protected override void OnAttack()
    {
        _summonTimer -= Time.deltaTime;
        
        if (_summonTimer <= 0)
        {
            SummonMinions();
            _summonTimer = summonCooldown;
        }
        else
        {
            base.OnAttack();
        }
    }
    
    private void SummonMinions()
    {
        // Instantiate weak enemies around boss
        for (int i = 0; i < 2; i++)
        {
            var angle = (i / 2f) * Mathf.PI * 2;
            var pos = transform.position + 
                new Vector3(Mathf.Cos(angle), Mathf.Sin(angle)) * 3f;
            
            Instantiate(minionPrefab, pos, Quaternion.identity);
        }
    }
}
```

---

## Subscribing to Events

### Listen to XP Gain

```csharp
using Game.CoreRuntime;

public class LevelTracker : MonoBehaviour
{
    private void Start()
    {
        GameSession.Instance.EventBus.Subscribe<XpGrantedEvent>(OnXpGained);
        GameSession.Instance.EventBus.Subscribe<LevelUpEvent>(OnLevelUp);
    }
    
    private void OnXpGained(XpGrantedEvent @event)
    {
        Debug.Log($"Gained {@event.xpAmount} XP from {@event.source}");
    }
    
    private void OnLevelUp(LevelUpEvent @event)
    {
        Debug.Log($"Leveled up to {@event.newLevel}!");
        // Play sound, particle effect, etc.
    }
    
    private void OnDestroy()
    {
        // Clean up subscriptions
        GameSession.Instance?.EventBus
            .Unsubscribe<XpGrantedEvent>(OnXpGained);
    }
}
```

### Listen to Spell Casts

```csharp
private void Start()
{
    GameSession.Instance.EventBus.Subscribe<SpellCastStartEvent>(OnSpellCast);
    GameSession.Instance.EventBus.Subscribe<SpellResolvedEvent>(OnSpellResolved);
}

private void OnSpellCast(SpellCastStartEvent @event)
{
    // Show cast bar
    Debug.Log($"Casting {nameof(@event.spell)}...");
}

private void OnSpellResolved(SpellResolvedEvent @event)
{
    // Update UI, play effects
    Debug.Log($"{@event.spell.displayName} resolved!");
}
```

---

## Adding Persistence (Save/Load)

### Register for Save System

```csharp
using Game.CoreRuntime;

public class PlayerProgress : MonoBehaviour, ISaveDataProvider
{
    public int questsCompleted;
    public Dictionary<string, bool> discoveredLocations;
    
    private void Start()
    {
        GameSession.Instance.SaveService.Register(this);
    }
    
    public SaveData Serialize()
    {
        return new SaveData
        {
            questsCompleted = this.questsCompleted,
            locations = discoveredLocations.Keys.ToList(),
        };
    }
    
    public void Deserialize(SaveData data)
    {
        questsCompleted = data.questsCompleted;
        discoveredLocations = data.locations
            .ToDictionary(l => l, l => true);
    }
}
```

---

# Расширение движка

> **Read in English:** [EXTENDING.md](EXTENDING.md)

Руководства для добавления новых систем, механик и функционала.

## Добавление нового спелла

### Шаг 1: Создать определение спелла (ScriptableObject)

```
1. В редакторе: Right-click > Create > Game/Spell > New Spell
2. Заполнить свойства:
   - ID: "lightning_strike"
   - Display Name: "Lightning Strike"
   - Cast Time: 0.8
   - Cooldown: 5.0
   - Mana Cost: 45
```

### Шаг 2: Создать кастомный эффект спелла (если нужно)

Наследовать `SpellEffectDefinition` и реализовать `Execute(SpellContext)`.

### Шаг 3: Добавить эффект к определению спелла

В ScriptableObject спелла добавить эффект в список effects.

### Шаг 4: Добавить на панель спеллов игрока (опционально)

В инспекторе SpellCasterRuntime добавить спелл в availableSpells.

## Добавление нового баффа/дебаффа

### Шаг 1: Создать данные эффекта (ScriptableObject)

```
Right-click > Create > Game/Effect > New Effect
Конфигурировать:
  - ID: "weakness_curse"
  - Effect Type: DebuffStats
  - Duration: 15.0
  - Stat Modifiers: Strength -10
```

### Шаг 2: Создать кастомный эффект (если нужно)

Наследовать `EffectData` для кастомной логики.

### Шаг 3: Применить эффект из спелла

Создать `ApplyEffectDefinition`, который вызывает `EffectHandler.ApplyEffect()`.

## Добавление нового характеристика персонажа

### Шаг 1: Расширить RuntimeStatId enum

```csharp
public enum RuntimeStatId
{
    // Существующие...
    
    // Новые
    FireResistance,
    ColdResistance,
    LootBonus,
}
```

### Шаг 2: Добавить в CharacterStats

```csharp
public float FireResistance => 
    _runtimeStatBlock.Calculate(fireResistance, RuntimeStatId.FireResistance);
```

### Шаг 3: Обновить производные статы

Использовать новую характеристику в `RecalculateDerivedStats()`.

## Добавление узла дерева навыков

### Шаг 1: Создать определение навыка

```
Right-click > Create > Game/Skill > New Skill Node
Конфигурировать:
  - Node ID: "skill_fireball_expert"
  - Required Level: 10
  - Passive Bonuses: Intelligence +5
```

### Шаг 2: Обновить дерево навыков

Добавить узел в `_allNodes` список.

### Шаг 3: Протестировать разблокировку

```csharp
skillService.UnlockSkill("skill_fireball_expert");
```

## Добавление нового типа врага

### Шаг 1: Создать префаб врага

Дублировать существующий, изменить:
- CharacterStats: увеличить атрибуты
- EnemyAI: дальность обнаружения/атаки
- Renderer: спрайт/цвет

### Шаг 2: Конфигурировать поведение ИИ

```csharp
[SerializeField] private float detectionRange = 12f;
[SerializeField] private float attackRange = 3f;
[SerializeField] private float chaseSpeed = 6.5f;
```

### Шаг 3: Добавить кастомное состояние (опционально)

Наследовать `EnemyAI` для специального поведения (босс-сумощик, и т.д.).

## Подписка на события

### Слушать получение опыта

```csharp
GameSession.Instance.EventBus.Subscribe<XpGrantedEvent>(OnXpGained);
GameSession.Instance.EventBus.Subscribe<LevelUpEvent>(OnLevelUp);
```

### Слушать касты спеллов

```csharp
GameSession.Instance.EventBus.Subscribe<SpellCastStartEvent>(OnSpellCast);
GameSession.Instance.EventBus.Subscribe<SpellResolvedEvent>(OnSpellResolved);
```

## Добавление сохранения/загрузки

### Регистрация в системе сохранения

```csharp
public class PlayerProgress : MonoBehaviour, ISaveDataProvider
{
    private void Start()
    {
        GameSession.Instance.SaveService.Register(this);
    }
    
    public SaveData Serialize() { /* ... */ }
    public void Deserialize(SaveData data) { /* ... */ }
}
```

---
