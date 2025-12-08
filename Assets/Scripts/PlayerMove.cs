using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMove : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float mouseSensitivity = 100f;
    public float heightOffset = 1.0f; // 땅에서 띄울 높이

    private CharacterController controller;
    private Transform cam;

    private Vector2 moveInput;
    private Vector2 lookInput;

    private float xRotation = 0f;

    private InputSystem_Actions input;   // ★ 추가

    void Awake()
    {
        input = new InputSystem_Actions();   // ★ 추가
    }

    void OnEnable()
    {
        input.Enable();                      // ★ 추가

        input.Player.Move.performed += ctx => moveInput = ctx.ReadValue<Vector2>();
        input.Player.Move.canceled += ctx => moveInput = Vector2.zero;

        input.Player.Look.performed += ctx => lookInput = ctx.ReadValue<Vector2>();
        input.Player.Look.canceled += ctx => lookInput = Vector2.zero;
    }

    void OnDisable()
    {
        input.Disable();                     // ★ 추가
    }

    void Start()
    {
        controller = GetComponent<CharacterController>();
        cam = Camera.main.transform;

        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        Move();
        Look();
    }


    void Move()
    {
        float speed = PlayerStats.instance.moveSpeed;   // 🔥 강화된 이동속도 적용

        Vector3 direction = transform.right * moveInput.x + transform.forward * moveInput.y;
        Vector3 move = direction * speed * Time.deltaTime;

        controller.Move(move);
    }


    void Look()
    {
        float mouseX = lookInput.x * mouseSensitivity * Time.deltaTime;
        float mouseY = lookInput.y * mouseSensitivity * Time.deltaTime;

        transform.Rotate(Vector3.up * mouseX);

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -80f, 80f);

        cam.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
    }
    void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (hit.collider.CompareTag("Enemy"))
        {
            GetComponent<PlayerHealth>()?.TakeDamage(1);
        }
    }

}
