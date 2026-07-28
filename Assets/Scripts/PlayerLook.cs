using UnityEngine;
using UnityEngine.InputSystem;

// プレイヤーのカメラにアタッチする。マウスで視点を見回す。
// 左右(yaw)は lookTarget（プレイヤー本体）を回転、上下(pitch)はこのカメラだけ回転させる。
public class PlayerLook : MonoBehaviour
{
    [SerializeField] private Transform lookTarget;
    [SerializeField] private float sensitivity = 0.2f;
    [SerializeField] private float minPitch = -80f;
    [SerializeField] private float maxPitch = 80f;
    [SerializeField] private bool lockCursor = true;

    private float pitch;

    private void Start()
    {
        if (lookTarget == null)
        {
            lookTarget = transform.parent;
        }

        float x = transform.localEulerAngles.x;
        pitch = x > 180f ? x - 360f : x;

        if (lockCursor)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    private void Update()
    {
        if (lockCursor && Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }

        if (Mouse.current == null)
        {
            return;
        }

        Vector2 delta = Mouse.current.delta.ReadValue();

        if (lookTarget != null)
        {
            lookTarget.Rotate(Vector3.up * delta.x * sensitivity, Space.World);
        }

        pitch -= delta.y * sensitivity;
        pitch = Mathf.Clamp(pitch, minPitch, maxPitch);
        transform.localRotation = Quaternion.Euler(pitch, 0f, 0f);
    }
}
