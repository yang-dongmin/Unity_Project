using UnityEngine;
using UnityEngine.UI;

public class WarningFlash : MonoBehaviour
{
    public Image img;
    public float flashSpeed = 4f;

    void Awake()
    {
        if (img == null)
            img = GetComponent<Image>(); // 자동으로 패널 이미지 가져옴
    }

    void Update()
    {
        // 깜빡임 (0~1)
        float a = (Mathf.Sin(Time.unscaledTime * flashSpeed) + 1f) * 0.5f;

        // 패널 색상 알파만 변경
        Color c = img.color;
        c.a = a;
        img.color = c;
    }
}
