using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Define a estrutura de dados para o conte�do que uma �nica lembran�a desbloqueia.
/// mapeia um ID de lembran�a a uma lista de IDs de personagens.
/// </summary>
[System.Serializable]
public class ConteudoDesbloqueavel
{
    [Tooltip("O ID �nico para esta lembran�a. Deve corresponder ao ID no objeto LembrancaSystem na cena.")]
    public string idDaLembranca;

    [Tooltip("A lista de IDs dos personagens que esta lembran�a ir� desbloquear no di�rio.")]
    public List<int> idsDePersonagensParaDesbloquear;
}