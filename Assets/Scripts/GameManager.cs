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

        // Libera o movimento IMEDIATAMENTE antes de qualquer outra operação
        ResumeSlimeMovement();
        
        // Depois aplica o elemento/aura
        targetAuraManager.SetAura(selectedElement);
        
        Debug.Log("[GameManager] Slime liberado.");
    }

    public void OnMachineEnter(int machineID)
    {
        if (machineID == 1)
        {
            Debug.Log("[GameManager] Slime entrou na Máquina 1.");
            StopSlimeMovement();
            // Permite mudança de modelo na primeira máquina
            if (targetSlimeObject != null)
            {
                SlimeModelManager modelManager = targetSlimeObject.GetComponent<SlimeModelManager>();
                if (modelManager != null)
                {
                    modelManager.allowModelChange = true;
                }
            }
        }
        else if (machineID == 2)
        {
            Debug.Log("[GameManager] Slime entrou na Máquina 2.");
            StopSlimeMovement();
            // Impede mudança de modelo na segunda máquina
            if (targetSlimeObject != null)
            {
                SlimeModelManager modelManager = targetSlimeObject.GetComponent<SlimeModelManager>();
                if (modelManager != null)
                {
                    modelManager.allowModelChange = false;
                }
            }
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

            // Aplica apenas a aura e os efeitos visuais, mantendo o modelo atual
            targetAuraManager.SetAura(bestIncomingElement);

            if (ReactionTrigger.Instance != null)
            {
                ReactionTrigger.Instance.TriggerReactionVFX(bestReaction, targetSlimeObject.transform.position, Quaternion.identity);
            }

            Debug.Log($"[BFS] {currentElement} + {bestIncomingElement} = {bestReaction} (Score: {maxEvaluation}).");
        }
        else
        {
            Debug.Log("[BFS] Sem elemento para reação.");
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

    public void ResumeSlimeMovement()
    {
        canSlimeMove = true;
        
        // Força atualização física imediata
        if (slimeRigidbody != null)
        {
            Physics.SyncTransforms();
            slimeRigidbody.WakeUp();
        }
    }
}