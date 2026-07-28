using System;
using UnityEngine;

// 本物の城に置いた当たり判定用の球体にアタッチする。
// レーザーが当たったら、モニター側の対応する菌モデル(monitorVisual)も一緒に消す。
public class Germ : MonoBehaviour
{
    public static event Action OnAnyGermCleared;

    [SerializeField] private GameObject monitorVisual;
    [SerializeField] private bool hideRenderer = true;
    [SerializeField] private ParticleSystem hitEffectPrefab;
    [SerializeField] private AudioClip hitSound;

    private void Awake()
    {
        if (hideRenderer)
        {
            Renderer rend = GetComponent<Renderer>();
            if (rend != null)
            {
                rend.enabled = false;
            }
        }
    }

    public void Activate()
    {
        gameObject.SetActive(true);

        if (monitorVisual != null)
        {
            monitorVisual.SetActive(true);
        }
    }

    public void Hit()
    {
        if (hitEffectPrefab != null)
        {
            ParticleSystem effect = Instantiate(hitEffectPrefab, transform.position, Quaternion.identity);
            Destroy(effect.gameObject, effect.main.duration + effect.main.startLifetime.constantMax);
        }

        if (hitSound != null)
        {
            AudioSource.PlayClipAtPoint(hitSound, transform.position);
        }

        if (monitorVisual != null)
        {
            Destroy(monitorVisual);
        }

        Destroy(gameObject);
        OnAnyGermCleared?.Invoke();
    }
}
