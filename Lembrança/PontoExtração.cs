using UnityEngine;

public class PontoExtração : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Jogador entrou na área de extração. Solicitando retorno...");

            // Pede ao gestor de cenas para lidar com o retorno.
            GerenciadorDeCenas.Instancia.RetornarDaLembranca();
        }
    }
}
