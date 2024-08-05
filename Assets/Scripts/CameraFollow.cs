using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField]
    Transform playerBody;
    [SerializeField]
    float followSpeed;
    [SerializeField]
    float followDistance;
    [SerializeField]
    Vector3 offset;
    [SerializeField]
    Quaternion rotation;

    private void LateUpdate()
    {
        Vector3 pos = Vector3.Lerp(transform.position, playerBody.position + offset + (-transform.forward * followDistance), followSpeed * Time.deltaTime);
        transform.position = pos;

        transform.rotation = rotation;
    }
    [ContextMenu("RefreshCamera")]
    void RefreshCameraEdit()
    {
        Vector3 pos = playerBody.position + offset + (-transform.forward * followDistance);
        transform.position = pos;

        transform.rotation = rotation;
    }
}
