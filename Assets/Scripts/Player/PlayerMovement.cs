using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class PlayerMovement : MonoBehaviour
{
    public float Speed;
    public float MaxSpeed;

    float currentSpeed;
    [Space]
    public Joystick joystick;
    public Animator anim;
    Rigidbody2D rb;
    Vector2 moveDir;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        #if UNITY_EDITOR_WIN

        Debug.Log("[Player Movement] : You can use the W,A,S,D to move since you are on PC");

        #endif
    }

    // Update is called once per frame
    void Update()
    {
        GetInput();

        anim.SetBool("Moving", moveDir != Vector2.zero);

        if(moveDir.x < 0)
        {
            //transform.localScale = new Vector3(-Math.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        }
        else
        {
            //transform.localScale = new Vector3(Math.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        }
    }

    private void FixedUpdate() {
        DoMovement();
    }

    void GetInput()
    {
        moveDir = joystick.Direction;

        #if UNITY_EDITOR_WIN

        moveDir.x += Input.GetAxisRaw("Horizontal");
        moveDir.y += Input.GetAxisRaw("Vertical");

        moveDir.x = Mathf.Clamp(moveDir.x, -1.0f, 1.0f);
        moveDir.y = Mathf.Clamp(moveDir.y, -1.0f, 1.0f);

        #endif
    }

    void DoMovement()
    {
        //currentSpeed = Mathf.MoveTowards(currentSpeed, Speed, acceleration * Time.deltaTime);
        //var delta = joystick.Direction * currentSpeed * Time.deltaTime;
        //transform.position += new Vector3(delta.x, delta.y, 0);

        rb.MovePosition(rb.position + (moveDir * (Speed * Time.deltaTime)));
    }

}
