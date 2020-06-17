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
    private bool HasntPressed = true;
    void Start()
    {
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

        if (player1Pressed && HasntPressed)
        {
            HasntPressed = false;
            Tip.Play();
            Invoke("PlayGame1P", 1);
        }
        else if (player2Pressed && HasntPressed)
        {
            HasntPressed = false;
            Tip.Play();
            Invoke("PlayGame2P", 1);
        }
    }

    private void PlayGame1P()
    {
        Tip.Stop();
        GameManager.instance.SetHighScoreInactive();
        GameManager.instance.StartGame(isTwoPlayer: false); 
    }
    
    private void PlayGame2P()
    {
        Tip.Stop();
        GameManager.instance.SetHighScoreInactive();
        GameManager.instance.StartGame(isTwoPlayer: true); 
    }
    
    IEnumerator Loopscore()
    {
        yield return new WaitForSeconds(ShowAfter);
        GameManager.instance.SetHighScoreActive();
        yield return new WaitForSeconds(RemoveAfter);
        GameManager.instance.SetHighScoreInactive();
        StartCoroutine(Loopscore());
    }
}
