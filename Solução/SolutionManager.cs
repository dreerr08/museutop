using UnityEngine;
using System.Collections.Generic;
using System.Linq; // Essencial para usar fun��es como .FirstOrDefault() e .Select()

/// <summary>
/// Gerenciador Singleton que det�m a "Matriz de Solu��es" - a verdade absoluta do jogo.
/// Ele armazena as informa��es corretas para cada personagem e fornece m�todos
/// para que outros sistemas possam consultar esses dados de forma segura.
/// </summary>
public class SolutionManager : MonoBehaviour
{
    #region Singleton Pattern

    // A propriedade est�tica 'Instance' � o cora��o do padr�o Singleton.
    // 'public get' permite que qualquer script leia esta inst�ncia.
    // 'private set' garante que apenas este script possa definir quem � a inst�ncia.
    public static SolutionManager Instance { get; private set; }

    // O m�todo Awake() � chamado pela Unity antes de qualquer m�todo Start().
    // � o lugar ideal para configurar o Singleton.
    private void Awake()
    {
        // VERIFICA��O 1: J� existe uma inst�ncia?
        if (Instance != null && Instance != this)
        {
            // Se sim, e n�o for esta, significa que outra inst�ncia foi criada por engano.
            // Devemos destruir este objeto duplicado para evitar conflitos.
            Debug.LogWarning("J� existe um SolutionManager na cena. Destruindo a nova inst�ncia.");
            Destroy(this.gameObject);
        }
        else
        {
            // VERIFICA��O 2: A inst�ncia ainda n�o foi definida.
            // Se n�o, esta se torna a inst�ncia �nica.
            Instance = this;

            // (Opcional, mas recomendado) Mant�m este objeto vivo ao carregar novas cenas.
            // Essencial se o seu jogo tiver m�ltiplas cenas (ex: menu, jogo, cr�ditos).
            DontDestroyOnLoad(this.gameObject);
        }
    }

    #endregion

    #region Solution Data

    // Usamos [SerializeField] em um campo privado.
    // Isso exp�e a vari�vel ao Inspector da Unity para que possamos preench�-la
    // arrastando e soltando, mas impede que outros scripts a modifiquem diretamente,
    // for�ando-os a usar os m�todos p�blicos que criamos (boa pr�tica de encapsulamento).
    [SerializeField]
    private List<SolucaoPersonagem> matrizDeSolucoes;

    #endregion

    #region Public Access Methods

    /// <summary>
    /// Busca e retorna a solu��o completa para um personagem espec�fico pelo seu ID.
    /// Este � o m�todo de consulta mais importante.
    /// </summary>
    /// <param name="id">O ID do personagem a ser procurado.</param>
    /// <returns>O objeto SolucaoPersonagem se encontrado; caso contr�rio, retorna null.</returns>
    public SolucaoPersonagem GetSolucaoPorId(int id)
    {
        // .FirstOrDefault() � um m�todo do LINQ que � perfeito para isso.
        // Ele varre a lista e retorna o primeiro elemento que corresponde � condi��o.
        // Se nenhum elemento for encontrado, ele retorna o valor padr�o (null para classes).
        return matrizDeSolucoes.FirstOrDefault(p => p.id == id);
    }

    /// <summary>
    /// Retorna uma lista com todas as solu��es de personagens.
    /// </summary>
    /// <returns>A lista completa de solu��es.</returns>
    public List<SolucaoPersonagem> GetTodasAsSolucoes()
    {
        return matrizDeSolucoes;
    }

    /// <summary>
    /// Retorna uma lista contendo apenas os nomes de todos os personagens.
    /// Extremamente �til para popular a UI do di�rio (menus suspensos de nomes).
    /// </summary>
    /// <returns>Uma lista de strings com os nomes completos.</returns>
    public List<string> GetTodosOsNomes()
    {
        // .Select() � um m�todo do LINQ que projeta cada elemento de uma sequ�ncia em um novo formato.
        // Aqui, estamos transformando uma List<SolucaoPersonagem> em uma List<string>.
        return matrizDeSolucoes.Select(p => p.nomeCompleto).ToList();
    }

    /// <summary>
    /// Retorna o n�mero total de personagens no jogo.
    /// </summary>
    public int GetTotalDePersonagens()
    {
        return matrizDeSolucoes.Count;
    }

    #endregion
}