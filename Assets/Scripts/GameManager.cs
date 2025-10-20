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
    /// Será definido pelo SlimeSpawner.
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
    /// </summary>
    public AuraManager auraManager;
    
    /// <summary>
    /// Referência ao SlimeSpawner para gerenciar o slime alvo.
    /// </summary>
    public SlimeSpawner slimeSpawner;

    // A lista de 'elementButtons' foi removida, pois os botões agora são 3D e interagem diretamente.

    /// <summary>
    /// Chamado no primeiro frame em que o script está ativo.
    /// Inicializa o objeto alvo sem nenhuma aura elemental.
    /// </summary>
    void Start()
    {
        // Inicializa o slime se o spawner estiver disponível
        if (slimeSpawner == null)
        {
            slimeSpawner = FindObjectOfType<SlimeSpawner>();
            if (slimeSpawner == null)
            {
                Debug.LogError("SlimeSpawner não encontrado na cena!");
            }
        }
        
        // Garante que o AuraManager existe antes de tentar usá-lo.
        if (auraManager != null)
        {
            auraManager.SetAura(ElementType.None); // Remove qualquer aura inicial.
        }
    }

    /// <summary>
    /// Método chamado quando um botão de elemento (agora um botão 3D) é clicado.
    /// Este método é chamado pelo script ElementButton3D.
    /// </summary>
    /// <param name="elementIndex">O índice do elemento selecionado, correspondendo ao valor do enum ElementType.</param>
    public void OnElementButtonClicked(int elementIndex)
    {
        // Converte o índice inteiro para o tipo ElementType correspondente.
        ElementType selectedElement = (ElementType)elementIndex;

        // Verifica se o objeto alvo já possui um elemento aplicado.
        if (currentObjectElement == ElementType.None)
        {
            // Se não houver elemento, esta é a primeira máquina: aplica o elemento ao objeto.
            currentObjectElement = selectedElement;
            Debug.Log($"Objeto imbuído com: {currentObjectElement}");
            
            // Atualiza a aura visual do objeto através do AuraManager.
            if (auraManager != null)
            {
                auraManager.SetAura(currentObjectElement);
            }
        }
        else
        {
            // Se já houver um elemento, esta é a segunda máquina: tenta uma reação elemental.
            // Usa o ReactionFinder para determinar o tipo de reação entre o elemento atual e o selecionado.
            ReactionType reaction = ReactionFinder.FindBestReaction(currentObjectElement, selectedElement);
            Debug.Log($"Reação entre {currentObjectElement} e {selectedElement}: {reaction}");
            
            // Após a reação, o elemento do objeto é resetado para 'None'.
            currentObjectElement = ElementType.None;
            // Remove a aura visual do objeto.
            if (auraManager != null)
            {
                auraManager.SetAura(ElementType.None);
            }

            // TODO: Implementar efeitos visuais e sonoros específicos para cada tipo de reação.
            // Isso pode envolver a instanciação de prefabs de partículas ou a reprodução de áudios.
            // TODO: Exibir o resultado da reação em uma interface de usuário (UI) para o jogador.
        }
    }

    /// <summary>
    /// Reseta o estado elemental do objeto alvo para 'None' e remove qualquer aura visual.
    /// Também respawna o slime se necessário.
    /// </summary>
    public void ResetObject()
    {
        currentObjectElement = ElementType.None;
        // Garante que o AuraManager existe antes de tentar usá-lo.
        if (auraManager != null)
        {
            auraManager.SetAura(ElementType.None);
        }
        
        // Respawna o slime se o spawner estiver disponível
        if (slimeSpawner != null)
        {
            slimeSpawner.RespawnSlime();
        }
        
        Debug.Log("Objeto resetado e slime respawnado.");
    }
    
    /// <summary>
    /// Atualiza o AuraManager quando o targetObject muda.
    /// </summary>
    /// <param name="newTarget">O novo objeto alvo.</param>
    public void UpdateTargetObject(GameObject newTarget)
    {
        targetObject = newTarget;
        
        // Atualiza o AuraManager para o novo objeto
        if (targetObject != null)
        {
            // Verifica se já existe um AuraManager no objeto
            auraManager = targetObject.GetComponent<AuraManager>();
            
            // Se não existir, adiciona um novo
            if (auraManager == null)
            {
                auraManager = targetObject.AddComponent<AuraManager>();
            }
            
            // Aplica o elemento atual, se houver
            auraManager.SetAura(currentObjectElement);
        }
    }
}

