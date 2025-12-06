using UnityEngine;

public class AutoDestroy : MonoBehaviour
{
    public float life = 0.1f;

    void Start()
    {
        Destroy(gameObject, life);
    }
}
