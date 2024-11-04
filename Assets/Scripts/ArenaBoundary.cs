using UnityEngine;
using System.Collections;

public class ArenaBoundary : MonoBehaviour
{
    public GameManager gameManager;
    public float respawnDelay = 2f;  // delay before respawning players

    private bool isKnockoutInProgress = false;  //prevents double scoring

    private void OnTriggerExit2D(Collider2D collision)
    {
        //skip if knockback is already in progress
        if (isKnockoutInProgress) return;

        //handles players exiting the arena
        if (collision.CompareTag("Player1"))
        {
            gameManager.Player2KnockedOutPlayer1();  //player 2 scores a point
            isKnockoutInProgress = true; 
            StartCoroutine(RespawnPlayers(collision.gameObject, "Player2"));
        }
        else if (collision.CompareTag("Player2"))
        {
            gameManager.Player1KnockedOutPlayer2();  //player 1 scores a point
            isKnockoutInProgress = true; 
            StartCoroutine(RespawnPlayers(collision.gameObject, "Player1"));
        }
    }

    //reset both players and remove snowballs
    private IEnumerator RespawnPlayers(GameObject knockedOutPlayer, string winningPlayerTag)
    {

        GameObject player1 = GameObject.FindWithTag("Player1");
        GameObject player2 = GameObject.FindWithTag("Player2");

        //deactivate the player who was knocked out and remove all snowballs
        knockedOutPlayer.SetActive(false);
        yield return new WaitForSeconds(respawnDelay);
        RemoveAllSnowballs();

        //reset and reactivate both players regardless of who was knocked out
        if (player1 != null)
        {
            ResetPlayer(player1, gameManager.player1StartPos);
        }

        if (player2 != null)
        {
            ResetPlayer(player2, gameManager.player2StartPos);
        }

        isKnockoutInProgress = false;
    }

    //reset player position, harvest state, and allow
    private void ResetPlayer(GameObject player, Transform startPosition)
    {
        player.transform.position = startPosition.position;
        player.SetActive(true);

        Harvest harvest = player.GetComponent<Harvest>();
        if (harvest != null)
        {
            harvest.ResetHarvestState();
        }

        ResetPlayerPhysics(player);
    }

    //destroy all active snowballs in the scene
    private void RemoveAllSnowballs()
    {
        GameObject[] snowballs = GameObject.FindGameObjectsWithTag("Snowball");
        foreach (GameObject snowball in snowballs)
        {
            Destroy(snowball);
        }
    }

    //reset player's velocity and rotation
    private void ResetPlayerPhysics(GameObject player)
    {
        Rigidbody2D rb = player.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.velocity = Vector2.zero;      
            rb.angularVelocity = 0f;       
        }
    }
}










