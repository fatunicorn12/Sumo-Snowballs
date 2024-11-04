using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnowballLauncher : MonoBehaviour
{
    public KeyCode launchKey;   //harvest/launch keys
    public KeyCode harvestKey; 
    public float baseLaunchForce = 5f;  //launch force
    public float snowballGrowthRate = 0.1f;
    public float maxSnowballSize = 2f;
    public GameObject snowballPrefab;
    public int requiredPresses = 10;  //harvest buttons needed to be pressed for new snowball

    private GameObject snowball;
    private Snowball currentSnowball;  //references Snowball script on the spawned snowball
    private float snowballScale = 1f;
    private float snowballOffsetDistance = 1.5f; //offset in front of player
    private PlayerMovement playerMovement;
    private Vector2 lastMovementDirection = Vector2.right;
    private bool hasSnowball = true; 
    private int currentPressCount = 0;  //current harvest key presses

    public delegate void SnowballLostHandler();
    public event SnowballLostHandler OnSnowballLost; //called when snowball is lost

    void Start()
    {
        playerMovement = GetComponent<PlayerMovement>();
    }

    void Update()
{
    //store movements for each player
    Vector2 currentMovementDirection = Vector2.zero;
    
    if (gameObject.name == "Player 1")
    {
        currentMovementDirection = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
    }
    else if (gameObject.name == "Player 2")
    {
        currentMovementDirection = new Vector2(Input.GetAxis("Horizontal2"), Input.GetAxis("Vertical2"));
    }

    //only update snowball position if snowball exists and the player is moving and set snowball's position based on player position/movement
    if (snowball != null)
    {
        if (currentMovementDirection != Vector2.zero)
        {
            lastMovementDirection = currentMovementDirection.normalized;
            snowball.transform.position = transform.position + (Vector3)lastMovementDirection * snowballOffsetDistance;

            //rotating snowball sprite
            Transform spriteTransform = snowball.GetComponentInChildren<SpriteRenderer>().transform;
            if (spriteTransform != null)
            {
                float rotationSpeed = 100f * snowballScale; //rotate faster as snowball grows, and different directions
                float direction = lastMovementDirection.x > 0 ? -1 : 1;
                spriteTransform.Rotate(Vector3.forward, direction * rotationSpeed * Time.deltaTime);
            }
        }
        else
        {
            //stop snowball in place
            snowball.transform.position = transform.position + (Vector3)lastMovementDirection * snowballOffsetDistance;
        }
    }

    //handles snowball launching if the player has one to throw
    if (Input.GetKeyDown(launchKey) && snowball != null)
    {
        LaunchSnowball();
    }

    //call handle harvesting if the player does not have a snowball
    if (!hasSnowball)
    {
        HandleHarvesting();
    }
}

//handles snowball growth and cumulative kncokback
   public void GrowSnowball(Vector2 movementDirection)
{
    //only increase snowball size only if it's not at max size
    if (snowball != null && snowballScale < maxSnowballSize)
    {
        snowballScale += snowballGrowthRate * Time.deltaTime;
        snowball.transform.localScale = Vector3.one * snowballScale;

        //update the knockback multiplier based on snowball size
        SnowballCollision snowballCollision = snowball.GetComponent<SnowballCollision>();
        if (snowballCollision != null)
        {
            snowballCollision.knockbackMultiplier = Mathf.Pow(snowballScale, 2);
        }

        //only rotate the sprite if the player is moving, redundant path to handle snowball growth
        if (movementDirection != Vector2.zero)
        {
            Transform spriteTransform = snowball.GetComponentInChildren<SpriteRenderer>().transform;

            if (spriteTransform != null)
            {
                float baseRotationSpeed = 100f;
                float rotationSpeed = baseRotationSpeed / snowballScale;
                float direction = movementDirection.x > 0 ? -1 : 1;
                spriteTransform.Rotate(Vector3.forward, direction * rotationSpeed * Time.deltaTime);
            }
            //update to handle last moved direction
            lastMovementDirection = movementDirection;
        }
        else
        {
            //reset lastMovementDirection if there is no movement
            lastMovementDirection = Vector2.zero;
        }
    }

    //slow down player as snowball grows
    playerMovement.AdjustMovementSpeed(snowballScale);
}


//handles snowball launching, detaching from player, adding launch force dynamically, and resetting the player/snowball after launch
    void LaunchSnowball()
{
    if (snowball == null) return;

    Rigidbody2D snowballRb = snowball.GetComponent<Rigidbody2D>();
    snowball.transform.parent = null; //detach snowball from player

    //launch force based on the snowball's scale, more force as it grows
    float launchForce = baseLaunchForce * Mathf.Pow(snowballScale, 2);
    snowballRb.AddForce(lastMovementDirection.normalized * launchForce, ForceMode2D.Impulse);

    //reset slowdown multiplier after launch
    playerMovement.ResetSlowDownMultiplier();

    //reset snowball and player state
    snowball = null;
    hasSnowball = false;
    currentPressCount = 0;

    OnSnowballLost?.Invoke();
}

//handles snowball creation, and reseting multipliers, ensuring that only one snowball can exist
    void CreateSnowball()
{
    if (currentSnowball != null)
    {
        Destroy(currentSnowball.gameObject);
    }

    Vector3 spawnPosition = transform.position + (Vector3)lastMovementDirection.normalized * snowballOffsetDistance;
    GameObject newSnowballObject = Instantiate(snowballPrefab, spawnPosition, Quaternion.identity, transform);

    currentSnowball = newSnowballObject.GetComponent<Snowball>();
    currentSnowball.SetOwner(this.gameObject);

    snowballScale = 1f;
    currentSnowball.transform.localScale = Vector3.one * snowballScale;

    SnowballCollision snowballCollision = newSnowballObject.GetComponent<SnowballCollision>();
    if (snowballCollision != null)
    {
        snowballCollision.knockbackMultiplier = 1f;
    }

    snowball = newSnowballObject;
    hasSnowball = true;
    currentPressCount = 0;
}


//handles harvest, checking against required presses
    void HandleHarvesting()
    {
        if (Input.GetKeyDown(harvestKey))
        {
            currentPressCount++;

            if (currentPressCount >= requiredPresses)
            {
                CreateSnowball();
            }
        }
    }

//helper reference to players spawned snowball
    public GameObject GetCurrentSnowball()
    {
        return snowball;
    }
    public void SetNewSnowball(GameObject newSnowball)
{
    snowball = newSnowball;
    currentSnowball = snowball.GetComponent<Snowball>();
    currentSnowball.SetOwner(this.gameObject);
    hasSnowball = true;
    playerMovement.ResetSlowDownMultiplier();

    snowballScale = 1f;
    snowball.transform.localScale = Vector3.one;
}

//event invoked to tell the game a player no longer has a snowball, and resets harvest count
    public void NotifySnowballLost()
{
    if (snowball != null)
    {
        Destroy(snowball);  //destroys the current snowball
        snowball = null;     //removes reference to the snowball
        hasSnowball = false;
        currentPressCount = 0; //reset harvest press count
        playerMovement.ResetSlowDownMultiplier(); 
    }
    OnSnowballLost?.Invoke();  // Invoke the event
}

}
