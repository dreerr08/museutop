using UnityEngine;

public class LembrancaSystem : MonoBehaviour
{
    [Header("Configuração da Lembrança")]
    [Tooltip("O ID único desta lembrança. Deve corresponder a uma entrada no BancoDeDesbloqueios.")]
    public string idDaLembranca;

    [Tooltip("O nome da cena de lembrança para carregar.")]
    public string cenaLembranca = "Lembrança1";

    private bool jogadorProximo = false;

    private void Update()
    {
        if (jogadorProximo && Input.GetKeyDown(KeyCode.E))
        {
            Debug.Log($"[LembrancaSystem] Jogador pressionou 'E' para a lembrança com ID: '{idDaLembranca}'.");

            if (string.IsNullOrEmpty(idDaLembranca))
            {
                Debug.LogError("[LembrancaSystem] ERRO: 'Id Da Lembrança' não está definido no Inspector para este objeto!");
                return;
            }

            if (PlayerState.Instance.JaConcluiuLembranca(idDaLembranca))
            {
                Debug.LogWarning($"[LembrancaSystem] AVISO: A lembrança '{idDaLembranca}' já foi concluída. Nenhuma ação será tomada.");
                return;
            }

            Debug.Log($"[LembrancaSystem] Solicitando ao GerenciadorDeCenas para ir para a lembrança '{idDaLembranca}'.");
            GerenciadorDeCenas.Instancia.IrParaLembranca(cenaLembranca, idDaLembranca);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("[LembrancaSystem] Jogador entrou na área de interação.");
            jogadorProximo = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("[LembrancaSystem] Jogador saiu da área de interação.");
            jogadorProximo = false;
        }
    }
}