using UnityEngine;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// Gerenciador Singleton que detém a "Matriz de Soluções" - a verdade absoluta do jogo.
/// </summary>
public class SolutionManager : MonoBehaviour
{
    public static SolutionManager Instance { get; private set; }

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

    [SerializeField]
    private List<SolucaoPersonagem> matrizDeSolucoes;

    public SolucaoPersonagem GetSolucaoPorId(int id)
    {
        return matrizDeSolucoes.FirstOrDefault(p => p.id == id);
    }

    public List<SolucaoPersonagem> GetTodasAsSolucoes()
    {
        return matrizDeSolucoes;
    }

    public List<string> GetTodosOsNomes()
    {
        return matrizDeSolucoes.Select(p => p.nomeCompleto).ToList();
    }

    /// <summary>
    /// (NOVO) Retorna uma lista contendo os Sprites de retrato de todos os personagens.
    /// Útil para popular o seletor de imagens da UI.
    /// </summary>
    /// <returns>Uma lista de Sprites de retrato.</returns>
    public List<Sprite> GetTodosOsRetratos()
    {
        return matrizDeSolucoes.Select(p => p.retrato).ToList();
    }

    public int GetTotalDePersonagens()
    {
        return matrizDeSolucoes.Count;
    }
}