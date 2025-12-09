using UnityEngine;
using System.Collections;

public class BossIntroController : MonoBehaviour
{
    public static BossIntroController instance;
    public bool introPlayed = false;


    [Header("카메라")]
    public Transform introCamPoint;    
    public Transform mainCamera;       
    public Transform cameraParent;     

    [Header("플레이어 구성요소")]
    public Transform weaponObject;      
    public PlayerMove playerMove;      

    [Header("연출 오브젝트")]
    public GameObject portal;           

    private Vector3 originalLocalPos;
    private Quaternion originalLocalRot;
    private Transform originalParent;

    void Awake()
    {
        instance = this;
    }

    public void PlayIntro(GameObject boss)
    {
        if (introPlayed)  
            return;

        introPlayed = true;

        LevelUpManager.instance.warningPanel.SetActive(false);

        StartCoroutine(IntroRoutine(boss));
    }

    IEnumerator IntroRoutine(GameObject boss)
    {
        // 1) 플레이어 조작 차단
        if (playerMove != null)
            playerMove.canControl = false;

        // 2) 무기 숨기기
        if (weaponObject != null)
            weaponObject.gameObject.SetActive(false);

        // 3) 카메라 상태 저장
        originalParent = mainCamera.parent;
        originalLocalPos = mainCamera.localPosition;
        originalLocalRot = mainCamera.localRotation;

        // 4) 카메라 분리
        mainCamera.SetParent(null);

        // 5) 카메라 인트로 위치로 이동
        mainCamera.position = introCamPoint.position;
        mainCamera.rotation = introCamPoint.rotation;

        // 6) 포탈 켜기
        if (portal != null)
            portal.SetActive(true);

        // 7) 보스 등장 모션
        BossEnemy be = boss.GetComponent<BossEnemy>();
        if (be != null)
            be.introLock = true;

        Vector3 endPos = boss.transform.position;
        Vector3 startPos = endPos + Vector3.down * 15f;
        boss.transform.position = startPos;

        float t = 0f;
        while (t < 1f)
        {
            t += Time.unscaledDeltaTime * 0.3f;
            boss.transform.position = Vector3.Lerp(startPos, endPos, t);
            yield return null;
        }

        yield return new WaitForSecondsRealtime(1f);

        // 8) 카메라 원위치 복구
        mainCamera.SetParent(originalParent);
        mainCamera.localPosition = originalLocalPos;
        mainCamera.localRotation = originalLocalRot;

        // 9) 무기 다시 보이기
        if (weaponObject != null)
            weaponObject.gameObject.SetActive(true);

        // 10) 포탈 끄기
        if (portal != null)
            portal.SetActive(false);

        // 11) 플레이어 조작 복구
        if (playerMove != null)
            playerMove.canControl = true;


        if (be != null)
            be.introLock = false;
    }
}
