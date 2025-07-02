using UnityEngine;

public class LembrancaSystem : MonoBehaviour
{
    public string cenaLembranca = "Lembran�a1"; // Nome da cena de lembran�a
    private bool jogadorProximo = false;

    private void Update()
    {
        // Quando o jogador pressiona a tecla "E" na �rea de intera��o
        if (jogadorProximo && Input.GetKeyDown(KeyCode.E))
        {
            Debug.Log("Tecla E pressionada! Solicitando transi��o para a cena de lembran�a...");

            // Pede ao gestor de cenas para lidar com a transi��o.
            GerenciadorDeCenas.Instancia.IrParaLembranca(cenaLembranca);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Jogador entrou na �rea de intera��o. Pressione 'E' para ativar a lembran�a.");
            jogadorProximo = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Jogador saiu da �rea de intera��o.");
            jogadorProximo = false;
        }
    }
}
