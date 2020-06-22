using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Analytics;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    private AudioSource Tip;
    
    private Animator animator;
    
    private float Samples = 60f;
    private float BeforeShow = 22f;
    private float AfterShow = 39f;
    private float Speed = 0.05f;
    private float ShowAfter = 0f;
    private float RemoveAfter = 0f;

    private bool NoMenu = true;
    private bool HasntPressed = true;
    void Start()
    {
        GameManager.instance.MenuRef.gameObject.SetActive(true);
        ShowAfter = BeforeShow / Speed / Samples;
        RemoveAfter = AfterShow / Speed / Samples;
        Tip = GameObject.Find("IntroBackground").GetComponent<AudioSource>();
        StartCoroutine("Loopscore");
        animator = GetComponent<Animator>();
        GameManager.instance.GetHighScore();
    }

    // Update is called once per frame
    void Update()
    {
        bool player1Pressed = Input.GetButtonUp("Player1Start");
        bool player2Pressed = Input.GetButtonUp("Player2Start");

        if (player1Pressed && HasntPressed && NoMenu)
        {
            HasntPressed = false;
            Tip.Play();
            Invoke("PlayGame1P", 1);
        }
        else if (player2Pressed && HasntPressed && NoMenu)
        {
            HasntPressed = false;
            Tip.Play();
            Invoke("PlayGame2P", 1);
        }
        
        if ((Input.GetKeyDown(KeyCode.O) || Input.GetKeyDown(KeyCode.H)) && NoMenu)
        {
            NoMenu = false;
            OpenMenu();
        }

        else if ((Input.GetKeyDown(KeyCode.O) || Input.GetKeyDown(KeyCode.H)) && !NoMenu)
        {
            NoMenu = true;
            CloseMenu();
        }
    }

    private void PlayGame1P()
    {
        Tip.Stop();
        GameManager.instance.SetHighScoreInactive();
        GameManager.instance.MenuRef.gameObject.SetActive(false);
        GameManager.instance.TwoPlayer = false;
        SceneManager.LoadScene("ScorePal");
    }
    
    private void PlayGame2P()
    {
        Tip.Stop();
        GameManager.instance.SetHighScoreInactive();
        GameManager.instance.MenuRef.gameObject.SetActive(false);
        GameManager.instance.TwoPlayer = true;
        SceneManager.LoadScene("ScorePal");
    }
    
    IEnumerator Loopscore()
    {
        yield return new WaitForSeconds(ShowAfter);
        GameManager.instance.SetHighScoreActive();
        yield return new WaitForSeconds(RemoveAfter);
        GameManager.instance.SetHighScoreInactive();
        StartCoroutine(Loopscore());
    }

    private void OpenMenu()
    {
        GameManager.instance.MenuRef.gameObject.SetActive(false);
        GameManager.instance.Menu.gameObject.SetActive(true);
    }
    
    private void CloseMenu()
    {
        GameManager.instance.Menu.gameObject.SetActive(false);
        GameManager.instance.MenuRef.gameObject.SetActive(true);
    }
}
