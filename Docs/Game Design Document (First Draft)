# Anomaly

A 2D side-scrolling exploration and light combat game built in Unity.

---

## Overview

Anomaly is a 2D exploration game built around a dual-state system: Normal State and Anomaly State.

The player discovers a mysterious object known as the Black Marker Stone and a ring that allows access to an altered version of reality. This alternate state modifies environmental rules, enemy behavior, level structure, and influences the final outcome of the game.

---

## Core Gameplay Loop (MVP)

The core gameplay loop is designed as follows:

Enter Level  
→ Explore and navigate  
→ Combat / Collect items / Overcome obstacles  
→ Activate Ring (Anomaly State)  
→ World transformation occurs  
→ Complete objective  
→ Reach exit  
→ Level completion / ending result  

---

## Core Features

### Player System
- 2D movement (WASD)
- Jumping, climbing, crouching
- Melee combat system
- Health system (HP)
- Corruption system

---

### Level System
- 2D side-scrolling level structure
- Multiple themed environments:
  - Human settlement
  - Forest
  - Mining area
- Unified level objectives (reach exit / complete task)

---

### Combat System
- Collision-based melee combat
- Animation-driven attack timing
- Simple enemy AI:
  - Distance-based detection
  - Contact damage
- Behavior changes under Anomaly State

---

## Anomaly State System (Core Mechanic)

The game is built around a dual-state system:

### Normal State
- Standard enemy behavior
- Stable environment layout
- Fully visible paths and objects

### Anomaly State
Activated through a ring item, this state introduces systemic changes:

- Environment becomes visually distorted
- UI and visual effects are altered
- Hidden paths and objects are revealed
- Enemy behavior is modified (disabled or enhanced)
- Corruption value increases over time

---

## Corruption System

Corruption represents the player's exposure to the Anomaly State.

- Increases when Anomaly State is active
- Influences gameplay difficulty and narrative outcome
- Acts as a long-term risk-reward mechanic

---

## Ending System

The ending is determined by the final Corruption value:

| Corruption Level | Ending Type |
|------------------|------------|
| Low              | Standard Ending |
| Medium           | Neutral Ending |
| High             | Corrupted Ending |

---

## Project Structure

The project follows a modular system design:

- Player System
- Enemy AI System
- Level System
- State System (Normal / Anomaly)
- UI System
- Combat System

---

## Planned Features (Not in MVP)

- Inventory system (item stacking)
- Dialogue system
- Save system (PlayerPrefs)
- Multi-level progression
- Farming system (UI placeholder only)

---

## Design Philosophy

- State-driven gameplay (Normal vs Anomaly)
- Mechanic-first design approach
- Modular and low-coupling systems
- Focus on a complete playable core loop (MVP)

---

## Goal

The goal of this project is to build a compact and complete 2D game prototype featuring:

- A clear gameplay loop
- A dual-state transformation mechanic
- A consistent system architecture
- Strong mechanic-driven player experience
