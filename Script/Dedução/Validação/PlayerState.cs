using UnityEngine;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// Gerenciador Singleton que armazena o estado atual das dedu��es do jogador
/// e o seu progresso de desbloqueio no jogo.
/// </summary>
public class PlayerState : MonoBehaviour
{
    public static PlayerState Instance { get; private set; }

    // --- Estado das Dedu��es ---
    private List<DeducaoJogador> listaDeDeducoes;

    // --- Estado do Progresso (NOVO) ---
    [Header("Progresso do Jogador")]
    private HashSet<string> lembrancasConcluidas = new HashSet<string>();
    private List<int> perfisDesbloqueadosNoDiario = new List<int>();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(this.gameObject);
            // A inicializa��o agora acontece de forma diferente.
            listaDeDeducoes = new List<DeducaoJogador>();
        }
    }

    // --- M�todos de Dedu��o ---

    public List<DeducaoJogador> GetTodasAsDeducoes()
    {
        return listaDeDeducoes;
    }

    public DeducaoJogador GetDeducaoPorId(int personagemId)
    {
        return listaDeDeducoes.FirstOrDefault(d => d.idPersonagem == personagemId);
    }

    // --- M�todos de Progresso (NOVOS) ---

    /// <summary>
    /// Verifica se uma lembran�a j� foi conclu�da pelo jogador.
    /// </summary>
    public bool JaConcluiuLembranca(string idLembranca)
    {
        return lembrancasConcluidas.Contains(idLembranca);
    }

    /// <summary>
    /// Marca uma lembran�a como conclu�da e desbloqueia os perfis de personagem associados.
    /// </summary>
    public void ConcluirLembranca(string idLembranca, List<int> idsParaDesbloquear)
    {
        if (lembrancasConcluidas.Contains(idLembranca)) return;

        lembrancasConcluidas.Add(idLembranca);
        Debug.Log($"Lembran�a '{idLembranca}' marcada como conclu�da.");

        foreach (var id in idsParaDesbloquear)
        {
            if (!perfisDesbloqueadosNoDiario.Contains(id))
            {
                perfisDesbloqueadosNoDiario.Add(id);
                // Cria uma nova entrada de dedu��o vazia para o novo perfil
                listaDeDeducoes.Add(new DeducaoJogador(id));
                Debug.Log($"Perfil de personagem com ID {id} desbloqueado para o di�rio.");
            }
        }
    }

    /// <summary>
    /// Retorna a lista de IDs de todos os perfis que devem estar vis�veis no di�rio.
    /// </summary>
    public List<int> GetPerfisDesbloqueados()
    {
        return perfisDesbloqueadosNoDiario;
    }
}