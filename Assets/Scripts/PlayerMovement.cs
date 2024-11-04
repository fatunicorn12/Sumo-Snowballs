using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 5f;
    public Rigidbody2D rb;

    private Vector2 movement;
    private SnowballLauncher snowballLauncher;
    private float slowDownMultiplier = 0.5f; //slows player down more as time passes
    private bool isStunned = false; 
    private float stunEndTime = 0f;  //tracks when stun should end

    void Start()
    {
        snowballLauncher = GetComponent<SnowballLauncher>();
    }

    void Update()
    {
        if (isStunned)
        {
            //if stunned, check if stun duration has ended
            if (Time.time >= stunEndTime)
            {
                isStunned = false;  //end stun, restore movement
            }
            else
            {
                return; //skip movement input if player is still stunned
            }
        }

        //movement inputs for players
        if (gameObject.name == "Player 1")
        {
            movement.x = Input.GetAxisRaw("Horizontal");
            movement.y = Input.GetAxisRaw("Vertical");
        }
        else if (gameObject.name == "Player 2")
        {
            movement.x = Input.GetAxisRaw("Horizontal2");
            movement.y = Input.GetAxisRaw("Vertical2");
        }

        //if player is moving, grow the snowball and store direction
        if (movement != Vector2.zero)
        {
            snowballLauncher.GrowSnowball(movement); //only grow snowball when moving
        }
    }

    void FixedUpdate()
    {
        if (!isStunned)
        {
            //default movement state/rotation of player 
            rb.MovePosition(rb.position + movement * moveSpeed * Time.fixedDeltaTime);
            RotatePlayer();
        }
    }

    //rotate player in the direction of movement, only if they are moving
    void RotatePlayer()
    {
        if (movement.magnitude > 0.1f)
        {
            float angle = Mathf.Atan2(movement.y, movement.x) * Mathf.Rad2Deg;
            rb.rotation = angle;
        }
    }

    public void AdjustMovementSpeed(float snowballSize)
    {
        //slow player speed based on the size of the snowball
        slowDownMultiplier += Time.deltaTime * 0.05f;
        moveSpeed = Mathf.Max(0.5f, 5f / (1f + (snowballSize - 1f) * slowDownMultiplier));
    }

    //default speed set after snowball is lost
    public void ResetSlowDownMultiplier()
    {
        slowDownMultiplier = 0.2f;
    }

    //applies stun to hit player
    public void ApplyStun(float duration)
    {
        isStunned = true;
        stunEndTime = Time.time + duration;
    }
}

