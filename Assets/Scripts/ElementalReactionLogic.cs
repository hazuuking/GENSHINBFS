using UnityEngine;

/// <summary>
/// Classe estática que implementa a lógica central do grafo de reações elementais.
/// Define como cada par de elementos interage para produzir uma reação.
/// </summary>
public static class ElementalReactionLogic
{
    public static ReactionType GetReaction(ElementType current, ElementType incoming, ElementType status = ElementType.None)
    {
        // 1. Reações secundárias (dependentes de status)
        if (status == ElementType.Quicken)
        {
            if (incoming == ElementType.Electro) return ReactionType.Aggravate;
            if (incoming == ElementType.Dendro) return ReactionType.Spread;
        }

        // 2. Reações principais
        switch (current)
        {
            case ElementType.Pyro:
                if (incoming == ElementType.Hydro) return ReactionType.Vaporize;
                if (incoming == ElementType.Cryo) return ReactionType.Melt;
                if (incoming == ElementType.Electro) return ReactionType.Overload;
                break;

            case ElementType.Hydro:
                if (incoming == ElementType.Pyro) return ReactionType.Vaporize; 
                if (incoming == ElementType.Cryo) return ReactionType.Freeze;
                if (incoming == ElementType.Electro) return ReactionType.ElectroCharged;
                if (incoming == ElementType.Dendro) return ReactionType.Bloom;
                break;

            case ElementType.Cryo:
                if (incoming == ElementType.Pyro) return ReactionType.Melt;
                if (incoming == ElementType.Hydro) return ReactionType.Freeze;
                if (incoming == ElementType.Electro) return ReactionType.Superconduct;
                break;

            case ElementType.Electro:
                if (incoming == ElementType.Pyro) return ReactionType.Overload;
                if (incoming == ElementType.Hydro) return ReactionType.ElectroCharged;
                if (incoming == ElementType.Cryo) return ReactionType.Superconduct;
                if (incoming == ElementType.Dendro) return ReactionType.Quicken;
                break;

            case ElementType.Dendro:
                if (incoming == ElementType.Pyro) return ReactionType.Burning;
                if (incoming == ElementType.Hydro) return ReactionType.Bloom;
                if (incoming == ElementType.Electro) return ReactionType.Quicken;
                break;
        }

        // 3. Reações especiais (Anemo / Geo)
        if (incoming == ElementType.Anemo &&
            (current == ElementType.Pyro || current == ElementType.Hydro || current == ElementType.Cryo || current == ElementType.Electro))
        {
            return ReactionType.Swirl;
        }

        if (incoming == ElementType.Geo &&
            (current != ElementType.None && current != ElementType.Anemo))
        {
            return ReactionType.Crystallize;
        }

        return ReactionType.None;
    }
}