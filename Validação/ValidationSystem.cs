using UnityEngine;
using System; // Necess�rio para usar 'Action'
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// Sistema Singleton respons�vel por validar as dedu��es do jogador.
/// Compara os dados do PlayerState com o SolutionManager e dispara eventos
/// quando um conjunto de dedu��es corretas � encontrado.
/// </summary>
public class ValidationSystem : MonoBehaviour
{
    public static ValidationSystem Instance { get; private set; }

    // Eventos s�o como "an�ncios" que outros scripts podem "ouvir".
    // Isso desacopla o sistema: a valida��o n�o precisa conhecer a UI ou o �udio,
    // ela apenas anuncia o resultado.

    /// <summary>
    /// Disparado quando um conjunto de 3 (ou mais) dedu��es � confirmado.
    /// Envia a lista das dedu��es que acabaram de ser confirmadas.
    /// </summary>
    public static event Action<List<DeducaoJogador>> OnDeductionsConfirmed;

    /// <summary>
    /// Disparado quando TODAS as dedu��es do jogo foram corretamente elucidadas.
    /// </summary>
    public static event Action OnAllDeductionsCorrect;

    private int acertosConfirmadosAtualmente = 0;

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

    /// <summary>
    /// O m�todo central do sistema. Deve ser chamado sempre que o jogador fizer uma altera��o no di�rio.
    /// </summary>
    public void ValidarTodasAsDeducoes()
    {
        var solucoes = SolutionManager.Instance.GetTodasAsSolucoes();
        var deducoes = PlayerState.Instance.GetTodasAsDeducoes();

        if (solucoes == null || deducoes == null)
        {
            Debug.LogError("SolutionManager ou PlayerState n�o est�o prontos!");
            return;
        }

        // Lista para guardar os acertos desta rodada de verifica��o
        List<DeducaoJogador> acertosNestaRodada = new List<DeducaoJogador>();

        // 1. Percorrer todas as dedu��es do jogador
        foreach (var deducao in deducoes)
        {
            // Pula as que j� foram confirmadas em rodadas anteriores
            if (deducao.estaConfirmado) continue;

            // Pega a solu��o correspondente para comparar
            SolucaoPersonagem solucao = SolutionManager.Instance.GetSolucaoPorId(deducao.idPersonagem);
            if (solucao == null) continue; // Seguran�a caso haja IDs inconsistentes

            // 2. Compara todos os 4 campos.
            // A dedu��o precisa estar completa para ser considerada.
            bool nomeCorreto = deducao.nomeEscolhido == solucao.nomeCompleto;
            bool fotoCorreta = deducao.fotoEscolhida == solucao.idDaFoto;
            bool papelCorreto = deducao.papelEscolhido.HasValue && deducao.papelEscolhido.Value == solucao.papel;
            bool destinoCorreto = deducao.destinoEscolhido.HasValue && deducao.destinoEscolhido.Value == solucao.destino;

            // Se tudo estiver 100% correto, adiciona � lista de poss�veis acertos.
            if (nomeCorreto && fotoCorreta && papelCorreto && destinoCorreto)
            {
                acertosNestaRodada.Add(deducao);
            }
        }

        // 3. Aplica a "Regra dos Tr�s"
        if (acertosNestaRodada.Count >= 3)
        {
            Debug.Log($"SUCESSO: {acertosNestaRodada.Count} dedu��es corretas encontradas! Confirmando...");

            // Marca cada dedu��o correta como "confirmada"
            foreach (var acerto in acertosNestaRodada)
            {
                acerto.estaConfirmado = true;
            }

            acertosConfirmadosAtualmente += acertosNestaRodada.Count;

            // 4. Anuncia o sucesso para quem estiver ouvindo!
            OnDeductionsConfirmed?.Invoke(acertosNestaRodada);

            // 5. Verifica se o jogo terminou
            if (acertosConfirmadosAtualmente == solucoes.Count)
            {
                Debug.Log("FIM DE JOGO: Todas as dedu��es foram resolvidas!");
                OnAllDeductionsCorrect?.Invoke();
            }
        }
    }
}