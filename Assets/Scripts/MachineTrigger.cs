using UnityEngine;

/// <summary>
/// Controla a detecção de entrada e saída de objetos nas áreas das máquinas.
/// </summary>
public class MachineTrigger : MonoBehaviour
{
    public int machineID;
    private GameManager gameManager;

    void Start()
    {
        // Encontra a instância do GameManager na cena.
        gameManager = FindObjectOfType<GameManager>();
        if (gameManager == null)
        {
            Debug.LogError("[MachineTrigger] GameManager não encontrado na cena!");
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (gameManager != null && other.gameObject == gameManager.targetSlimeObject)
        {
            Debug.Log($"Objeto alvo entrou na Máquina {machineID}.");

            if (machineID == 2)
            {
                // Na segunda máquina, aciona a lógica de reação ótima.
                gameManager.TriggerOptimalReaction();
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (gameManager != null && other.gameObject == gameManager.targetSlimeObject)
        {
            Debug.Log($"Objeto alvo saiu da Máquina {machineID}.");
        }
    }
}


