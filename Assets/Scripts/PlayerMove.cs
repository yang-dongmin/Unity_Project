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
        // 1) XZ 방향으로만 이동 벡터 계산
        Vector3 direction = transform.right * moveInput.x + transform.forward * moveInput.y;
        Vector3 move = direction * moveSpeed * Time.deltaTime;

        // 2) 우선 평면 기준으로 이동
        controller.Move(move);

        // 3) 현재 XZ 위치에서 Terrain 높이 샘플링해서 Y 맞추기
        Terrain terrain = Terrain.activeTerrain;
        if (terrain != null)
        {
            Vector3 pos = transform.position;

            float terrainY = terrain.SampleHeight(pos) + terrain.GetPosition().y;

            // 땅 바로 위로 스냅
            pos.y = terrainY + heightOffset;
            transform.position = pos;
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
