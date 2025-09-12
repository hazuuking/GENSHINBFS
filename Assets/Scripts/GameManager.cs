using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Gerencia a lógica principal do jogo, incluindo a aplicação de elementos ao objeto alvo
/// e o cálculo das reações elementais.
/// </summary>
public class GameManager : MonoBehaviour
{
    /// <summary>
    /// O objeto 3D que receberá os elementos e exibirá as auras.
    /// Deve ser atribuído no Inspector da Unity.
    /// </summary>
    public GameObject targetObject; 

    /// <summary>
    /// O ElementType atualmente aplicado ao objeto alvo.
    /// 'None' indica que nenhum elemento está aplicado.
    /// </summary>
    public ElementType currentObjectElement = ElementType.None; 

    /// <summary>
    /// Referência ao componente AuraManager no objeto alvo.
    /// Usado para controlar os efeitos visuais da aura elemental.
    /// Deve ser atribuído no Inspector da Unity.
    /// </summary>
    public AuraManager auraManager; 

    /// <summary>
    /// Uma lista de configurações de efeito para cada reação.
    /// Configurada no Inspector da Unity.
    /// </summary>
    [System.Serializable]
    public class ReactionEffect
    {
        /// <summary>
        /// O tipo de reação ao qual este efeito visual está associado.
        /// </summary>
        public ReactionType reactionType;
        /// <summary>
        /// O prefab do GameObject que contém o sistema de partículas ou outros componentes visuais
        /// que representam o efeito desta reação.
        /// </summary>
        public GameObject effectPrefab; 
    }
    public List<ReactionEffect> reactionEffects; 

    /// <summary>
    /// Chamado no primeiro frame em que o script está ativo.
    /// Inicializa o objeto alvo sem nenhuma aura elemental.
    /// </summary>
    void Start()
    {
        // Garante que o AuraManager existe antes de tentar usá-lo.
        if (auraManager != null)
        {
            auraManager.SetAura(ElementType.None); // Remove qualquer aura inicial.
        }
    }

    /// <summary>
    /// Método chamado quando um botão de elemento (agora um botão 3D) é clicado na PRIMEIRA MÁQUINA.
    /// Este método é chamado pelo script ElementButton3D.
    /// </summary>
    /// <param name="elementIndex">O índice do elemento selecionado, correspondendo ao valor do enum ElementType.</param>
    public void OnElementButtonClicked(int elementIndex)
    {
        // Converte o índice inteiro para o tipo ElementType correspondente.
        ElementType selectedElement = (ElementType)elementIndex;

        // A lógica foi atualizada: se o objeto já possui um elemento, ele será substituído pelo novo.
        // Isso permite que o usuário troque o elemento na primeira máquina antes de ir para a segunda.
        currentObjectElement = selectedElement;
        Debug.Log($"Objeto agora está imbuído com: {currentObjectElement}");
        
        if (auraManager != null)
        {
            auraManager.SetAura(currentObjectElement);
        }
    }

    /// <summary>
    /// Este método é chamado quando o objeto alvo entra na área da SEGUNDA MÁQUINA.
    /// Ele automaticamente encontra o melhor segundo elemento para reagir com o elemento atual do objeto
    /// e aciona a reação correspondente.
    /// </summary>
    public void TriggerOptimalReaction()
    {
        if (currentObjectElement != ElementType.None)
        {
            ElementType bestIncomingElement = ElementType.None;
            ReactionType bestReaction = ReactionType.None;
            float maxEvaluation = -1.0f;

            // Itera sobre todos os elementos possíveis (exceto None) para encontrar o melhor segundo elemento.
            // Começa de 1 para pular ElementType.None.
            for (int i = 1; i <= (int)ElementType.Geo; i++)
            {
                ElementType potentialIncomingElement = (ElementType)i;
                ReactionType potentialReaction = ElementalReaction.GetReaction(currentObjectElement, potentialIncomingElement);
                float currentEvaluation = ReactionEvaluator.EvaluateReaction(potentialReaction);

                // Se a avaliação da reação potencial for maior que a máxima encontrada até agora,
                // atualiza a melhor reação e o elemento correspondente.
                if (currentEvaluation > maxEvaluation)
                {
                    maxEvaluation = currentEvaluation;
                    bestReaction = potentialReaction;
                    bestIncomingElement = potentialIncomingElement;
                }
            }

            Debug.Log($"Objeto com {currentObjectElement}. Melhor elemento para reagir: {bestIncomingElement}. Reação: {bestReaction} (Avaliação: {maxEvaluation})");
            
            // Instancia o efeito da melhor reação encontrada.
            ReactionEffect effectToPlay = reactionEffects.Find(eff => eff.reactionType == bestReaction);
            if (effectToPlay != null && effectToPlay.effectPrefab != null)
            {
                Instantiate(effectToPlay.effectPrefab, targetObject.transform.position, Quaternion.identity);
                Debug.Log($"Efeito de {bestReaction} instanciado.");
            }
            else if (bestReaction != ReactionType.None)
            {
                Debug.LogWarning($"Prefab de efeito não encontrado para a reação: {bestReaction}");
            }

            // Resetar o elemento do objeto após a reação.
            currentObjectElement = ElementType.None;
            // Remove a aura visual do objeto.
            if (auraManager != null)
            {
                auraManager.SetAura(ElementType.None);
            }
        }
        else
        {
            Debug.Log("Objeto não possui elemento para reagir na segunda máquina. Certifique-se de que um elemento foi aplicado na primeira máquina.");
        }
    }

    /// <summary>
    /// Reseta o estado elemental do objeto alvo para 'None' e remove qualquer aura visual.
    /// Este método é útil para testes ou para reiniciar o ciclo de aplicação de elementos.
    /// </summary>
    public void ResetObject()
    {
        currentObjectElement = ElementType.None;
        // Garante que o AuraManager existe antes de tentar usá-lo.
        if (auraManager != null)
        {
            auraManager.SetAura(ElementType.None);
        }
        Debug.Log("Objeto resetado.");
    }
}

