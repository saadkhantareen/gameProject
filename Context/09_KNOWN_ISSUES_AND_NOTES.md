# Known Issues, Bugs & Technical Debt

## Potential Bugs

### 1. Difficulty Multiplier Compounding
**File:** `GameManager.cs` → `ApplyDifficultySettings()`

The method multiplies the AI and flashlight **current** values rather than resetting them from base values first. If `SetDifficulty()` or `ApplyDifficultySettings()` is called more than once, the values compound exponentially.

**Example:** Calling twice on Normal (1.0x) is fine, but switching from Easy to Hard would apply Hard multipliers on already-reduced Easy values instead of the original base values.

**Fix needed:** Store original base values and multiply from those instead.

---

### 2. Duplicate Pause Logic
**Files:** `GameManager.cs` (line 91) AND `ScreenManager.cs` (line 87)

Both scripts listen for the **P key** to toggle pause. This creates a conflict:
- `GameManager.TogglePause()` only toggles `isPaused` flag and timeScale
- `ScreenManager.Update()` toggles pause menu UI, timeScale, and cursor

Both execute on the same keypress, potentially causing desync between `GameManager.isPaused` and `ScreenManager.isPaused`.

**Fix needed:** Remove pause handling from one of them (probably GameManager).

---

### 3. Duplicate Interaction Scripts
**Files:** `PlayerInteract.cs` AND `PlayerInteraction.cs`

Two different interaction systems exist:
- `PlayerInteraction.cs` — general purpose, raycasts from camera center, detects PickupItem + Door
- `PlayerInteract.cs` — legacy, raycasts forward from transform, only detects "Key" tagged objects

If both are on the player, they could conflict or double-process key pickups.

**Fix needed:** Consolidate into one system or ensure only one is active.

---

### 4. Enemy Attack = Instant Kill
**File:** `GrannyStyleAI.cs` → `AttackPlayer()`

`damageAmount = 100` against `maxHealth = 100` means every attack is an instant kill. This might be intentional but makes the health bar pointless since there's no partial damage scenario.

---

### 5. Missing PlayerHealth Fallback
**File:** `GrannyStyleAI.cs` → `AttackPlayer()` (line 207-211)

If `PlayerHealth` component is not found on the player, it directly calls `GameManager.TriggerGameOver()`. This bypasses any death sound/animation.

---

### 6. FindObjectsOfType Performance
**Files:** `AutoDoor.cs`, `SoundEmitter.cs`

- `AutoDoor.Update()` calls `GameObject.FindGameObjectsWithTag("Enemy")` every frame
- `SoundEmitter.EmitSound()` calls `FindObjectsByType<GrannyStyleAI>()` on every sound

These are expensive operations every frame. Should cache references.

---

### 7. ExitDoor Auto-Win on Trigger Enter
**File:** `ExitDoor.cs` (line 24-27)

If the player enters the exit trigger zone while already meeting all conditions, it immediately triggers win without requiring E key press. This could cause accidental wins.

---

### 8. Class Name vs File Name Mismatch
- File `MobileMovements.cs` contains class `MobileMoveButtons`
- File `Playerfootsteps.cs` contains class `PlayerFootsteps`
- File `Playerhealth.cs` contains class `PlayerHealth`
- File `Soundemitter.cs` contains class `SoundEmitter`

Unity handles this fine, but it can cause confusion.

---

## Technical Debt

### No Save System
No save/load functionality. Game restarts from scratch each time.

### No Animation System
- No player animations (arms, weapon)
- No enemy walk/attack animations (if the model supports them, they're not hooked up)
- Door animations are code-driven rotation only

### No Inventory UI
Keys and items are tracked as boolean flags / counters, but there's no visual inventory screen.

### Damage Flash Not Implemented
`PlayerHealth.DamageFlash()` coroutine exists but only waits — no actual visual effect is applied.

### No Journal Page Reading
`journalPagesFound` is tracked but there's no UI to read or display collected journal pages.

### Healing Items Missing
`PlayerHealth.Heal()` exists but no healing items are placed in the game.

### StarterAssets Controller Unused
The `StarterAssets/FirstPersonController` (CharacterController-based) is imported but appears unused — the game uses the Mini First Person Controller (Rigidbody-based) instead.

---

## Scene Notes

### Recovery Scenes
`Assets/_Recovery/` contains 3 backup scenes: `0.unity`, `0 (1).unity`, `0 (2).unity`
These appear to be earlier versions of the gameplay scene.

### Saad Scene
`Assets/Scenes/saad.unity` is an alternate gameplay scene (559 KB, larger than SampleScene at 480 KB). May contain additional level design or be a development copy.
