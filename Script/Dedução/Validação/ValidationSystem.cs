using UnityEngine;
using System; // Necessário para usar 'Action'
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// Sistema Singleton responsável por validar as deduções do jogador.
/// Compara os dados do PlayerState com o SolutionManager e dispara eventos
/// quando um conjunto de deduções corretas é encontrado.
/// </summary>
public class ValidationSystem : MonoBehaviour
{
    public static ValidationSystem Instance { get; private set; }

    // Eventos são como "anúncios" que outros scripts podem "ouvir".
    // Isso desacopla o sistema: a validação não precisa conhecer a UI ou o áudio,
    // ela apenas anuncia o resultado.

    /// <summary>
    /// Disparado quando um conjunto de 3 (ou mais) deduções é confirmado.
    /// Envia a lista das deduções que acabaram de ser confirmadas.
    /// </summary>
    public static event Action<List<DeducaoJogador>> OnDeductionsConfirmed;

    /// <summary>
    /// Disparado quando TODAS as deduções do jogo foram corretamente elucidadas.
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
    /// O método central do sistema. Deve ser chamado sempre que o jogador fizer uma alteração no diário.
    /// </summary>
    public void ValidarTodasAsDeducoes()
    {
        var solucoes = SolutionManager.Instance.GetTodasAsSolucoes();
        var deducoes = PlayerState.Instance.GetTodasAsDeducoes();

        if (solucoes == null || deducoes == null)
        {
            Debug.LogError("SolutionManager ou PlayerState não estão prontos!");
            return;
        }

        // Lista para guardar os acertos desta rodada de verificação
        List<DeducaoJogador> acertosNestaRodada = new List<DeducaoJogador>();

        // 1. Percorrer todas as deduções do jogador
        foreach (var deducao in deducoes)
        {
            // Pula as que já foram confirmadas em rodadas anteriores
            if (deducao.estaConfirmado) continue;

            // Pega a solução correspondente para comparar
            SolucaoPersonagem solucao = SolutionManager.Instance.GetSolucaoPorId(deducao.idPersonagem);
            if (solucao == null) continue; // Segurança caso haja IDs inconsistentes

            // 2. Compara todos os 4 campos.
            // A dedução precisa estar completa para ser considerada.
            bool nomeCorreto = deducao.nomeEscolhido == solucao.nomeCompleto;
            bool fotoCorreta = deducao.fotoEscolhida == solucao.idDaFoto;
            bool papelCorreto = deducao.papelEscolhido.HasValue && deducao.papelEscolhido.Value == solucao.papel;
            bool destinoCorreto = deducao.destinoEscolhido.HasValue && deducao.destinoEscolhido.Value == solucao.destino;

            // Se tudo estiver 100% correto, adiciona à lista de possíveis acertos.
            if (nomeCorreto && fotoCorreta && papelCorreto && destinoCorreto)
            {
                acertosNestaRodada.Add(deducao);
            }
        }

        // 3. Aplica a "Regra dos Três"
        if (acertosNestaRodada.Count >= 3)
        {
            Debug.Log($"SUCESSO: {acertosNestaRodada.Count} deduções corretas encontradas! Confirmando...");

            // Marca cada dedução correta como "confirmada"
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
                Debug.Log("FIM DE JOGO: Todas as deduções foram resolvidas!");
                OnAllDeductionsCorrect?.Invoke();
            }
        }
    }
}