using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Snowball : MonoBehaviour
{
    public GameObject ownerPlayer; 
    private Collider2D snowballCollider;
    public AudioClip collisionSound;
    public AudioClip snowballBlock;
    public AudioClip snowballBreak;

    void Start()
    {
        snowballCollider = GetComponent<Collider2D>();

        if (ownerPlayer != null)
        {
            IgnoreOwnerCollision();
        }
    }

    //set owner of the snowball to ignore collisions with your own snowball
    public void SetOwner(GameObject player)
    {
        ownerPlayer = player;
        if (snowballCollider == null)
        {
            snowballCollider = GetComponent<Collider2D>();
        }
        IgnoreOwnerCollision();
    }

    //helper for collision avoidance
    private void IgnoreOwnerCollision()
    {
        Collider2D ownerCollider = ownerPlayer.GetComponent<Collider2D>();
        if (ownerCollider != null)
        {
            Physics2D.IgnoreCollision(snowballCollider, ownerCollider);
        }
    }

    //normal snowball collisions
    void OnCollisionEnter2D(Collision2D collision)
{
    if (collision.gameObject == ownerPlayer)
    {
        Physics2D.IgnoreCollision(collision.collider, snowballCollider);
        return;
    }

    //collision with another snowball
    Snowball otherSnowball = collision.gameObject.GetComponent<Snowball>();
    if (otherSnowball != null)
    {
        HandleSnowballCollision(otherSnowball);
        return;
    }

    //collision with opposing players
    if ((ownerPlayer.CompareTag("Player1") && collision.gameObject.CompareTag("Player2")) ||
        (ownerPlayer.CompareTag("Player2") && collision.gameObject.CompareTag("Player1")))
    {
        //apply knockback to hit player
        Rigidbody2D enemyRb = collision.gameObject.GetComponent<Rigidbody2D>();
        if (enemyRb != null)
        {
            Vector2 knockbackDirection = (collision.gameObject.transform.position - transform.position).normalized;
            float knockbackForce = 10f;
            enemyRb.AddForce(knockbackDirection * knockbackForce, ForceMode2D.Impulse);
        }

        //notify the hit player that they lost their snowball (if they have one)
        SnowballLauncher hitPlayerLauncher = collision.gameObject.GetComponent<SnowballLauncher>();
        if (hitPlayerLauncher != null && hitPlayerLauncher.GetCurrentSnowball() != null)
        {
            hitPlayerLauncher.NotifySnowballLost();
        }

        //notify SnowballLauncher that this snowball is lost
        SnowballLauncher launcher = ownerPlayer.GetComponent<SnowballLauncher>();
        if (launcher != null)
        {
            launcher.NotifySnowballLost();
        }
        AudioManager.instance.PlaySound(collisionSound);
        Destroy(gameObject); //destroy this snowball after collision
    }
}

void HandleSnowballCollision(Snowball otherSnowball)
{
    //snowball to snowball collision (bigger ones overpower smaller ones)
    float thisSnowballSize = transform.localScale.x;
    float otherSnowballSize = otherSnowball.transform.localScale.x;
    float sizeDifferenceThreshold = 0.25f; // 25% size difference threshold

    if (thisSnowballSize >= otherSnowballSize * (1 + sizeDifferenceThreshold))
    {
        //if this snowball is significantly larger, destroy the other one
        NotifySnowballLost(otherSnowball.ownerPlayer);
        AudioManager.instance.PlaySound(snowballBreak);
        Destroy(otherSnowball.gameObject); //destroy other (smaller) snowball
    }
    else if (otherSnowballSize >= thisSnowballSize * (1 + sizeDifferenceThreshold))
    {
        //other snowball is significantly larger, destroy this snowball
        NotifySnowballLost(ownerPlayer);
        AudioManager.instance.PlaySound(snowballBreak);
        Destroy(gameObject); //destroy this (smaller) snowball
    }
    else
    {
        //break both snowballs if close in size
        AudioManager.instance.PlaySound(snowballBlock);
        NotifyBothSnowballsLost(otherSnowball);
    }
}

//helper to know to regain harvest ability on single snowball lost
void NotifySnowballLost(GameObject player)
{
    SnowballLauncher launcher = player.GetComponent<SnowballLauncher>();
    if (launcher != null)
    {
        launcher.NotifySnowballLost();
    }
}

//handles double snowball (breaking) collision, destroy both snowballs
void NotifyBothSnowballsLost(Snowball otherSnowball)
{
    NotifySnowballLost(ownerPlayer);
    NotifySnowballLost(otherSnowball.ownerPlayer);
    Destroy(gameObject);
    Destroy(otherSnowball.gameObject);
}

}





