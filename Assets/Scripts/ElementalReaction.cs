using System;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// Define os tipos de reações elementais que podem ocorrer no Genshin Impact.
/// </summary>
public enum ReactionType
{
    /// <summary>
    /// Nenhuma reação ocorreu.
    /// </summary>
    None,
    /// <summary>
    /// Reação de Sobrecarga (Pyro + Electro).
    /// </summary>
    Overload,
    /// <summary>
    /// Reação de Estilhaçar (Cryo + Dano Físico).
    /// </summary>
    Shatter,
    /// <summary>
    /// Reação de Redemoinho (Anemo + Pyro/Hydro/Electro/Cryo).
    /// </summary>
    Swirl,
    /// <summary>
    /// Reação de Supercondutor (Cryo + Electro).
    /// </summary>
    Superconduct,
    /// <summary>
    /// Reação de Eletricamente Carregado (Hydro + Electro).
    /// </summary>
    ElectroCharged,
    /// <summary>
    /// Reação de Fusão (Pyro + Cryo ou Cryo + Pyro).
    /// </summary>
    Melt,
    /// <summary>
    /// Reação de Vaporização (Hydro + Pyro ou Pyro + Hydro).
    /// </summary>
    Vaporize,
    /// <summary>
    /// Reação de Congelar (Hydro + Cryo).
    /// </summary>
    Freeze,
    /// <summary>
    /// Reação de Cristalização (Geo + Pyro/Hydro/Electro/Cryo).
    /// </summary>
    Crystallize,
    /// <summary>
    /// Reação de Queimadura (Dendro + Pyro).
    /// </summary>
    Burning,
    /// <summary>
    /// Reação de Florescimento (Dendro + Hydro).
    /// </summary>
    Bloom,
    /// <summary>
    /// Reação de Aceleração (Dendro + Electro).
    /// </summary>
    Quicken,
    /// <summary>
    /// Reação de Intensificação (Quicken + Electro).
    /// </summary>
    Aggravate,
    /// <summary>
    /// Reação de Propagação (Quicken + Dendro).
    /// </summary>
    Spread
}

/// <summary>
/// Gerencia as definições e a lógica para determinar as reações elementais.
/// </summary>
public class ElementalReaction
{
    /// <summary>
    /// O primeiro elemento envolvido na reação.
    /// </summary>
    public ElementType Element1 { get; private set; }
    /// <summary>
    /// O segundo elemento envolvido na reação.
    /// </summary>
    public ElementType Element2 { get; private set; }
    /// <summary>
    /// O tipo de reação resultante.
    /// </summary>
    public ReactionType Type { get; private set; }

    /// <summary>
    /// Construtor para criar uma nova instância de ElementalReaction.
    /// </summary>
    /// <param name="e1">O primeiro ElementType.</param>
    /// <param name="e2">O segundo ElementType.</param>
    /// <param name="type">O ReactionType resultante.</param>
    public ElementalReaction(ElementType e1, ElementType e2, ReactionType type)
    {
        Element1 = e1;
        Element2 = e2;
        Type = type;
    }

    /// <summary>
    /// Um dicionário estático que mapeia combinações de dois elementos para o tipo de reação resultante.
    /// As chaves são Tuples de ElementType, representando as combinações de elementos.
    /// </summary>
    public static Dictionary<Tuple<ElementType, ElementType>, ReactionType> AllReactions { get; private set; }

    /// <summary>
    /// Construtor estático que inicializa o dicionário AllReactions com todas as reações elementais básicas.
    /// </summary>
    static ElementalReaction()
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
    /// <returns>O ReactionType resultante da combinação dos dois elementos. Retorna None se nenhuma reação ocorrer.</returns>
    public static ReactionType GetReaction(ElementType existingElement, ElementType incomingElement)
    {
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
                existingElement == ElementType.Dendro) // Geo também pode cristalizar com Dendro
            {
                return ReactionType.Crystallize;
            }
        }

        // Lógica para reações de Intensificação (Aggravate) e Propagação (Spread).
        // Estas reações dependem de uma aura de Aceleração (Quicken) pré-existente.
        // Para simplificar neste modelo de reação direta, assumimos que Quicken é um estado.
        // Um sistema mais complexo rastrearia auras ativas no alvo.
        if (existingElement == ReactionType.Quicken.ToElementType() && incomingElement == ElementType.Electro)
        {
            return ReactionType.Aggravate;
        }
        if (existingElement == ReactionType.Quicken.ToElementType() && incomingElement == ElementType.Dendro)
        {
            return ReactionType.Spread;
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

/// <summary>
/// Classe de extensão para converter um ReactionType em um ElementType.
/// Esta é uma simplificação para a lógica de GetReaction e pode precisar de um sistema mais robusto de rastreamento de auras em um jogo completo.
/// </summary>
public static class ReactionTypeExtensions
{
    /// <summary>
    /// Converte um ReactionType específico (como Quicken) em um ElementType para simular a presença de uma aura.
    /// </summary>
    /// <param name="reactionType">O ReactionType a ser convertido.</param>
    /// <returns>O ElementType correspondente à aura, ou None se não houver correspondência direta.</returns>
    public static ElementType ToElementType(this ReactionType reactionType)
    {
        switch (reactionType)
        {
            case ReactionType.Quicken: return ElementType.Dendro; // Placeholder para a aura de Aceleração
            default: return ElementType.None;
        }
    }
}

