using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Define a estrutura de dados para o conteúdo que uma única lembrança desbloqueia.
/// mapeia um ID de lembrança a uma lista de IDs de personagens.
/// </summary>
[System.Serializable]
public class ConteudoDesbloqueavel
{
    [Tooltip("O ID único para esta lembrança. Deve corresponder ao ID no objeto LembrancaSystem na cena.")]
    public string idDaLembranca;

    [Tooltip("A lista de IDs dos personagens que esta lembrança irá desbloquear no diário.")]
    public List<int> idsDePersonagensParaDesbloquear;
}