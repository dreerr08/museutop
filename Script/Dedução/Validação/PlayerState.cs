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
        Debug.Log($"[PlayerState] Tentando concluir a lembrança '{idLembranca}' com {idsParaDesbloquear.Count} IDs para desbloquear.");

        if (lembrancasConcluidas.Contains(idLembranca))
        {
            Debug.LogWarning("[PlayerState] AVISO: Tentativa de concluir uma lembrança que já estava na lista de concluídas.");
            return;
        }

        lembrancasConcluidas.Add(idLembranca);
        Debug.Log($"[PlayerState] Lembrança '{idLembranca}' adicionada à lista de concluídas.");

        foreach (var id in idsParaDesbloquear)
        {
            if (!perfisDesbloqueadosNoDiario.Contains(id))
            {
                perfisDesbloqueadosNoDiario.Add(id);
                listaDeDeducoes.Add(new DeducaoJogador(id));
                Debug.Log($"[PlayerState] SUCESSO: Perfil de personagem com ID {id} foi adicionado à lista de desbloqueados e uma nova DeducaoJogador foi criada.");
            }
            else
            {
                Debug.Log($"[PlayerState] INFO: O perfil com ID {id} já estava desbloqueado. Nenhuma nova dedução foi criada para ele.");
            }
        }
    }

    public List<int> GetPerfisDesbloqueados()
    {
        Debug.Log($"[PlayerState] GetPerfisDesbloqueados foi chamado. A retornar uma lista com {perfisDesbloqueadosNoDiario.Count} perfis.");
        return perfisDesbloqueadosNoDiario;
    }
}