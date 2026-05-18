# AI Quick-Start Prompt

> **Copy-paste this file (or the entire Context folder) into any AI chat to give it full project context.**

## Project Summary

This is a **Unity 6 (6000.3.15f1) first-person survival horror escape game** called **Haunted Game** using **URP**. The player is trapped in a haunted house and must collect 4 candles + a key while avoiding an AI enemy ("Granny"), then escape through the exit door.

## Tech Stack
- **Engine:** Unity 6 (6000.3.15f1)
- **Render Pipeline:** URP 17.3.0
- **Language:** C#
- **Physics:** Rigidbody-based player, NavMeshAgent-based AI
- **UI:** Unity UI (Canvas + Sliders + TextMeshPro)
- **Input:** Legacy Input (Input.GetKey) + partial New Input System support
- **AI Navigation:** com.unity.ai.navigation 2.0.12

## File Map (Custom Scripts Only)

```
Assets/Scripts/
├── GameManager.cs          — Singleton. Game state, collectible tracking, difficulty system
├── GrannyStyleAI.cs        — Enemy AI. NavMesh patrol → chase → attack. Sound detection
├── PlayerHealth.cs         — 100 HP system. TakeDamage → Die → GameOver
├── FlashlightController.cs — Toggle F key. Battery drains at 2/sec. Dims light with battery
├── PlayerInteraction.cs    — Raycast from camera center. Detects PickupItem + Door. E/Click
├── PlayerInteract.cs       — Legacy raycast. Only detects "Key" tag. Uses KeyManager
├── PickupItem.cs           — Battery/Candle/Key/JournalPage. OnPickup → GameManager.CollectItem
├── Door.cs                 — Interactive door. Optional key requirement. 90° rotation. Alerts AI
├── AutoDoor.cs             — Proximity auto-open. Reacts to Player + Enemy tags
├── ExitDoor.cs             — Win condition door. Checks candles + key → TriggerWin
├── AudioManager.cs         — Singleton. Central SFX/music. PlayFootstep/Door/Pickup/Death etc
├── UIManager.cs            — Singleton. HUD sliders + text. Health/Battery/Candles/Prompts
├── ScreenManager.cs        — Singleton. GameOver/Win/Pause screens. P key pause
├── KeyManager.cs           — Key collection UI. Counter text + notifications
├── PlayerFootsteps.cs      — Walk/run footstep sounds + SoundEmitter alerts AI
├── SoundEmitter.cs         — Noise propagation. EmitSound → all GrannyStyleAI.HearNoise
├── MainMenuController.cs   — Start button loads SampleScene. Quit exits app
└── MobileMovements.cs      — Virtual D-pad input for mobile (class: MobileMoveButtons)

Assets/Mini First Person Controller/Scripts/
├── FirstPersonMovement.cs  — Rigidbody WASD movement. Run/Jump/Mobile support
├── FirstPersonLook.cs      — Mouse/touch camera pitch. Outputs LookDelta
├── CameraFollow.cs         — Syncs camera yaw to player body
└── Components/
    ├── Crouch.cs            — Hold Ctrl. Lowers head + collider. Slows speed
    ├── Jump.cs              — Legacy AddForce jump (may be unused)
    ├── GroundCheck.cs       — Raycast ground detection
    ├── Zoom.cs              — Scroll wheel FOV zoom
    └── FirstPersonAudio.cs  — Walk/run/crouch/jump/land audio

Assets/Editor/
├── ApplyMedievalDoorTextures.cs — Tools menu: fixes pink door materials
└── NavMeshCleaner.cs            — Tools menu: wipes all NavMesh components
```

## Key Patterns

1. **Singletons** — GameManager, UIManager, AudioManager, ScreenManager all use `static instance` pattern (no DontDestroyOnLoad)
2. **Interaction** — Raycast from camera center → detect component (PickupItem/Door) → E key / left click
3. **Sound → AI** — SoundEmitter.EmitSound() → finds all GrannyStyleAI → HearNoise() → chase
4. **Difficulty** — Multipliers applied at Start() to AI speed/hearing/vision and flashlight drain

## Win/Lose Conditions

- **Win:** Collect all candles (if required) + have the right key + reach ExitDoor + press E
- **Lose:** Health reaches 0 from enemy attack (100 damage vs 100 HP = instant kill)

## Scenes

- `MainMenu` (index 0) — Title screen
- `SampleScene` (index 1) — Main gameplay

## Tags: Player, Key, Enemy
## Layers: Player (6), obstacle (7)

## Context Files in This Folder

| File | Contents |
|------|----------|
| 01_PROJECT_OVERVIEW.md | Game concept, Unity version, folder structure, packages |
| 02_ARCHITECTURE_AND_SYSTEMS.md | Singleton pattern, dependency graph, game flow, difficulty |
| 03_ALL_SCRIPTS_REFERENCE.md | Every script with fields, methods, and behavior |
| 04_PLAYER_SYSTEMS.md | Movement, health, flashlight, interaction, footsteps |
| 05_ENEMY_AI_SYSTEM.md | AI states, detection, patrol, chase, attack, sound |
| 06_ITEMS_DOORS_INTERACTION.md | Pickup items, door types, interaction flow |
| 07_UI_AUDIO_SCENES.md | HUD, screens, audio clips, scene management |
| 08_3D_ASSETS_AND_ENVIRONMENT.md | 3D models, environment, editor tools |
| 09_KNOWN_ISSUES_AND_NOTES.md | Bugs, technical debt, development notes |
| 10_AI_QUICKSTART_PROMPT.md | This file — summary for AI assistants |
| 11_SOURCE_CODE_PART1.md | Full code: GameManager, AudioManager, UIManager, ScreenManager, MainMenu |
| 12_SOURCE_CODE_PART2.md | Full code: GrannyStyleAI, PlayerHealth, FlashlightController, PlayerFootsteps |
| 13_SOURCE_CODE_PART3.md | Full code: PickupItem, Door, AutoDoor, ExitDoor, PlayerInteraction, SoundEmitter, KeyManager, Mobile |
| 14_SOURCE_CODE_PART4.md | Full code: FirstPersonMovement, FirstPersonLook, CameraFollow, Crouch, Editor tools |
