# Full Source Code — Part 4 (FPS Controller + Editor Tools)

## FirstPersonMovement.cs
```csharp
using System.Collections.Generic;
using UnityEngine;

public class FirstPersonMovement : MonoBehaviour
{
    public float speed = 5;

    [Header("Running")]
    public MobileMoveButtons mobileButtons;
    public bool canRun = true;
    public bool IsRunning { get; private set; }
    public float runSpeed = 9;
    public KeyCode runningKey = KeyCode.LeftShift;

    [Header("Jumping")]
    public float jumpForce = 5f;
    public KeyCode jumpKey = KeyCode.Space;
    public LayerMask groundLayer = 1;
    public Transform groundCheck;
    public float groundDistance = 0.4f;

    [Header("Rotation")]
    public float mouseSensitivity = 2f;
    public float bodyRotationSpeed = 10f;
    public FirstPersonLook lookInput;
    public float lookYawOverrideThreshold = 0.001f;

    private bool isGrounded;
    private Rigidbody rb;
    private float yaw = 0f;

    public List<System.Func<float>> speedOverrides = new List<System.Func<float>>();

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        yaw = transform.eulerAngles.y;
    }

    void Update()
    {
        CheckGroundStatus();
        float yawInput = Input.GetAxisRaw("Mouse X") * mouseSensitivity;
        if (lookInput != null) yawInput = lookInput.LookDelta.x;
        yaw += yawInput;
        transform.rotation = Quaternion.Euler(0f, yaw, 0f);
        if (Input.GetKeyDown(jumpKey) && isGrounded) Jump();
    }

    void FixedUpdate()
    {
        IsRunning = canRun && Input.GetKey(runningKey);
        float targetMovingSpeed = IsRunning ? runSpeed : speed;
        if (speedOverrides.Count > 0)
            targetMovingSpeed = speedOverrides[speedOverrides.Count - 1]();

        float h = mobileButtons ? mobileButtons.MoveInput.x : Input.GetAxis("Horizontal");
        float v = mobileButtons ? mobileButtons.MoveInput.y : Input.GetAxis("Vertical");

        Vector3 forward = Quaternion.Euler(0f, yaw, 0f) * Vector3.forward;
        Vector3 right = Quaternion.Euler(0f, yaw, 0f) * Vector3.right;
        Vector3 moveDirection = forward * v + right * h;
        moveDirection.y = 0f;

        Vector3 targetVelocity = moveDirection * targetMovingSpeed;
        rb.linearVelocity = new Vector3(targetVelocity.x, rb.linearVelocity.y, targetVelocity.z);
    }

    void CheckGroundStatus()
    {
        if (groundCheck != null)
            isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundLayer);
    }

    void Jump()
    {
        rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);
        rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
    }
}
```

## FirstPersonLook.cs
```csharp
using UnityEngine;

public class FirstPersonLook : MonoBehaviour
{
    public float sensitivity = 2;
    public float smoothing = 1.5f;
    [Header("Touch Look")]
    public bool useTouchLook = true;
    public float touchSensitivity = 0.08f;
    public bool allowMouseLook = false;
    public bool mouseDragToLook = true;
    [Header("Cursor")]
    public bool lockCursor = false;

    public Vector2 LookDelta { get; private set; }
    Vector2 velocity;
    Vector2 frameVelocity;

    void Start()
    {
        if (lockCursor && !Application.isMobilePlatform)
        { Cursor.lockState = CursorLockMode.Locked; Cursor.visible = false; }
        else { Cursor.lockState = CursorLockMode.None; Cursor.visible = true; }
    }

    void Update()
    {
        Vector2 lookDelta = Vector2.zero;
        if (useTouchLook && Application.isMobilePlatform && Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Moved) lookDelta = touch.deltaPosition * touchSensitivity;
        }
        else if (mouseDragToLook && Input.GetMouseButton(0))
            lookDelta = new Vector2(Input.GetAxisRaw("Mouse X"), Input.GetAxisRaw("Mouse Y")) * sensitivity;
        else if (allowMouseLook)
            lookDelta = new Vector2(Input.GetAxisRaw("Mouse X"), Input.GetAxisRaw("Mouse Y")) * sensitivity;

        Vector2 rawFrameVelocity = lookDelta;
        frameVelocity = Vector2.Lerp(frameVelocity, rawFrameVelocity, 1 / smoothing);
        LookDelta = frameVelocity;
        velocity += frameVelocity;
        velocity.y = Mathf.Clamp(velocity.y, -90, 90);
        transform.localRotation = Quaternion.AngleAxis(-velocity.y, Vector3.right);
    }
}
```

## CameraFollow.cs
```csharp
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Header("Target")]
    public Transform target;
    [Header("Smoothing")]
    public float rotationSmoothing = 3f;
    private float currentYaw;

    void Start() { if (target != null) currentYaw = target.eulerAngles.y; }

    void LateUpdate()
    {
        if (!target) return;
        currentYaw = target.eulerAngles.y;
        transform.rotation = Quaternion.Euler(transform.localEulerAngles.x, currentYaw, 0f);
    }
}
```

## Crouch.cs
```csharp
using UnityEngine;

public class Crouch : MonoBehaviour
{
    public KeyCode key = KeyCode.LeftControl;
    [Header("Slow Movement")]
    public FirstPersonMovement movement;
    public float movementSpeed = 2;
    [Header("Low Head")]
    public Transform headToLower;
    [HideInInspector] public float? defaultHeadYLocalPosition;
    public float crouchYHeadPosition = 1;
    public CapsuleCollider colliderToLower;
    [HideInInspector] public float? defaultColliderHeight;

    public bool IsCrouched { get; private set; }
    public event System.Action CrouchStart, CrouchEnd;

    void LateUpdate()
    {
        if (Input.GetKey(key))
        {
            if (headToLower)
            {
                if (!defaultHeadYLocalPosition.HasValue) defaultHeadYLocalPosition = headToLower.localPosition.y;
                headToLower.localPosition = new Vector3(headToLower.localPosition.x, crouchYHeadPosition, headToLower.localPosition.z);
            }
            if (colliderToLower)
            {
                if (!defaultColliderHeight.HasValue) defaultColliderHeight = colliderToLower.height;
                float loweringAmount = defaultHeadYLocalPosition.HasValue 
                    ? defaultHeadYLocalPosition.Value - crouchYHeadPosition 
                    : defaultColliderHeight.Value * .5f;
                colliderToLower.height = Mathf.Max(defaultColliderHeight.Value - loweringAmount, 0);
                colliderToLower.center = Vector3.up * colliderToLower.height * .5f;
            }
            if (!IsCrouched) { IsCrouched = true; SetSpeedOverrideActive(true); CrouchStart?.Invoke(); }
        }
        else if (IsCrouched)
        {
            if (headToLower)
                headToLower.localPosition = new Vector3(headToLower.localPosition.x, defaultHeadYLocalPosition.Value, headToLower.localPosition.z);
            if (colliderToLower)
            { colliderToLower.height = defaultColliderHeight.Value; colliderToLower.center = Vector3.up * colliderToLower.height * .5f; }
            IsCrouched = false; SetSpeedOverrideActive(false); CrouchEnd?.Invoke();
        }
    }

    void SetSpeedOverrideActive(bool state)
    {
        if (!movement) return;
        if (state) { if (!movement.speedOverrides.Contains(SpeedOverride)) movement.speedOverrides.Add(SpeedOverride); }
        else { if (movement.speedOverrides.Contains(SpeedOverride)) movement.speedOverrides.Remove(SpeedOverride); }
    }

    float SpeedOverride() => movementSpeed;
}
```

## Editor: ApplyMedievalDoorTextures.cs
```csharp
using UnityEngine;
using UnityEditor;

public class ApplyMedievalDoorTextures : EditorWindow
{
    [MenuItem("Tools/Fix Medieval Door Textures")]
    public static void ApplyTextures()
    {
        string texturePath = "Assets/medieval-door-pack/textures";
        Texture2D albedo = AssetDatabase.LoadAssetAtPath<Texture2D>($"{texturePath}/SM_DoorEntranceCastle_Albedo.png");
        Texture2D metallic = AssetDatabase.LoadAssetAtPath<Texture2D>($"{texturePath}/SM_DoorEntranceCastle_Metallic.png");
        Texture2D normal = AssetDatabase.LoadAssetAtPath<Texture2D>($"{texturePath}/SM_DoorEntranceCastle_Normal.png");
        Texture2D ao = AssetDatabase.LoadAssetAtPath<Texture2D>($"{texturePath}/SM_DoorEntranceCastle_AO.png");

        if (albedo == null)
        {
            string[] guids = AssetDatabase.FindAssets("SM_DoorEntranceCastle_Albedo");
            if (guids.Length > 0) albedo = AssetDatabase.LoadAssetAtPath<Texture2D>(AssetDatabase.GUIDToAssetPath(guids[0]));
            guids = AssetDatabase.FindAssets("SM_DoorEntranceCastle_Metallic");
            if (guids.Length > 0) metallic = AssetDatabase.LoadAssetAtPath<Texture2D>(AssetDatabase.GUIDToAssetPath(guids[0]));
            guids = AssetDatabase.FindAssets("SM_DoorEntranceCastle_Normal");
            if (guids.Length > 0) normal = AssetDatabase.LoadAssetAtPath<Texture2D>(AssetDatabase.GUIDToAssetPath(guids[0]));
            guids = AssetDatabase.FindAssets("SM_DoorEntranceCastle_AO");
            if (guids.Length > 0) ao = AssetDatabase.LoadAssetAtPath<Texture2D>(AssetDatabase.GUIDToAssetPath(guids[0]));
        }

        if (albedo == null) { Debug.LogError("Could not find Door Albedo texture!"); return; }

        Material mat = new Material(Shader.Find("Standard"));
        mat.SetTexture("_MainTex", albedo);
        if (metallic != null) { mat.SetTexture("_MetallicGlossMap", metallic); mat.SetFloat("_Metallic", 1f); }
        if (normal != null) { mat.SetTexture("_BumpMap", normal); mat.EnableKeyword("_NORMALMAP"); }
        if (ao != null) { mat.SetTexture("_OcclusionMap", ao); }

        if (!AssetDatabase.IsValidFolder("Assets/medieval-door-pack/Materials"))
            AssetDatabase.CreateFolder("Assets/medieval-door-pack", "Materials");
        AssetDatabase.CreateAsset(mat, "Assets/medieval-door-pack/Materials/M_DoorEntranceCastle.mat");

        GameObject[] objects = GameObject.FindObjectsOfType<GameObject>();
        int appliedCount = 0;
        foreach (GameObject obj in objects)
        {
            if (obj.name.Contains("DoorEntrance"))
            {
                MeshRenderer renderer = obj.GetComponent<MeshRenderer>();
                if (renderer != null) { renderer.material = mat; appliedCount++; EditorUtility.SetDirty(obj); }
            }
        }
        AssetDatabase.SaveAssets();
    }
}
```

## Editor: NavMeshCleaner.cs
```csharp
using UnityEngine;
using UnityEditor;
using Unity.AI.Navigation;

public class NavMeshCleaner : Editor
{
    [MenuItem("Tools/Wipe All NavMesh Components")]
    public static void CleanUpNavMesh()
    {
        int surfaceCount = 0, modifierCount = 0;
        NavMeshSurface[] surfaces = FindObjectsOfType<NavMeshSurface>(true);
        foreach (var surface in surfaces) { Undo.DestroyObjectImmediate(surface); surfaceCount++; }
        NavMeshModifier[] modifiers = FindObjectsOfType<NavMeshModifier>(true);
        foreach (var mod in modifiers) { Undo.DestroyObjectImmediate(mod); modifierCount++; }
        UnityEditor.AI.NavMeshBuilder.ClearAllNavMeshes();
    }
}
```
