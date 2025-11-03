using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoundsCheck : MonoBehaviour
{
    // ---------- INSCRIBED ----------
    [Header("Inscribed")]
    public float radius = 1f;
    public bool keepOnScreen = true;
    public float camWidth;
    public float camHeight;

    // ---------- DYNAMIC ----------
    [Header("Dynamic")]
    public bool isOnScreen = true;
    public bool offRight, offLeft, offUp, offDown;

    // clamp style
    public enum eType { center, inset, outset }
    public eType boundsType = eType.center;

    // IMPORTANT: these names match your other scripts exactly
    public enum eScreenLocs { OnScreen, offRight, offLeft, offUp, offDown }

    // ---------- SINGLETON ----------
    public static BoundsCheck S;

    void Awake()
    {
        if (S == null) S = this;
        else Destroy(this);

        camHeight = Camera.main.orthographicSize;
        camWidth = camHeight * Camera.main.aspect;
    }

    void LateUpdate()
    {
        Vector3 pos = transform.position;
        isOnScreen = true;
        offRight = offLeft = offUp = offDown = false;

        switch (boundsType)
        {
            case eType.inset:
                if (pos.x > camWidth - radius) { isOnScreen = false; offRight = true; }
                if (pos.x < -camWidth + radius) { isOnScreen = false; offLeft  = true; }
                if (pos.y > camHeight - radius){ isOnScreen = false; offUp    = true; }
                if (pos.y < -camHeight + radius){isOnScreen = false; offDown  = true; }
                break;

            case eType.outset:
                if (pos.x > camWidth + radius) { isOnScreen = false; offRight = true; }
                if (pos.x < -camWidth - radius){ isOnScreen = false; offLeft  = true; }
                if (pos.y > camHeight + radius){ isOnScreen = false; offUp    = true; }
                if (pos.y < -camHeight - radius){isOnScreen = false; offDown  = true; }
                break;

            default: // center
                if (pos.x > camWidth)          { pos.x = camWidth;  isOnScreen = false; offRight = true; }
                if (pos.x < -camWidth)         { pos.x = -camWidth; isOnScreen = false; offLeft  = true; }
                if (pos.y > camHeight)         { pos.y = camHeight; isOnScreen = false; offUp    = true; }
                if (pos.y < -camHeight)        { pos.y = -camHeight;isOnScreen = false; offDown  = true; }
                break;
        }

        if (keepOnScreen && !isOnScreen)
        {
            transform.position = pos;
            isOnScreen = true;
            offRight = offLeft = offUp = offDown = false;
        }
    }

    // this is what Enemy / ProjectileHero call
    public bool LocIs(eScreenLocs checkLoc)
    {
        switch (checkLoc)
        {
            case eScreenLocs.OnScreen: return isOnScreen;
            case eScreenLocs.offRight: return offRight;
            case eScreenLocs.offLeft:  return offLeft;
            case eScreenLocs.offUp:    return offUp;
            case eScreenLocs.offDown:  return offDown;
        }
        return false;
    }

    void OnDrawGizmos()
    {
        if (!Application.isPlaying) return;
        Vector3 boundSize = new Vector3(camWidth * 2, camHeight * 2, 0.1f);
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(Vector3.zero, boundSize);
    }
}
