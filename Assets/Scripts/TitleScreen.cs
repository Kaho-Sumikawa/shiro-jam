using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

// タイトル画面 兼 遊び方表示にアタッチする。
// 「スタート」でゲーム開始、「遊び方」で遊び方パネルを開閉する。
// ゲーム開始後も howToPlayKey (デフォルトTab) でいつでも遊び方を開閉できる。
public class TitleScreen : MonoBehaviour
{
    [SerializeField] private GameObject titlePanel;
    [SerializeField] private GameObject howToPlayPanel;
    [SerializeField] private Button startButton;
    [SerializeField] private Button howToPlayButton;
    [SerializeField] private Button backButton;
    [SerializeField] private MonoBehaviour[] scriptsToEnableOnStart;
    [SerializeField] private Rigidbody playerRigidbody;
    [SerializeField] private Key howToPlayKey = Key.Tab;
    [SerializeField] private GameObject tabHint;

    private bool gameStarted;

    private void Awake()
    {
        SetScriptsEnabled(false);

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        if (howToPlayPanel != null)
        {
            howToPlayPanel.SetActive(false);
        }
        if (tabHint != null)
        {
            tabHint.SetActive(false);
        }

        if (startButton != null)
        {
            startButton.onClick.AddListener(HandleStart);
        }
        if (howToPlayButton != null)
        {
            howToPlayButton.onClick.AddListener(ShowHowToPlay);
        }
        if (backButton != null)
        {
            backButton.onClick.AddListener(HideHowToPlay);
        }
    }

    private void Update()
    {
        if (!gameStarted || Keyboard.current == null || !Keyboard.current[howToPlayKey].wasPressedThisFrame)
        {
            return;
        }

        if (howToPlayPanel != null && howToPlayPanel.activeSelf)
        {
            HideHowToPlay();
        }
        else
        {
            ShowHowToPlay();
        }
    }

    private void HandleStart()
    {
        gameStarted = true;

        if (titlePanel != null)
        {
            titlePanel.SetActive(false);
        }

        ResumeGameplay();
    }

    private void ShowHowToPlay()
    {
        if (titlePanel != null)
        {
            titlePanel.SetActive(false);
        }
        if (howToPlayPanel != null)
        {
            howToPlayPanel.SetActive(true);
        }

        SetScriptsEnabled(false);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        if (tabHint != null)
        {
            tabHint.SetActive(false);
        }
    }

    private void HideHowToPlay()
    {
        if (howToPlayPanel != null)
        {
            howToPlayPanel.SetActive(false);
        }

        if (gameStarted)
        {
            ResumeGameplay();
        }
        else if (titlePanel != null)
        {
            titlePanel.SetActive(true);
        }
    }

    private void ResumeGameplay()
    {
        SetScriptsEnabled(true);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        if (tabHint != null)
        {
            tabHint.SetActive(true);
        }
    }

    private void SetScriptsEnabled(bool value)
    {
        foreach (MonoBehaviour script in scriptsToEnableOnStart)
        {
            if (script != null)
            {
                script.enabled = value;
            }
        }

        if (playerRigidbody != null)
        {
            playerRigidbody.isKinematic = !value;
        }
    }
}
