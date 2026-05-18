# Player Systems

## Player Controller (Mini First Person Controller)

The primary player controller uses a **Rigidbody-based** movement system from the "Mini First Person Controller" package, customized for this game.

### Components on Player Object

| Component              | Script                     | Purpose                           |
|------------------------|----------------------------|-----------------------------------|
| Rigidbody              | (Unity built-in)           | Physics-based movement            |
| CapsuleCollider        | (Unity built-in)           | Player collision                  |
| FirstPersonMovement    | FirstPersonMovement.cs     | WASD movement + jumping           |
| FirstPersonLook        | FirstPersonLook.cs         | Mouse/touch camera pitch          |
| CameraFollow           | CameraFollow.cs            | Camera yaw follows player body    |
| PlayerInteraction      | PlayerInteraction.cs       | Raycast item/door interaction     |
| PlayerHealth           | Playerhealth.cs            | 100 HP health system              |
| PlayerFootsteps        | Playerfootsteps.cs         | Footstep sounds + AI alerts       |
| FlashlightController   | FlashlightController.cs    | Flashlight toggle + battery       |
| SoundEmitter           | Soundemitter.cs            | Noise propagation for footsteps   |
| Crouch                 | Crouch.cs                  | Crouch mechanic                   |

### Movement

- **Walk Speed:** 5 units/s
- **Run Speed:** 9 units/s (hold Left Shift)
- **Jump Force:** 5 (impulse)
- **Mouse Sensitivity:** 2
- **Ground Check:** Physics.CheckSphere at `groundCheck` transform

Movement is Rigidbody-based: velocity is set directly in FixedUpdate. Direction is relative to player facing (yaw).

### Camera System

- `FirstPersonLook` handles **pitch** (up/down) on the camera's local rotation
- `FirstPersonMovement` handles **yaw** (left/right) on the player body
- `CameraFollow` syncs the camera's yaw to the player's yaw smoothly
- Supports: mouse drag, touch (mobile), optional cursor lock

### Controls

| Action       | Key/Input           | Script                  |
|-------------|---------------------|-------------------------|
| Move         | WASD               | FirstPersonMovement     |
| Look         | Mouse / Touch drag  | FirstPersonLook         |
| Sprint       | Left Shift          | FirstPersonMovement     |
| Jump         | Space               | FirstPersonMovement     |
| Crouch       | Left Ctrl           | Crouch                  |
| Interact     | E / Left Click      | PlayerInteraction       |
| Flashlight   | F                   | FlashlightController    |
| Pause        | P                   | ScreenManager           |
| Zoom         | Scroll wheel        | Zoom                    |

## Health System

- **Max HP:** 100
- **Damage per enemy attack:** 100 (instant kill on Normal difficulty)
- On damage: UI health bar updates, red flash coroutine starts
- On death: plays death scream SFX, disables CharacterController and all movement scripts, triggers Game Over
- Healing method exists (`Heal(int)`) but no healing items are currently implemented

## Flashlight System

- **Max Battery:** 100
- **Drain Rate:** 2 per second (modified by difficulty)
- **Light Intensity:** scales from 0 to 3 based on battery percentage
- Toggle with **F key** — plays click sound
- Battery items restore 30 battery by default
- When battery reaches 0: flashlight turns off automatically
- UI battery slider updates every frame

## Interaction System

Two interaction scripts exist:

1. **PlayerInteraction.cs** (primary) — raycasts from screen center via playerCamera
   - Detects: PickupItem, Door
   - Shows contextual UI prompts via UIManager

2. **PlayerInteract.cs** (legacy/alternate) — raycasts forward from transform
   - Only detects: "Key" tagged objects
   - Uses KeyManager for UI

## Footstep & Sound Alert System

- Checks Rigidbody horizontal velocity each frame
- Walking (speed > 0.1): emits sound every 0.5s with 5-unit radius
- Running (Shift held or speed > 5): emits sound every 0.3s with 20-unit radius
- Sound emission alerts ALL GrannyStyleAI enemies in the scene
- Also plays AudioManager.PlayFootstep() for audible feedback
