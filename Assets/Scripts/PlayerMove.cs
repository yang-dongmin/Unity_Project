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

    private InputSystem_Actions input;

    private float verticalVelocity = 0f;
    private float gravity = -9.81f;


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
        float speed = PlayerStats.instance.moveSpeed;

        Vector3 direction = transform.right * moveInput.x + transform.forward * moveInput.y;
        Vector3 move = direction * speed;

        // 🔥 중력 적용
        if (controller.isGrounded)
            verticalVelocity = -1f;     // 땅에 붙여두기
        else
            verticalVelocity += gravity * Time.deltaTime;

        move.y = verticalVelocity;

        controller.Move(move * Time.deltaTime);

        // 🔥 Terrain 지면 높이에 붙이기
        Terrain terrain = Terrain.activeTerrain;
        if (terrain != null)
        {
            Vector3 pos = transform.position;
            float terrainY = terrain.SampleHeight(pos) + terrain.GetPosition().y;

            if (pos.y < terrainY + heightOffset)
            {
                pos.y = terrainY + heightOffset;
                transform.position = pos;
                verticalVelocity = 0f; // 지면에 닿으면 중력 초기화
            }
        }

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
