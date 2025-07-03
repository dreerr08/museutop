using UnityEngine;

public class LembrancaSystem : MonoBehaviour
{
    public string cenaLembranca = "Lembrança1"; // Nome da cena de lembrança
    private bool jogadorProximo = false;

    private void Update()
    {
        // Quando o jogador pressiona a tecla "E" na área de interação
        if (jogadorProximo && Input.GetKeyDown(KeyCode.E))
        {
            Debug.Log("Tecla E pressionada! Solicitando transição para a cena de lembrança...");

            // Pede ao gestor de cenas para lidar com a transição.
            GerenciadorDeCenas.Instancia.IrParaLembranca(cenaLembranca);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Jogador entrou na área de interação. Pressione 'E' para ativar a lembrança.");
            jogadorProximo = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Jogador saiu da área de interação.");
            jogadorProximo = false;
        }
    }
}
