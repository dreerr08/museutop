// Arquivo: dreerr08/museutop/museutop-6ea31f3c45b1c0f813e03be5a1425dc73cd4b2a0/Script/Dedução/Solução/SolucaoPersonagem.cs

// O atributo [System.Serializable] é extremamente importante para o Unity.
// Ele permite que você crie e edite instâncias desta classe diretamente
// no Inspector da Unity, tornando muito fácil preencher os dados do seu jogo
// sem precisar alterar o código.
[System.Serializable]
public class SolucaoPersonagem
{
    // NOTA: Esta classe não herda de MonoBehaviour, pois ela é apenas um
    // contêiner de dados, não um componente que vive em um GameObject.

    /// <summary>
    /// ID único para referenciar este personagem de forma programática (ex: 1, 2, 3...).
    /// </summary>
    public int id;

    /// <summary>
    /// O nome completo e correto do personagem.
    /// </summary>
    public string nomeCompleto;

    /// <summary>
    /// (ALTERADO) A imagem (Sprite) do retrato do personagem.
    /// Este campo substitui o antigo 'idDaFoto'.
    /// </summary>
    public UnityEngine.Sprite retrato;

    /// <summary>
    /// O verdadeiro envolvimento deste personagem no roubo.
    /// Usa o enum que definimos para garantir consistência.
    /// </summary>
    public PapelNoRoubo papel;

    /// <summary>
    /// O verdadeiro destino deste personagem.
    /// Usa o enum para garantir consistência.
    /// </summary>
    public DestinoFinal destino;

    /// <summary>
    /// (NOVO) O ID do personagem que assassinou este.
    /// Use 0 se não foi assassinado (ex: acidente, fugiu, etc.).
    /// </summary>
    public int idAssassino;
}