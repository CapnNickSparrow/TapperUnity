using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BarTap : MonoBehaviour
{
    // Ints
    public int TapIndex;
    
    // Bools
    public bool IsPlayerAtTap;

    public bool IsFlipped;

    // Start is called before the first frame update
    void Start()
    {
        // Renders the Tap Sprite
        SpriteRenderer renderer = GetComponent<SpriteRenderer>();

        IsFlipped = renderer.flipX;
        
        // Player isn't standard at the tap
        IsPlayerAtTap = false;
    }

    // Get the Location of the Vector
    public Vector3 GetShiftPositionVector()
    {
        BoxCollider2D tapCollider = GetComponent<BoxCollider2D>();
        Vector3 positionWithOffset = this.transform.position;
        positionWithOffset.x += tapCollider.offset.x;
        positionWithOffset.y += tapCollider.offset.y;

        return positionWithOffset;
    }
}
