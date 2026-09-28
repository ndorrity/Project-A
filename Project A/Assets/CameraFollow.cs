using UnityEngine;

public class CarCameraFollow : MonoBehaviour
{
    public Transform target;                // car or a camera target empty object
    public Vector3 offset = new Vector3(0, 4, -8);
    public float followSpeed = 6f;          // how fast the camera moves
    public float rotateSpeed = 6f;          // how fast the camera rotates

    void LateUpdate()
    {
        if (!target) return;

        // Smooth position
        Vector3 desiredPos = target.position + target.TransformDirection(offset);
        transform.position = Vector3.Lerp(transform.position, desiredPos, followSpeed * Time.deltaTime);

        // Smooth rotation
        Quaternion desiredRot = Quaternion.LookRotation(target.position - transform.position, Vector3.up);
        transform.rotation = Quaternion.Slerp(transform.rotation, desiredRot, rotateSpeed * Time.deltaTime);
    }
}
