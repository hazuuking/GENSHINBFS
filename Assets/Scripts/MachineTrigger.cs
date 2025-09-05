using UnityEngine;

/// <summary>
/// Script para detectar a entrada e saída do objeto alvo em uma área de máquina.
/// </summary>
public class MachineTrigger : MonoBehaviour
{
    /// <summary>
    /// Referência ao GameManager para interagir com a lógica do jogo.
    /// Deve ser atribuído no Inspector da Unity.
    /// </summary>
    public GameManager gameManager; 
    /// <summary>
    /// Indica se esta é a primeira máquina (true) ou a segunda (false).
    /// </summary>
    public bool isFirstMachine = true; 

    /// <summary>
    /// Chamado quando outro Collider entra no trigger.
    /// </summary>
    /// <param name="other">O Collider que entrou no trigger.</param>
    void OnTriggerEnter(Collider other)
    {
        // Verifica se o objeto que entrou no trigger é o objeto alvo.
        if (other.gameObject == gameManager.targetObject)
        {
            Debug.Log($"TargetObject entrou na {(isFirstMachine ? "Primeira" : "Segunda")} Máquina.");
            // Aqui você pode adicionar lógica para habilitar/desabilitar botões, etc.,
            // dependendo de qual máquina o objeto alvo entrou.
        }
    }

    /// <summary>
    /// Chamado quando outro Collider sai do trigger.
    /// </summary>
    /// <param name="other">O Collider que saiu do trigger.</param>
    void OnTriggerExit(Collider other)
    {
        // Verifica se o objeto que saiu do trigger é o objeto alvo.
        if (other.gameObject == gameManager.targetObject)
        {
            Debug.Log($"TargetObject saiu da {(isFirstMachine ? "Primeira" : "Segunda")} Máquina.");
        }
    }
}