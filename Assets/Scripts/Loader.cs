using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Loader : MonoBehaviour
{
    // Connect to the GameManager
    public GameObject gameManager;

    // Awake is always called before any Start functions
    void Awake()
    {
        // If there isn't a isn't a Game Manager yet make the Game Manager
        if (GameManager.instance == null)
        {
            Instantiate(gameManager);
        }
    }
}
