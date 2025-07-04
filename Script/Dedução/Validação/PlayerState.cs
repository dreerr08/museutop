using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class PlayerState : MonoBehaviour
{
    public static PlayerState Instance { get; private set; }
    private List<DeducaoJogador> listaDeDeducoes = new List<DeducaoJogador>();
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
        }
    }

    public List<DeducaoJogador> GetTodasAsDeducoes() { return listaDeDeducoes; }
    public DeducaoJogador GetDeducaoPorId(int personagemId) { return listaDeDeducoes.FirstOrDefault(d => d.idPersonagem == personagemId); }
    public bool JaConcluiuLembranca(string idLembranca) { return lembrancasConcluidas.Contains(idLembranca); }

    public void ConcluirLembranca(string idLembranca, List<int> idsParaDesbloquear)
    {
        if (lembrancasConcluidas.Contains(idLembranca)) return;
        lembrancasConcluidas.Add(idLembranca);
        Debug.Log($"[PlayerState] Lembrança '{idLembranca}' concluída.");

        foreach (var id in idsParaDesbloquear)
        {
            if (!perfisDesbloqueadosNoDiario.Contains(id))
            {
                perfisDesbloqueadosNoDiario.Add(id);
                listaDeDeducoes.Add(new DeducaoJogador(id));
                Debug.Log($"[PlayerState] Perfil {id} desbloqueado.");
            }
        }
    }

    public List<int> GetPerfisDesbloqueados()
    {
        return perfisDesbloqueadosNoDiario;
    }

    public HashSet<string> GetLembrancasConcluidas()
    {
        return lembrancasConcluidas;
    }

    /// <summary>
    /// Carrega os dados de um save, com logs detalhados.
    /// </summary>
    public void LoadData(GameData data)
    {
        Debug.Log($"--- [PlayerState] A RECEBER DADOS PARA CARREGAR ---");

        // Log do estado ANTES da alteração
        Debug.Log($"[PlayerState] ESTADO ANTES DO LOAD: Perfis={this.perfisDesbloqueadosNoDiario.Count}, Lembranças={this.lembrancasConcluidas.Count}");

        if (data != null)
        {
            // Carrega um jogo existente
            this.listaDeDeducoes = data.listaDeDeducoes;
            this.lembrancasConcluidas = new HashSet<string>(data.lembrancasConcluidas);
            this.perfisDesbloqueadosNoDiario = data.perfisDesbloqueadosNoDiario;
            Debug.Log($"[PlayerState] Dados de save recebidos. Perfis a carregar: {data.perfisDesbloqueadosNoDiario.Count}, Lembranças: {data.lembrancasConcluidas.Count}");
        }
        else
        {
            // Reinicia para um novo jogo
            this.listaDeDeducoes.Clear();
            this.lembrancasConcluidas.Clear();
            this.perfisDesbloqueadosNoDiario.Clear();
            Debug.LogWarning("[PlayerState] Recebeu dados nulos. O estado foi REINICIADO para um novo jogo.");
        }

        // Log do estado DEPOIS da alteração
        Debug.Log($"[PlayerState] ESTADO DEPOIS DO LOAD: Perfis={this.perfisDesbloqueadosNoDiario.Count}, Lembranças={this.lembrancasConcluidas.Count}");
        Debug.Log($"--- [PlayerState] FIM DO PROCESSO DE LOAD ---");
    }
}