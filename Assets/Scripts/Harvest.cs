using System.Collections;
using UnityEngine;

public class Harvest : MonoBehaviour
{
    public KeyCode harvestKey; 
    public GameObject snowballPrefab;
    public Transform snowballSpawnPoint;
    public int requiredPresses = 10;

    public AudioClip harvestSound;
    private SnowballLauncher snowballLauncher;
    private bool hasSnowball = false;
    public int currentPressCount = 0;  //public for GameManager access

    private GameManager gameManager;

    void Start()
    {
        snowballLauncher = GetComponent<SnowballLauncher>();
        gameManager = FindObjectOfType<GameManager>();  

        //when the snowball is lost subscribe to event from SnowballLauncher
        snowballLauncher.OnSnowballLost += ResetHarvestState;
    }

    void Update()
    {
        if (!hasSnowball && snowballLauncher.GetCurrentSnowball() == null)
        {
            if (Input.GetKeyDown(harvestKey))
            {
                currentPressCount++;
                AudioManager.instance.PlaySound(harvestSound);
                gameManager.UpdateHarvestUI();

                if (currentPressCount >= requiredPresses)
                {
                    CreateNewSnowball();
                }
            }
        }
    }

    void CreateNewSnowball()
    {
        currentPressCount = 0;

        //spawn new snowball and reset its scale
        GameObject newSnowball = Instantiate(snowballPrefab, snowballSpawnPoint.position, snowballSpawnPoint.rotation);
        newSnowball.transform.localScale = Vector3.one;

        //pass the new snowball to the SnowballLauncher to handle
        snowballLauncher.SetNewSnowball(newSnowball);
        hasSnowball = true;
        gameManager.UpdateHarvestUI();
    }
    
    public void OnSnowballLost()
    {
        ResetHarvestState();
    }

    //resets harvest state
    public void ResetHarvestState()
    {
        hasSnowball = false;
        currentPressCount = 0;
        gameManager.UpdateHarvestUI();
    }
}
