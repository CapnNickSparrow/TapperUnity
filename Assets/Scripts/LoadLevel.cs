using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadLevel : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        // Sets the Score Palette to true
        GameManager.instance.ScorePal.SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {
        // If User presses Spacebar and GameMode is Singeplayer then set Score Palette to inactive and start the game with Singeplayer
        if (Input.GetKeyDown(KeyCode.Space) && GameManager.instance.TwoPlayer == false)
        {
            GameManager.instance.ScorePal.SetActive(false);
            GameManager.instance.StartGame(isTwoPlayer: false);    
        }
        
        // If User presses Spacebar and GameMode is Multiplayer then set Score Palette to inactive and start the game with Multiplayer
        else if (Input.GetKeyDown(KeyCode.Space) && GameManager.instance.TwoPlayer == true)
        {
            GameManager.instance.ScorePal.SetActive(false);
            GameManager.instance.StartGame(isTwoPlayer: true); 
        }
    }
}
