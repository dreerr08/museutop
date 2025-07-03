using UnityEngine;
using System.Collections; // NOVO: Necess�rio para usar Corrotinas
using System.Collections.Generic;

[RequireComponent(typeof(Collider))]
public class OutlineOnCollide : MonoBehaviour
{
    [Header("Configura��es do Efeito")]
    [Tooltip("O material que cont�m o shader de contorno com a propriedade '_FadeAlpha'.")]
    public Material outlineMaterial;

    [Tooltip("Dura��o do efeito de fade in e fade out em segundos.")]
    public float fadeDuration = 1.0f; // NOVO: Vari�vel para controlar a velocidade do fade

    // --- Vari�veis Internas ---
    private Renderer _renderer;
    private Material _materialInstance; // MUDAN�A: Usaremos uma inst�ncia do material para cada objeto
    private Coroutine _fadeCoroutine;   // NOVO: Para controlar a corrotina em andamento

    // Otimiza��o: Pegamos o ID da propriedade do shader uma vez para evitar usar strings repetidamente.
    private static readonly int FadeAlphaID = Shader.PropertyToID("_FadeAlpha");

    void Awake()
    {
        _renderer = GetComponent<Renderer>();
        if (_renderer == null)
        {
            Debug.LogError($"O objeto '{name}' n�o tem um componente Renderer!", this);
            enabled = false; // Desativa o script se n�o houver renderer
            return;
        }

        // MUDAN�A CR�TICA: N�o salvamos os materiais antigos para remov�-los depois.
        // Em vez disso, criamos uma inst�ncia �nica do nosso material de contorno
        // e a adicionamos permanentemente � lista de materiais do objeto.
        _materialInstance = new Material(outlineMaterial);

        var materials = new List<Material>();
        materials.AddRange(_renderer.sharedMaterials); // Pega os materiais originais
        materials.Add(_materialInstance);             // Adiciona nosso novo material de efeito
        _renderer.materials = materials.ToArray();

        // Garante que o efeito comece totalmente invis�vel.
        _materialInstance.SetFloat(FadeAlphaID, 0f);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // MUDAN�A: Em vez de chamar ApplyOutline, iniciamos a corrotina de fade.

            // Se j� houver um fade acontecendo, pare-o para evitar conflitos.
            if (_fadeCoroutine != null)
            {
                StopCoroutine(_fadeCoroutine);
            }
            // Inicia a corrotina para fazer o fade-in (ir para 100% de opacidade).
            _fadeCoroutine = StartCoroutine(FadeEffect(true));
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // MUDAN�A: Em vez de chamar RemoveOutline, iniciamos a corrotina de fade.

            // Se j� houver um fade acontecendo, pare-o.
            if (_fadeCoroutine != null)
            {
                StopCoroutine(_fadeCoroutine);
            }
            // Inicia a corrotina para fazer o fade-out (ir para 0% de opacidade).
            _fadeCoroutine = StartCoroutine(FadeEffect(false));
        }
    }

    // NOVO: A CORROTINA QUE FAZ A M�GICA DO FADE
    /// <summary>
    /// Corrotina que anima o valor de FadeAlpha ao longo do tempo.
    /// </summary>
    /// <param name="fadeIn">Se true, faz fade-in. Se false, faz fade-out.</param>
    private IEnumerator FadeEffect(bool fadeIn)
    {
        Debug.Log(fadeIn ? "Iniciando Fade-in..." : "Iniciando Fade-out...");

        float targetAlpha = fadeIn ? 1.0f : 0.0f;
        float startAlpha = _materialInstance.GetFloat(FadeAlphaID);

        float elapsedTime = 0f;

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            // Interpola suavemente do valor inicial para o final, baseado no tempo
            float currentAlpha = Mathf.Lerp(startAlpha, targetAlpha, elapsedTime / fadeDuration);

            // Define o valor da transpar�ncia no material a cada frame
            _materialInstance.SetFloat(FadeAlphaID, currentAlpha);

            // Espera at� o pr�ximo frame para continuar o loop
            yield return null;
        }

        // Ao final do loop, garante que o valor seja exatamente o alvo para corrigir imprecis�es.
        _materialInstance.SetFloat(FadeAlphaID, targetAlpha);
        Debug.Log("Fade finalizado.");
    }

    // OS M�TODOS ApplyOutline E RemoveOutline N�O S�O MAIS NECESS�RIOS E FORAM REMOVIDOS.
}