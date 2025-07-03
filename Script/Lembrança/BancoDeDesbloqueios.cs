using UnityEngine;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// Um ScriptableObject que atua como uma base de dados central para todos os desbloqueios do jogo.
/// Ele cont�m uma lista de todo o conte�do desbloque�vel.
/// </summary>
[CreateAssetMenu(fileName = "BancoDeDesbloqueios", menuName = "MuseuTop/Banco de Desbloqueios", order = 1)]
public class BancoDeDesbloqueios : ScriptableObject
{
    [Tooltip("A lista completa de todas as configura��es de desbloqueio para cada lembran�a no jogo.")]
    public List<ConteudoDesbloqueavel> todosOsDesbloqueios;

    /// <summary>
    /// Encontra e retorna os IDs dos personagens a serem desbloqueados para um determinado ID de lembran�a.
    /// </summary>
    /// <param name="idLembranca">O ID da lembran�a a ser procurada.</param>
    /// <returns>Uma lista de IDs de personagens, ou uma lista vazia se o ID n�o for encontrado.</returns>
    public List<int> GetPersonagensParaDesbloquear(string idLembranca)
    {
        var desbloqueio = todosOsDesbloqueios.FirstOrDefault(d => d.idDaLembranca == idLembranca);
        if (desbloqueio != null)
        {
            return desbloqueio.idsDePersonagensParaDesbloquear;
        }
        // Retorna uma lista vazia para evitar erros se o ID n�o for encontrado.
        return new List<int>();
    }
}