using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;
    public float smoothSpeed = 10f;
    public float zOffset = -10f;
    public Vector3 offset;

    void LateUpdate()
    {
        offset = new Vector3(0, 3, zOffset);
        if (!target) return;

        Vector3 desiredPos = target.position + offset;
        transform.position = Vector3.Lerp(transform.position, desiredPos, smoothSpeed * Time.deltaTime);
    }
}