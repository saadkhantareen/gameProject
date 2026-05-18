# Enemy AI System (GrannyStyleAI)

## Overview

The game features a "Granny" enemy that patrols the house via waypoints and chases the player when detected. Uses Unity's **NavMeshAgent** for pathfinding.

## AI States

```
┌──────────┐    Player within chaseRange    ┌──────────┐
│  PATROL  │ ─────────────────────────────→ │  CHASE   │
│          │                                │          │
│ Walks    │ ←───────────────────────────── │ Runs     │
│ waypoints│    Player beyond losePlayerRange│ at player│
└──────────┘                                └──────────┘
                                                 │
                                                 │ Player within attackRange
                                                 ▼
                                            ┌──────────┐
                                            │  ATTACK  │
                                            │ (100 dmg)│
                                            └──────────┘
```

## Detection

| Trigger                      | Range              | Result            |
|------------------------------|--------------------|-------------------|
| Player distance < chaseRange | 50 units (default) | Chase mode        |
| Player distance > loseRange  | 60 units (default) | Back to patrol    |
| Player distance < attackRange| 2 units (default)  | Attack player     |
| HearNoise() within range     | 20 units (default) | Chase mode        |

**Y-axis fix:** Distance checks flatten the Y-axis so height differences don't confuse detection.

## Patrol Behavior

- Moves between child Transforms of `waypointContainer` at `patrolSpeed` (1.5)
- Cycles through waypoints sequentially
- Switches to next waypoint when `remainingDistance < 2`
- Requires a `waypointContainer` Transform with child GameObjects as waypoints

## Chase Behavior

- Pursues player at `chaseSpeed` (6.0)
- Targets player's ground-level position (Y flattened)
- Attacks on cooldown (1 second) when within attack range
- Returns to patrol when player escapes beyond `losePlayerRange` (60)

## Attack

- Deals `damageAmount` (100) via `PlayerHealth.TakeDamage()`
- If no PlayerHealth component found → directly calls `GameManager.TriggerGameOver()`
- Attack cooldown: 1 second
- Default 100 damage = instant kill against 100 HP player

## Sound Detection

- `HearNoise(Vector3 noisePosition)` is called by `SoundEmitter`
- If noise is within `hearingRange` (20 units) → enters Chase mode
- Sources of noise: player footsteps, door opening, item drops

## Setup Requirements

1. GameObject needs: NavMeshAgent, Rigidbody (auto-set to kinematic + freeze rotation)
2. Must be near a baked NavMesh — auto-warps within 10 units at Start
3. Needs `waypointContainer` assigned with child Transform waypoints
4. Player object must have tag "Player"
5. Enemy should have tag "Enemy" (for AutoDoor detection)
6. Needs AudioSource component for footstep sounds

## AI Footsteps

- Plays `AudioManager.grannyFootstep` clip every 0.5s while moving
- Uses agent's own AudioSource component

## Debug Gizmos (visible in Scene view when selected)

- **Yellow sphere:** Chase range
- **Red sphere:** Attack range
- **Blue sphere:** Hearing range

## Difficulty Scaling

The GameManager multiplies these base values on Start:
- `chaseSpeed` and `patrolSpeed` × AI Speed Multiplier
- `hearingRange` × AI Hearing Multiplier
- `chaseRange` × AI Vision Multiplier
