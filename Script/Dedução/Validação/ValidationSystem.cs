using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// Sistema Singleton responsável por validar as deduções do jogador.
/// </summary>
public class ValidationSystem : MonoBehaviour
{
    public static ValidationSystem Instance { get; private set; }
    public static event Action<List<DeducaoJogador>> OnDeductionsConfirmed;
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

    public void ValidarTodasAsDeducoes()
    {
        var solucoes = SolutionManager.Instance.GetTodasAsSolucoes();
        var deducoes = PlayerState.Instance.GetTodasAsDeducoes();

        if (solucoes == null || deducoes == null)
        {
            Debug.LogError("SolutionManager ou PlayerState não estão prontos!");
            return;
        }

        List<DeducaoJogador> acertosNestaRodada = new List<DeducaoJogador>();

        foreach (var deducao in deducoes)
        {
            if (deducao.estaConfirmado) continue;

            SolucaoPersonagem solucao = SolutionManager.Instance.GetSolucaoPorId(deducao.idPersonagem);
            if (solucao == null) continue;

            // MUDANÇA NA VALIDAÇÃO:
            // Agora, verificamos se o valor escolhido NÃO É "Desconhecido" antes de comparar.
            bool nomeCorreto = deducao.nomeEscolhido == solucao.nomeCompleto;
            bool papelCorreto = deducao.papelEscolhido != PapelNoRoubo.Desconhecido && deducao.papelEscolhido == solucao.papel;
            bool destinoCorreto = deducao.destinoEscolhido != DestinoFinal.Desconhecido && deducao.destinoEscolhido == solucao.destino;

            if (nomeCorreto && papelCorreto && destinoCorreto)
            {
                acertosNestaRodada.Add(deducao);
            }
        }

        if (acertosNestaRodada.Count >= 3)
        {
            Debug.Log($"SUCESSO: {acertosNestaRodada.Count} deduções corretas encontradas! Confirmando...");

            foreach (var acerto in acertosNestaRodada)
            {
                acerto.estaConfirmado = true;
            }

            acertosConfirmadosAtualmente += acertosNestaRodada.Count;

            OnDeductionsConfirmed?.Invoke(acertosNestaRodada);

            if (acertosConfirmadosAtualmente == solucoes.Count)
            {
                Debug.Log("FIM DE JOGO: Todas as deduções foram resolvidas!");
                OnAllDeductionsCorrect?.Invoke();
            }
        }
    }
}