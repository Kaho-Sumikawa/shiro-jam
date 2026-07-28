using UnityEngine;
using UnityEngine.InputSystem;

// 銃（常時装備、着脱処理なし）にアタッチする。
// 左クリックでレーザーを発射し、マウスホイールでスコープの倍率(カメラFOV)を変える。
public class GunController : MonoBehaviour
{
    [SerializeField] private Camera scopeCamera;
    [SerializeField] private Transform muzzle;
    [SerializeField] private Transform beamVisual;
    [SerializeField] private float beamThickness = 0.03f;
    [SerializeField] private float laserDuration = 0.2f;
    [SerializeField] private float range = 100f;
    [SerializeField] private LayerMask hitMask = ~0;

    [SerializeField] private float minFov = 15f;
    [SerializeField] private float maxFov = 60f;
    [SerializeField] private float zoomSpeed = 0.05f;

    [SerializeField] private ParticleSystem muzzleFlash;
    [SerializeField] private ParticleSystem impactEffectPrefab;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip fireSound;
    [SerializeField] private AudioClip impactSound;
    [SerializeField] private PlayerLook playerLook;
    [SerializeField] private GermSpawner germSpawner;
    [SerializeField] private Collider castleCollider;

    private float laserTimer;

    private void Awake()
    {
        if (scopeCamera == null)
        {
            scopeCamera = GetComponentInParent<Camera>();
        }
        if (muzzle == null)
        {
            muzzle = transform;
        }
        if (beamVisual != null)
        {
            beamVisual.gameObject.SetActive(false);
        }
        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
        }
        if (playerLook == null && scopeCamera != null)
        {
            playerLook = scopeCamera.GetComponent<PlayerLook>();
        }
    }

    private void Update()
    {
        HandleZoom();
        HandleFire();
        HandleLaserVisual();
    }

    private void HandleZoom()
    {
        if (Mouse.current == null || scopeCamera == null)
        {
            return;
        }

        float scroll = Mouse.current.scroll.ReadValue().y;
        if (Mathf.Approximately(scroll, 0f))
        {
            return;
        }

        scopeCamera.fieldOfView = Mathf.Clamp(
            scopeCamera.fieldOfView - scroll * zoomSpeed,
            minFov,
            maxFov);
    }

    private void HandleFire()
    {
        if (Mouse.current == null || !Mouse.current.leftButton.wasPressedThisFrame || scopeCamera == null)
        {
            return;
        }

        if (playerLook != null && !playerLook.enabled)
        {
            return;
        }

        if (muzzleFlash != null)
        {
            muzzleFlash.Play();
        }
        if (audioSource != null && fireSound != null)
        {
            audioSource.PlayOneShot(fireSound);
        }

        Vector3 origin = scopeCamera.transform.position;
        Vector3 direction = scopeCamera.transform.forward;
        Vector3 endPoint = origin + direction * range;

        RaycastHit[] hits = Physics.RaycastAll(origin, direction, range, hitMask);
        System.Array.Sort(hits, (a, b) => a.distance.CompareTo(b.distance));

        Germ hitGerm = null;
        RaycastHit germHit = default;
        RaycastHit? closestHit = hits.Length > 0 ? hits[0] : (RaycastHit?)null;

        foreach (RaycastHit candidate in hits)
        {
            if (candidate.collider.TryGetComponent(out Germ germ))
            {
                hitGerm = germ;
                germHit = candidate;
                break;
            }
        }

        if (hitGerm != null)
        {
            endPoint = germHit.point;
            SpawnImpactEffect(germHit.point, germHit.normal);
            hitGerm.Hit();
        }
        else if (closestHit.HasValue)
        {
            endPoint = closestHit.Value.point;

            if (germSpawner != null && closestHit.Value.collider == castleCollider)
            {
                germSpawner.SpawnOne();
            }
        }

        ShowLaser(endPoint);
    }

    private void SpawnImpactEffect(Vector3 point, Vector3 normal)
    {
        if (impactEffectPrefab != null)
        {
            ParticleSystem effect = Instantiate(impactEffectPrefab, point, Quaternion.LookRotation(normal));
            Destroy(effect.gameObject, effect.main.duration + effect.main.startLifetime.constantMax);
        }

        if (impactSound != null)
        {
            AudioSource.PlayClipAtPoint(impactSound, point);
        }
    }

    private void ShowLaser(Vector3 endPoint)
    {
        if (beamVisual == null)
        {
            return;
        }

        Vector3 origin = muzzle.position;
        Vector3 offset = endPoint - origin;
        float distance = offset.magnitude;

        beamVisual.position = origin + offset * 0.5f;
        beamVisual.rotation = Quaternion.FromToRotation(Vector3.up, offset.normalized);
        // 標準のCylinderは高さ2(ローカルYが-1〜1)なので、距離に合わせるにはscale.yを半分にする
        beamVisual.localScale = new Vector3(beamThickness, distance * 0.5f, beamThickness);

        beamVisual.gameObject.SetActive(true);
        laserTimer = laserDuration;
    }

    private void HandleLaserVisual()
    {
        if (beamVisual == null || !beamVisual.gameObject.activeSelf)
        {
            return;
        }

        laserTimer -= Time.deltaTime;
        if (laserTimer <= 0f)
        {
            beamVisual.gameObject.SetActive(false);
        }
    }
}
