using UnityEngine;

/// <summary>
/// Handles spawning and managing the target slime object with proper physics and collision detection.
/// </summary>
public class SlimeSpawner : MonoBehaviour
{
    [Header("Slime Configuration")]
    [Tooltip("The slime prefab to spawn")]
    public GameObject slimePrefab;
    
    [Tooltip("The point where the slime will spawn")]
    public Transform spawnPoint;
    
    [Tooltip("Reference to the GameManager")]
    public GameManager gameManager;
    
    [Header("Physics Settings")]
    [Tooltip("Mass of the slime")]
    public float slimeMass = 1.0f;
    
    [Tooltip("Radius of the slime collider")]
    public float colliderRadius = 0.5f;
    
    [Tooltip("Bounciness of the slime (0-1)")]
    [Range(0, 1)]
    public float bounciness = 0.5f;
    
    [Tooltip("Friction of the slime (0-1)")]
    [Range(0, 1)]
    public float friction = 0.3f;
    
    private GameObject spawnedSlime;
    private PhysicMaterial slimePhysicMaterial;
    
    void Start()
    {
        // Create physics material for the slime
        CreateSlimePhysicMaterial();
        
        // Find GameManager if not assigned
        if (gameManager == null)
        {
            gameManager = FindObjectOfType<GameManager>();
            if (gameManager == null)
            {
                Debug.LogError("GameManager not found! Please assign it in the inspector.");
            }
        }
        
        // Spawn the slime
        SpawnSlime();
    }
    
    /// <summary>
    /// Creates a physics material for the slime with the specified properties
    /// </summary>
    private void CreateSlimePhysicMaterial()
    {
        slimePhysicMaterial = new PhysicMaterial("SlimePhysicMaterial");
        slimePhysicMaterial.bounciness = bounciness;
        slimePhysicMaterial.dynamicFriction = friction;
        slimePhysicMaterial.staticFriction = friction;
        slimePhysicMaterial.frictionCombine = PhysicMaterialCombine.Average;
        slimePhysicMaterial.bounceCombine = PhysicMaterialCombine.Average;
    }
    
    /// <summary>
    /// Spawns a new slime at the spawn point with proper physics components
    /// </summary>
    public void SpawnSlime()
    {
        if (slimePrefab == null || spawnPoint == null)
        {
            Debug.LogError("Slime prefab or spawn point not assigned!");
            return;
        }
        
        // Destroy previous slime if it exists
        if (spawnedSlime != null)
        {
            Destroy(spawnedSlime);
        }
        
        // Instantiate the new slime
        spawnedSlime = Instantiate(slimePrefab, spawnPoint.position, spawnPoint.rotation);
        
        // Add or get Rigidbody
        Rigidbody rb = spawnedSlime.GetComponent<Rigidbody>();
        if (rb == null)
        {
            rb = spawnedSlime.AddComponent<Rigidbody>();
        }
        
        // Configure Rigidbody
        rb.mass = slimeMass;
        rb.drag = 0.5f;
        rb.angularDrag = 0.05f;
        rb.useGravity = true;
        rb.isKinematic = false;
        rb.interpolation = RigidbodyInterpolation.Interpolate;
        rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
        
        // Add or get SphereCollider
        SphereCollider sphereCollider = spawnedSlime.GetComponent<SphereCollider>();
        if (sphereCollider == null)
        {
            sphereCollider = spawnedSlime.AddComponent<SphereCollider>();
        }
        
        // Configure SphereCollider
        sphereCollider.radius = colliderRadius;
        sphereCollider.material = slimePhysicMaterial;
        sphereCollider.isTrigger = false;
        
        // Ensure the slime is on the default layer for proper collision detection
        spawnedSlime.layer = LayerMask.NameToLayer("Default");
        
        // Update the GameManager's target object
        if (gameManager != null)
        {
            gameManager.targetSlimeObject = spawnedSlime;
            
            // Get or add required components that GameManager expects
            if (spawnedSlime.GetComponent<ElementalAuraManager>() == null)
            {
                spawnedSlime.AddComponent<ElementalAuraManager>();
            }
            
            if (spawnedSlime.GetComponent<SlimeModelManager>() == null)
            {
                spawnedSlime.AddComponent<SlimeModelManager>();
            }
        }
        
        Debug.Log("Slime spawned successfully with physics and collision detection.");
    }
    
    /// <summary>
    /// Respawns the slime at the spawn point
    /// </summary>
    public void RespawnSlime()
    {
        SpawnSlime();
    }
}