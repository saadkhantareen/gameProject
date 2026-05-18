# 3D Assets & Environment

## Asset Packs Used

### Horror House (`Assets/horror house/`)
- Main haunted house 3D model
- Contains source model and textures

### Lurker (`Assets/lurker/`)
- Enemy character 3D model (the "Granny" enemy)
- Contains materials, source model, and textures
- Torso texture: `TorsoMat_baseColor.png` (also at root of Assets)

### Candle Light (`Assets/candle-light/`)
- Candle 3D model used as collectible items
- Contains materials, source model, and textures

### Flashlight Battery (`Assets/flashlight-battery/`)
- Battery 3D model for battery pickup items
- Contains source model

### Resident Evil Keys (`Assets/resident-evil-keys/`)
- Key 3D models for door unlock items
- Contains source model and textures

### Medieval Door Pack (`Assets/medieval-door-pack/`)
- Door 3D models used in the game
- Contains materials, source model, and textures
- Has editor tool (`ApplyMedievalDoorTextures.cs`) to fix pink/missing textures
- Textures: Albedo, Metallic, Normal, AO for SM_DoorEntranceCastle

### Church of St Peter Stourton (`Assets/church-of-st-peter-stourton/`)
- Church 3D model (exterior environment)
- Additional church model at `Assets/church.glb` (48 MB)

### Dead by Daylight The First (`Assets/dead-by-daylight-the-first/`)
- Character model (possibly player or additional enemy)
- Contains source model and textures

### 3DForge / Town Creator Kit LITE (`Assets/3DForge/`, `Assets/Town Creator Kit LITE/`)
- Fantasy exterior building assets
- Contains FBX models, prefabs, and textures
- Sub-categories: Blueprints, FantasyExteriors

## Environment Setup

### Skybox
- Material: `Assets/Materials/Skybox1.mat`
- HDR: `Assets/Materials/rogland_clear_night_2k.hdr` (clear night sky)
- Creates dark, nighttime horror atmosphere

### Lighting
- Settings file: `Assets/New Lighting Settings.lighting`
- Uses URP with default volume profile
- Dark ambient lighting for horror feel

### NavMesh
- Built using `com.unity.ai.navigation` 2.0.12
- Required for GrannyStyleAI pathfinding
- Editor tool available: Tools → Wipe All NavMesh Components (`NavMeshCleaner.cs`)

---

## Editor Tools

### Fix Medieval Door Textures (Tools menu)
**File:** `Assets/Editor/ApplyMedievalDoorTextures.cs`

- Searches for door textures (Albedo, Metallic, Normal, AO)
- Creates a Standard shader material
- Auto-applies to all GameObjects named "DoorEntrance"
- Fixes pink/missing material issues

### Wipe All NavMesh Components (Tools menu)
**File:** `Assets/Editor/NavMeshCleaner.cs`

- Removes all NavMeshSurface components
- Removes all NavMeshModifier components
- Clears baked NavMesh data
- Useful for NavMesh cleanup/rebuild
