using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Gerencia a lógica principal do jogo, orquestrando a aplicação de elementos e as reações.
/// Esta versão é simplificada e funciona em conjunto com o ElementalAuraManager.
/// </summary>
public class GameManager : MonoBehaviour
{
    [Header("Configuração do Alvo")]
    [Tooltip("O objeto 3D (slime) que receberá os elementos. Deve ser atribuído manualmente no Inspector.")]
    public GameObject targetSlimeObject;

    // Referência interna ao ElementalAuraManager do slime, obtida no Start.
    private ElementalAuraManager targetAuraManager;

    /// <summary>
    /// Chamado no primeiro frame em que o script está ativo.
    /// </summary>
    void Start()
    {
        // Valida e configura o objeto alvo inicial.
        if (targetSlimeObject == null)
        {
            Debug.LogError("[GameManager] O 'Target Slime Object' não foi atribuído no Inspector! O sistema não funcionará.");
            this.enabled = false; // Desativa o GameManager se o slime não for configurado.
            return;
        }

        // Obtém o componente ElementalAuraManager do slime alvo.
        targetAuraManager = targetSlimeObject.GetComponent<ElementalAuraManager>();
        if (targetAuraManager == null)
        {
            Debug.LogError($"[GameManager] O objeto {targetSlimeObject.name} não possui o componente 'ElementalAuraManager'.");
            this.enabled = false;
            return;
        }

        // Garante que o slime comece em um estado limpo.
        targetAuraManager.ClearAuras();
    }

    /// <summary>
    /// Método chamado quando um botão de elemento (3D) é clicado na PRIMEIRA MÁQUINA.
    /// </summary>
    /// <param name="elementIndex">O índice do elemento selecionado.</param>
    public void OnElementButtonClicked(int elementIndex)
    {
        if (targetAuraManager == null) return;

        ElementType selectedElement = (ElementType)elementIndex;
        Debug.Log($"[GameManager] Botão pressionado: {selectedElement}. Aplicando ao slime...");

        // Delega a aplicação do elemento e a lógica de reação para o ElementalAuraManager.
        targetAuraManager.ApplyElement(selectedElement);
    }

    /// <summary>
    /// Chamado quando o objeto alvo entra na SEGUNDA MÁQUINA para acionar a reação ótima.
    /// </summary>
    public void TriggerOptimalReaction()
    {
        if (targetAuraManager == null) return;

        ElementType currentElement = targetAuraManager.currentAura; // Pega o elemento atual do AuraManager

        if (currentElement != ElementType.None)
        {
            ElementType bestIncomingElement = ElementType.None;
            ReactionType bestReaction = ReactionType.None;
            float maxEvaluation = -1.0f;

            // Itera sobre todos os elementos para encontrar a melhor reação.
            for (int i = 1; i <= (int)ElementType.Geo; i++)
            {
                ElementType potentialIncomingElement = (ElementType)i;
                
                // Simula a reação para avaliação
                ReactionType potentialReaction = ElementalReactionLogic.GetReaction(currentElement, potentialIncomingElement, targetAuraManager.currentStatus);
                float currentEvaluation = ReactionEvaluator.EvaluateReaction(potentialReaction);

                if (currentEvaluation > maxEvaluation)
                {
                    maxEvaluation = currentEvaluation;
                    bestReaction = potentialReaction;
                    bestIncomingElement = potentialIncomingElement;
                }
            }

            Debug.Log($"[GameManager] Slime com {currentElement}. Melhor elemento para reagir: {bestIncomingElement}. Reação: {bestReaction} (Avaliação: {maxEvaluation})");

            // Aplica o melhor elemento encontrado para causar a reação ótima.
            targetAuraManager.ApplyElement(bestIncomingElement);
        }
        else
        {
            Debug.Log("[GameManager] Slime não possui elemento para reagir na segunda máquina.");
        }
    }
}