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
            else
            {
                // Set reference to this SlimeSpawner in GameManager to prevent double spawning
                gameManager.slimeSpawner = this;
                
                // Let GameManager handle the spawning
                // DO NOT spawn here to avoid duplication
            }
        }
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
    /// <returns>The spawned slime GameObject</returns>
    public GameObject SpawnSlime()
    {
        if (slimePrefab == null || spawnPoint == null)
        {
            Debug.LogError("Slime prefab or spawn point not assigned!");
            return null;
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
        rb.angularDrag = 0.5f; // Changed from 0.05f to 0.5f
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
        
        // Garante que todos os filhos também tenham colisão
        foreach (Transform child in spawnedSlime.transform)
        {
            // Adiciona colliders aos filhos se necessário
            if (child.GetComponent<Collider>() == null)
            {
                child.gameObject.AddComponent<BoxCollider>();
            }
        }
        
        // Verifica se as máquinas têm colliders configurados como triggers
        CheckMachineColliders();
        
        // Update the GameManager's target object
        if (gameManager != null)
        {
            gameManager.targetSlimeObject = spawnedSlime;
            
            // Get or add ElementalAuraManager
            ElementalAuraManager auraManager = spawnedSlime.GetComponent<ElementalAuraManager>();
            if (auraManager == null)
            {
                auraManager = spawnedSlime.AddComponent<ElementalAuraManager>();
            }
            gameManager.targetAuraManager = auraManager;
            
            // Get or add SlimeModelManager
            SlimeModelManager modelManager = spawnedSlime.GetComponent<SlimeModelManager>();
            if (modelManager == null)
            {
                modelManager = spawnedSlime.AddComponent<SlimeModelManager>();
            }
            gameManager.targetModelManager = modelManager;
        }
        
        Debug.Log("Slime spawned successfully with physics and collision detection.");
        return spawnedSlime;
    }
    
    // Método para verificar se as máquinas têm colliders configurados como triggers
    private void CheckMachineColliders()
    {
        MachineTrigger[] machineTriggers = FindObjectsOfType<MachineTrigger>();
        foreach (MachineTrigger trigger in machineTriggers)
        {
            Collider collider = trigger.GetComponent<Collider>();
            if (collider != null && !collider.isTrigger)
            {
                Debug.LogWarning("Machine collider is not set as trigger: " + trigger.gameObject.name);
                collider.isTrigger = true;
            }
            else if (collider == null)
            {
                Debug.LogError("Machine has no collider: " + trigger.gameObject.name);
                BoxCollider newCollider = trigger.gameObject.AddComponent<BoxCollider>();
                newCollider.isTrigger = true;
                newCollider.size = new Vector3(3, 3, 3); // Tamanho padrão
            }
        }
    }
    
    /// <summary>
    /// Respawns the slime at the spawn point
    /// </summary>
    public void RespawnSlime()
    {
        // Garantir que qualquer slime existente seja destruído
        if (spawnedSlime != null)
        {
            Destroy(spawnedSlime);
            spawnedSlime = null;
        }
        
        // Pequeno atraso para garantir que o objeto anterior seja completamente destruído
        Invoke("DelayedSpawn", 0.1f);
    }
    
    private void DelayedSpawn()
    {
        SpawnSlime();
        Debug.Log("Slime respawned with delay to prevent physics issues");
    }
    
    /// <summary>
    /// Returns the currently spawned slime object
    /// </summary>
    /// <returns>The spawned slime GameObject or null if none exists</returns>
    public GameObject GetSpawnedSlime()
    {
        return spawnedSlime;
    }
}