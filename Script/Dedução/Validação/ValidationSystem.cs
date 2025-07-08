// Arquivo: dreerr08/museutop/museutop-6ea31f3c45b1c0f813e03be5a1425dc73cd4b2a0/Script/Dedu��o/Valida��o/ValidationSystem.cs

using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// Sistema Singleton respons�vel por validar as dedu��es do jogador.
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
            Debug.LogError("SolutionManager ou PlayerState n�o est�o prontos!");
            return;
        }

        List<DeducaoJogador> acertosNestaRodada = new List<DeducaoJogador>();

        foreach (var deducao in deducoes)
        {
            if (deducao.estaConfirmado) continue;

            SolucaoPersonagem solucao = SolutionManager.Instance.GetSolucaoPorId(deducao.idPersonagem);
            if (solucao == null) continue;

            // MUDAN�A NA VALIDA��O:
            bool nomeCorreto = deducao.nomeEscolhido == solucao.nomeCompleto;
            bool papelCorreto = deducao.papelEscolhido != PapelNoRoubo.Desconhecido && deducao.papelEscolhido == solucao.papel;
            bool destinoCorreto = deducao.destinoEscolhido != DestinoFinal.Desconhecido && deducao.destinoEscolhido == solucao.destino;
            bool assassinoCorreto = deducao.idAssassinoEscolhido == solucao.idAssassino; // NOVA CONDI��O

            // A valida��o agora inclui o assassino
            if (nomeCorreto && papelCorreto && destinoCorreto && assassinoCorreto)
            {
                acertosNestaRodada.Add(deducao);
            }
        }

        if (acertosNestaRodada.Count >= 3)
        {
            Debug.Log($"SUCESSO: {acertosNestaRodada.Count} dedu��es corretas encontradas! Confirmando...");

            foreach (var acerto in acertosNestaRodada)
            {
                acerto.estaConfirmado = true;
            }

            acertosConfirmadosAtualmente += acertosNestaRodada.Count;

            OnDeductionsConfirmed?.Invoke(acertosNestaRodada);

            if (acertosConfirmadosAtualmente == solucoes.Count)
            {
                Debug.Log("FIM DE JOGO: Todas as dedu��es foram resolvidas!");
                OnAllDeductionsCorrect?.Invoke();
            }
        }
    }
}