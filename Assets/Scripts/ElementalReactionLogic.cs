using UnityEngine;


/// <summary>
/// Classe estática que implementa a **lógica central do grafo de reações elementais**.
/// Esta classe define as "arestas" do grafo: dado um "nó" inicial (elemento atual) e um "nó" de entrada
/// (elemento aplicado), ela determina qual "aresta" (reação) é percorrida.
/// </summary>
public static class ElementalReactionLogic
{
// O ReactionType é definido em ReactionEvaluator.cs.
// Usamos o 'using static' para evitar a repetição de ReactionEvaluator.ReactionType.


    /// <summary>
    /// Calcula a reação elemental resultante da aplicação de um novo elemento (<c>incoming</c>)
    /// sobre um objeto que já possui um elemento base (<c>current</c>).
    /// </summary>
    /// <param name="current">O elemento base que já está aplicado ao objeto (o nó de partida no grafo).</param>
    /// <param name="incoming">O novo elemento que está sendo aplicado (o nó de destino potencial).</param>
    /// <param name="status">Um estado elemental adicional (ex: Quicken) que pode alterar o resultado da reação.</param>
    /// <returns>O tipo de reação (<c>ReactionType</c>) que ocorre, representando a aresta percorrida no grafo.</returns>
    public static ReactionEvaluator.ReactionType GetReaction(ElementType current, ElementType incoming, ElementType status = ElementType.None)
    {
        // ------------------------------------------------------------------------------------------------
        // 1. Lógica para reações de segundo nível (Aggravate/Spread)
        // Estas reações dependem de um estado prévio (Quicken) e não apenas dos dois elementos.
        // Isso simula um caminho de reação de dois passos (Dendro + Electro -> Quicken -> Aggravate/Spread).
        // ------------------------------------------------------------------------------------------------
        if (status == ElementType.Quicken)
        {
            if (incoming == ElementType.Electro) return ReactionEvaluator.ReactionType.Aggravate;
            if (incoming == ElementType.Dendro) return ReactionEvaluator.ReactionType.Spread;
        }

        // ------------------------------------------------------------------------------------------------
        // 2. Lógica de Reações Elementais Primárias (Switch-Case)
        // O switch-case simula a busca na matriz de adjacência do grafo de reações.
        // O 'current' elemento define a linha, e o 'incoming' define a coluna.
        // ------------------------------------------------------------------------------------------------
        switch (current)
        {
            case ElementType.Pyro:
                // Reações iniciadas por Pyro
                if (incoming == ElementType.Hydro) return ReactionEvaluator.ReactionType.Vaporize;
                if (incoming == ElementType.Cryo) return ReactionEvaluator.ReactionType.Melt;
                if (incoming == ElementType.Electro) return ReactionEvaluator.ReactionType.Overload;
                break;

            case ElementType.Hydro:
                // Reações iniciadas por Hydro
                // Note que Vaporize é simétrico (Pyro + Hydro = Hydro + Pyro).
                if (incoming == ElementType.Pyro) return ReactionEvaluator.ReactionType.Vaporize; 
                if (incoming == ElementType.Cryo) return ReactionEvaluator.ReactionType.Freeze;
                if (incoming == ElementType.Electro) return ReactionEvaluator.ReactionType.ElectroCharged;
                if (incoming == ElementType.Dendro) return ReactionEvaluator.ReactionType.Bloom;
                break;

            case ElementType.Cryo:
                // Reações iniciadas por Cryo
                // Note que Melt é simétrico (Pyro + Cryo = Cryo + Pyro).
                if (incoming == ElementType.Pyro) return ReactionEvaluator.ReactionType.Melt;
                if (incoming == ElementType.Hydro) return ReactionEvaluator.ReactionType.Freeze;
                if (incoming == ElementType.Electro) return ReactionEvaluator.ReactionType.Superconduct;
                break;

            case ElementType.Electro:
                // Reações iniciadas por Electro
                if (incoming == ElementType.Pyro) return ReactionEvaluator.ReactionType.Overload;
                if (incoming == ElementType.Hydro) return ReactionEvaluator.ReactionType.ElectroCharged;
                if (incoming == ElementType.Cryo) return ReactionEvaluator.ReactionType.Superconduct;
                if (incoming == ElementType.Dendro) return ReactionEvaluator.ReactionType.Quicken;
                break;

            case ElementType.Dendro:
                // Reações iniciadas por Dendro
                if (incoming == ElementType.Pyro) return ReactionEvaluator.ReactionType.Burning;
                if (incoming == ElementType.Hydro) return ReactionEvaluator.ReactionType.Bloom;
                if (incoming == ElementType.Electro) return ReactionEvaluator.ReactionType.Quicken;
                break;
        }

        // ------------------------------------------------------------------------------------------------
        // 3. Lógica para reações de suporte (Anemo e Geo)
        // Estas reações são geralmente não-simétricas e dependem apenas do elemento que está sendo aplicado.
        // ------------------------------------------------------------------------------------------------
        
        // Reação de Redemoinho (Swirl): Anemo reage com qualquer elemento aplicável no alvo.
        if (incoming == ElementType.Anemo && (current == ElementType.Pyro || current == ElementType.Hydro || current == ElementType.Cryo || current == ElementType.Electro))
        {
            return ReactionEvaluator.ReactionType.Swirl;
        }

        // Reação de Cristalização (Crystallize): Geo reage com qualquer elemento aplicável, exceto Anemo.
        if (incoming == ElementType.Geo && (current != ElementType.None && current != ElementType.Anemo))
        {
            return ReactionEvaluator.ReactionType.Crystallize;
        }

        // Se nenhuma combinação acima for encontrada, nenhuma reação ocorre.
        return ReactionEvaluator.ReactionType.None;
    }
}