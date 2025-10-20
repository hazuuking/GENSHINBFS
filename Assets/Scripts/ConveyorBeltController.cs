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

        // Obtém o Collider e garante que é um trigger
        beltCollider = GetComponent<Collider>();
        if (beltCollider == null)
        {
            Debug.LogError("ConveyorBeltController requer um Collider no mesmo GameObject!");
            enabled = false;
            return;
        }

        beltCollider.isTrigger = true;
    }

    void Update()
    {
        // Move a textura para dar a sensação de movimento
        float offsetU = Time.time * scrollSpeed * scrollDirection.x;
        float offsetV = Time.time * scrollSpeed * scrollDirection.y;
        beltRenderer.material.SetTextureOffset("_MainTex", new Vector2(offsetU, offsetV));
    }

    void OnTriggerStay(Collider other)
    {
        Rigidbody rb = other.attachedRigidbody;

        if (rb != null && !rb.isKinematic)
        {
            // Direção local da esteira (usando orientação do objeto)
            Vector3 moveDir = (transform.right * scrollDirection.x + transform.forward * scrollDirection.y).normalized;

            // --- Força principal (empurra o slime pra frente) ---
            rb.AddForce(moveDir * conveyorForce, ForceMode.Acceleration);

            // --- Resistência / controle de velocidade ---
            // Mantém o slime numa velocidade-alvo estável, sem atravessar
            Vector3 desiredVelocity = moveDir * targetSpeed;
            rb.velocity = Vector3.Lerp(rb.velocity, desiredVelocity, Time.deltaTime * damping);
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
