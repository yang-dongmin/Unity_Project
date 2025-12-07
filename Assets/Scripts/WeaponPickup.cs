using UnityEngine;

public class WeaponPickup : MonoBehaviour
{
    public WeaponData weaponToGive;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;

        PlayerWeapon pw = other.GetComponent<PlayerWeapon>();
        if (pw == null)
            return;

        // 무기 교체
        pw.ApplyWeaponData(weaponToGive);

        // 스포너에게 내가 없어졌다고 알리기
        if (WeaponSpawner.instance != null)
            WeaponSpawner.instance.currentWeaponPickup = null;

        // 픽업 오브젝트 삭제
        Destroy(gameObject);
    }
}
