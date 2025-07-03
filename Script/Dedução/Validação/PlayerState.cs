using UnityEngine;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// Gerenciador Singleton que armazena o estado atual das deduções do jogador
/// e o seu progresso de desbloqueio no jogo.
/// </summary>
public class PlayerState : MonoBehaviour
{
    public static PlayerState Instance { get; private set; }

    // --- Estado das Deduções ---
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
            // A inicialização agora acontece de forma diferente.
            listaDeDeducoes = new List<DeducaoJogador>();
        }
    }

    // --- Métodos de Dedução ---

    public List<DeducaoJogador> GetTodasAsDeducoes()
    {
        return listaDeDeducoes;
    }

    public DeducaoJogador GetDeducaoPorId(int personagemId)
    {
        return listaDeDeducoes.FirstOrDefault(d => d.idPersonagem == personagemId);
    }

    // --- Métodos de Progresso (NOVOS) ---

    /// <summary>
    /// Verifica se uma lembrança já foi concluída pelo jogador.
    /// </summary>
    public bool JaConcluiuLembranca(string idLembranca)
    {
        return lembrancasConcluidas.Contains(idLembranca);
    }

    /// <summary>
    /// Marca uma lembrança como concluída e desbloqueia os perfis de personagem associados.
    /// </summary>
    public void ConcluirLembranca(string idLembranca, List<int> idsParaDesbloquear)
    {
        if (lembrancasConcluidas.Contains(idLembranca)) return;

        lembrancasConcluidas.Add(idLembranca);
        Debug.Log($"Lembrança '{idLembranca}' marcada como concluída.");

        foreach (var id in idsParaDesbloquear)
        {
            if (!perfisDesbloqueadosNoDiario.Contains(id))
            {
                perfisDesbloqueadosNoDiario.Add(id);
                // Cria uma nova entrada de dedução vazia para o novo perfil
                listaDeDeducoes.Add(new DeducaoJogador(id));
                Debug.Log($"Perfil de personagem com ID {id} desbloqueado para o diário.");
            }
        }
    }

    /// <summary>
    /// Retorna a lista de IDs de todos os perfis que devem estar visíveis no diário.
    /// </summary>
    public List<int> GetPerfisDesbloqueados()
    {
        return perfisDesbloqueadosNoDiario;
    }
}