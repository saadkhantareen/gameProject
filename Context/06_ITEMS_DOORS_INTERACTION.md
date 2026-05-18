# Items, Doors & Interaction System

## Collectible Items (PickupItem.cs)

### Item Types

| Type         | Effect                                     | Tracked By           |
|-------------|--------------------------------------------|-----------------------|
| Battery     | Adds `batteryAmount` (30) to flashlight    | FlashlightController  |
| Candle      | Increments `candlesCollected`              | GameManager           |
| Key         | Sets `hasBasementKey` or `hasSmallKey`     | GameManager           |
| JournalPage | Increments `journalPagesFound`             | GameManager           |

### Pickup Flow

```
Player looks at item (raycast from screen center)
  → UIManager shows "Press E to pick up [itemName]"
  → Player presses E or Left Click
    → PickupItem.OnPickup() called
      → AudioManager.PlayPickup()
      → GameManager.CollectItem(type, name)
      → If Key: KeyManager.PickUpKey() (UI notification)
      → If Battery: FlashlightController.AddBattery(amount)
      → Destroy(gameObject)
```

### Item Noise System

Items with Rigidbody can alert the AI if dropped:
- `makesNoiseWhenDropped` (default true)
- Triggers on collision if `relativeVelocity.magnitude > 2`
- Emits sound with `dropNoiseRadius` (default 15 units)
- SoundEmitter notifies all GrannyStyleAI enemies

### 3D Models Used

- **Candles:** `Assets/candle-light/` folder
- **Batteries:** `Assets/flashlight-battery/` folder
- **Keys:** `Assets/resident-evil-keys/` folder

---

## Door System

### Regular Doors (Door.cs)

Interactive doors that the player opens manually.

**Configuration:**
- `requiresKey` (bool) — if true, checks GameManager for the required key
- `requiredKeyName` — "SmallKey" or "BasementKey"
- `openSpeed` (float, 2) — smooth rotation speed
- `doorNoiseRadius` (float, 20) — sound alert radius

**Opening Animation:** Smooth 90° Y rotation via Quaternion.Slerp

**Flow:**
```
Player looks at door → "Press E to open door"
  → Door.TryOpen()
    → If requiresKey && !hasKey → "You need the [keyName]"
    → Else → OpenDoor()
      → Rotates 90° with animation
      → AudioManager.PlayDoor()
      → SoundEmitter.EmitSound() → alerts AI
```

### Auto Doors (AutoDoor.cs)

Proximity-based doors that open/close automatically.

**Configuration:**
- `doorTransform` — the actual door mesh to rotate
- `openAngle` (-180°) — rotation angle
- `openSpeed` (20) — fast Slerp
- `triggerDistance` (3) — detection distance

**Behavior:**
- Opens when Player (tag "Player") OR any Enemy (tag "Enemy") is within trigger distance
- Closes automatically when everyone moves away
- Smooth Slerp animation in both directions

### Exit Door (ExitDoor.cs)

The escape door — triggers the win condition.

**Configuration:**
- `requiresAllCandles` (bool, false) — whether all candles must be collected
- `requiresKey` (bool, true) — whether a key is needed
- `requiredKeyName` ("BasementKey") — which key
- `blockingCollider` — solid collider that prevents walking through

**Win Conditions Checked:**
1. If `requiresAllCandles`: candlesCollected >= candlesNeeded
2. If `requiresKey`: player has the specified key

**Behavior:**
- OnTriggerEnter: checks conditions, shows prompt, auto-wins if conditions met
- E key press: TryEscape() → disables blocking collider → GameManager.TriggerWin()
- Contextual messages: "find X more candle(s)", "find the key", "Press E to ESCAPE!"

**3D Models:** `Assets/medieval-door-pack/` folder

---

## Interaction Detection

### Primary System (PlayerInteraction.cs)
- Raycasts from **screen center** using `playerCamera`
- Range: 2.5 units
- Detects: any object with `PickupItem` or `Door` component
- Trigger: E key or Left Mouse Click

### Legacy System (PlayerInteract.cs)
- Raycasts **forward** from transform.position
- Range: 3 units
- Only detects: objects with tag "Key"
- Uses KeyManager for UI prompts

**Note:** Both scripts may be on the player — PlayerInteraction is the general-purpose one.
