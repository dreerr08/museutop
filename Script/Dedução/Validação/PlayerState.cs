using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class PlayerState : MonoBehaviour
{
    public static PlayerState Instance { get; private set; }
    private List<DeducaoJogador> listaDeDeducoes;
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
            listaDeDeducoes = new List<DeducaoJogador>();
        }
    }

    public List<DeducaoJogador> GetTodasAsDeducoes() { return listaDeDeducoes; }
    public DeducaoJogador GetDeducaoPorId(int personagemId) { return listaDeDeducoes.FirstOrDefault(d => d.idPersonagem == personagemId); }
    public bool JaConcluiuLembranca(string idLembranca) { return lembrancasConcluidas.Contains(idLembranca); }

    public void ConcluirLembranca(string idLembranca, List<int> idsParaDesbloquear)
    {
        Debug.Log($"[PlayerState] Tentando concluir a lembran�a '{idLembranca}' com {idsParaDesbloquear.Count} IDs para desbloquear.");

        if (lembrancasConcluidas.Contains(idLembranca))
        {
            Debug.LogWarning("[PlayerState] AVISO: Tentativa de concluir uma lembran�a que j� estava na lista de conclu�das.");
            return;
        }

        lembrancasConcluidas.Add(idLembranca);
        Debug.Log($"[PlayerState] Lembran�a '{idLembranca}' adicionada � lista de conclu�das.");

        foreach (var id in idsParaDesbloquear)
        {
            if (!perfisDesbloqueadosNoDiario.Contains(id))
            {
                perfisDesbloqueadosNoDiario.Add(id);
                listaDeDeducoes.Add(new DeducaoJogador(id));
                Debug.Log($"[PlayerState] SUCESSO: Perfil de personagem com ID {id} foi adicionado � lista de desbloqueados e uma nova DeducaoJogador foi criada.");
            }
            else
            {
                Debug.Log($"[PlayerState] INFO: O perfil com ID {id} j� estava desbloqueado. Nenhuma nova dedu��o foi criada para ele.");
            }
        }
    }

    public List<int> GetPerfisDesbloqueados()
    {
        Debug.Log($"[PlayerState] GetPerfisDesbloqueados foi chamado. A retornar uma lista com {perfisDesbloqueadosNoDiario.Count} perfis.");
        return perfisDesbloqueadosNoDiario;
    }
}