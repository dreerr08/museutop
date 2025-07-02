using UnityEngine;
using System.Collections; // NOVO: Necessário para usar Corrotinas
using System.Collections.Generic;

[RequireComponent(typeof(Collider))]
public class OutlineOnCollide : MonoBehaviour
{
    [Header("Configurações do Efeito")]
    [Tooltip("O material que contém o shader de contorno com a propriedade '_FadeAlpha'.")]
    public Material outlineMaterial;

    [Tooltip("Duração do efeito de fade in e fade out em segundos.")]
    public float fadeDuration = 1.0f; // NOVO: Variável para controlar a velocidade do fade

    // --- Variáveis Internas ---
    private Renderer _renderer;
    private Material _materialInstance; // MUDANÇA: Usaremos uma instância do material para cada objeto
    private Coroutine _fadeCoroutine;   // NOVO: Para controlar a corrotina em andamento

    // Otimização: Pegamos o ID da propriedade do shader uma vez para evitar usar strings repetidamente.
    private static readonly int FadeAlphaID = Shader.PropertyToID("_FadeAlpha");

    void Awake()
    {
        _renderer = GetComponent<Renderer>();
        if (_renderer == null)
        {
            Debug.LogError($"O objeto '{name}' não tem um componente Renderer!", this);
            enabled = false; // Desativa o script se não houver renderer
            return;
        }

        // MUDANÇA CRÍTICA: Não salvamos os materiais antigos para removê-los depois.
        // Em vez disso, criamos uma instância única do nosso material de contorno
        // e a adicionamos permanentemente à lista de materiais do objeto.
        _materialInstance = new Material(outlineMaterial);

        var materials = new List<Material>();
        materials.AddRange(_renderer.sharedMaterials); // Pega os materiais originais
        materials.Add(_materialInstance);             // Adiciona nosso novo material de efeito
        _renderer.materials = materials.ToArray();

        // Garante que o efeito comece totalmente invisível.
        _materialInstance.SetFloat(FadeAlphaID, 0f);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // MUDANÇA: Em vez de chamar ApplyOutline, iniciamos a corrotina de fade.

            // Se já houver um fade acontecendo, pare-o para evitar conflitos.
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
            // MUDANÇA: Em vez de chamar RemoveOutline, iniciamos a corrotina de fade.

            // Se já houver um fade acontecendo, pare-o.
            if (_fadeCoroutine != null)
            {
                StopCoroutine(_fadeCoroutine);
            }
            // Inicia a corrotina para fazer o fade-out (ir para 0% de opacidade).
            _fadeCoroutine = StartCoroutine(FadeEffect(false));
        }
    }

    // NOVO: A CORROTINA QUE FAZ A MÁGICA DO FADE
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

            // Define o valor da transparência no material a cada frame
            _materialInstance.SetFloat(FadeAlphaID, currentAlpha);

            // Espera até o próximo frame para continuar o loop
            yield return null;
        }

        // Ao final do loop, garante que o valor seja exatamente o alvo para corrigir imprecisões.
        _materialInstance.SetFloat(FadeAlphaID, targetAlpha);
        Debug.Log("Fade finalizado.");
    }

    // OS MÉTODOS ApplyOutline E RemoveOutline NÃO SÃO MAIS NECESSÁRIOS E FORAM REMOVIDOS.
}