# Development Status & Roadmap

> **Also available in Russian:** [DEVELOPMENT_STATUS_RU.md](DEVELOPMENT_STATUS_RU.md)

## Current Project State

**Branch:** `spells_system` (active development)

**Latest Commit:** 3dce6c9 - Refactor SceneRuntimeBootstrap to improve DamageTextManager initialization

**Last Updated:** April 28, 2026

---

## System Completion Status

### ✅ Fully Implemented & Tested

| System | Status | Notes |
|--------|--------|-------|
| **CoreRuntime** | Complete | GameSession, EventBus, SaveService working |
| **Combat** | Complete | Damage types, targeting, stat modifiers implemented |
| **Character Stats** | Complete | All base and derived attributes functional |
| **PlayerController** | Complete | Movement, sprint, dash fully implemented |
| **EnemyAI** | Complete | Patrol, chase, attack behaviors working |
| **Inventory** | Complete | Slot-based system with stacking |
| **UI Basics** | Complete | Health/mana/stamina bars, damage text display |

### 🚧 In Active Development

| System | Progress | Current Task |
|--------|----------|--------------|
| **Spells** | 85% | Testing new spell effects, UI refinement |
| **Skills** | 90% | Finalizing skill tree unlock system |
| **Effects** | 95% | Stacking rules, edge case handling |
| **Spell UI** | 70% | Action bar implementation, cooldown display |

### 📋 Not Started

| System | Planned For | Notes |
|--------|-------------|-------|
| **Quests** | v1.1 | Dialog, objectives, rewards |
| **Loot Generation** | v1.1 | Random item drops, rarity system |
| **Dialogue System** | v1.2 | NPC interactions, branching paths |
| **Particles** | v1.2 | Visual effects for spells/abilities |
| **Multiplayer Sync** | v2.0 | Network framework, session handling |
| **Advanced Pathfinding** | v2.0 | Dynamic obstacle avoidance |

---

## Known Issues & Limitations

### High Priority (v1.0 blockers)

| Issue | Impact | Workaround | Fix Target |
|-------|--------|-----------|-----------|
| **Spell casting doesn't interrupt movement** | Gameplay feel | Player must manually stop before casting | v1.0 RC1 |
| **Effect stacking visual not clear** | UX | Add stack count to buff icons | v1.0 RC2 |
| **Enemy pathfinding sometimes fails on obstacles** | AI | Place navmesh manually on all prefabs | v1.0.1 |

### Medium Priority (v1.1+)

| Issue | Impact | Notes |
|-------|--------|-------|
| **No sound effects** | Immersion | Requires audio engineer |
| **Limited spell variety** | Content | Need to add 20+ spells |
| **No difficulty scaling** | Replayability | Enemies don't scale with level |
| **UI doesn't support controller** | Accessibility | Mouse-only currently |

### Low Priority (Future)

- Particle effect system not implemented
- No player customization UI
- No statistics tracking

---

## Recent Changes (Last 5 Commits)

```
3dce6c9 - Refactor SceneRuntimeBootstrap to improve DamageTextManager initialization
876af78 - Add Core meta files and remove outdated test scripts  
4a99c3c - Enhance character and enemy systems with damage text management
b5d87e1 - Merge pull request #9 (Enemy-NPC system)
ac241d4 - Add .cursorignore with Unity project ignore patterns
```

### Uncommitted Changes (Currently on `spells_system`)

**Modified:**
- `Assets/Game/Scripts/Core/SceneRuntimeBootstrap.cs` — GameSession initialization improvements
- `Assets/Game/Scenes/SampleScene.unity` — Updated entity references

**New (Not committed):**
- `Assets/Game/Scripts/Combat/` — 4 files
- `Assets/Game/Scripts/Spells/` — 5 files
- `Assets/Game/Scripts/Skills/` — 3 files
- `Assets/Game/Scripts/CoreRuntime/` — 5 files

---

## Version Timeline

### ✅ v0.9 (Current — 2026-04-28)

**Completed:**
- Core engine architecture (EventBus, GameSession)
- Character system (stats, player controller, enemy AI)
- Combat system (damage, targeting, effects)
- Inventory system
- UI basics (health bars, damage text)

**Status:** Feature-complete for MVP, finalizing spell system

### 📋 v1.0 (Target: 2026-05-15)

**Goals:**
- Spell system fully working with 10+ spells
- Skill tree unlockable and functional
- Boss encounter (mini-boss enemy)
- 30-minute gameplay loop complete
- All known high-priority issues resolved

**Blockers to v1.0:**
- ❓ Spell casting interruption fix
- ❓ Additional spell effects creation
- ❓ Skill tree UI visualization

### 📋 v1.1 (Target: 2026-06-15)

**Planned:**
- Quest system (5 story quests)
- Loot generation with rarity
- Difficulty scaling (Normal/Hard/Nightmare)
- 20+ additional spells
- Sound effects
- 60-minute gameplay loop

### 📋 v1.2+ (Backlog)

**Future:**
- Dialogue system
- Particle effects
- Additional enemy types
- New game+ mode
- Leaderboards

### 📋 v2.0 (Speculative)

**Long-term:**
- Multiplayer framework
- Advanced AI (formations, tactics)
- Dynamic quest generation
- Seasonal content

---

## Git Workflow & Branches

### Main Branches

| Branch | Purpose | Current State |
|--------|---------|---------------|
| `main` | Release/stable | Latest: ac241d4 |
| `v1.0` | v1.0 release candidate | Synced with spells_system |
| `spells_system` | Active development | HEAD (3dce6c9) |

### Feature Branches (Merged)

- `Enemy-NPC` (merged → v1.0)
- `effects_mng` (merged → v1.0)
- `inventory_v1` (merged → v1.0)
- `PlayerController` (merged → v1.0)

### Next Steps for Branches

1. **Finalize `spells_system`** — Complete pending changes, commit
2. **Create `v1.0.0` release** — Tag and push to main
3. **Create `quests_system`** — Start v1.1 work

---

## Performance Metrics

### Current Performance (SampleScene)

| Metric | Value | Target |
|--------|-------|--------|
| FPS (idle) | 300+ | 60 |
| FPS (combat, 5 enemies) | 180+ | 60 |
| Memory (loaded scene) | ~120MB | < 200MB |
| Script compilation | ~2-3 sec | < 5 sec |

### Profiler Hot Spots

1. **FixedUpdate:** NavMeshAgent pathfinding (5 enemies)
2. **Update:** Event subscriptions on UI update
3. **Memory:** Asset bundles not implemented (each spell definition loaded)

**Optimization roadmap:** Implement asset bundles in v1.1

---

## Asset & Content Inventory

### Spells (Implemented)
- ✅ DirectDamage spell (default)
- ✅ Heal spell (basic)
- 🚧 Fireball (in progress)
- 🚧 Lightning (in progress)

### Spells (Planned)
- Frost bolt
- Meteor
- Chain lightning
- Holy light
- Darkness wave
- (+ 20 more planned for v1.1)

### Enemy Types
- ✅ Generic Enemy
- 📋 Orc (planned for v1.0)
- 📋 Golem (planned for v1.0)
- 📋 Mini-boss (planned for v1.0)

### Items
- ✅ Health Potion
- ✅ Mana Potion
- ✅ Stamina Potion
- 📋 Equipment (planned for v1.1)

---

## Testing Status

### Unit Tests
- ❌ Not implemented
- 📋 Planned for v1.1

### Integration Tests
- ⚠️ Manual only (play in editor and test)
- Core systems: Verified working
- Edge cases: Need automated testing

### Known Test Gaps
- Spell effect interactions (overflow, null targets)
- Effect stacking with multiple modifiers
- Save/load with active buffs

---

## Code Quality

### Documentation
- ✅ Architecture documented
- ✅ Systems documented
- ✅ Extension guide complete
- 🚧 Code comments (inline)
- ❌ Video tutorials

### Code Standards
- ✅ Consistent naming (PascalCase types, camelCase fields)
- ✅ Separation of concerns
- 🚧 Comments on complex logic
- 📋 Code review process (to be formalized)

### Technical Debt
1. **SceneRuntimeBootstrap:** Could use dependency injection
2. **CharacterStats:** Getting large (~400 lines), could split
3. **EventBus:** No priority system or error handling
4. **Duplicate code:** UI update patterns repeated

**Debt backlog for v1.1**

---

## Dependencies & Requirements

### Engine
- Unity 2022.3 LTS or newer
- .NET Framework 4.x

### Packages
- Unity Input System 1.5.1+
- NavMesh Components 1.1.4+
- TextMesh Pro (built-in)

### No External Plugins
- Pure C# implementation
- No physics dependencies beyond built-in Rigidbody2D
- No AI middleware (custom AI)

---

## Deployment Status

### Build Configurations
- ✅ Development builds work
- ⚠️ Release builds untested
- 📋 WebGL export (planned for v1.1)
- 📋 Mobile (planned for v2.0)

### Distribution
- 📋 itch.io release (v1.0)
- 📋 GitHub release (v1.0)
- 📋 Steam (v2.0+, speculative)

---

## Contact & Contribution

**Project Owner:** Tut-Kak-Tut  
**Repository:** [Local Git]  
**Current Focus:** Finalizing spell system for v1.0

---

# Статус разработки и Roadmap

> **Read in English:** [DEVELOPMENT_STATUS.md](DEVELOPMENT_STATUS.md)

## Текущее состояние проекта

**Ветка:** `spells_system` (активная разработка)

**Последний коммит:** 3dce6c9 - Refactor SceneRuntimeBootstrap

**Обновлено:** 28 апреля 2026

---

## Статус систем

### ✅ Полностью реализовано и протестировано

| Система | Статус | Заметки |
|---------|--------|---------|
| **CoreRuntime** | Готово | GameSession, EventBus работают |
| **Combat** | Готово | Типы урона, таргетирование работают |
| **Character Stats** | Готово | Все атрибуты функциональны |
| **PlayerController** | Готово | Движение, спринт, рывок готовы |
| **EnemyAI** | Готово | Поведение ИИ работает |
| **Inventory** | Готово | Слот-система со стакированием |
| **UI** | Готово | Полоски здоровья, урон, текст |

### 🚧 В активной разработке

| Система | Прогресс | Текущая задача |
|---------|----------|----------------|
| **Spells** | 85% | Тестирование эффектов |
| **Skills** | 90% | Финализация разблокировки |
| **Effects** | 95% | Правила стакирования |
| **Spell UI** | 70% | Панель действий |

### 📋 Не начато

| Система | Планируется для | Заметки |
|---------|-----------------|---------|
| **Quесты** | v1.1 | Диалоги, награды |
| **Лут** | v1.1 | Случайные предметы |
| **Диалоговая система** | v1.2 | NPC диалоги |
| **Частицы** | v1.2 | Визуальные эффекты |

---

## Известные проблемы

### Высокий приоритет (v1.0 блокеры)

| Проблема | Влияние | Статус |
|----------|--------|--------|
| **Каст спелла не прерывает движение** | Геймплей | Нужен фикс |
| **Стакирование эффектов не ясно** | UX | Добавить счетчик |
| **ИИ иногда не находит путь** | AI | Размещение navmesh |

### Средний приоритет

- Нет звуковых эффектов
- Ограниченный выбор спеллов
- Враги не масштабируются с уровнем

---

## Версионирование

### ✅ v0.9 (Текущая — 28.04.2026)

**Готово:** MVP, все базовые системы

### 📋 v1.0 (Целевая дата: 15.05.2026)

**Цели:**
- Спеллы полностью работают (10+)
- Дерево навыков функционально
- Босс-встреча
- 30-минутный геймплей

### 📋 v1.1 (Целевая дата: 15.06.2026)

**Планы:**
- Система квестов
- Лут с редкостью
- Масштабирование сложности
- 20+ новых спеллов
- Звуки

### 📋 v2.0+ (Бэклог)

**Дальнее будущее:**
- Мультиплеер
- Продвинутый ИИ
- Сезонный контент

---

## Метрики производительности

| Метрика | Значение | Целевое |
|---------|----------|---------|
| FPS (холостой ход) | 300+ | 60 |
| FPS (бой, 5 врагов) | 180+ | 60 |
| Память (сцена) | ~120MB | < 200MB |
| Компиляция скриптов | 2-3 сек | < 5 сек |

---

## Инвентарь контента

### Спеллы (Реализовано)
- ✅ DirectDamage
- ✅ Heal
- 🚧 Fireball

### Типы врагов
- ✅ Обычный враг
- 📋 Орк (планируется)
- 📋 Голем (планируется)

### Предметы
- ✅ Health Potion
- ✅ Mana Potion
- ✅ Stamina Potion

---

## Качество кода

### Документация
- ✅ Архитектура описана
- ✅ Системы описаны
- ✅ Руководство расширения
- 🚧 Встроенные комментарии
- ❌ Видео-туториалы

### Технический долг
1. SceneRuntimeBootstrap — нужна DI
2. CharacterStats — слишком большой класс
3. EventBus — нет приоритета и обработки ошибок
4. Повторение кода — UI patterns

---

## Требования

- Unity 2022.3 LTS+
- Input System 1.5.1+
- NavMesh Components 1.1.4+
- TextMesh Pro (встроен)

---

## Развертывание

### Построение
- ✅ Development builds работают
- ⚠️ Release builds не протестированы
- 📋 WebGL (v1.1)
- 📋 Мобильные (v2.0)

### Распространение
- 📋 itch.io (v1.0)
- 📋 GitHub (v1.0)
- 📋 Steam (v2.0+)

---
