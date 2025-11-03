using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hero : MonoBehaviour
{
    // ---------- Singleton ----------
    public static Hero S { get; private set; }

    [Header("Inscribed")]
    // movement
    public float speed = 30f;
    public float rollMult = -45f;
    public float pitchMult = 30f;

    // shooting
    public GameObject projectilePrefab;
    public float projectileSpeed = 40f;

    [Header("Dynamic"), Range(0, 4), SerializeField]
    private float _shieldLevel = 1;

    // to avoid double-hits
    private GameObject lastTriggerGo = null;

    void Awake()
    {
        if (S == null)
        {
            S = this;
        }
        else
        {
            Debug.LogError("Hero.Awake() - second Hero created!");
        }
    }

    void Update()
    {
        // -------- Movement --------
        float hAxis = Input.GetAxis("Horizontal");
        float vAxis = Input.GetAxis("Vertical");

        Vector3 pos = transform.position;
        pos.x += hAxis * speed * Time.deltaTime;
        pos.y += vAxis * speed * Time.deltaTime;
        transform.position = pos;

        // tilt the ship
        transform.rotation = Quaternion.Euler(vAxis * pitchMult,
                                              hAxis * rollMult,
                                              0);

        // -------- Shooting --------
        if (Input.GetKeyDown(KeyCode.Space))
        {
            TempFire();
        }
    }

    void TempFire()
    {
        // make sure we have a prefab set
        if (projectilePrefab == null)
        {
            Debug.LogWarning("Hero.TempFire(): projectilePrefab is NULL!");
            return;
        }

        GameObject projGO = Instantiate<GameObject>(projectilePrefab);
        projGO.transform.position = transform.position;

        Rigidbody rb = projGO.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.velocity = Vector3.up * projectileSpeed;
        }
    }

    // -------------------------------------------------------
    // This is what PowerUp.cs is trying to call
    public void AbsorbPowerUp(PowerUp p)
    {
        // basic version: react to shield, log others
        switch (p.type)
        {
            case eWeaponType.shield:
                shieldLevel++;
                break;

            default:
                Debug.Log("Absorbed PowerUp of type: " + p.type);
                break;
        }

        // PowerUp cleanup
        Destroy(p.gameObject);
    }
    // -------------------------------------------------------

    void OnTriggerEnter(Collider other)
    {
        // --- Ignore our own projectiles ---
        if (other.gameObject.CompareTag("ProjectileHero"))
        {
            return;
        }

        // --- Ignore starfield background collisions ---
        string n = other.gameObject.name;
        if (n.StartsWith("Starfield"))
        {
            return;
        }

        Transform rootT = other.gameObject.transform.root;
        GameObject go = rootT.gameObject;

        if (go == lastTriggerGo) return;
        lastTriggerGo = go;

        // did we hit an enemy?
        Enemy enemy = go.GetComponent<Enemy>();
        if (enemy != null)
        {
            shieldLevel--;
            Destroy(go);
            return;
        }

        // did we hit a powerup directly?
        PowerUp pUp = go.GetComponent<PowerUp>();
        if (pUp != null)
        {
            AbsorbPowerUp(pUp);
            return;
        }

        Debug.Log("Hero hit by: " + go.name);
    }

    // ------- shield property -------
    public float shieldLevel
    {
        get { return _shieldLevel; }
        private set
        {
            _shieldLevel = Mathf.Min(value, 4);
            if (_shieldLevel < 0)
            {
                // hero died
                Destroy(this.gameObject);

                // Tell Main to handle Game Over if available
                if (Main.S != null)
                {
                    Main.S.HeroDied();
                }
            }
        }
    }
}
