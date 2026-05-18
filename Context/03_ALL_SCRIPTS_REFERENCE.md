# All Scripts Reference

## Custom Game Scripts (Assets/Scripts/)

### 1. GameManager.cs
**Path:** `Assets/Scripts/GameManager.cs` | **Singleton:** Yes (`GameManager.instance`)

Central game state manager. Tracks collectibles, game state, and difficulty.

**Public Fields:**
- `candlesCollected` (int, default 0) — candles the player has picked up
- `candlesNeeded` (int, default 4) — candles required to escape
- `hasBasementKey` (bool) — whether player has the basement key
- `hasSmallKey` (bool) — whether player has the small key
- `journalPagesFound` (int) — journal pages collected
- `gameOver`, `gameWon`, `isPaused` (bool) — game state flags
- `currentDifficulty` (enum: Easy/Normal/Hard/Extreme)

**Key Methods:**
- `CollectItem(ItemType, itemName)` — called by PickupItem.OnPickup(). Handles Battery, Candle, Key, JournalPage
- `TriggerGameOver()` — stops time, shows game over screen
- `TriggerWin()` — stops time, shows win screen
- `SetDifficulty(Difficulty)` — changes difficulty and applies settings
- `ApplyDifficultySettings()` — finds all GrannyStyleAI and FlashlightController, multiplies their values

**Controls:** P key toggles pause

---

### 2. GrannyStyleAI.cs
**Path:** `Assets/Scripts/GrannyStyleAI.cs` | **Requires:** NavMeshAgent, Rigidbody (set kinematic)

The enemy AI. Uses NavMeshAgent for pathfinding. Two states: Patrol and Chase.

**Public Fields:**
- `chaseRange` (float, 50) — distance to start chasing
- `attackRange` (float, 2) — distance to attack
- `losePlayerRange` (float, 60) — distance to stop chasing
- `chaseSpeed` (float, 6) / `patrolSpeed` (float, 1.5)
- `waypointContainer` (Transform) — parent of child waypoint Transforms
- `attackCooldown` (float, 1) / `damageAmount` (int, 100)
- `hearingRange` (float, 20)

**Behavior:**
- **Patrol:** Moves between waypoints. Switches to chase if player within `chaseRange`
- **Chase:** Pursues player at `chaseSpeed`. Attacks if within `attackRange`. Returns to patrol if player beyond `losePlayerRange`
- **Attack:** Calls `PlayerHealth.TakeDamage(100)`. If no PlayerHealth found, calls `GameManager.TriggerGameOver()`
- **Sound:** `HearNoise(position)` — if within hearing range, enters chase mode
- **Footsteps:** Plays `AudioManager.grannyFootstep` every 0.5s while moving
- **Y-axis fix:** Flattens player Y position to avoid 3D distance confusion

**Setup Requirements:** Must be placed on/near NavMesh. Uses `NavMesh.SamplePosition` + `agent.Warp` to snap to NavMesh at Start.

---

### 3. PlayerHealth.cs (Playerhealth.cs)
**Path:** `Assets/Scripts/Playerhealth.cs`

Player health system with damage, death, and healing.

**Public Fields:**
- `maxHealth` (int, 100) / `currentHealth` (int)
- `damageScreenFlashDuration` (float, 0.3) / `damageColor` (red, 30% alpha)

**Key Methods:**
- `TakeDamage(int)` — reduces health, updates UI, triggers damage flash coroutine, calls Die() at 0
- `Die()` — plays death sound, disables CharacterController and all scripts containing "Movement"/"Controller"/"Player" in name, calls GameManager.TriggerGameOver()
- `Heal(int)` — restores health up to max
- `GetHealthPercent()` — returns 0-1 float

---

### 4. FlashlightController.cs
**Path:** `Assets/Scripts/FlashlightController.cs`

Flashlight with battery drain and toggle.

**Public Fields:**
- `maxBattery` (float, 100) / `drainRate` (float, 2) — battery per second when on
- `toggleKey` (KeyCode.F)
- `flashlight` (Light reference)

**Behavior:**
- Drains battery while on. Light intensity = batteryPercent * 3
- When battery hits 0, flashlight turns off automatically
- `AddBattery(float)` — refills battery (called by PickupItem for Battery type)
- Updates UIManager every frame with battery level

---

### 5. PlayerInteraction.cs
**Path:** `Assets/Scripts/PlayerInteraction.cs`

Main interaction system. Raycasts from screen center to detect interactables.

**Public Fields:**
- `interactRange` (float, 2.5) / `interactKey` (KeyCode.E)
- `playerCamera` (Camera reference)

**Behavior:**
- Raycasts from screen center each frame
- Detects `PickupItem` → shows "Press E to pick up [name]" → E or left-click picks up
- Detects `Door` → shows "Press E to open door" → E or left-click opens
- Hides prompt when not looking at interactable

---

### 6. PlayerInteract.cs
**Path:** `Assets/Scripts/PlayerInteract.cs`

Alternate/legacy interaction system. Raycasts forward from transform.

**Public Fields:**
- `interactRange` (float, 3) / `keyManager` (KeyManager reference)

**Behavior:**
- Raycasts from transform.position forward
- Only detects objects tagged "Key"
- On E or left-click: calls PickupItem.OnPickup() or falls back to KeyManager.PickUpKey()

**Note:** This coexists with PlayerInteraction.cs — likely one is used and the other is legacy.

---

### 7. Door.cs
**Path:** `Assets/Scripts/Door.cs`

Interactive door that rotates 90° when opened.

**Public Fields:**
- `requiresKey` (bool) / `requiredKeyName` (string, "SmallKey")
- `isOpen` (bool) / `openSpeed` (float, 2)
- `makesNoise` (bool, true) / `doorNoiseRadius` (float, 20)

**Behavior:**
- `TryOpen()` — checks if key is required & if player has it, then opens
- Opens with smooth Quaternion.Slerp rotation (90° Y)
- Plays AudioManager.PlayDoor() on open
- Emits sound via SoundEmitter → can alert AI

---

### 8. AutoDoor.cs
**Path:** `Assets/Scripts/AutoDoor.cs`

Proximity-based auto-opening door. Opens when player OR enemy is near.

**Public Fields:**
- `doorTransform` (Transform) — the actual door mesh to rotate
- `openAngle` (float, -180) / `openSpeed` (float, 20) / `triggerDistance` (float, 3)

**Behavior:**
- Checks distance to player (tag "Player") and all enemies (tag "Enemy") every frame
- Smoothly rotates door open/closed with Slerp
- Closes automatically when no one is nearby

---

### 9. ExitDoor.cs
**Path:** `Assets/Scripts/ExitDoor.cs`

The escape door. Checks win conditions and triggers game win.

**Public Fields:**
- `requiresAllCandles` (bool, false) / `requiresKey` (bool, true)
- `requiredKeyName` (string, "BasementKey")
- `blockingCollider` (Collider) — solid collider that blocks passage

**Behavior:**
- On trigger enter: checks if player meets escape conditions, auto-wins if yes
- Shows contextual prompts: "find X more candle(s)", "find the key", or "Press E to ESCAPE!"
- On E key: disables blocking collider and triggers win
- `CanEscape()` — checks candle count and key possession

---

### 10. PickupItem.cs
**Path:** `Assets/Scripts/PickupItem.cs`

Collectible item component for batteries, candles, keys, journal pages.

**Enum ItemType:** Battery, Candle, Key, JournalPage

**Public Fields:**
- `itemType` (ItemType) / `batteryAmount` (float, 30) / `itemName` (string)
- `makesNoiseWhenDropped` (bool, true) / `dropNoiseRadius` (float, 15)

**Behavior:**
- `OnPickup()` — plays pickup sound, calls GameManager.CollectItem(), destroys self
- For Key type: also notifies KeyManager.PickUpKey()
- For Battery type: refills flashlight via FlashlightController.AddBattery()
- OnCollisionEnter: if dropped with velocity > 2, emits sound (alerts AI)

---

### 11. AudioManager.cs
**Path:** `Assets/Scripts/AudioManager.cs` | **Singleton:** Yes

Central audio manager with music and SFX sources.

**Audio Clip Fields:**
- `backgroundMusic` — looping background music
- `footstepSound`, `doorSound`, `pickupSound`, `flashlightClick`, `deathScream`, `grannyFootstep`

**Methods:** PlayMusic(), PlaySFX(), PlayFootstep(), PlayDoor(), PlayPickup(), PlayFlashlightClick(), PlayDeath(), PlayGrannyFootstep()

---

### 12. UIManager.cs
**Path:** `Assets/Scripts/UIManager.cs` | **Singleton:** Yes

HUD manager using Unity UI (Sliders + TextMeshPro).

**UI References:**
- `healthSlider` (Slider) / `batterySlider` (Slider)
- `candleText` (TextMeshProUGUI) — shows "Candles: X/Y"
- `crosshair` (Image)
- `interactionText` (TextMeshProUGUI) — context prompt

**Methods:** UpdateHealth(), UpdateBattery(), UpdateCandles(), ShowInteractionPrompt(), HideInteractionPrompt()

---

### 13. ScreenManager.cs
**Path:** `Assets/Scripts/ScreenManager.cs` | **Singleton:** Yes

Manages overlay screens (game over, win, pause).

**References:** `gameOverScreen`, `winScreen`, `pauseMenu` (GameObjects)

**Methods:**
- `ShowGameOver()` / `ShowWinScreen()` — activates panel, unlocks cursor
- `PauseGame()` / `ResumeGame()` — toggles pause menu, time scale, cursor
- `RestartGame()` — reloads current scene
- `LoadMainMenu()` — loads "MainMenu" scene
- **P key** toggles pause in Update()

---

### 14. KeyManager.cs
**Path:** `Assets/Scripts/KeyManager.cs`

UI helper for key collection display.

**References:** `keysCounterText`, `interactionPromptText`, `notificationText` (TextMeshProUGUI)

**Methods:**
- `PickUpKey()` — increments counter, shows notification
- `ShowInteractionPrompt()` / `HideInteractionPrompt()`
- `ShowNotification(message)` — displays message for 5 seconds

---

### 15. PlayerFootsteps.cs (Playerfootsteps.cs)
**Path:** `Assets/Scripts/Playerfootsteps.cs` | **Requires:** SoundEmitter

Player footstep sound + noise emission system.

**Public Fields:**
- `walkStepInterval` (0.5) / `runStepInterval` (0.3)
- `walkNoiseRadius` (5) / `runNoiseRadius` (20)
- `sprintKey` (LeftShift) / `runSpeedThreshold` (5)

**Behavior:**
- Checks Rigidbody horizontal velocity each frame
- If moving: plays footstep audio at intervals and emits sound (alerts AI)
- Running = LeftShift held OR speed > threshold → louder, more frequent

---

### 16. SoundEmitter.cs (Soundemitter.cs)
**Path:** `Assets/Scripts/Soundemitter.cs`

Sound propagation component for the AI hearing system.

**Public Fields:** `soundRadius` (float, 15), `debugMode` (bool)

**Methods:**
- `EmitSound(position, radiusOverride)` — finds all GrannyStyleAI, calls HearNoise on each
- `EmitSoundHere()` — shortcut using transform.position

---

### 17. MainMenuController.cs
**Path:** `Assets/Scripts/MainMenuController.cs`

Main menu buttons.

**Methods:**
- `StartGame()` — loads "SampleScene"
- `QuitGame()` — Application.Quit() (+ editor stop in editor)
- Start() — unlocks cursor, sets timeScale = 1

---

### 18. MobileMovements.cs
**Path:** `Assets/Scripts/MobileMovements.cs` | **Class name:** MobileMoveButtons

Virtual button input for mobile controls.

**Property:** `MoveInput` (Vector2)
**Methods:** PressUp/ReleaseUp, PressDown/ReleaseDown, PressLeft/ReleaseLeft, PressRight/ReleaseRight

---

## FPS Controller Scripts (Mini First Person Controller/)

### FirstPersonMovement.cs
Rigidbody-based FPS movement. WASD + mouse. Supports running (Shift), jumping (Space), mobile button input, speed overrides for crouch.

### FirstPersonLook.cs
Camera pitch control. Supports mouse drag, touch look, and cursor locking. Outputs `LookDelta` for body rotation.

### CameraFollow.cs
Smoothly follows player yaw rotation. Keeps pitch from FirstPersonLook untouched.

### Crouch.cs
Hold LeftCtrl to crouch. Lowers camera head position and capsule collider. Reduces movement speed via speed override system.

### Jump.cs
Legacy jump component using Rigidbody.AddForce. **Note:** FirstPersonMovement.cs has its own jump — both may coexist.

### GroundCheck.cs
Raycasts down to check if grounded. Fires `Grounded` event on landing.

### Zoom.cs
Mouse scroll wheel zoom. Lerps camera FOV between 60 and 15.

### FirstPersonAudio.cs
Audio for walking/running/crouching/jumping/landing. Uses FirstPersonMovement.IsRunning and Crouch.IsCrouched states.

## StarterAssets Scripts

### FirstPersonController.cs (StarterAssets namespace)
CharacterController-based FPS controller. Uses Unity Input System. Supports Cinemachine camera. **May not be the active controller** — the Mini First Person Controller appears to be the primary one used.

### BasicRigidBodyPush.cs
Pushes rigidbodies on CharacterController collision. Configurable push layers and strength.

### StarterAssetsInputs.cs
Input wrapper for the new Input System. Provides move, look, jump, sprint inputs.

## Editor Scripts (Assets/Editor/)

### ApplyMedievalDoorTextures.cs
Editor tool (Tools → Fix Medieval Door Textures). Finds door textures, creates a Standard shader material, applies to all "DoorEntrance" objects.

### NavMeshCleaner.cs
Editor tool (Tools → Wipe All NavMesh Components). Removes all NavMeshSurface and NavMeshModifier components and clears baked data.
