using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tip : MonoBehaviour
{
    private Rigidbody2D rBodyT;
    private BoxCollider2D boxColliderT;
    // Start is called before the first frame update
    void Start()
    {
        boxColliderT = GetComponent<BoxCollider2D>();
        rBodyT = GetComponent<Rigidbody2D>();
    }
}
