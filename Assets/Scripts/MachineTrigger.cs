using UnityEngine;

/// <summary>
/// Componente de detecção de zona de máquina.
/// Este script é anexado aos <c>Colliders</c> configurados como <c>Is Trigger</c> que definem as áreas de atuação das máquinas.
/// Sua função é atuar como um sensor que notifica o <c>GameManager</c> quando o objeto alvo (o Slime)
/// entra na área de processamento, sinalizando uma mudança de estado no fluxo do jogo.
/// </summary>
public class MachineTrigger : MonoBehaviour
{
    // --- VARIÁVEIS PÚBLICAS (CONFIGURÁVEIS NO INSPECTOR) ---

    /// <summary>
    /// <c>[Tooltip]</c> Identificador único para distinguir as máquinas.
    /// <c>machineID = 1</c> corresponde à Máquina de Aplicação Elemental (entrada manual).
    /// <c>machineID = 2</c> corresponde à Máquina de Reação Ótima (processamento automático via BFS/DFS).
    /// </summary>
    [Tooltip("ID desta máquina (1 para a primeira, 2 para a segunda). Usado para que o GameManager saiba qual máquina foi ativada.")]
    public int machineID;

    // --- REFERÊNCIAS INTERNAS (PRIVADAS) ---

    /// <summary>
    /// Referência ao <c>GameManager</c> principal da cena, o orquestrador do fluxo do jogo.
    /// </summary>
    private GameManager gameManager;

    // --- MÉTODOS DO UNITY ---

    /// <summary>
    /// Chamado no início, antes do primeiro frame.
    /// </summary>
    void Start()
    {
        // Procura e armazena a referência do GameManager. Esta é uma forma comum de
        // estabelecer comunicação entre scripts em Unity, garantindo que o trigger possa
        // enviar eventos ao controlador central.
        gameManager = FindObjectOfType<GameManager>();
        if (gameManager == null)
        {
            Debug.LogError("[MachineTrigger] GameManager não foi encontrado na cena! A notificação de entrada de máquina falhará.");
        }
    }

    /// <summary>
    /// Método do Unity chamado automaticamente quando um objeto com <c>Collider</c> entra neste <c>Trigger</c>.
    /// Este evento é crucial para o controle de estado do Slime (parar/iniciar movimento).
    /// </summary>
    /// <param name="other">O <c>Collider</c> do objeto que entrou na área.</param>
    void OnTriggerEnter(Collider other)
    {
        // 1. Verificação de Pré-condições: Garante que o GameManager existe e que o objeto
        // que entrou no trigger é o Slime alvo (identificado pelo GameManager).
        if (gameManager != null)
        {
            // Usa root para aceitar colliders de filhos do slime.
            GameObject otherRoot = other.transform.root.gameObject;
            if (otherRoot == gameManager.targetSlimeObject)
            {
            // 2. Notificação do Evento: Chama o método de manipulação de evento no GameManager,
            // passando o ID desta máquina. O GameManager usará este ID para determinar
            // a ação apropriada (ex: parar o Slime na Máquina 1 e esperar input, ou
            // executar a lógica de reação na Máquina 2).
            gameManager.OnMachineEnter(machineID);
            }
        }
    }
}


