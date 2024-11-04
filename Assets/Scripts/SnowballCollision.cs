using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnowballCollision : MonoBehaviour
{
    public float knockbackMultiplier = 1f; //snowball knockback
    public float stunDuration = 0.5f;       //duration of stun
    public GameObject ownerPlayer;          //player that created the snowball reference

    //calculates player knockback and resets velocity to avoid stacking hits
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject == ownerPlayer)
        {
            return;
        }

        if (collision.gameObject.CompareTag("Player1") || collision.gameObject.CompareTag("Player2"))
        {
            GameObject player = collision.gameObject;
            Rigidbody2D playerRb = player.GetComponent<Rigidbody2D>();
            PlayerMovement playerMovement = player.GetComponent<PlayerMovement>();

            if (playerRb != null && playerMovement != null)
            {
                float snowballSize = transform.localScale.x;
                playerRb.velocity = Vector2.zero;
                Vector2 knockbackDirection = (player.transform.position - transform.position).normalized;
                float knockbackForce = knockbackMultiplier * Mathf.Pow(snowballSize, 2);
                playerRb.AddForce(knockbackDirection * knockbackForce, ForceMode2D.Impulse);

                //apply stun effect to the player and reset velocity corotine after a short delay
                playerMovement.ApplyStun(stunDuration);
                StartCoroutine(DelayedResetPlayerVelocity(playerRb, 0.1f));
            }

            //double check destroying snowball after applying knockback
            Destroy(gameObject);
        }
    }

    //helper to reset the player's velocity after a delay
    private IEnumerator DelayedResetPlayerVelocity(Rigidbody2D playerRb, float delay)
    {
        yield return new WaitForSeconds(delay);
        playerRb.velocity = Vector2.zero; 
    }
}








