using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class CowScript : MonoBehaviour
{
    // Start is called before the first frame update

    private Vector2 Direction;
    private Rigidbody2D Rigidbody2D;
    
    public float Speed;
    [FormerlySerializedAs("IsDead")] public bool Paused;

    [SerializeField] private InputActionReference moveActionToUse;
    
    private void Start()
    {
        Rigidbody2D = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    private void Update()
    {
        if (!Paused)
        {
            Direction = moveActionToUse.action.ReadValue<Vector2>();
            if (Direction == Vector2.zero)
            {
                Direction.x = Input.GetAxisRaw("Horizontal");
                Direction.y = Input.GetAxisRaw("Vertical");
            }
        }
    }

    private void FixedUpdate()
    {
        if (!Paused)
        {
            Rigidbody2D.MovePosition(Rigidbody2D.position + Direction * (Speed * Time.fixedDeltaTime));
        }
    }
}
