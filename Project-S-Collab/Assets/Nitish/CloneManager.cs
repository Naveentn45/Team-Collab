using UnityEngine;
using UnityEngine.InputSystem;

public class CloneManager : MonoBehaviour
{
    [Header("References")]
    public GameObject clonePrefab;
    public CameraFollow cameraFollow;

    [Header("Settings")]
    public string trapTag = "Trap"; // Tag your traps/enemies with this

    // State
    private GameObject currentClone;
    private PlayerMovement mainMovement;
    private PlayerMovement cloneMovement;
    private bool isControllingClone = false;

    void Start()
    {
        mainMovement = GetComponent<PlayerMovement>();

        // Ensure main character starts with control
        mainMovement.canControl = true;
    }

    void Update()
    {
        HandleInput();
    }

    void HandleInput()
    {
        // --- R: Create Clone ---
        if (Keyboard.current.rKey.wasPressedThisFrame)
        {
            if (currentClone == null)
            {
                SpawnClone();
            }
        }

        // --- Tab: Switch Control ---
        if (Keyboard.current.tabKey.wasPressedThisFrame)
        {
            if (currentClone != null)
            {
                SwitchControl();
            }
        }

        // --- T: Kill Clone Instantly ---
        if (Keyboard.current.tKey.wasPressedThisFrame)
        {
            if (currentClone != null)
            {
                KillClone(false); // false = disappear instantly
            }
        }
    }

    void SpawnClone()
    {
        // Spawn at main character's position
        currentClone = Instantiate(clonePrefab, transform.position, transform.rotation);

        // Setup collision ignore
        Collider mainCol = GetComponent<Collider>();
        Collider cloneCol = currentClone.GetComponent<Collider>();
        if (mainCol && cloneCol) Physics.IgnoreCollision(mainCol, cloneCol, true);

        // Get movement reference
        cloneMovement = currentClone.GetComponent<PlayerMovement>();

        // Automatically switch control to the new clone
        SwitchControl();
    }

    void SwitchControl()
    {
        isControllingClone = !isControllingClone;

        if (isControllingClone)
        {
            // Control Clone
            mainMovement.canControl = false;
            cloneMovement.canControl = true;

            // Camera follows clone
            cameraFollow.playerTarget = currentClone.transform;
        }
        else
        {
            // Control Main Character
            mainMovement.canControl = true;
            cloneMovement.canControl = false;

            // Camera follows main
            cameraFollow.playerTarget = this.transform;
        }
    }

    // Public method so the Clone can call it when it dies
    public void KillClone(bool leaveBody)
    {
        if (currentClone == null) return;

        // If we were controlling the clone, switch back to main
        if (isControllingClone)
        {
            SwitchControl(); // Switches back to main
        }
        else
        {
            // Just ensure main has control (safety)
            mainMovement.canControl = true;
        }

        // Handle the body
        if (leaveBody)
        {
            // 1. Disable scripts so they don't interfere with physics
            if (cloneMovement != null) cloneMovement.canControl = false;

            CloneEntity entity = currentClone.GetComponent<CloneEntity>();
            if (entity != null) entity.enabled = false;

            // 2. Handle Rigidbody Physics (Fall Over)
            Rigidbody rb = currentClone.GetComponent<Rigidbody>();
            if (rb != null)
            {
                // Unfreeze Rotation (X, Y, Z)
                // This unchecks the boxes in the Inspector
                rb.constraints = RigidbodyConstraints.None;

                // Optional: Add a small random spin so it doesn't just fall like a plank
                // This makes the death look more dynamic
                rb.AddTorque(Random.onUnitSphere * 2f, ForceMode.Impulse);
            }

            // Clear reference so we can spawn a new one
            currentClone = null;
            cloneMovement = null;
        }
        else
        {
            // Disappear instantly
            Destroy(currentClone);
            currentClone = null;
            cloneMovement = null;
        }
    }
}