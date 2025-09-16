using UnityEngine;

/// <summary>
/// Script para detectar a entrada e saída do objeto alvo em uma área de máquina.
/// </summary>
public class MachineTrigger : MonoBehaviour
{
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
        // Verifica se o objeto que entrou no trigger é o slime alvo do GameManager
        if (GameManager.Instance != null && other.gameObject == GameManager.Instance.targetSlimeObject)
        {
            Debug.Log($"TargetObject entrou na {(isFirstMachine ? "Primeira" : "Segunda")} Máquina.");
            
            if (!isFirstMachine)
            {
                // Se for a segunda máquina, aciona a lógica de reação
                // O GameManager agora determinará o melhor elemento para reagir
                GameManager.Instance.TriggerReactionMachine();
            }
            // Lógica para a primeira máquina (seleção de elemento) é tratada pelos ElementButton3D
        }
    }

    /// <summary>
    /// Chamado quando outro Collider sai do trigger.
    /// </summary>
    /// <param name="other">O Collider que saiu do trigger.</param>
    void OnTriggerExit(Collider other)
    {
        // Verifica se o objeto que saiu do trigger é o slime alvo do GameManager
        if (GameManager.Instance != null && other.gameObject == GameManager.Instance.targetSlimeObject)
        {
            Debug.Log($"TargetObject saiu da {(isFirstMachine ? "Primeira" : "Segunda")} Máquina.");
            // Opcional: Lógica para resetar o slime ou prepará-lo para a próxima fase
            if (!isFirstMachine)
            {
                GameManager.Instance.ResetTargetSlime();
            }
        }
    }
}


