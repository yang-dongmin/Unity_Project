using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float speed = 20f;
    public int damage = 1;
    public float lifeTime = 3f;

    public GameObject hitFlash;

    void Start()
    {
        Destroy(gameObject, lifeTime);
    }

    void Update()
    {
        transform.position += transform.forward * speed * Time.deltaTime;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            other.GetComponent<EnemyHealth>()?.TakeDamage(damage);

            if (hitFlash != null)
                Instantiate(hitFlash, transform.position, Quaternion.identity);

            Destroy(gameObject);
        }
    }
}
