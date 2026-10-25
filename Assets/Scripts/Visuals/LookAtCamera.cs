using UnityEngine;

/// Поворачивает world-space UI лицом к камере.
public class LookAtCamera : MonoBehaviour
{
    private Transform cam;

    private void LateUpdate()
    {
        if (cam == null)
        {
            if (Camera.main == null) return;
            cam = Camera.main.transform;
        }
        transform.forward = cam.forward;
    }
}
