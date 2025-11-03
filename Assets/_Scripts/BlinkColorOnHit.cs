using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlinkColorOnHit : MonoBehaviour {
    [Header("Inscribed")]
    public Color blinkColor = Color.red;
    public float blinkDuration = 0.1f;

    [Header("Dynamic")]
    public bool showingColor = false;
    public float blinkCompleteTime;
    public bool ignoreOnCollisionEnter = false;

    private Material[] materials;
    private Color[] originalColors;
    private BoundsCheck bndCheck;

    void Awake() {
        Renderer[] rends = GetComponentsInChildren<Renderer>();
        materials = new Material[rends.Length];
        originalColors = new Color[rends.Length];
        for (int i = 0; i < rends.Length; i++) {
            materials[i] = rends[i].material;
            originalColors[i] = materials[i].color;
        }
        bndCheck = GetComponent<BoundsCheck>();
    }

    void Update() {
        if (showingColor && Time.time > blinkCompleteTime) {
            for (int i = 0; i < materials.Length; i++) {
                materials[i].color = originalColors[i];
            }
            showingColor = false;
        }
    }

    void OnCollisionEnter(Collision coll) {
        if (ignoreOnCollisionEnter) return;

        ProjectileHero p = coll.gameObject.GetComponent<ProjectileHero>();
        if (p != null) {
            SetColors();
        }
    }

    public void SetColors() {
        for (int i = 0; i < materials.Length; i++) {
            materials[i].color = blinkColor;
        }
        showingColor = true;
        blinkCompleteTime = Time.time + blinkDuration;
    }
}
