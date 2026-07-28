using UnityEngine;
using UnityEngine.InputSystem;

// 手元モニター用の城モデルにアタッチする。マウス左ドラッグで城を回転させる。
// プレイヤーがモニターの近くにいる時だけ操作を受け付ける。
public class CastleRotator : MonoBehaviour
{
    [SerializeField] private float rotationSpeed = 0.2f;
    [SerializeField] private float minPitch = -80f;
    [SerializeField] private float maxPitch = 80f;
    [SerializeField] private Transform player;
    [SerializeField] private float maxDistance = 2f;
    [SerializeField] private PlayerLook playerLook;

    private float yaw;
    private float pitch;
    private float roll;

    private void Start()
    {
        Vector3 e = transform.eulerAngles;
        yaw = e.y;
        pitch = e.x > 180f ? e.x - 360f : e.x;
        roll = e.z;

        if (player == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
            {
                player = playerObj.transform;
            }
        }

        if (playerLook == null && player != null)
        {
            playerLook = player.GetComponentInChildren<PlayerLook>();
        }
    }

    private void Update()
    {
        bool inRange = player != null && Vector3.Distance(player.position, transform.position) <= maxDistance;

        if (playerLook != null)
        {
            playerLook.enabled = !inRange;
        }

        if (!inRange || Mouse.current == null || !Mouse.current.leftButton.isPressed)
        {
            return;
        }

        Vector2 delta = Mouse.current.delta.ReadValue();
        yaw -= delta.x * rotationSpeed;
        pitch += delta.y * rotationSpeed;
        pitch = Mathf.Clamp(pitch, minPitch, maxPitch);

        // 上下方向(pitch)を常に固定の右方向軸で回すことで、
        // 城が左右にどれだけ回転していても上下ドラッグの向きが変わらないようにする
        transform.rotation = Quaternion.AngleAxis(pitch, Vector3.right)
            * Quaternion.AngleAxis(yaw, Vector3.up)
            * Quaternion.AngleAxis(roll, Vector3.forward);
    }
}
