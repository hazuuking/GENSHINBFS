using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Gerenciador de Aura Elemental (Visual Effects - VFX).
/// Este componente é responsável por traduzir o estado lógico elemental (<c>ElementType</c>)
/// em feedback visual para o usuário, instanciando e gerenciando o prefab de VFX
/// correspondente ao elemento aplicado.
/// </summary>
public class ElementalAuraManager : MonoBehaviour
{
    // --- VARIÁVEIS DE ESTADO ---

    /// <summary>
    /// O <c>ElementType</c> da aura atualmente ativa no objeto.
    /// É usado para evitar a reativação desnecessária do mesmo efeito.
    /// </summary>
    public ElementType currentAura = ElementType.None;

    /// <summary>
    /// O <c>ElementType</c> do status elemental atual (ex: Quicken, Wet, etc.).
    /// Usado para reações de segundo nível (ex: Aggravate/Spread em Quicken).
    /// </summary>
    public ElementType currentStatus = ElementType.None;

    // --- ESTRUTURA DE DADOS (SERIALIZÁVEL) ---

    /// <summary>
    /// Classe interna serializável para mapear um <c>ElementType</c> a um <c>GameObject</c> (o prefab do VFX).
    /// Permite a configuração do mapeamento Elemento -> Efeito Visual diretamente no Inspector.
    /// </summary>
    [System.Serializable]
    public class AuraEffect
    {
        public ElementType elementType;
        public GameObject vfxPrefab;
    }

    // --- VARIÁVEIS PÚBLICAS (CONFIGURÁVEIS NO INSPECTOR) ---

    /// <summary>
    /// Lista que armazena o mapeamento de todos os elementos e seus respectivos prefabs de VFX.
    /// </summary>
    public List<AuraEffect> auraVFXPrefabs;

    // --- VARIÁVEIS INTERNAS (PRIVADAS) ---

    /// <summary>
    /// Referência à instância atual do VFX que está ativa na cena.
    /// É usada para destruir o efeito anterior antes de aplicar um novo.
    /// </summary>
    private GameObject currentVFXInstance;

    // --- MÉTODOS DE CONTROLE ---

    /// <summary>
    /// Define a aura elemental para o objeto.
    /// Este é o ponto de integração entre a lógica de jogo (o elemento aplicado) e a visualização (o VFX).
    /// </summary>
    /// <param name="newAura">O novo <c>ElementType</c> da aura a ser aplicada.</param>
    public void SetAura(ElementType newAura)
    {
        // 1. Otimização: Se a aura for a mesma, sai do método.
        if (currentAura == newAura) return; 

        // 2. Limpeza: Destrói o VFX atualmente ativo para garantir que apenas um efeito esteja visível por vez.
        if (currentVFXInstance != null)
        {
            Destroy(currentVFXInstance);
            currentVFXInstance = null;
        }

        // 3. Atualiza o estado da aura.
        currentAura = newAura;

        // 4. Instanciação do Novo VFX:
        if (currentAura != ElementType.None)
        {
            // Busca o mapeamento de efeito correspondente ao novo elemento.
            AuraEffect effect = auraVFXPrefabs.Find(e => e.elementType == currentAura);
            
            if (effect != null && effect.vfxPrefab != null)
            {
                // Instancia o prefab do VFX. O novo VFX é instanciado na posição do objeto
                // e é definido como filho do objeto (<c>transform</c>) para que se mova junto.
                currentVFXInstance = Instantiate(effect.vfxPrefab, transform.position, Quaternion.identity, transform);
                Debug.Log($"[ElementalAuraManager] Aura de {currentAura} ativada.");
            }
            else
            {
                Debug.LogWarning($"[ElementalAuraManager] Prefab de VFX para {currentAura} não encontrado na lista 'auraVFXPrefabs'.");
            }
        }
        else
        {
            // Caso a nova aura seja None, apenas loga a desativação.
            Debug.Log("[ElementalAuraManager] Aura desativada (Elemento None).");
        }
    }
}