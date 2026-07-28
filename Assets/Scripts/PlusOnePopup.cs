using UnityEngine;

// 外して菌が増えた時に「+1」を一瞬表示するUIにアタッチする。
public class PlusOnePopup : MonoBehaviour
{
    [SerializeField] private float displayDuration = 1f;

    private float timer;

    private void Awake()
    {
        gameObject.SetActive(false);
    }

    public void Show()
    {
        gameObject.SetActive(true);
        timer = displayDuration;
    }

    private void Update()
    {
        timer -= Time.deltaTime;
        if (timer <= 0f)
        {
            gameObject.SetActive(false);
        }
    }
}
