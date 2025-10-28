using UnityEngine;

/// <summary>
/// Classe estática responsável por atribuir um valor de avaliação (score) a cada tipo de reação elemental.
/// No contexto da teoria dos grafos e do algoritmo de Busca em Largura (BFS) / Busca em Profundidade (DFS)
/// para encontrar o caminho ideal, esta função atua como a **função de custo** ou **função heurística**
/// que guia a escolha da reação mais "ótima" na Máquina 2.
/// </summary>
public static class ReactionEvaluator
{
    /// <summary>
    /// Enumeração que lista todos os tipos de reações elementais possíveis no sistema.
    /// É utilizada como chave para a função de avaliação.
    /// </summary>
    public enum ReactionType
    {
        None,           // Nenhuma reação
        Overload,       // Sobrecarga (Pyro + Electro)
        Shatter,        // Estilhaçar (Dano físico em alvo congelado)
        Swirl,          // Redemoinho (Anemo + Pyro/Hydro/Cryo/Electro)
        Superconduct,   // Supercondução (Cryo + Electro)
        ElectroCharged, // Eletricamente Carregado (Hydro + Electro)
        Melt,           // Fusão (Pyro + Cryo ou Cryo + Pyro)
        Vaporize,       // Vaporização (Pyro + Hydro ou Hydro + Pyro)
        Freeze,         // Congelar (Hydro + Cryo)
        Crystallize,    // Cristalização (Geo + Pyro/Hydro/Cryo/Electro)
        Burning,        // Queimadura (Pyro + Dendro)
        Bloom,          // Florescimento (Hydro + Dendro)
        Quicken,        // Catalisação (Electro + Dendro)
        Aggravate,      // Intensificação (Electro em Quicken)
        Spread          // Propagação (Dendro em Quicken)
    }

    /// <summary>
    /// Retorna um valor numérico (score) que representa a avaliação de uma reação específica.
    /// Este score é utilizado pelo <c>GameManager</c> para selecionar a reação de maior valor
    /// (a mais "ótima") na Máquina 2, simulando uma decisão estratégica.
    /// </summary>
    /// <param name="reactionType">O tipo de reação a ser avaliado.</param>
    /// <returns>Um valor float representando a avaliação da reação. Valores mais altos são preferidos.</returns>
    public static float EvaluateReaction(ReactionType reactionType)
    {
        // Os valores são definidos com base na utilidade percebida no jogo:
        // Reações de amplificação (Melt/Vaporize) têm os scores mais altos.
        // Reações de controle de grupo (Freeze) e dano alto (Aggravate/Spread) vêm em seguida.
        // Reações de suporte (Crystallize) têm scores mais baixos.
        switch (reactionType)
        {
            case ReactionType.Melt: return 9.0f; // Fusão: Maior amplificação de dano.
            case ReactionType.Vaporize: return 8.5f; // Vaporização: Alta amplificação de dano.
            case ReactionType.Aggravate: return 8.0f; // Intensificação: Aumento significativo de dano Electro.
            case ReactionType.Spread: return 8.0f; // Propagação: Aumento significativo de dano Dendro.
            case ReactionType.Freeze: return 7.5f; // Congelar: Forte controle de grupo (CC).
            case ReactionType.Overload: return 7.0f; // Sobrecarga: Alto dano em área com explosão.
            case ReactionType.ElectroCharged: return 6.5f; // Eletricamente Carregado: Dano contínuo e se espalha.
            case ReactionType.Swirl: return 6.0f; // Redemoinho: Espalha elementos, excelente para múltiplos alvos.
            case ReactionType.Quicken: return 6.0f; // Catalisação: Reação preparatória para Aggravate/Spread.
            case ReactionType.Superconduct: return 5.0f; // Supercondução: Redução de resistência física.
            case ReactionType.Burning: return 4.5f; // Queimadura: Dano contínuo.
            case ReactionType.Shatter: return 4.0f; // Estilhaçar: Dano físico extra em alvo congelado.
            case ReactionType.Crystallize: return 3.0f; // Cristalização: Gera escudos, foco em defesa/sobrevivência.
            case ReactionType.None: return 0.0f; // Nenhuma reação: Valor base.
            default: return 1.0f; // Valor padrão para reações não especificadas.
        }
    }
}