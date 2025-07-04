using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuPrincipalUI : MonoBehaviour
{
    public string nomeCenaPrincipal = "Museu";
    public string nomeCenaDeSaves = "TelaDeSaves";

    public void OnNovoJogoClick()
    {
        bool slotEncontrado = SaveLoadManager.Instance.PrepararNovoJogo(-1);
        if (slotEncontrado)
        {
            SceneManager.LoadScene(nomeCenaPrincipal);
        }
        else
        {
            SceneManager.LoadScene(nomeCenaDeSaves);
        }
    }

    public void OnContinuarClick()
    {
        SceneManager.LoadScene(nomeCenaDeSaves);
    }

    public void OnSairClick()
    {
        Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}