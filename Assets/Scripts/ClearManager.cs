using TMPro;
using UnityEngine;

// シーン内のGermをすべて数え、全部消えたらクリアパネルを表示する。
// 残り菌数もUIに表示する。
public class ClearManager : MonoBehaviour
{
    [SerializeField] private GameObject clearPanel;
    [SerializeField] private TMP_Text remainingText;
    [SerializeField] private AudioClip clearSound;
    [SerializeField] private MonoBehaviour[] scriptsToDisableOnClear;
    [SerializeField] private Rigidbody playerRigidbody;

    private int remainingGerms;

    private void Start()
    {
        remainingGerms = FindObjectsByType<Germ>(FindObjectsSortMode.None).Length;

        if (clearPanel != null)
        {
            clearPanel.SetActive(false);
        }

        UpdateDisplay();
    }

    private void OnEnable()
    {
        Germ.OnAnyGermCleared += HandleGermCleared;
    }

    private void OnDisable()
    {
        Germ.OnAnyGermCleared -= HandleGermCleared;
    }

    private void HandleGermCleared()
    {
        remainingGerms--;
        UpdateDisplay();

        if (remainingGerms <= 0 && clearPanel != null)
        {
            clearPanel.SetActive(true);

            if (clearSound != null)
            {
                AudioSource.PlayClipAtPoint(clearSound, transform.position);
            }

            foreach (MonoBehaviour script in scriptsToDisableOnClear)
            {
                if (script != null)
                {
                    script.enabled = false;
                }
            }

            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;

            if (playerRigidbody != null)
            {
                playerRigidbody.isKinematic = true;
            }
        }
    }

    public void AddGerm()
    {
        remainingGerms++;
        UpdateDisplay();
    }

    private void UpdateDisplay()
    {
        if (remainingText != null)
        {
            remainingText.text = $"残り：{remainingGerms}";
        }
    }
}
