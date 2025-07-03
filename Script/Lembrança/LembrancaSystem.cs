using UnityEngine;

public class LembrancaSystem : MonoBehaviour
{
    [Header("Configura��o da Lembran�a")]
    [Tooltip("O ID �nico desta lembran�a. Deve corresponder a uma entrada no BancoDeDesbloqueios.")]
    public string idDaLembranca;

    [Tooltip("O nome da cena de lembran�a para carregar.")]
    public string cenaLembranca = "Lembran�a1";

    private bool jogadorProximo = false;

    private void Update()
    {
        if (jogadorProximo && Input.GetKeyDown(KeyCode.E))
        {
            Debug.Log($"[LembrancaSystem] Jogador pressionou 'E' para a lembran�a com ID: '{idDaLembranca}'.");

            if (string.IsNullOrEmpty(idDaLembranca))
            {
                Debug.LogError("[LembrancaSystem] ERRO: 'Id Da Lembran�a' n�o est� definido no Inspector para este objeto!");
                return;
            }

            if (PlayerState.Instance.JaConcluiuLembranca(idDaLembranca))
            {
                Debug.LogWarning($"[LembrancaSystem] AVISO: A lembran�a '{idDaLembranca}' j� foi conclu�da. Nenhuma a��o ser� tomada.");
                return;
            }

            Debug.Log($"[LembrancaSystem] Solicitando ao GerenciadorDeCenas para ir para a lembran�a '{idDaLembranca}'.");
            GerenciadorDeCenas.Instancia.IrParaLembranca(cenaLembranca, idDaLembranca);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("[LembrancaSystem] Jogador entrou na �rea de intera��o.");
            jogadorProximo = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("[LembrancaSystem] Jogador saiu da �rea de intera��o.");
            jogadorProximo = false;
        }
    }
}