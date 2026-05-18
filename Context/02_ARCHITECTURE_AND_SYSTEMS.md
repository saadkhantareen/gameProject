# Architecture & Systems

## Singleton Pattern

The game uses singletons accessed via `ClassName.instance`:

| Manager           | File                  | Purpose                                    |
|-------------------|-----------------------|--------------------------------------------|
| `GameManager`     | `GameManager.cs`      | Game state, item tracking, difficulty       |
| `UIManager`       | `UIManager.cs`        | HUD updates (health, battery, candles)      |
| `AudioManager`    | `AudioManager.cs`     | Centralized audio playback                  |
| `ScreenManager`   | `ScreenManager.cs`    | Game Over, Win, Pause screens               |

**None use `DontDestroyOnLoad`** — they are re-created per scene.

## System Dependency Graph

```
GameManager (central hub)
├── Tracks: candlesCollected, hasBasementKey, hasSmallKey, journalPagesFound
├── Receives: PickupItem.OnPickup() → CollectItem()
├── Calls: UIManager.UpdateCandles(), ScreenManager.ShowGameOver/ShowWinScreen()
├── Controls: Difficulty → Applied to GrannyStyleAI + FlashlightController

UIManager (HUD)
├── UpdateHealth() ← PlayerHealth
├── UpdateBattery() ← FlashlightController
├── UpdateCandles() ← GameManager
├── ShowInteractionPrompt() ← PlayerInteraction, ExitDoor

AudioManager (audio hub)
├── PlayFootstep() ← PlayerFootsteps
├── PlayDoor() ← Door
├── PlayPickup() ← PickupItem
├── PlayFlashlightClick() ← FlashlightController
├── PlayDeath() ← PlayerHealth
├── PlayGrannyFootstep() ← GrannyStyleAI

ScreenManager (screen overlays)
├── ShowGameOver() ← GameManager.TriggerGameOver()
├── ShowWinScreen() ← GameManager.TriggerWin()
├── PauseGame()/ResumeGame() ← P key
├── RestartGame()/LoadMainMenu() ← UI buttons

SoundEmitter (AI alert system)
├── EmitSound() → Notifies ALL GrannyStyleAI.HearNoise()
├── Used by: PlayerFootsteps, Door, PickupItem
```

## Game Flow

```
MainMenu → [Start] → SampleScene
  ├── Player moves/interacts
  ├── Collects candles + keys
  ├── Avoids AI enemy
  ├── Win: ExitDoor → GameManager.TriggerWin()
  └── Lose: PlayerHealth.Die() → GameManager.TriggerGameOver()
End screens: [Restart] or [Main Menu]
```

## Difficulty System

4 levels multiply AI and flashlight parameters:

| Setting                  | Easy | Normal | Hard | Extreme |
|--------------------------|------|--------|------|---------|
| AI Speed Multiplier      | 0.7  | 1.0    | 1.3  | 1.5     |
| AI Hearing Multiplier    | 0.8  | 1.0    | 1.5  | 2.0     |
| AI Vision Multiplier     | 0.8  | 1.0    | 1.3  | 1.5     |
| Flashlight Drain Mult.   | 0.5  | 1.0    | 1.5  | 2.0     |

**Potential Bug:** `ApplyDifficultySettings()` multiplies base values, so calling it multiple times compounds the effect.

## Sound Detection System

1. `SoundEmitter` attached to noise-making objects
2. `EmitSound(position, radius)` → finds all `GrannyStyleAI` enemies
3. Each enemy checks `HearNoise(position)` against `hearingRange`
4. If in range → Chase mode

**Sound sources:**
- Player footsteps (walk: 5 units, run: 20 units)
- Doors opening (default 20 units)
- Dropped items (impact velocity > 2, default 15 units)
