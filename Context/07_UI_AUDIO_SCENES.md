# UI, Audio & Scene Management

## UI System (UIManager.cs)

### HUD Elements

| Element           | Type              | What It Shows                    |
|-------------------|-------------------|----------------------------------|
| `healthSlider`    | Slider (0-1)      | Player health percentage         |
| `batterySlider`   | Slider (0-1)      | Flashlight battery percentage    |
| `candleText`      | TextMeshProUGUI   | "Candles: X/Y"                   |
| `crosshair`       | Image             | Center crosshair                 |
| `interactionText` | TextMeshProUGUI   | Context prompt (hidden by default)|

### Update Sources

- Health: updated by `PlayerHealth.TakeDamage()` and `Start()`
- Battery: updated every frame by `FlashlightController.Update()`
- Candles: updated by `GameManager.CollectItem()` when candle collected
- Interaction prompt: shown/hidden by `PlayerInteraction` and `ExitDoor`

### Key Manager UI (KeyManager.cs)

Separate UI system for key collection:
- `keysCounterText` — shows "keys: X/1"
- `interactionPromptText` — shows/hides when near key
- `notificationText` — shows "Picked up the key!" for 5 seconds

---

## Screen Manager (ScreenManager.cs)

### Screens

| Screen         | When Shown                  | Buttons Available      |
|---------------|-----------------------------|-----------------------|
| Game Over     | Player health reaches 0     | Restart, Main Menu    |
| Win Screen    | Player escapes through exit | Restart, Main Menu    |
| Pause Menu    | P key pressed               | Resume, Restart, Main Menu |

### Behavior
- All screens unlocks cursor (CursorLockMode.None, visible)
- Game Over / Win: Time.timeScale = 0 (frozen)
- Pause: toggles timeScale 0/1
- Restart: reloads current scene
- Main Menu: loads "MainMenu" scene

---

## Audio System (AudioManager.cs)

### Audio Sources

- `musicSource` — for background music (looping)
- `sfxSource` — for one-shot sound effects

### Audio Clips (assigned in Inspector)

| Clip               | File in Assets/Audios/    | Used By                |
|--------------------|---------------------------|------------------------|
| `backgroundMusic`  | Background.mp3            | Plays on Start (loop)  |
| `footstepSound`    | footstep.mp3              | PlayerFootsteps        |
| `doorSound`        | (assigned in inspector)   | Door.OpenDoor()        |
| `pickupSound`      | collecting.mp3            | PickupItem.OnPickup()  |
| `flashlightClick`  | Flashlight.mp3            | FlashlightController   |
| `deathScream`      | Scream.mp3                | PlayerHealth.Die()     |
| `grannyFootstep`   | Ghost footstep.mp3        | GrannyStyleAI.Update() |

### All Audio Files

| File                 | Size    | Purpose                    |
|---------------------|---------|----------------------------|
| Background.mp3      | 7.2 MB  | Background ambient music   |
| Flashlight.mp3      | 21 KB   | Flashlight click sound     |
| Game MEnu.mp3       | 740 KB  | Main menu music            |
| Ghost footstep.mp3  | 961 KB  | Granny footstep sounds     |
| Ghost.mp3           | 869 KB  | Ghost ambient sound        |
| JumpScare.mp3       | 259 KB  | Jump scare sound           |
| Scream.mp3          | 961 KB  | Death scream               |
| collecting.mp3      | 28 KB   | Item pickup sound          |
| footstep.mp3        | 125 KB  | Player footstep sound      |

---

## Scene Management

### MainMenu Scene
- Controlled by `MainMenuController.cs`
- Unlocks cursor, sets timeScale = 1
- **Start Game** → loads "SampleScene"
- **Quit** → Application.Quit()

### SampleScene (Gameplay)
- All singleton managers initialize here
- Player, AI, items, doors, and environment
- On win/lose: ScreenManager shows overlay
- Restart reloads same scene

### Scene Transitions
```
MainMenu → "Start Game" → SampleScene
SampleScene → "Restart" → SampleScene (reload)
SampleScene → "Main Menu" → MainMenu
```

---

## Materials & Visual Assets

| Material/Asset             | Purpose                              |
|---------------------------|--------------------------------------|
| Skybox1.mat               | Night sky skybox                     |
| rogland_clear_night_2k.hdr| HDR environment for skybox           |
| ExitDoor.mat              | Exit door material                   |
| TorsoMat_baseColor.mat    | Enemy/character torso texture        |
| New Physics Material       | Physics material for surfaces        |
