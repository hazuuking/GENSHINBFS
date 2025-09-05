/// <summary>
/// Fornece um método para avaliar o "valor" ou "impacto" de cada tipo de reação elemental.
/// Os valores são arbitrários e podem ser ajustados para refletir a importância relativa de cada reação no contexto do jogo.
/// </summary>
public static class ReactionEvaluator
{
    /// <summary>
    /// Avalia o impacto de uma reação elemental específica.
    /// </summary>
    /// <param name="reactionType">O tipo de reação elemental a ser avaliado.</param>
    /// <returns>Um valor float que representa a avaliação da reação. Valores mais altos indicam reações mais impactantes.</returns>
    public static float EvaluateReaction(ReactionType reactionType)
    {
        switch (reactionType)
        {
            case ReactionType.Overload: return 10.0f; // Alto dano, empurra inimigos
            case ReactionType.Shatter: return 7.0f; // Dano moderado
            case ReactionType.Swirl: return 8.0f; // Espalha o elemento, bom para dano em área
            case ReactionType.Superconduct: return 6.0f; // Dano moderado, reduz resistência física
            case ReactionType.ElectroCharged: return 9.0f; // Dano contínuo
            case ReactionType.Melt: return 12.0f; // Alto multiplicador de dano
            case ReactionType.Vaporize: return 11.0f; // Alto multiplicador de dano
            case ReactionType.Freeze: return 5.0f; // Controle de multidão
            case ReactionType.Crystallize: return 4.0f; // Geração de escudo
            case ReactionType.Burning: return 7.5f; // Dano Pyro contínuo
            case ReactionType.Bloom: return 8.5f; // Gera núcleos Dendro
            case ReactionType.Quicken: return 6.5f; // Prepara para Intensificação/Propagação
            case ReactionType.Aggravate: return 10.5f; // Aumenta o dano Electro
            case ReactionType.Spread: return 9.5f; // Aumenta o dano Dendro
            default: return 0.0f; // Reação desconhecida ou sem impacto
        }
    }
}

