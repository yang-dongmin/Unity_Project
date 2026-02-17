using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using UnityEngine.InputSystem;


public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [Header("Panels")]
    public GameObject gameOverPanel;
    public GameObject optionsPanel;

    [Header("Buttons")]
    public Button restartButton;
    public Button exitButton;      // 게임오버 패널 Exit
    public Button optionsExitButton;  // 옵션 메뉴 Exit (옵션 버튼과 동일 기능)

    [Header("Cameras")]
    public Camera mainCamera;
    public Camera gameOverCamera;

    [Header("UI")]
    public Image Crosshair1;
    public Image Crosshair2;

    public TextMeshProUGUI killCountText;

    [Header("Audio")]
    public Slider bgmSlider;
    public Slider sfxSlider;
    public AudioSource bgmSource;
    public AudioSource sfxSource;

    private bool isGameOver = false;
    public bool isOptionOpen = false;

    private int killCount = 0;

    public GameObject clearPanel;

    public void GameClear()
    {
        Time.timeScale = 0f;
        clearPanel.SetActive(true);

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }



    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        // 초기 비활성화
        gameOverPanel.SetActive(false);
        if (optionsPanel != null)
            optionsPanel.SetActive(false);

        gameOverCamera.enabled = false;

        // BGM 슬라이더 설정
        if (bgmSource != null && bgmSlider != null)
        {
            bgmSlider.value = bgmSource.volume;
            bgmSlider.onValueChanged.AddListener(SetBGMVolume);
        }
        
        // sfx 슬라이더 설정
        if (sfxSource != null && sfxSlider != null)
        {
            sfxSlider.value = sfxSource.volume;
            sfxSlider.onValueChanged.AddListener(SetSFXVolume);
        }

        // 버튼 등록
        restartButton.onClick.AddListener(RestartGame);

        // 게임오버 Exit 버튼도 같은 함수 사용
        exitButton.onClick.AddListener(ExitToIntro);

        // 옵션 메뉴 Exit 버튼도 같은 함수 사용
        if (optionsExitButton != null)
            optionsExitButton.onClick.AddListener(ExitToIntro);
    }


    void Update()
    {
        // ------------------------
        // 1) 게임오버 감지
        // ------------------------
        if (!isGameOver)
        {
            GameObject player = GameObject.FindWithTag("Player");

            if (player == null)
            {
                GameOver(null);
                return;
            }
        }

        // ------------------------
        // 2) 게임오버 상태에서는 ESC 금지
        // ------------------------
        if (isGameOver) 
            return;

        // ------------------------
        // 3) ESC 로 옵션 열기/닫기
        // ------------------------
        if (Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            if (optionsPanel.activeSelf)
                CloseOptions();
            else
                OpenOptions();
        }
    }



    // ================================================
    // 게임오버 처리
    // ================================================
    public void GameOver(Transform player)
    {
        isGameOver = true;

        // 메인 카메라 OFF
        if (mainCamera != null)
            mainCamera.enabled = false;

        // 게임오버 카메라 ON
        if (gameOverCamera != null)
        {
            gameOverCamera.gameObject.SetActive(true);
            gameOverCamera.enabled = true;

            if (player != null)
            {
                Vector3 pos = player.position + new Vector3(0, 20, 0);
                gameOverCamera.transform.position = pos;
                gameOverCamera.transform.rotation = Quaternion.Euler(90, 0, 0);
            }
        }

        // 크로스헤어 제거
        if (Crosshair1 != null) Crosshair1.enabled = false;
        if (Crosshair2 != null) Crosshair2.enabled = false;

        // UI
        gameOverPanel.SetActive(true);

        // 커서 활성화
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        // 일시정지
        Time.timeScale = 0f;
    }



    // ================================================
    // 옵션 메뉴
    // ================================================
    public void OpenOptions()
    {
        isOptionOpen = true;
        optionsPanel.SetActive(true);

        Time.timeScale = 0f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void CloseOptions()
    {
        isOptionOpen = false;
        optionsPanel.SetActive(false);

        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }



    // ================================================
    // Exit / Restart / BGM Functions
    // ================================================
    public void ExitToIntro()
    {
        Time.timeScale = 1f;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        SceneManager.LoadScene("IntroScene");
    }

    void RestartGame()
    {
        Time.timeScale = 1f;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void SetBGMVolume(float value)
    {
        if (bgmSource != null)
            bgmSource.volume = value;
    }

    public void SetSFXVolume(float value)
    {
        if (sfxSource != null)
            sfxSource.volume = value;
    }

    public void AddKill()
    {
        killCount++;
        killCountText.text = "Kills: " + killCount;
    }
}
