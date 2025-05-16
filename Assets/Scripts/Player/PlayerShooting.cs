using UnityEngine;

public class PlayerShooting : MonoBehaviour
{
    // Configuración para ambas armas
    public enum WeaponMode { Rifle, Shotgun }
    public WeaponMode currentWeapon = WeaponMode.Rifle;

    // Configuración del rifle
    public int rifleDamagePerShot = 20;
    public float rifleTimeBetweenBullets = 0.15f;
    public float rifleRange = 100f;

    // Configuración de la escopeta
    public int shotgunDamagePerPellet = 10;
    public int shotgunPellets = 8;
    public float shotgunTimeBetweenShots = 0.5f;
    public float shotgunRange = 30f;
    public float shotgunSpreadAngle = 25f;

    float timer;
    Ray shootRay = new Ray();
    RaycastHit shootHit;
    int shootableMask;
    ParticleSystem gunParticles;
    LineRenderer gunLine;
    AudioSource gunAudio;
    Light gunLight;
    float effectsDisplayTime = 0.2f;

    void Awake()
    {
        shootableMask = LayerMask.GetMask("Shootable");
        gunParticles = GetComponent<ParticleSystem>();
        gunLine = GetComponent<LineRenderer>();
        gunAudio = GetComponent<AudioSource>();
        gunLight = GetComponent<Light>();

        // Escalar daño por nivel
        string sceneName = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;

        if (sceneName == "Level 02")
        {
            rifleDamagePerShot = 25;
            shotgunDamagePerPellet = 12;
        }
        else if (sceneName == "Level 03")
        {
            rifleDamagePerShot = 30;
            shotgunDamagePerPellet = 15;
        }
    }

    void Update()
    {
        // Cambiar de arma al presionar Q
        if (Input.GetKeyDown(KeyCode.Q))
        {
            SwitchWeapon();
        }

        timer += Time.deltaTime;

        if (Input.GetButton("Fire1") && timer >= GetCurrentFireRate() && Time.timeScale != 0)
        {
            Shoot();
        }

        if (timer >= GetCurrentFireRate() * effectsDisplayTime)
        {
            DisableEffects();
        }
    }

    void SwitchWeapon()
    {
        currentWeapon = currentWeapon == WeaponMode.Rifle ? WeaponMode.Shotgun : WeaponMode.Rifle;
        // Aquí puedes agregar efectos de sonido o UI para indicar el cambio de arma
        Debug.Log("Cambiando a: " + currentWeapon);
    }

    float GetCurrentFireRate()
    {
        return currentWeapon == WeaponMode.Rifle ? rifleTimeBetweenBullets : shotgunTimeBetweenShots;
    }

    public void DisableEffects()
    {
        gunLine.enabled = false;
        gunLight.enabled = false;
    }

    void Shoot()
    {
        timer = 0f;

        gunAudio.Play();
        gunLight.enabled = true;
        gunParticles.Stop();
        gunParticles.Play();

        if (currentWeapon == WeaponMode.Rifle)
        {
            ShootRifle();
        }
        else
        {
            ShootShotgun();
        }
    }

    void ShootRifle()
    {
        gunLine.enabled = true;
        gunLine.positionCount = 2;
        gunLine.SetPosition(0, transform.position);

        shootRay.origin = transform.position;
        shootRay.direction = transform.forward;

        if (Physics.Raycast(shootRay, out shootHit, rifleRange, shootableMask))
        {
            EnemyHealth enemyHealth = shootHit.collider.GetComponent<EnemyHealth>();
            if (enemyHealth != null)
            {
                enemyHealth.TakeDamage(rifleDamagePerShot, shootHit.point);
            }
            gunLine.SetPosition(1, shootHit.point);
        }
        else
        {
            gunLine.SetPosition(1, shootRay.origin + shootRay.direction * rifleRange);
        }
    }

    void ShootShotgun()
    {
        gunLine.enabled = true;
        // Para la escopeta necesitamos más posiciones en el LineRenderer
        gunLine.positionCount = shotgunPellets * 2; // 2 puntos por cada perdigón

        Vector3 baseDirection = transform.forward;
        Vector3 shootOrigin = transform.position;

        for (int i = 0; i < shotgunPellets; i++)
        {
            // Calcular dirección con dispersión
            Vector3 shotDirection = GetRandomShotDirection(baseDirection, shotgunSpreadAngle);
            
            int startIndex = i * 2;
            int endIndex = startIndex + 1;
            
            gunLine.SetPosition(startIndex, shootOrigin);

            shootRay.origin = shootOrigin;
            shootRay.direction = shotDirection;

            if (Physics.Raycast(shootRay, out shootHit, shotgunRange, shootableMask))
            {
                EnemyHealth enemyHealth = shootHit.collider.GetComponent<EnemyHealth>();
                if (enemyHealth != null)
                {
                    enemyHealth.TakeDamage(shotgunDamagePerPellet, shootHit.point);
                }
                gunLine.SetPosition(endIndex, shootHit.point);
            }
            else
            {
                gunLine.SetPosition(endIndex, shootRay.origin + shootRay.direction * shotgunRange);
            }
        }
    }

    Vector3 GetRandomShotDirection(Vector3 baseDirection, float maxAngle)
    {
        // Generar una dirección aleatoria dentro de un cono
        float spread = Random.Range(0f, maxAngle);
        Vector3 spreadDirection = Random.insideUnitCircle * Mathf.Tan(spread * Mathf.Deg2Rad);
        
        Quaternion spreadRotation = Quaternion.LookRotation(baseDirection + spreadDirection);
        return spreadRotation * Vector3.forward;
    }
} 