using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{

    public float MOVE_SPEED = 5f;

    public Rigidbody2D RIGID_BODY;
    public Animator ANIMATOR;
    public ConsoleView CONSOLE;

    Vector2 MOVEMENT;

    // Update is called once per frame
    void Update() // Input
    {
        if (!CONSOLE.VIEW_CONTAINER.activeSelf)
        {
            MOVEMENT.x = Input.GetAxisRaw("Horizontal");
            MOVEMENT.y = Input.GetAxisRaw("Vertical");

            /*  TO-DO
             *  Add way to remember direction so the character doesn't face forward after moving backwards?
             */

            ANIMATOR.SetFloat("Horizontal", MOVEMENT.x);
            ANIMATOR.SetFloat("Vertical", MOVEMENT.y);
            ANIMATOR.SetFloat("Speed", MOVEMENT.sqrMagnitude);
        }
    }

    private void FixedUpdate() // Movement
    {
        RIGID_BODY.MovePosition(RIGID_BODY.position + (MOVEMENT * MOVE_SPEED * Time.fixedDeltaTime));
    }

}
