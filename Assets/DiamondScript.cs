using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DiamondScript : MonoBehaviour
{
    public CapsuleCollider2D CowCollider;
    
    public event EventHandler CowTouched;
    private CapsuleCollider2D Collider;
    
    // Start is called before the first frame update
    private void Start()
    {
        Collider = GetComponent<CapsuleCollider2D>();

        var euler = transform.eulerAngles;
	    euler.z = UnityEngine.Random.Range(0.0f, 360.0f);
	    transform.eulerAngles = euler;
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

    private void FixedUpdate()
    {
        var euler = transform.eulerAngles;
	    euler.z += 1f;
	    transform.eulerAngles = euler;
    }
}
