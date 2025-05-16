using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class EnemyAttack : MonoBehaviour
{
    public float timeBetweenAttacks = 0.5f;
    public int attackDamage = 10;

    Animator anim;
    GameObject player;
    PlayerHealth playerHealth;
    EnemyHealth enemyHealth;
    bool playerInRange;
    float timer;

    void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        playerHealth = player.GetComponent<PlayerHealth>();
        enemyHealth = GetComponent<EnemyHealth>();
        anim = GetComponent<Animator>();

        // Ajustar daño según el nivel actual
        string sceneName = SceneManager.GetActiveScene().name;

        if (sceneName == "Level 02")
        {
            attackDamage = 15; // más daño en nivel 2
        }
        else if (sceneName == "Level 03")
        {
            attackDamage = 20; // más daño en nivel 3
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == player)
        {
            playerInRange = true;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject == player)
        {
            playerInRange = false;
        }
    }

    void Update()
    {
        timer += Time.deltaTime;

        if (timer >= timeBetweenAttacks && playerInRange && enemyHealth.CurrentHealth > 0)
        {
            Attack();
        }

        if (playerHealth.CurrentHealth <= 0)
        {
            anim.SetTrigger("PlayerDead");
        }
    }

    void Attack()
    {
        timer = 0f;

        if (playerHealth.CurrentHealth > 0)
        {
            playerHealth.TakeDamage(attackDamage);
        }
    }
}

