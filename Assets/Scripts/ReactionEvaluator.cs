using UnityEngine;

/// <summary>
/// Classe estática responsável por atribuir um valor de avaliação (score) a cada tipo de reação elemental.
/// Atua como a função de custo / heurística usada pela máquina de decisão.
/// </summary>
public static class ReactionEvaluator
{
    /// <summary>
    /// Retorna um valor numérico (score) que representa a avaliação de uma reação específica.
    /// Este score é usado para determinar a "melhor" reação possível.
    /// </summary>
    public static float EvaluateReaction(ReactionType reactionType)
    {
        switch (reactionType)
        {
            case ReactionType.Melt: return 9.0f;
            case ReactionType.Vaporize: return 8.5f;
            case ReactionType.Aggravate: return 8.0f;
            case ReactionType.Spread: return 8.0f;
            case ReactionType.Freeze: return 7.5f;
            case ReactionType.Overload: return 7.0f;
            case ReactionType.ElectroCharged: return 6.5f;
            case ReactionType.Swirl: return 6.0f;
            case ReactionType.Quicken: return 6.0f;
            case ReactionType.Superconduct: return 5.0f;
            case ReactionType.Burning: return 4.5f;
            case ReactionType.Shatter: return 4.0f;
            case ReactionType.Crystallize: return 3.0f;
            case ReactionType.None: return 0.0f;
            default: return 1.0f;
        }
    }
}