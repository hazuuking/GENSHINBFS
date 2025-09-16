using System;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// Gerencia as definições e a lógica para determinar as reações elementais.
/// </summary>
public static class ElementalReactionLogic
{
    /// <summary>
    /// Um dicionário estático que mapeia combinações de dois elementos para o tipo de reação resultante.
    /// As chaves são Tuples de ElementType, representando as combinações de elementos.
    /// </summary>
    public static Dictionary<Tuple<ElementType, ElementType>, ReactionType> AllReactions { get; private set; }

    /// <summary>
    /// Construtor estático que inicializa o dicionário AllReactions com todas as reações elementais básicas.
    /// </summary>
    static ElementalReactionLogic()
    {
        AllReactions = new Dictionary<Tuple<ElementType, ElementType>, ReactionType>
        {
            // Reações Transformativas
            { Tuple.Create(ElementType.Pyro, ElementType.Electro), ReactionType.Overload },
            { Tuple.Create(ElementType.Electro, ElementType.Pyro), ReactionType.Overload },
            { Tuple.Create(ElementType.Cryo, ElementType.Hydro), ReactionType.Freeze },
            { Tuple.Create(ElementType.Hydro, ElementType.Cryo), ReactionType.Freeze },
            { Tuple.Create(ElementType.Cryo, ElementType.Electro), ReactionType.Superconduct },
            { Tuple.Create(ElementType.Electro, ElementType.Cryo), ReactionType.Superconduct },
            { Tuple.Create(ElementType.Hydro, ElementType.Electro), ReactionType.ElectroCharged },
            { Tuple.Create(ElementType.Electro, ElementType.Hydro), ReactionType.ElectroCharged },
            
            // Reações Amplificantes
            { Tuple.Create(ElementType.Pyro, ElementType.Cryo), ReactionType.Melt },
            { Tuple.Create(ElementType.Cryo, ElementType.Pyro), ReactionType.Melt },
            { Tuple.Create(ElementType.Pyro, ElementType.Hydro), ReactionType.Vaporize },
            { Tuple.Create(ElementType.Hydro, ElementType.Pyro), ReactionType.Vaporize },

            // Reações Dendro (básicas)
            { Tuple.Create(ElementType.Dendro, ElementType.Pyro), ReactionType.Burning },
            { Tuple.Create(ElementType.Pyro, ElementType.Dendro), ReactionType.Burning },
            { Tuple.Create(ElementType.Dendro, ElementType.Hydro), ReactionType.Bloom },
            { Tuple.Create(ElementType.Hydro, ElementType.Dendro), ReactionType.Bloom },
            { Tuple.Create(ElementType.Dendro, ElementType.Electro), ReactionType.Quicken },
            { Tuple.Create(ElementType.Electro, ElementType.Dendro), ReactionType.Quicken }
        };
    }

    /// <summary>
    /// Determina o tipo de reação elemental que ocorre entre um elemento existente em um alvo e um elemento que está sendo aplicado.
    /// </summary>
    /// <param name="existingElement">O ElementType que já está aplicado ao alvo.</param>
    /// <param name="incomingElement">O ElementType que está sendo aplicado ao alvo.</param>
    /// <param name="currentStatus">O status elemental atual do alvo (ex: Quicken).</param>
    /// <returns>O ReactionType resultante da combinação dos dois elementos. Retorna None se nenhuma reação ocorrer.</returns>
    public static ReactionType GetReaction(ElementType existingElement, ElementType incomingElement, ElementType currentStatus)
    {
        // Lógica para reações de Intensificação (Aggravate) e Propagação (Spread).
        // Estas reações dependem de uma aura de Aceleração (Quicken) pré-existente.
        if (currentStatus == ElementType.Quicken)
        {
            if (incomingElement == ElementType.Electro)
                return ReactionType.Aggravate;
            if (incomingElement == ElementType.Dendro)
                return ReactionType.Spread;
        }

        // Lógica para reações de Redemoinho (Swirl) - Anemo reage com Pyro, Hydro, Electro ou Cryo.
        if (incomingElement == ElementType.Anemo)
        {
            if (existingElement == ElementType.Pyro || existingElement == ElementType.Hydro || 
                existingElement == ElementType.Electro || existingElement == ElementType.Cryo)
            {
                return ReactionType.Swirl;
            }
        }
        // Lógica para reações de Cristalização (Crystallize) - Geo reage com Pyro, Hydro, Electro, Cryo ou Dendro.
        if (incomingElement == ElementType.Geo)
        {
            if (existingElement == ElementType.Pyro || existingElement == ElementType.Hydro || 
                existingElement == ElementType.Electro || existingElement == ElementType.Cryo ||
                existingElement == ElementType.Dendro)
            {
                return ReactionType.Crystallize;
            }
        }

        // Tenta encontrar a reação na ordem direta (existente + entrante).
        Tuple<ElementType, ElementType> combination1 = Tuple.Create(existingElement, incomingElement);
        // Tenta encontrar a reação na ordem inversa (entrante + existente), pois muitas reações são simétricas.
        Tuple<ElementType, ElementType> combination2 = Tuple.Create(incomingElement, existingElement);

        if (AllReactions.ContainsKey(combination1))
        {
            return AllReactions[combination1];
        }
        if (AllReactions.ContainsKey(combination2))
        {
            return AllReactions[combination2];
        }

        // Se nenhuma reação for encontrada, retorna None.
        return ReactionType.None;
    }
}


