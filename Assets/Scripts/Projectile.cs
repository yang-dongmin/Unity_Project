using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float speed = 20f;
    public int damage = 1;
    public float lifeTime = 3f;

    void Start()
    {
        Destroy(gameObject, lifeTime);
    }

    void Update()
    {
        Debug.DrawRay(transform.position, transform.forward * 2f, Color.red);
        transform.position += transform.forward * speed * Time.deltaTime;
    }


    void OnTriggerEnter(Collider other)
    {
        Debug.Log("Hit: " + other.name);

        if (other.CompareTag("Enemy"))
        {
            Debug.Log("Enemy Hit!");
            other.GetComponent<EnemyHealth>()?.TakeDamage(damage);
            Destroy(gameObject);
        }
    }
}
