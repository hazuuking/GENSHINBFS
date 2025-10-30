using UnityEngine;

/// <summary>
/// Componente de controle físico e de estado do Slime.
/// Este script garante que o Slime tenha as propriedades físicas corretas (Rigidbody e Collider)
/// e gerencia o estado de contato com a esteira, essencial para o controle de movimento
/// e para a estabilidade física durante o transporte.
/// </summary>
public class SlimeController : MonoBehaviour
{
    // --- VARIÁVEIS INTERNAS ---

    /// <summary>
    /// Referência ao componente de física (<c>Rigidbody</c>) do Slime.
    /// </summary>
    private Rigidbody rb;

    /// <summary>
    /// Flag booleana que indica se o Slime está atualmente em contato com a esteira transportadora.
    /// </summary>
    private bool onEsteira;

    // --- MÉTODOS DO UNITY ---

    /// <summary>
    /// <c>Start()</c>: Chamado na inicialização.
    /// Configura as propriedades de física do Slime para garantir um comportamento realista.
    /// </summary>
    void Start()
    {
        // Obtém a referência do Rigidbody.
        rb = GetComponent<Rigidbody>();

        // 1. Configurações de Física:
        rb.useGravity = true;           // O Slime deve ser afetado pela gravidade.
        rb.isKinematic = false;         // O Slime é um objeto dinâmico, movido por forças ou manipulação de posição.
        // Interpolação para suavizar o movimento, importante quando a posição é manipulada no FixedUpdate (pelo ConveyorBeltController).
        rb.interpolation = RigidbodyInterpolation.Interpolate; 
        // Detecção de colisão contínua para evitar que o Slime "atravesse" objetos em alta velocidade.
        rb.collisionDetectionMode = CollisionDetectionMode.Continuous; 

        // Estabilidade: aumentar amortecimento rotacional e congelar rotações X/Z para evitar tombos
        rb.angularDrag = 2.0f;
        rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
        // Amortecimento linear moderado para reduzir oscilações
        rb.drag = 1.5f;

        // 2. Configuração do Collider:
        // Garante que o Slime tenha um Collider para interagir fisicamente.
        Collider col = GetComponent<Collider>();
        if (col == null)
        {
            col = gameObject.AddComponent<SphereCollider>();
        }
        col.isTrigger = false; // Deve ser um Collider sólido para detectar colisão.

        // 3. Força a atualização do sistema de física.
        Physics.SyncTransforms();
        rb.WakeUp();
    }

    /// <summary>
    /// <c>Update()</c>: Chamado a cada frame.
    /// Usado para aplicar correções de estabilidade vertical.
    /// </summary>
    void Update()
    {
        // Correção de Estabilidade: Quando o Slime está na esteira, sua velocidade vertical (eixo Y)
        // deve ser zerada se for negativa (caindo), para evitar que ele afunde na esteira
        // devido a imprecisões do motor de física.
        if (onEsteira && rb.velocity.y <= 0)
        {
            // Mantém as velocidades horizontais (X e Z) e zera a vertical (Y).
            rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);
        }
    }

    /// <summary>
    /// <c>OnCollisionEnter()</c>: Chamado quando o Slime entra em contato com outro Collider sólido.
    /// </summary>
    /// <param name="collision">Informações sobre a colisão.</param>
    void OnCollisionEnter(Collision collision)
    {
        // Verifica se o objeto colidido é a esteira (identificada pela Tag).
        if (collision.gameObject.CompareTag("Esteira"))
        {
            onEsteira = true;
            Debug.Log("[SlimeController] Slime pousou na esteira!");
        }
    }

    /// <summary>
    /// <c>OnCollisionExit()</c>: Chamado quando o Slime perde contato com outro Collider sólido.
    /// </summary>
    /// <param name="collision">Informações sobre a colisão.</param>
    void OnCollisionExit(Collision collision)
    {
        // Verifica se o Slime saiu da esteira.
        if (collision.gameObject.CompareTag("Esteira"))
        {
            onEsteira = false;
            Debug.Log("[SlimeController] Slime saiu da esteira!");
        }
    }
}