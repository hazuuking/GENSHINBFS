using UnityEngine;

/// <summary>
/// Avalia a "qualidade" ou "impacto" de uma reação elemental.
/// Os valores de avaliação podem ser ajustados para simular diferentes prioridades (ex: dano, controle de grupo).
/// </summary>
public static class ReactionEvaluator
{
    /// <summary>
    /// Retorna um valor numérico que representa a avaliação de uma reação específica.
    /// Valores mais altos indicam reações "melhores" ou mais desejáveis.
    /// </summary>
    /// <param name="reactionType">O tipo de reação a ser avaliado.</param>
    /// <returns>Um float representando a avaliação da reação.</returns>
    public static float EvaluateReaction(ReactionType reactionType)
    {
        switch (reactionType)
        {
            case ReactionType.Overload: return 7.0f; // Alto dano em área
            case ReactionType.Shatter: return 4.0f; // Dano físico extra
            case ReactionType.Swirl: return 6.0f; // Espalha elementos, bom para múltiplos alvos
            case ReactionType.Superconduct: return 5.0f; // Reduz resistência física, bom para equipes físicas
            case ReactionType.ElectroCharged: return 6.5f; // Dano contínuo e se espalha
            case ReactionType.Melt: return 9.0f; // Alto dano amplificado
            case ReactionType.Vaporize: return 8.5f; // Alto dano amplificado
            case ReactionType.Freeze: return 7.5f; // Controle de grupo forte
            case ReactionType.Crystallize: return 3.0f; // Gera escudos, defesa
            case ReactionType.Burning: return 4.5f; // Dano contínuo
            case ReactionType.Bloom: return 5.5f; // Gera sementes que explodem
            case ReactionType.Quicken: return 6.0f; // Prepara para Aggravate/Spread
            case ReactionType.Aggravate: return 8.0f; // Aumenta dano Electro
            case ReactionType.Spread: return 8.0f; // Aumenta dano Dendro
            case ReactionType.None: return 0.0f; // Nenhuma reação, valor baixo
            default: return 1.0f; // Valor padrão para reações não especificadas
        }
    }
}


