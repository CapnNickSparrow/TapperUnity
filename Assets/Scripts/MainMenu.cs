using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Analytics;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    // Connects to 
    private AudioSource Tip;
    
    private Animator animator;
    
    //  Timing Values Floats
    private float ShowAfter;
    private float RemoveAfter;

    // Bools
    private bool NoMenu;
    private bool HasntPressed = true;
    
    void Start()
    {
        // Set NoMenu to false so Menu can Open
        NoMenu = true;
        
        // Set Menu Quickly Active for a Second and then turn it off to get info from the Mixer
        GameManager.instance.Menu.gameObject.SetActive(true);
        GameManager.instance.Menu.gameObject.SetActive(false);
        GameManager.instance.MenuRef.gameObject.SetActive(true);

        // Sets the Timing Values to set Objects Active and Inactive
        ShowAfter = Constants.BEFORE_SHOW / Constants.SPEED / Constants.SAMPLES;
        RemoveAfter = Constants.AFTER_SHOW / Constants.SPEED / Constants.SAMPLES;
        
        // Find the Tip Audio Source
        Tip = GameObject.Find("IntroBackground").GetComponent<AudioSource>();
        
        // Loop Highscore Screen Start
        StartCoroutine("Loopscore");
        
        // Connects to the Animator
        animator = GetComponent<Animator>();
        
        // Gets the Highscore information
        GameManager.instance.GetHighScore();
    }

    // Update is called once per frame
    void Update()
    {
        // If User pressed 1, 1 Player is true, if press 2, 2 Player is true
        bool player1Pressed = Input.GetButtonUp("Player1Start");
        bool player2Pressed = Input.GetButtonUp("Player2Start");

        // Will run the PlayGame1 or 2P with sound effects to according Bool
        if (player1Pressed && HasntPressed && NoMenu)
        {
            HasntPressed = false;
            Tip.Play();
            Invoke("PlayGame1P", Constants.WAIT_1_SEC);
        }
        else if (player2Pressed && HasntPressed && NoMenu)
        {
            HasntPressed = false;
            Tip.Play();
            Invoke("PlayGame2P", Constants.WAIT_1_SEC);
        }
        
        // The Player Press "O" or "H" the menu overlay will pop up with settings and set NoMenu to false, so the User can't select amount of players when he is in the menu 
        if ((Input.GetKeyDown(KeyCode.O) || Input.GetKeyDown(KeyCode.H)) && NoMenu)
        {
            OpenMenu();
        }
        
        // The Player Press "O" or "H" the menu overlay will pop down with settings and set NoMenu to true, so the User can't select amount of players
        else if ((Input.GetKeyDown(KeyCode.O) || Input.GetKeyDown(KeyCode.H)) && !NoMenu)
        {
            CloseMenu();
        }
    }

    // Set the Tip Audio to stop and sets the Menu Object to inactive and Loads the Score Palette with the TwoPlayer variable to false
    private void PlayGame1P()
    {
        Tip.Stop();
        GameManager.instance.SetHighScoreInactive();
        GameManager.instance.MenuRef.gameObject.SetActive(false);
        GameManager.instance.TwoPlayer = false;
        SceneManager.LoadScene("ScorePal");
    }
    
    // Set the Tip Audio to stop and sets the Menu Object to inactive and Loads the Score Palette with the TwoPlayer variable to true
    private void PlayGame2P()
    {
        Tip.Stop();
        GameManager.instance.SetHighScoreInactive();
        GameManager.instance.MenuRef.gameObject.SetActive(false);
        GameManager.instance.TwoPlayer = true;
        SceneManager.LoadScene("ScorePal");
    }
    
    // Loops highscore properly, with some Math I calculated the exact time when the HighScore Dia Pop Up to set the HighScore Active on the Same time as the Dia is Active
    IEnumerator Loopscore()
    {
        yield return new WaitForSeconds(ShowAfter);
        GameManager.instance.SetHighScoreActive();
        yield return new WaitForSeconds(RemoveAfter);
        GameManager.instance.SetHighScoreInactive();
        StartCoroutine(Loopscore());
    }

    // Opens up the Menu Items and set Reference Information to false
    private void OpenMenu()
    {
        GameManager.instance.Menu.gameObject.SetActive(true);
        GameManager.instance.MenuRef.gameObject.SetActive(false);
        NoMenu = false;
    }
    
    // Closes up the Menu Items and set Reference Information to true
    private void CloseMenu()
    {
        GameManager.instance.Menu.gameObject.SetActive(false);
        GameManager.instance.MenuRef.gameObject.SetActive(true);
        NoMenu = true;
    }
}
