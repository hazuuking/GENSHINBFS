using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Componente responsável pela simulação da esteira transportadora.
/// Controla o movimento dos objetos que entram em seu <c>Trigger Collider</c>,
/// aplicando uma força constante. O controle de movimento do Slime é centralizado
/// no <c>GameManager</c> através da variável <c>canSlimeMove</c>, implementando
/// um sistema de estado de máquina simples (parado/movendo).
/// </summary>
public class ConveyorBeltController : MonoBehaviour
{
    // --- VARIÁVEIS PÚBLICAS (CONFIGURÁVEIS NO INSPECTOR) ---

    /// <summary>
    /// <c>[Tooltip]</c> A taxa de velocidade constante aplicada aos objetos na esteira.
    /// </summary>
    [Tooltip("A velocidade com que a esteira move os objetos.")]
    public float moveSpeed = 1.0f;

    /// <summary>
    /// <c>[Tooltip]</c> A direção do movimento no espaço local da esteira.
    /// <c>transform.TransformDirection</c> será usado para converter isso para o espaço global.
    /// </summary>
    [Tooltip("A direção do movimento no eixo local da esteira. (0,0,1) significa para a frente.")]
    public Vector3 moveDirection = new Vector3(0, 0, 1);

    // --- REFERÊNCIAS INTERNAS (PRIVADAS) ---

    /// <summary>
    /// Lista de <c>Rigidbody</c>s de todos os objetos atualmente dentro da área de influência da esteira.
    /// O uso de uma lista permite rastrear múltiplos objetos, embora apenas o Slime seja movido.
    /// </summary>
    private List<Rigidbody> objectsOnBelt = new List<Rigidbody>();

    /// <summary>
    /// Referência ao <c>GameManager</c> para consultar a permissão de movimento do Slime.
    /// </summary>
    private GameManager gameManager;

    // --- MÉTODOS DO UNITY ---

    /// <summary>
    /// Chamado no início da cena.
    /// </summary>
    void Start()
    {
        // Busca a referência do GameManager para sincronizar o movimento do Slime.
        gameManager = FindObjectOfType<GameManager>();
        if (gameManager == null)
        {
            Debug.LogError("[ConveyorBeltController] GameManager não encontrado na cena! O controle de movimento do Slime será ignorado.");
        }
    }

    /// <summary>
    /// Chamado quando um objeto entra na área do trigger da esteira.
    /// </summary>
    /// <param name="other">O <c>Collider</c> do objeto que entrou.</param>
    void OnTriggerEnter(Collider other)
    {
        // Tenta obter o componente Rigidbody. O movimento da esteira deve ser aplicado
        // via manipulação de Rigidbody para interagir corretamente com o sistema de física.
        Rigidbody rb = other.GetComponent<Rigidbody>();
        if (rb != null && !objectsOnBelt.Contains(rb))
        {
            objectsOnBelt.Add(rb);
        }
    }

    /// <summary>
    /// Chamado quando um objeto sai da área do trigger da esteira.
    /// </summary>
    /// <param name="other">O <c>Collider</c> do objeto que saiu.</param>
    void OnTriggerExit(Collider other)
    {
        Rigidbody rb = other.GetComponent<Rigidbody>();
        if (rb != null)
        {
            objectsOnBelt.Remove(rb);
        }
    }

    /// <summary>
    /// Chamado a cada passo de física (taxa fixa). É o local ideal para aplicar forças ou
    /// manipular a posição de <c>Rigidbody</c>s, garantindo estabilidade física.
    /// </summary>
    void FixedUpdate()
    {
        // 1. Verificação de Estado: O movimento só é aplicado se o GameManager permitir (canSlimeMove = true).
        if (gameManager != null && gameManager.canSlimeMove)
        {
            // 2. Itera sobre todos os objetos na esteira.
            foreach (Rigidbody rb in objectsOnBelt)
            {
                // 3. Filtragem: Aplica o movimento *apenas* ao Slime alvo, ignorando outros objetos.
                // Considera colliders/rigidbodies em filhos do slime (modelos instanciados).
                GameObject rbRoot = rb.transform.root.gameObject;
                if (rbRoot == gameManager.targetSlimeObject)
                {
                    // Cálculo do Movimento: Aplicar movimento como velocidade para evitar "teleporte" de posição,
                    // reduzindo explosões de física e ejeções inesperadas.
                    Vector3 worldMoveDirection = transform.TransformDirection(moveDirection).normalized;
                    Vector3 targetHorizontalVelocity = worldMoveDirection * moveSpeed;

                    // Suaviza a transição da velocidade atual para a velocidade da esteira
                    Vector3 currentVel = rb.velocity;
                    Vector3 currentHorizontal = new Vector3(currentVel.x, 0f, currentVel.z);
                    Vector3 newHorizontal = Vector3.Lerp(currentHorizontal, targetHorizontalVelocity, 0.25f);

                    // Mantém componente vertical estável, evitando empurrões para cima.
                    float newY = Mathf.Max(currentVel.y, 0f);
                    rb.velocity = new Vector3(newHorizontal.x, newY, newHorizontal.z);
                }
            }
        }
    }

    /// <summary>
    /// Movimento aplicado durante contato sólido com a esteira.
    /// Usa colisão direta para cenários onde não há Trigger configurado.
    /// </summary>
    void OnCollisionStay(Collision collision)
    {
        if (gameManager == null || !gameManager.canSlimeMove)
            return;

        // Aceita colisões vindas de filhos do slime (ex.: prefab visual com collider).
        GameObject colRoot = collision.transform.root.gameObject;
        if (colRoot != gameManager.targetSlimeObject)
            return;

        Rigidbody rb = collision.rigidbody != null ? collision.rigidbody : collision.gameObject.GetComponent<Rigidbody>();
        if (rb == null) return;

        // Direção e velocidade alvo da esteira (aplicação direta e estável)
        Vector3 worldMoveDirection = transform.TransformDirection(moveDirection).normalized;
        Vector3 targetHorizontalVelocity = worldMoveDirection * moveSpeed;

        // Aplica velocidade horizontal exata da esteira; componente vertical mantida em 0
        rb.velocity = new Vector3(targetHorizontalVelocity.x, 0f, targetHorizontalVelocity.z);
    }
}
