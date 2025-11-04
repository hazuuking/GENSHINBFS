using UnityEngine;

/// <summary>
/// <c>GameManager</c>: O Orquestrador Principal do Sistema de Simulação de Reações Elementais.
/// Este script atua como o controlador de estado (State Machine) do fluxo do jogo,
/// integrando a entrada do usuário (botões), a detecção de máquina (triggers) e a
/// lógica de reação elemental baseada em teoria dos grafos (BFS/DFS simulada).
/// </summary>
public class GameManager : MonoBehaviour
{
    // --- VARIÁVEIS PÚBLICAS (CONFIGURÁVEIS NO INSPECTOR) ---

    [Header("Configuração do Alvo")]
    [Tooltip("O GameObject do slime que será controlado. Arraste o slime da sua cena para este campo.")]
    public GameObject targetSlimeObject;

    // --- REFERÊNCIAS INTERNAS (PRIVADAS) ---
    private ElementalAuraManager targetAuraManager;
    private Rigidbody slimeRigidbody;

    // --- CONTROLE DE ESTADO ---
    public bool canSlimeMove { get; private set; } = true;

    // --- MÉTODOS DO UNITY ---
    void Start()
    {
        if (targetSlimeObject == null)
        {
            Debug.LogError("[GameManager] O 'Target Slime Object' não foi atribuído no Inspector! O sistema não pode funcionar.");
            this.enabled = false;
            return;
        }

        targetAuraManager = targetSlimeObject.GetComponent<ElementalAuraManager>();
        if (targetAuraManager == null)
        {
            Debug.LogError($"[GameManager] O objeto {targetSlimeObject.name} não possui o componente 'ElementalAuraManager'.");
            this.enabled = false;
            return;
        }

        slimeRigidbody = targetSlimeObject.GetComponent<Rigidbody>();
        if (slimeRigidbody == null)
        {
            Debug.LogError($"[GameManager] O objeto {targetSlimeObject.name} não possui um componente 'Rigidbody'.");
            this.enabled = false;
            return;
        }

        targetAuraManager.SetAura(ElementType.None);
        ResumeSlimeMovement();
    }

    // --- EVENTOS DE INTERAÇÃO ---
    public void OnElementButtonClicked(ElementType selectedElement)
    {
        if (targetAuraManager == null) return;

        targetAuraManager.SetAura(selectedElement);
        ResumeSlimeMovement();
        Debug.Log($"[GameManager] Elemento {selectedElement} aplicado. Slime liberado para se mover da Máquina 1.");
    }

    public void OnMachineEnter(int machineID)
    {
        if (machineID == 1)
        {
            Debug.Log("[GameManager] Slime entrou na Máquina 1. Movimento parado, aguardando seleção de elemento.");
            StopSlimeMovement();
        }
        else if (machineID == 2)
        {
            Debug.Log("[GameManager] Slime entrou na Máquina 2. Movimento parado permanentemente. Acionando lógica de reação ótima.");
            StopSlimeMovement();
            TriggerOptimalReaction();
        }
    }

    // --- LÓGICA DE REAÇÃO ÓTIMA ---
    private void TriggerOptimalReaction()
    {
        if (targetAuraManager == null) return;

        ElementType currentElement = targetAuraManager.currentAura;

        if (currentElement != ElementType.None)
        {
            ElementType bestIncomingElement = ElementType.None;
            ReactionType bestReaction = ReactionType.None;
            float maxEvaluation = -1.0f;

            for (int i = 1; i <= (int)ElementType.Geo; i++)
            {
                ElementType potentialIncomingElement = (ElementType)i;

                ReactionType potentialReaction = ElementalReactionLogic.GetReaction(
                    currentElement,
                    potentialIncomingElement,
                    targetAuraManager.currentStatus
                );

                float currentEvaluation = ReactionEvaluator.EvaluateReaction(potentialReaction);

                if (currentEvaluation > maxEvaluation)
                {
                    maxEvaluation = currentEvaluation;
                    bestReaction = potentialReaction;
                    bestIncomingElement = potentialIncomingElement;
                }
            }

            targetAuraManager.SetAura(bestIncomingElement);

            if (ReactionTrigger.Instance != null)
            {
                ReactionTrigger.Instance.TriggerReactionVFX(bestReaction, targetSlimeObject.transform.position, Quaternion.identity);
            }

            Debug.Log($"[GameManager - Lógica Ótima] Slime com {currentElement} + {bestIncomingElement} = {bestReaction} (Score: {maxEvaluation}).");
        }
        else
        {
            Debug.Log("[GameManager] Slime chegou na Máquina 2 sem elemento. Nenhuma reação para acionar.");
        }
    }

    // --- CONTROLE DE MOVIMENTO ---
    private void StopSlimeMovement()
    {
        canSlimeMove = false;
        if (slimeRigidbody != null)
        {
            slimeRigidbody.velocity = Vector3.zero;
            slimeRigidbody.angularVelocity = Vector3.zero;
        }
    }

    private void ResumeSlimeMovement()
    {
        canSlimeMove = true;
        Debug.Log("[GameManager] Movimento do slime retomado (canSlimeMove = true).");
    }
}