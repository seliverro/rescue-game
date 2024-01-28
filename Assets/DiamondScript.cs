using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DiamondScript : MonoBehaviour
{
    public CircleCollider2D CowCollider;
    
    public event EventHandler CowTouched;
    private CircleCollider2D Collider;
    
    // Start is called before the first frame update
    private void Start()
    {
        Collider = GetComponent<CircleCollider2D>();
    }

    // Update is called once per frame
    private void Update()
    {
        if (CowCollider.bounds.Intersects(Collider.bounds))
        {
            CowTouched?.Invoke(this, EventArgs.Empty);
            Destroy(gameObject);
        }
    }
}
