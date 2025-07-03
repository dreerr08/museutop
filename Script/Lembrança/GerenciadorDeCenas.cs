// GerenciadorDeCenas.cs
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class GerenciadorDeCenas : MonoBehaviour
{
    // Singleton para garantir que apenas uma instância deste gestor exista.
    public static GerenciadorDeCenas Instancia { get; private set; }

    // A referência à imagem de fade agora é privada e gerida internamente.
    private Image fadeImage;
    public float fadeSpeed = 1f;
    public Color fadeColor = Color.black; // Permite customizar a cor do fade no Inspector.

    // Variáveis para guardar o estado entre as cenas.
    private string cenaDeRetorno;
    private Vector3 posicaoDeRetorno;
    private float rotacaoXCameraDeRetorno;
    private float rotacaoYJogadorDeRetorno;

    private void Awake()
    {
        // Implementação do padrão Singleton.
        if (Instancia == null)
        {
            Instancia = this;
            DontDestroyOnLoad(gameObject); // Torna este objeto persistente entre as cenas.
            SceneManager.sceneLoaded += OnSceneLoaded; // Subscreve ao evento de cena carregada.

            // Cria o Canvas e a Imagem de fade dinamicamente para que também sejam persistentes.
            SetupFadeCanvas();
        }
        else
        {
            Destroy(gameObject); // Destrói duplicatas.
        }
    }

    /// <summary>
    /// Inicia a transição para uma cena de lembrança, guardando os dados de retorno.
    /// </summary>
    public void IrParaLembranca(string nomeCenaLembranca)
    {
        this.cenaDeRetorno = SceneManager.GetActiveScene().name;
        GameObject player = GameObject.FindWithTag("Player");
        if (player != null)
        {
            this.posicaoDeRetorno = player.transform.position;
            this.rotacaoYJogadorDeRetorno = player.transform.eulerAngles.y;

            // **ALTERAÇÃO PRINCIPAL AQUI: Pegar o pitch diretamente do controller**
            FirstPersonController fpController = player.GetComponent<FirstPersonController>();
            if (fpController != null)
            {
                this.rotacaoXCameraDeRetorno = fpController.GetCurrentPitch();
            }
            else
            {
                // Fallback caso não encontre o controller. O ideal é que ele sempre exista.
                this.rotacaoXCameraDeRetorno = Camera.main.transform.localEulerAngles.x;
                Debug.LogWarning("FirstPersonController não encontrado para salvar o pitch. Usando eulerAngles da câmera como fallback.");
            }
        }

        Debug.Log($"Indo para a lembrança: {nomeCenaLembranca}. Posição: {posicaoDeRetorno}. Rotação Jogador Y: {rotacaoYJogadorDeRetorno}, Rotação Câmera X: {rotacaoXCameraDeRetorno}");
        StartCoroutine(FadeAndLoadScene(nomeCenaLembranca));
    }

    /// <summary>
    /// Inicia a transição de volta para a cena original.
    /// </summary>
    public void RetornarDaLembranca()
    {
        if (!string.IsNullOrEmpty(cenaDeRetorno))
        {
            Debug.Log($"Retornando para a cena: {cenaDeRetorno}");
            StartCoroutine(FadeAndLoadScene(cenaDeRetorno));
        }
        else
        {
            Debug.LogError("Nome da cena de retorno está vazio! Não é possível retornar.");
        }
    }

    // Corrotina que lida com o fade e o carregamento da cena.
    private IEnumerator FadeAndLoadScene(string sceneName)
    {
        yield return StartCoroutine(FadeOut());
        SceneManager.LoadScene(sceneName);
    }

    // **ALTERAÇÃO**
    // Este método agora é mais simples e delega a lógica de restauração.
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log($"Cena '{scene.name}' carregada.");

        // Se a cena carregada for a cena para a qual devíamos retornar...
        if (scene.name == cenaDeRetorno)
        {
            // Inicia uma corrotina para restaurar o estado.
            // Isso evita a "race condition" com o primeiro Update() do FirstPersonController.
            StartCoroutine(RestaurarEstadoDoJogador());
        }

        // Inicia o fade in na nova cena, independentemente de qual seja.
        StartCoroutine(FadeIn());
    }

    // **NOVA CORROTINA**
    /// <summary>
    /// Corrotina que restaura a posição e rotação do jogador APÓS o primeiro frame ter sido renderizado.
    /// </summary>
    private IEnumerator RestaurarEstadoDoJogador()
    {
        // A MUDANÇA CRÍTICA ESTÁ AQUI: Espera até o fim do frame atual.
        // Isso garante que o Update() do FirstPersonController já rodou uma vez,
        // limpando qualquer input residual de mouse.
        yield return new WaitForEndOfFrame();

        GameObject player = GameObject.FindWithTag("Player");
        if (player != null)
        {
            CharacterController charController = player.GetComponent<CharacterController>();
            if (charController != null) charController.enabled = false;

            player.transform.position = posicaoDeRetorno;

            if (charController != null) charController.enabled = true;

            Debug.Log($"Posição do jogador restaurada para: {posicaoDeRetorno}");

            FirstPersonController fpController = player.GetComponent<FirstPersonController>();
            if (fpController != null)
            {
                fpController.InicializarRotacao(rotacaoYJogadorDeRetorno, rotacaoXCameraDeRetorno);
                Debug.Log($"Rotação inicializada via FirstPersonController. RotaçãoY: {rotacaoYJogadorDeRetorno}, RotaçãoX: {rotacaoXCameraDeRetorno}");
            }
            else
            {
                Debug.LogError("FirstPersonController não encontrado no jogador para inicializar a rotação!");
            }
        }
        else
        {
            Debug.LogError("Jogador não encontrado na cena de retorno para restaurar a posição!");
        }
    }

    // Corrotinas de Fade (In e Out)
    private IEnumerator FadeIn()
    {
        fadeImage.gameObject.SetActive(true);
        float elapsedTime = 0f;
        Color c = fadeColor;

        while (elapsedTime < fadeSpeed)
        {
            elapsedTime += Time.deltaTime;
            c.a = Mathf.Lerp(1, 0, elapsedTime / fadeSpeed);
            fadeImage.color = c;
            yield return null;
        }
        fadeImage.gameObject.SetActive(false);
    }

    private IEnumerator FadeOut()
    {
        fadeImage.gameObject.SetActive(true);
        float elapsedTime = 0f;
        Color c = fadeColor;

        while (elapsedTime < fadeSpeed)
        {
            elapsedTime += Time.deltaTime;
            c.a = Mathf.Lerp(0, 1, elapsedTime / fadeSpeed);
            fadeImage.color = c;
            yield return null;
        }
    }

    // Método para criar o Canvas e a Imagem de fade.
    private void SetupFadeCanvas()
    {
        GameObject canvasGO = new GameObject("FadeCanvas");
        canvasGO.transform.SetParent(this.transform);
        Canvas canvas = canvasGO.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 100;
        canvasGO.AddComponent<CanvasScaler>();
        canvasGO.AddComponent<GraphicRaycaster>();
        GameObject imageGO = new GameObject("FadeImage");
        imageGO.transform.SetParent(canvasGO.transform);
        fadeImage = imageGO.AddComponent<Image>();
        fadeImage.color = new Color(fadeColor.r, fadeColor.g, fadeColor.b, 0);
        RectTransform rectTransform = imageGO.GetComponent<RectTransform>();
        rectTransform.anchorMin = new Vector2(0, 0);
        rectTransform.anchorMax = new Vector2(1, 1);
        rectTransform.pivot = new Vector2(0.5f, 0.5f);
        rectTransform.sizeDelta = Vector2.zero;
        rectTransform.anchoredPosition = Vector2.zero;
        imageGO.SetActive(false);
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}