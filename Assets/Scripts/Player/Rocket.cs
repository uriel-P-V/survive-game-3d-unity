using UnityEngine;

public class Rocket : MonoBehaviour
{
    public float speed = 20f; // Velocidad del cohete
    public int damage = 100;   // Daño del cohete
    public float lifetime = 5f; // Tiempo de vida del cohete

    private Vector3 direction;

    void Start()
    {
        Destroy(gameObject, lifetime); // Destruye el cohete después de un tiempo
    }

    public void Initialize(Vector3 dir, int damage)
    {
        direction = dir;
        this.damage = damage;
    }

    void Update()
    {
        transform.Translate(direction * speed * Time.deltaTime, Space.World); // Mueve el cohete

        // Detecta colisiones con enemigos
        RaycastHit hit;
        if (Physics.Raycast(transform.position, direction, out hit, speed * Time.deltaTime))
        {
            EnemyHealth enemyHealth = hit.collider.GetComponent<EnemyHealth>();
            if (enemyHealth != null)
            {
                enemyHealth.TakeDamage(damage, hit.point); // Aplica daño al enemigo
            }

            // Destruye el cohete al impactar
            Destroy(gameObject);
        }
    }
}
