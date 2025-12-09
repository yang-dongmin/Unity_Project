using UnityEngine;
using UnityEngine.UI;
using System.Collections;


public class PlayerHealth : MonoBehaviour
{
    private PlayerStats stats;

    [Header("UI References")]
    public Slider hpSlider;
    public RectTransform hpSliderRect;

    public GameObject damagepanel;
    private Image dmgImage;

    private float baseWidth;

    // 🔥 무적 여부 체크
    private bool isInvincible = false;

    private void Start()
    {
        stats = PlayerStats.instance;

        hpSlider.maxValue = stats.maxHealth;
        hpSlider.value = stats.currentHealth;

        baseWidth = hpSliderRect.sizeDelta.x;

        if (damagepanel != null)
        {
            dmgImage = damagepanel.GetComponent<Image>();
            dmgImage.color = new Color(1, 0, 0, 0);   // 처음엔 완전 투명
        }
    }

    private void Update()
    {
        hpSlider.value = stats.currentHealth;
        hpSlider.maxValue = stats.maxHealth;

        float newWidth = baseWidth + (stats.maxHealth - 5) * 20f;
        hpSliderRect.sizeDelta = new Vector2(newWidth, hpSliderRect.sizeDelta.y);
    }

    public void TakeDamage(int amount)
    {
        // 🔥 무적 상태면 데미지 무시
        if (isInvincible) return;

        stats.currentHealth -= amount;

        if (stats.currentHealth <= 0)
        {   
            stats.currentHealth = 0;          
            hpSlider.value = 0; 

            Debug.Log("Player Dead");
            gameObject.SetActive(false);
            return;
        }


        StartCoroutine(HitFlash());
        StartCoroutine(InvincibleRoutine());
    }

    IEnumerator HitFlash()
    {
        if (damagepanel != null && !damagepanel.activeSelf)
        damagepanel.SetActive(true);

        // 1) 점점 빨개짐 (0 → 0.4)
        float t = 0;
        while (t < 1)
        {
            t += Time.deltaTime * 10f;
            dmgImage.color = new Color(1, 0, 0, Mathf.Lerp(0, 0.4f, t));
            yield return null;
        }

        // 2) 다시 투명해짐 (0.4 → 0)
        t = 0;
        while (t < 1)
        {
            t += Time.deltaTime * 6f;
            dmgImage.color = new Color(1, 0, 0, Mathf.Lerp(0.4f, 0, t));
            yield return null;
        }
        damagepanel.SetActive(false);
    }

    // -------------------------------------
    // 🔥 1초 무적 코루틴
    // -------------------------------------
    private IEnumerator InvincibleRoutine()
    {
        isInvincible = true;

        // 여기서 피격 효과 넣어도 됨 (깜빡임 등)
        // ex) playerMeshRenderer.enabled = false → true 반복

        yield return new WaitForSeconds(1f);

        isInvincible = false;
    }
}
