using UnityEngine;

/// <summary>
/// Controla a esteira transportadora:
/// - Move a textura visualmente (efeito de rolagem)
/// - Aplica força física em objetos com Rigidbody (para movê-los)
/// - Adiciona resistência para manter velocidade estável e evitar deslize infinito
/// </summary>
[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(Renderer))]
public class ConveyorBeltController : MonoBehaviour
{
    [Header("Configurações de Movimento Visual")]
    [Tooltip("Velocidade de rolagem da textura (efeito visual).")]
    public float scrollSpeed = 0.5f;

    [Tooltip("Direção da textura e movimento físico da esteira (X = horizontal, Y = para frente).")]
    public Vector2 scrollDirection = new Vector2(0, 1);

    [Header("Configurações Físicas")]
    [Tooltip("Força aplicada ao objeto para empurrá-lo na direção da esteira.")]
    public float conveyorForce = 8f;

    [Tooltip("Velocidade-alvo que o objeto deve manter sobre a esteira.")]
    public float targetSpeed = 4f;

    [Tooltip("Fator de resistência para desacelerar o objeto quando sai da esteira.")]
    public float damping = 2f;

    private Renderer beltRenderer;
    private Collider beltCollider;

    void Start()
    {
        // Obtém o Renderer (pra animar textura)
        beltRenderer = GetComponent<Renderer>();
        if (beltRenderer == null)
        {
            Debug.LogError("ConveyorBeltController requer um Renderer no mesmo GameObject!");
            enabled = false;
            return;
        }

        // Obtém o Collider e garante que NÃO é um trigger
        beltCollider = GetComponent<Collider>();
        if (beltCollider == null)
        {
            Debug.LogError("ConveyorBeltController requer um Collider no mesmo GameObject!");
            enabled = false;
            return;
        }

        // Importante: NÃO é trigger para permitir colisão física
        beltCollider.isTrigger = false;
        
        // Adiciona um segundo collider como trigger para detectar o slime
        BoxCollider triggerCollider = gameObject.AddComponent<BoxCollider>();
        triggerCollider.isTrigger = true;
        triggerCollider.size = beltCollider.bounds.size * 1.1f; // Ligeiramente maior
    }

    void Update()
    {
        // Move a textura para dar a sensação de movimento
        float offsetU = Time.time * scrollSpeed * scrollDirection.x;
        float offsetV = Time.time * scrollSpeed * scrollDirection.y;
        beltRenderer.material.SetTextureOffset("_MainTex", new Vector2(offsetU, offsetV));
    }

    // Detecta colisão física com o collider não-trigger
    void OnCollisionStay(Collision collision)
    {
        Rigidbody rb = collision.rigidbody;

        if (rb != null && !rb.isKinematic)
        {
            // Direção local da esteira (usando orientação do objeto)
            Vector3 moveDir = (transform.right * scrollDirection.x + transform.forward * scrollDirection.y).normalized;

            // Log de diagnóstico para confirmar contato
            Debug.Log($"Esteira em contato com: {rb.gameObject.name}. v\u0307={Vector3.Dot(rb.velocity, moveDir):F2}");

            // Aceleração robusta na direção da esteira baseada na diferença de velocidade
            float currentAlong = Vector3.Dot(rb.velocity, moveDir);
            float delta = targetSpeed - currentAlong;
            // Muda a velocidade instantaneamente na componente da direção
            rb.AddForce(moveDir * delta, ForceMode.VelocityChange);

            // Leve sustentação para evitar afundar na esteira
            rb.AddForce(Vector3.up * 0.5f, ForceMode.Acceleration);
        }
    }
    
    // Ainda mantém o trigger para detecção adicional
    void OnTriggerStay(Collider other)
    {
        Rigidbody rb = other.attachedRigidbody;

        if (rb != null && !rb.isKinematic)
        {
            // Direção local da esteira (usando orientação do objeto)
            Vector3 moveDir = (transform.right * scrollDirection.x + transform.forward * scrollDirection.y).normalized;
            
            // Apoio mínimo: só um leve empurrão caso a colisão esteja instável
            rb.AddForce(moveDir * (conveyorForce * 0.25f), ForceMode.Acceleration);
        }
    }

    void OnTriggerExit(Collider other)
    {
        // Quando o slime sai da esteira, ele perde o empurrão
        Rigidbody rb = other.attachedRigidbody;
        if (rb != null && !rb.isKinematic)
        {
            // Suaviza pra ele parar aos poucos
            rb.velocity *= 0.5f;
        }
    }
}
