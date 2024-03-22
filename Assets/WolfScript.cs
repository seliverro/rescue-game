using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class WolfScript : MonoBehaviour
{
    public Rigidbody2D Rigidbody2D;
    public Vector2 Direction; 
    public float Speed;
    public GameObject Cow;
    public event EventHandler CowTouched;
    
    private float LeftConstraint;
    private float RightConstraint;
    private float TopConstraint;
    private float BottomConstraint;

    private const float _BUFFER = 0.1f;
    private PolygonCollider2D Collider; 
    private CircleCollider2D CowCollider;
    public Func<bool> GetPausedStatus;

    // Start is called before the first frame update
    private void Start()
    {
        Collider = GetComponent<PolygonCollider2D>();
        CowCollider = Cow.GetComponent<CircleCollider2D>();
        
        LeftConstraint = Camera.main.ScreenToWorldPoint( new Vector3(0.0f, 0.0f) ).x;
        RightConstraint = Camera.main.ScreenToWorldPoint( new Vector3(Screen.width, 0.0f) ).x;
        
        BottomConstraint = Camera.main.ScreenToWorldPoint( new Vector3(0.0f, 0.0f) ).y;
        TopConstraint = Camera.main.ScreenToWorldPoint( new Vector3(0.0f, Screen.height ) ).y;
    }

    // Update is called once per frame
    private void Update()
    {
        if (CowCollider.bounds.Intersects(Collider.bounds))
        {
            CowTouched?.Invoke(this, EventArgs.Empty);
        }
    }

    private void FixedUpdate()
    {
        if (GetPausedStatus())
            return; 
        
        Rigidbody2D.MovePosition(Rigidbody2D.position + Direction * (Speed * Time.fixedDeltaTime));
        
        if (Rigidbody2D.position.x < LeftConstraint - _BUFFER) { 
            Rigidbody2D.MovePosition(new Vector2(RightConstraint, Rigidbody2D.position.y));
        }

        if (Rigidbody2D.position.x > RightConstraint + _BUFFER) {
            Rigidbody2D.MovePosition(new Vector2(LeftConstraint, Rigidbody2D.position.y));
        }
        
        if (Rigidbody2D.position.y > TopConstraint + _BUFFER) {
            Rigidbody2D.MovePosition(new Vector2(Rigidbody2D.position.x, BottomConstraint));
        }
        
        if (Rigidbody2D.position.y < BottomConstraint - _BUFFER) {
            Rigidbody2D.MovePosition(new Vector2(Rigidbody2D.position.x, TopConstraint));
        }
    }

    public void SetDirection(Vector2 direction)
    {
        Direction = direction;
        
        var euler = transform.eulerAngles;
	    euler.z = UnityEngine.Random.Range(0.0f, 360.0f);
	    transform.eulerAngles = euler;

        if ( Direction.x > 0)
        {
            transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
        }
    } 
}
