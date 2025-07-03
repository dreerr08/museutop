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
            // O BLOCO DE VERIFICAÇÃO FOI REMOVIDO DAQUI.
            // Agora, o sistema sempre tentará ativar a lembrança ao pressionar 'E'.

            if (string.IsNullOrEmpty(idDaLembranca))
            {
                Debug.LogError("[LembrancaSystem] ERRO: 'Id Da Lembrança' não está definido no Inspector para este objeto!");
                return;
            }

            Debug.Log($"[LembrancaSystem] Jogador ativou a lembrança com ID: '{idDaLembranca}'.");
            GerenciadorDeCenas.Instancia.IrParaLembranca(cenaLembranca, idDaLembranca);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            jogadorProximo = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            jogadorProximo = false;
        }
    }
}