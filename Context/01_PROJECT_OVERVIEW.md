# Haunted Game — Project Overview

## What Is This Game?

**Haunted Game** is a **first-person horror/survival escape game** built in **Unity 6 (6000.3.15f1)** using the **Universal Render Pipeline (URP)**. The player is trapped inside a haunted house and must collect items (candles, keys, batteries) while avoiding an AI enemy ("Granny") that patrols the house and chases the player when it detects them. The goal is to collect all required items and escape through the exit door.

## Game Genre & Inspirations

- **Genre:** First-person survival horror
- **Inspiration:** Granny (mobile horror game), Resident Evil (key-based exploration), Dead by Daylight (chase mechanics)
- **Platform:** PC (keyboard/mouse) + Mobile (touch controls)

## Core Gameplay Loop

1. Player spawns inside a haunted house
2. Explore the house using a flashlight (battery drains over time)
3. Collect **4 candles** scattered around the house
4. Find **keys** (BasementKey, SmallKey) to unlock doors
5. Avoid the **Granny AI enemy** — she patrols, hears sounds, and chases you
6. If caught, player takes damage (100 HP system) → Game Over on death
7. Once all candles are collected and you have the right key → escape through the **Exit Door** → Win!

## Unity Version & Render Pipeline

- **Unity Version:** `6000.3.15f1` (Unity 6)
- **Render Pipeline:** Universal Render Pipeline (URP) 17.3.0
- **Input System:** Unity New Input System 1.19.0 (coexists with legacy Input)

## Build Scenes (in build order)

| Index | Scene Name     | Path                              | Purpose               |
|-------|---------------|-----------------------------------|------------------------|
| 0     | MainMenu      | `Assets/Scenes/MainMenu.unity`    | Title screen / Start   |
| 1     | SampleScene   | `Assets/Scenes/SampleScene.unity` | Main gameplay scene    |

There is also a `saad.unity` scene in the Scenes folder (appears to be an alternate/backup gameplay scene) and recovery scenes in `Assets/_Recovery/`.

## Tags & Layers

### Custom Tags
- `Key` — Used on key pickup objects so the `PlayerInteract` raycast can detect them
- `Enemy` — Used on the AI enemy (Granny) for `AutoDoor` detection

### Custom Layers
- `Player` (Layer 6) — The player character
- `obstacle` (Layer 7) — Obstacle objects (used for physics/navmesh)

## Key Packages (from manifest.json)

| Package                          | Version  | Purpose                        |
|----------------------------------|----------|--------------------------------|
| com.unity.ai.navigation         | 2.0.12   | NavMesh for AI pathfinding     |
| com.unity.cinemachine            | 3.1.6    | Camera system                  |
| com.unity.inputsystem            | 1.19.0   | New Input System               |
| com.unity.render-pipelines.universal | 17.3.0 | URP rendering                |
| com.unity.ugui                   | 2.0.0    | UI system (Canvas, Sliders)    |
| com.unity.visualscripting        | 1.9.11   | Visual scripting (may not be used) |
| com.unity.modules.ai             | 1.0.0    | NavMeshAgent support           |

## Project Folder Structure

```
Haunted-Game/
├── Assets/
│   ├── Scripts/              ← All custom game scripts (18 .cs files)
│   ├── Scenes/               ← MainMenu, SampleScene, saad
│   ├── Audios/               ← Sound effects (footsteps, ghost, scream, etc.)
│   ├── Materials/            ← Skybox, physics material, textures
│   ├── Editor/               ← Editor tools (door texture fixer, navmesh cleaner)
│   ├── Mini First Person Controller/ ← FPS controller (Rigidbody-based)
│   ├── StarterAssets/        ← Unity StarterAssets FPS (CharacterController-based)
│   ├── 3DForge/              ← Town Creator Kit LITE (exterior building assets)
│   ├── Town Creator Kit LITE/ ← Fantasy exterior building prefabs/FBX/textures
│   ├── horror house/         ← Horror house 3D model
│   ├── lurker/               ← Lurker enemy 3D model
│   ├── candle-light/         ← Candle light 3D model
│   ├── flashlight-battery/   ← Flashlight battery 3D model
│   ├── resident-evil-keys/   ← Key 3D models
│   ├── medieval-door-pack/   ← Door 3D models + textures
│   ├── church-of-st-peter-stourton/ ← Church 3D model
│   ├── dead-by-daylight-the-first/  ← Dead by Daylight character model
│   ├── _Recovery/            ← Backup scenes (0.unity, 0 (1).unity, 0 (2).unity)
│   └── TextMesh Pro/         ← TextMeshPro assets
├── ProjectSettings/          ← Unity project configuration
├── Packages/                 ← Package manifest
└── Library/                  ← Unity cache (auto-generated)
```
