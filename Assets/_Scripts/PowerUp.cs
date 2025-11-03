using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUp : MonoBehaviour {

    [Header("Inscribed")]
    public Vector2 rotMinMax = new Vector2(15, 90);
    public Vector2 driftMinMax = new Vector2(.25f, 2);
    public float lifeTime = 6f;
    public float fadeTime = 4f;

    [Header("Dynamic")]
    public eWeaponType type;

    private float birthTime;
    private Rigidbody rb;
    private TextMesh letter;
    private Vector3 rotPerSecond;

    void Awake() {
        rb = GetComponent<Rigidbody>();

        Vector3 vel = Vector3.down;
        vel.y *= Random.Range(driftMinMax.x, driftMinMax.y);
        rb.velocity = vel;

        rotPerSecond = new Vector3(
            Random.Range(rotMinMax.x, rotMinMax.y),
            Random.Range(rotMinMax.x, rotMinMax.y),
            Random.Range(rotMinMax.x, rotMinMax.y)
        );

        birthTime = Time.time;
    }

    void Update() {
        transform.Rotate(rotPerSecond * Time.deltaTime);

        float u = (Time.time - birthTime) / lifeTime;
        if (u > 1) {
            Destroy(gameObject);
        }
    }

    public void SetType(eWeaponType wt) {
        type = wt;
        WeaponDefinition def = Main.GET_WEAPON_DEFINITION(type);

        Renderer rend = GetComponent<Renderer>();
        if (rend != null) {
            rend.material.color = def.color;
        }

        Transform letterT = transform.Find("letter");
        if (letterT != null) {
            letter = letterT.GetComponent<TextMesh>();
            letter.text = def.letter;
        }
    }

    void OnTriggerEnter(Collider other) {
        Hero hero = other.GetComponent<Hero>();
        if (hero != null) {
            hero.AbsorbPowerUp(this);
            Destroy(gameObject);
        }
    }
}
