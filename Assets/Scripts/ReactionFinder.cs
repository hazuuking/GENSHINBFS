using System;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// Classe responsável por encontrar reações elementais e, em cenários mais complexos, sequências ótimas de reações.
/// </summary>
public class ReactionFinder
{
    /// <summary>
    /// Encontra a melhor reação direta entre um elemento existente e um elemento que está sendo aplicado.
    /// Para o cenário de duas máquinas, esta função é usada para determinar a reação imediata.
    /// </summary>
    /// <param name="currentElement">O ElementType atualmente aplicado ao objeto.</param>
    /// <param name="incomingElement">O ElementType que está sendo aplicado.</param>
    /// <returns>O ReactionType resultante da combinação. Retorna None se nenhuma reação ocorrer.</returns>
    public static ReactionType FindBestReaction(ElementType currentElement, ElementType incomingElement)
    {
        // Chama o método GetReaction da classe ElementalReaction para determinar a reação.
        ReactionType reaction = ElementalReaction.GetReaction(currentElement, incomingElement);
        return reaction;
    }

    /// <summary>
    /// (Cenário mais complexo) Encontra uma sequência ótima de reações elementais usando Busca em Largura (BFS).
    /// Esta função explora diferentes combinações de elementos para encontrar a sequência que resulta na maior avaliação total.
    /// </summary>
    /// <param name="initialElement">O ElementType inicial do objeto.</param>
    /// <param name="availableElements">Uma lista de ElementType que podem ser aplicados em sequência.</param>
    /// <returns>Uma lista de ReactionType que representa a sequência de reações com a maior avaliação.</returns>
    public static List<ReactionType> FindOptimalReactionSequence(ElementType initialElement, List<ElementType> availableElements)
    {
        // Fila para o algoritmo BFS. Cada item na fila é uma tupla contendo:
        // 1. O ElementType atual do objeto.
        // 2. A lista de reações que levaram a este estado.
        Queue<Tuple<ElementType, List<ReactionType>>> queue = new Queue<Tuple<ElementType, List<ReactionType>>>();
        // Adiciona o estado inicial à fila.
        queue.Enqueue(Tuple.Create(initialElement, new List<ReactionType>()));

        List<ReactionType> bestSequence = new List<ReactionType>();
        float maxEvaluation = -1.0f; // Inicializa a avaliação máxima com um valor baixo.

        // Loop principal do BFS.
        while (queue.Count > 0)
        {
            // Remove o estado atual da fila.
            var current = queue.Dequeue();
            ElementType currentElement = current.Item1;
            List<ReactionType> currentSequence = current.Item2;

            // Avalia a sequência de reações atual somando as avaliações de cada reação na sequência.
            float currentEvaluation = currentSequence.Sum(r => ReactionEvaluator.EvaluateReaction(r));

            // Se a avaliação da sequência atual for maior que a avaliação máxima encontrada até agora, atualiza a melhor sequência.
            if (currentEvaluation > maxEvaluation)
            {
                maxEvaluation = currentEvaluation;
                bestSequence = new List<ReactionType>(currentSequence);
            }

            // Explora os próximos estados aplicando cada elemento disponível.
            foreach (ElementType nextElement in availableElements)
            {
                // Tenta encontrar a reação entre o elemento atual e o próximo elemento.
                ReactionType nextReaction = ElementalReaction.GetReaction(currentElement, nextElement);
                if (nextReaction != ReactionType.None)
                {
                    // Cria uma nova sequência adicionando a reação encontrada.
                    List<ReactionType> newSequence = new List<ReactionType>(currentSequence);
                    newSequence.Add(nextReaction);
                    // Adiciona o novo estado (com o próximo elemento como o elemento atual e a nova sequência) à fila.
                    // Nota: Em Genshin, elementos podem coexistir, o que exigiria uma representação de estado mais complexa.
                    // Aqui, para simplificar, assumimos que o novo elemento se torna o elemento principal para a próxima reação.
                    queue.Enqueue(Tuple.Create(nextElement, newSequence)); 
                }
            }
        }
        // Retorna a melhor sequência de reações encontrada.
        return bestSequence;
    }
}

