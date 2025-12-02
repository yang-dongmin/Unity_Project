using UnityEngine;

public class CameraRotate : MonoBehaviour
{
    public float rotateSpeed = 3f;

    void Update()
    {
        transform.Rotate(0, rotateSpeed * Time.deltaTime, 0, Space.World);
    }
}
