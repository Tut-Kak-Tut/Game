# RPG Game Engine - Project Documentation

> **Also available in Russian:** [README_RU.md](README_RU.md)

## Overview

This is a modular **Unity RPG Engine** built with a **data-driven, event-driven architecture**. The project features:

- **Spell System**: Modular spell framework with effects, cooldowns, and resource management
- **Combat System**: Targetable combat with stat modifiers, damage types, and team mechanics
- **Skill Tree**: Experience-based progression with prerequisite system
- **Effect System**: Stackable buffs/debuffs with DOT and HOT support
- **Character System**: Rich stat system with player controller and enemy AI
- **Inventory System**: Slot-based inventory with stackable items
- **Event Bus**: Pub/Sub architecture for loose coupling between systems

## Project Structure

```
Assets/Game/
├── Scripts/
│   ├── CoreRuntime/      # Game lifecycle, events, save system
│   ├── Combat/           # Combat mechanics, stat modifiers, targeting
│   ├── Spells/           # Spell definitions, casting, effects
│   ├── Skills/           # Skill tree, progression, XP
│   ├── Character/        # Stats, player, enemy AI
│   ├── Effects/          # Effect application, buffs/debuffs
│   ├── Inventory/        # Inventory management, items
│   └── UI/               # UI controllers and displays
├── Scenes/
│   └── SampleScene.unity # Main gameplay scene
├── Resources/
│   ├── Spells/           # ScriptableObject spell definitions
│   ├── Skills/           # Skill tree configurations
│   ├── Items/            # Item definitions
│   └── Effects/          # Effect data
└── Prefabs/              # UI prefabs, character prefabs
```

## Quick Start

### Prerequisites
- Unity 2022.3+
- Input System package
- NavMesh Components
- TextMesh Pro

### Setup
1. Clone and open the project in Unity
2. Open `Assets/Game/Scenes/SampleScene.unity`
3. Play to test

### Key Components
- **Player**: GameObject with `PlayerController`, `CharacterStats`, `SpellCasterRuntime`
- **Enemies**: GameObjects with `EnemyAI`, each linked to Player as target
- **GameSession**: DontDestroyOnLoad singleton managing event bus and saves

## Architecture Overview

```
┌─────────────────────────────────────────────────────────────┐
│                    GameSession (Singleton)                  │
│              ┌─────────────────┬──────────────┐             │
│              ▼                 ▼              ▼             │
│          EventBus         SaveService   Diagnostics         │
└──────────────┬──────────────────────────────────────────────┘
               │
    ┌──────────┼────────────┬─────────────┬──────────┐
    ▼          ▼            ▼             ▼          ▼
 Input    Character      Combat       Spells    Skills
 System   System         System       System    System
    │      │ │ │          │ │          │ │       │ │
    ▼      ▼ ▼ ▼          ▼ ▼          ▼ ▼       ▼ ▼
   Move   Stats  AI    Damage  Heal  Cast  Effect  XP  Prog
   Sprint DMG    Path  Modify  Cooldown     Stack  Level Tree
   Dash   Regen  Patrol Types   Resolve      DoT  Unlock
```

## Core Systems (CoreRuntime)

| Component | Purpose |
|-----------|---------|
| **GameSession** | Singleton lifecycle manager, persists across scenes |
| **RuntimeEventBus** | Pub/Sub system with event replay buffer |
| **SaveService** | Serialization with versioning support |

## Combat Flow

1. Player/Enemy **Input** → `PlayerController` / `EnemyAI`
2. Movement applies stat modifiers via `RuntimeStatBlock`
3. **Spell Cast** triggers `SpellCasterRuntime`
4. Spell effects execute: damage, healing, modifiers
5. Active effects tick (DOT, HOT, buffs/debuffs)
6. UI updates via event subscriptions

## Development Status

### ✅ Completed
- Core event system and singleton
- Combat with stat modifiers
- Spell casting framework
- Skill progression
- Effect system (buffs/debuffs/DoT)
- Inventory system
- Character/Enemy AI

### 🚧 In Development
- Spells system UI refinement
- Additional spell effects
- Skill tree visualization

### 📋 Roadmap
- Quest system
- Loot generation
- Multiplayer sync framework
- Advanced pathfinding
- Particle effects system

## Documentation

- **[ARCHITECTURE.md](ARCHITECTURE.md)** — System design and patterns
- **[SYSTEMS.md](SYSTEMS.md)** — Detailed system documentation
- **[EXTENDING.md](EXTENDING.md)** — Adding new spells, effects, skills
- **[DEVELOPMENT_STATUS.md](DEVELOPMENT_STATUS.md)** — Full status and roadmap

## Git Workflow

| Branch | Purpose |
|--------|---------|
| `main` | Stable releases |
| `v1.0` | Version 1.0 release |
| `spells_system` | Active development |

## Contact & Contribution

This is an active project. For issues or contributions, refer to the git repository structure.

---

# Документация проекта RPG Game Engine

> **Read in English:** [README.md](README.md)

## Описание

Это модульный **Unity RPG движок** с **data-driven и event-driven архитектурой**. Проект включает:

- **Система заклинаний**: Модульный фреймворк с эффектами, кулдаунами, управлением ресурсами
- **Боевая система**: Таргетирование с модификаторами статов и командной системой
- **Дерево навыков**: Прогрессия через опыт с системой предварительных условий
- **Система эффектов**: Стакуемые баффы/дебаффы с поддержкой ДОТ и ХОТ
- **Система персонажей**: Богатая система статов с контроллером игрока и ИИ врагов
- **Инвентарь**: Слот-based система с поддержкой стакования
- **Event Bus**: Pub/Sub архитектура для слабой связи между системами

## Структура проекта

```
Assets/Game/
├── Scripts/
│   ├── CoreRuntime/      # Жизненный цикл, события, система сохранений
│   ├── Combat/           # Боевая механика, модификаторы, таргетирование
│   ├── Spells/           # Определение спеллов, каст, эффекты
│   ├── Skills/           # Дерево навыков, прогрессия, опыт
│   ├── Character/        # Статы, игрок, ИИ врагов
│   ├── Effects/          # Применение эффектов, баффы/дебаффы
│   ├── Inventory/        # Управление инвентарем, предметы
│   └── UI/               # UI контроллеры и отображение
├── Scenes/
│   └── SampleScene.unity # Главная игровая сцена
├── Resources/
│   ├── Spells/           # ScriptableObject определения спеллов
│   ├── Skills/           # Конфигурации дерева навыков
│   ├── Items/            # Определения предметов
│   └── Effects/          # Данные эффектов
└── Prefabs/              # Префабы UI, персонажей
```

## Быстрый старт

### Требования
- Unity 2022.3+
- Input System пакет
- NavMesh Components
- TextMesh Pro

### Установка
1. Откройте проект в Unity
2. Откройте `Assets/Game/Scenes/SampleScene.unity`
3. Нажмите Play для тестирования

### Ключевые компоненты
- **Player**: GameObject с `PlayerController`, `CharacterStats`, `SpellCasterRuntime`
- **Враги**: GameObject с `EnemyAI`, связанный с Player как целью
- **GameSession**: Singleton с DontDestroyOnLoad, управляет event bus и сохранениями

## Архитектура

```
┌─────────────────────────────────────────────────────────────┐
│                    GameSession (Singleton)                  │
│              ┌─────────────────┬──────────────┐             │
│              ▼                 ▼              ▼             │
│          EventBus         SaveService   Diagnostics         │
└──────────────┬──────────────────────────────────────────────┘
               │
    ┌──────────┼────────────┬─────────────┬──────────┐
    ▼          ▼            ▼             ▼          ▼
 Input    Character      Combat       Spells    Skills
 System   System         System       System    System
    │      │ │ │          │ │          │ │       │ │
    ▼      ▼ ▼ ▼          ▼ ▼          ▼ ▼       ▼ ▼
   Move   Stats  AI    Damage  Heal  Cast  Effect  XP  Prog
   Sprint DMG    Path  Modify  Cooldown     Stack  Level Tree
   Dash   Regen  Patrol Types   Resolve      DoT  Unlock
```

## Основные системы (CoreRuntime)

| Компонент | Назначение |
|-----------|-----------|
| **GameSession** | Singleton-менеджер жизненного цикла, сохраняется между сценами |
| **RuntimeEventBus** | Pub/Sub система с буфером повтора событий |
| **SaveService** | Сериализация с поддержкой версионирования |

## Поток боевой механики

1. **Ввод** игрока/врага → `PlayerController` / `EnemyAI`
2. Движение применяет модификаторы через `RuntimeStatBlock`
3. **Каст спелла** вызывает `SpellCasterRuntime`
4. Эффекты спелла выполняются: урон, исцеление, модификаторы
5. Активные эффекты тикают (ДОТ, ХОТ, баффы/дебаффы)
6. UI обновляется через подписку на события

## Статус разработки

### ✅ Завершено
- Ядро event system и singleton
- Боевая система с модификаторами статов
- Фреймворк кастования спеллов
- Прогрессия навыков
- Система эффектов (баффы/дебаффы/ДОТ)
- Система инвентаря
- Персонажи и ИИ врагов

### 🚧 В разработке
- Рафинирование UI системы спеллов
- Добавление новых эффектов спеллов
- Визуализация дерева навыков

### 📋 Roadmap
- Система квестов
- Генерация лута
- Фреймворк синхронизации для мультиплеера
- Продвинутая навигация
- Система частиц

## Документация

- **[ARCHITECTURE.md](ARCHITECTURE.md)** — Дизайн систем и паттерны
- **[SYSTEMS.md](SYSTEMS.md)** — Подробная документация систем
- **[EXTENDING.md](EXTENDING.md)** — Добавление новых спеллов, эффектов, навыков
- **[DEVELOPMENT_STATUS.md](DEVELOPMENT_STATUS.md)** — Полный статус и roadmap

## Git Workflow

| Ветка | Назначение |
|-------|-----------|
| `main` | Стабильные релизы |
| `v1.0` | Релиз версии 1.0 |
| `spells_system` | Активная разработка |

---
