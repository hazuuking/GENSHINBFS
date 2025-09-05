using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Gerencia os efeitos visuais de aura elemental em um GameObject.
/// Permite associar prefabs de efeitos de partículas a diferentes tipos de elementos
/// e ativar/desativar a aura correspondente.
/// </summary>
public class AuraManager : MonoBehaviour
{
    /// <summary>
    /// Classe interna serializável para associar um ElementType a um prefab de efeito de aura.
    /// Isso permite configurar no Inspector da Unity qual prefab visual corresponde a cada elemento.
    /// </summary>
    [System.Serializable]
    public class ElementAura
    {
        /// <summary>
        /// O tipo de elemento ao qual este efeito de aura está associado.
        /// </summary>
        public ElementType elementType;
        /// <summary>
        /// O prefab do GameObject que contém o sistema de partículas ou outros componentes visuais
        /// que representam a aura para este elemento.
        /// </summary>
        public GameObject auraEffectPrefab; 
    }

    /// <summary>
    /// Uma lista de configurações de aura para cada elemento.
    /// Configurada no Inspector da Unity.
    /// </summary>
    public List<ElementAura> elementAuras; 

    /// <summary>
    /// Referência à instância atual do prefab de aura que está ativa no GameObject.
    /// Usado para destruir a aura anterior antes de aplicar uma nova.
    /// </summary>
    private GameObject currentAuraInstance; 

    /// <summary>
    /// Define a aura visual do GameObject com base no tipo de elemento fornecido.
    /// Se já houver uma aura ativa, ela será destruída antes que a nova seja instanciada.
    /// </summary>
    /// <param name="newElementType">O ElementType da aura a ser aplicada.</param>
    public void SetAura(ElementType newElementType)
    {
        // Destrói a instância da aura atualmente ativa, se houver.
        if (currentAuraInstance != null)
        {
            Destroy(currentAuraInstance);
            currentAuraInstance = null; // Limpa a referência.
        }

        // Procura na lista de configurações de aura pelo prefab correspondente ao novo tipo de elemento.
        ElementAura auraToApply = elementAuras.Find(aura => aura.elementType == newElementType);

        // Se um prefab de aura for encontrado para o elemento e não for nulo.
        if (auraToApply != null && auraToApply.auraEffectPrefab != null)
        {
            // Instancia o prefab da aura como um filho deste GameObject (o GameObject ao qual este script está anexado).
            // Isso garante que a aura se mova e gire junto com o objeto pai.
            currentAuraInstance = Instantiate(auraToApply.auraEffectPrefab, transform);
            // Define a posição local da aura para o centro do objeto pai.
            currentAuraInstance.transform.localPosition = Vector3.zero; 
            Debug.Log($"Aura de {newElementType} aplicada.");
        }
        // Se o elemento não for 'None' (nenhuma aura) e nenhum prefab for encontrado, emite um aviso.
        else if (newElementType != ElementType.None)
        {
            Debug.LogWarning($"Prefab de aura não encontrado para o elemento: {newElementType}");
        }
    }
}

