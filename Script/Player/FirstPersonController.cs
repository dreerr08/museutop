// FirstPersonController.cs
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class FirstPersonController : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float speed = 6f; // Velocidade de movimento (caminhada)
    [SerializeField] private float gravity = -9.81f;

    [Header("Mouse Settings")]
    [SerializeField] private float mouseSensitivity = 100f;
    [SerializeField] private float minPitchAngle = -90f;
    [SerializeField] private float maxPitchAngle = 90f;

    private CharacterController controller;
    private Transform cam;
    private float pitch = 0f;
    private Vector3 velocity;

    void Awake()
    {
        controller = GetComponent<CharacterController>();
        cam = Camera.main.transform;
    }

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        HandleMouseLook();
        HandleMovement();
    }

    void HandleMouseLook()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        pitch -= mouseY;
        pitch = Mathf.Clamp(pitch, minPitchAngle, maxPitchAngle);

        cam.localRotation = Quaternion.Euler(pitch, 0f, 0f);
        transform.Rotate(Vector3.up * mouseX);
    }

    void HandleMovement()
    {
        if (controller.isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        float horiz = Input.GetAxis("Horizontal");
        float vert = Input.GetAxis("Vertical");
        Vector3 moveDirection = transform.right * horiz + transform.forward * vert;

        controller.Move(moveDirection * speed * Time.deltaTime);

        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }

    // **NOVA ADI��O**
    /// <summary>
    /// Retorna o valor atual da rota��o vertical (pitch) da c�mera.
    /// � a maneira correta de obter a rota��o X para salvar o estado.
    /// </summary>
    public float GetCurrentPitch()
    {
        return pitch;
    }

    /// <summary>
    /// Permite que um script externo (como um GerenciadorDeCenas) defina a rota��o inicial.
    /// </summary>
    /// <param name="rotacaoY">A rota��o para a esquerda/direita do corpo do jogador.</param>
    /// <param name="rotacaoX">A rota��o para cima/baixo da c�mera.</param>
    public void InicializarRotacao(float rotacaoY, float rotacaoX)
    {
        // Define a rota��o do corpo do jogador (olhar para os lados)
        transform.localEulerAngles = new Vector3(0f, rotacaoY, 0f);

        // Define a rota��o vertical (pitch) da c�mera e aplica-a imediatamente
        // Garantimos que o pitch restaurado respeite os limites definidos.
        pitch = Mathf.Clamp(rotacaoX, minPitchAngle, maxPitchAngle);
        cam.localRotation = Quaternion.Euler(pitch, 0f, 0f);
    }
}