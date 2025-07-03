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
            // O BLOCO DE VERIFICA��O FOI REMOVIDO DAQUI.
            // Agora, o sistema sempre tentar� ativar a lembran�a ao pressionar 'E'.

            if (string.IsNullOrEmpty(idDaLembranca))
            {
                Debug.LogError("[LembrancaSystem] ERRO: 'Id Da Lembran�a' n�o est� definido no Inspector para este objeto!");
                return;
            }

            Debug.Log($"[LembrancaSystem] Jogador ativou a lembran�a com ID: '{idDaLembranca}'.");
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