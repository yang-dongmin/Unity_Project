using UnityEngine;

public class WeaponSway : MonoBehaviour
{
    public float swayAmount = 0.05f;
    public float smoothAmount = 6f;

    private Vector3 initialPos;

    void Start()
    {
        initialPos = transform.localPosition;
    }

    void Update()
    {
        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");

        // 마우스 움직임 기반 흔들림
        Vector3 swayPos = new Vector3(
            mouseX * -swayAmount,
            mouseY * -swayAmount,
            0f
        );

        // 부드러운 이동
        transform.localPosition = Vector3.Lerp(
            transform.localPosition,
            initialPos + swayPos,
            Time.deltaTime * smoothAmount
        );
    }
}
