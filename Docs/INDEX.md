# Documentation Index

**RPG Game Engine — Complete Documentation**

## 🎨 Vision & Design

| Document | Purpose | Audience |
|----------|---------|----------|
| [GDD.md](GDD.md) | Game vision, world, core loop, classes, mechanics roadmap | Designers, project team |

## 📚 Main Documentation

| Document | Purpose | Audience |
|----------|---------|----------|
| [README.md](README.md) | Project overview, setup, quick start | Everyone |
| [ARCHITECTURE.md](ARCHITECTURE.md) | System design, patterns, data flow | Developers |
| [SYSTEMS.md](SYSTEMS.md) | Detailed system specifications | Developers |
| [EXTENDING.md](EXTENDING.md) | How to add features and systems | Developers |
| [DEVELOPMENT_STATUS.md](DEVELOPMENT_STATUS.md) | Status, roadmap, issues | Project team |

## 🎯 Quick Links by Role

### 🎨 Designer / Game Director
1. Start: [GDD.md](GDD.md) — Vision, world, classes, core loop
2. Cross-ref: [DEVELOPMENT_STATUS.md](DEVELOPMENT_STATUS.md) — What exists vs. what's planned

### 🚀 New Developer
1. Start: [README.md](README.md) — Setup & overview
2. Learn: [ARCHITECTURE.md](ARCHITECTURE.md) — How it works
3. Reference: [SYSTEMS.md](SYSTEMS.md) — Component details

### 🔧 Adding a Feature
1. Read: [EXTENDING.md](EXTENDING.md) — Step-by-step guides
2. Reference: [SYSTEMS.md](SYSTEMS.md) — API specs
3. Check: [DEVELOPMENT_STATUS.md](DEVELOPMENT_STATUS.md) — Current state

### 📊 Project Manager
1. Review: [DEVELOPMENT_STATUS.md](DEVELOPMENT_STATUS.md) — Progress & roadmap
2. Check: [README.md](README.md) — Current status

### 🐛 Debugging
1. Refer: [SYSTEMS.md](SYSTEMS.md) — Component responsibilities
2. Check: [ARCHITECTURE.md](ARCHITECTURE.md) — Data flow
3. Look: [DEVELOPMENT_STATUS.md](DEVELOPMENT_STATUS.md) — Known issues

## 📖 Documentation Structure

### GDD.md
- Vision & design pillars
- Setting, tone, lore direction
- World structure (tutorial → hub → 5 regions → procedural dungeons)
- Core gameplay loop
- Combat & death modes (Standard / Hardcore)
- Character progression (3 base classes × subclasses + passive tree)
- Loot philosophy (hybrid)
- Quests & emergent discovery
- NPCs, dialogue, vendors
- Crafting (4 professions)
- Mounts & companions
- Economy (gold/silver, 1:100)
- Day/night cycle
- Streaming architecture for regions
- Roadmap & open questions

### README.md
- Project description
- Features overview
- Setup & quick start
- High-level architecture
- Git workflow

### ARCHITECTURE.md
- Design philosophy
- Core architecture
- System layers
- Data flow examples
- Design patterns used
- Performance notes

### SYSTEMS.md
- CoreRuntime systems
- Combat system
- Character system
- Spells system
- Skills system
- Effects system
- Inventory system
- UI system

### EXTENDING.md
- Adding new spells (step-by-step)
- Adding buffs/debuffs
- Adding character stats
- Adding skill tree nodes
- Adding enemy types
- Event subscriptions
- Save/load integration

### DEVELOPMENT_STATUS.md
- System completion status
- Known issues & limitations
- Version timeline & roadmap
- Git branches
- Performance metrics
- Content inventory
- Testing status
- Code quality
- Dependencies

## 🔗 Navigation

All documents support bilingual content (English/Russian with clear separators).

**Available in:**
- English: Main sections at the top
- Russian: Main sections at the bottom (after `---` separator)

## 📝 Version History

| Version | Date | Changes |
|---------|------|---------|
| 1.0 | 2026-04-28 | Initial complete documentation |
| 1.1 | 2026-05-10 | Added GDD.md (game vision & design document) |

## ✅ Documentation Checklist

- [x] README with overview and setup
- [x] ARCHITECTURE with system design
- [x] SYSTEMS with detailed specs
- [x] EXTENDING with practical examples
- [x] DEVELOPMENT_STATUS with roadmap
- [x] Bilingual (English/Russian)
- [x] ASCII diagrams for architecture
- [x] Roadmap included

---

# Индекс документации

**Полная документация RPG Game Engine**

## 📚 Основная документация

Все документы доступны выше на английском, выше русской версии — см. структуру выше.

## 🎨 Видение и дизайн

| Документ | Назначение | Аудитория |
|----------|------------|-----------|
| [GDD.md](GDD.md) | Видение игры, мир, основной цикл, классы, дорожная карта механик | Дизайнеры, команда |

## 🎯 Быстрые ссылки по ролям

### 🎨 Дизайнер / Гейм-директор
1. Начните: [GDD.md](GDD.md) — Видение, мир, классы, основной цикл
2. Сопоставьте: [DEVELOPMENT_STATUS.md](DEVELOPMENT_STATUS.md) — Что уже есть vs. что в планах

### 🚀 Новый разработчик
1. Начните: [README.md](README.md) — Обзор и настройка
2. Изучите: [ARCHITECTURE.md](ARCHITECTURE.md) — Как это работает
3. Справочник: [SYSTEMS.md](SYSTEMS.md) — Детали компонентов

### 🔧 Добавление функционала
1. Прочитайте: [EXTENDING.md](EXTENDING.md) — Пошаговые руководства
2. Справочник: [SYSTEMS.md](SYSTEMS.md) — API
3. Проверьте: [DEVELOPMENT_STATUS.md](DEVELOPMENT_STATUS.md) — Текущее состояние

### 📊 Менеджер проекта
1. Просмотр: [DEVELOPMENT_STATUS.md](DEVELOPMENT_STATUS.md) — Прогресс и roadmap
2. Проверка: [README.md](README.md) — Статус

### 🐛 Отладка
1. Справочник: [SYSTEMS.md](SYSTEMS.md) — Ответственность компонентов
2. Проверка: [ARCHITECTURE.md](ARCHITECTURE.md) — Поток данных
3. Поиск: [DEVELOPMENT_STATUS.md](DEVELOPMENT_STATUS.md) — Известные проблемы

## 📖 Структура документации

### GDD.md
- Видение и принципы дизайна
- Сеттинг, тон, направление лора
- Структура мира (туториал → хаб → 5 регионов → процедурные подземелья)
- Основной игровой цикл
- Бой и режимы смерти (Standard / Hardcore)
- Прогрессия персонажа (3 базовых класса × подклассы + дерево пассивок)
- Философия лута (гибрид)
- Квесты и эмерджентное исследование
- NPC, диалоги, торговцы
- Ремёсла (4 профессии)
- Маунты и компаньоны
- Экономика (золотые/серебряные, 1:100)
- Цикл дня/ночи
- Архитектура стриминга регионов
- Дорожная карта и открытые вопросы

### README.md
- Описание проекта
- Обзор функций
- Настройка и быстрый старт
- Архитектура
- Git workflow

### ARCHITECTURE.md
- Философия дизайна
- Основная архитектура
- Слои систем
- Примеры потока данных
- Используемые паттерны

### SYSTEMS.md
- Системы CoreRuntime
- Боевая система
- Система персонажей
- Система спеллов
- Система навыков
- Система эффектов
- Система инвентаря
- UI система

### EXTENDING.md
- Добавление новых спеллов
- Добавление баффов/дебаффов
- Добавление характеристик
- Добавление узлов навыков
- Добавление типов врагов
- Подписка на события
- Интеграция сохранений

### DEVELOPMENT_STATUS.md
- Статус завершения систем
- Известные проблемы
- Версионирование и roadmap
- Git ветки
- Метрики производительности
- Инвентарь контента
- Статус тестирования
- Качество кода
- Зависимости

---
