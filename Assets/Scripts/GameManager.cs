﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.XR.Interaction;
using UnityEngine.SceneManagement;

public enum ScoreKey
{
    EmptyMug,
    Tip,
    Customer,
    HardCustomer,
    HarderCustomer,
    LevelFinish
}

public enum GameMode
{
    SinglePlayer,
    TwoPlayer
}

public class GameManager : MonoBehaviour
{
    public static GameManager instance = null;

    public LevelManager levelManager;

    public LevelUIManager levelUIManager;

    private Dictionary<ScoreKey, int> _scoreTable;

    public Dictionary<ScoreKey, int> ScoreTable
    {
        get
        {
            if (_scoreTable == null)
            {
                _scoreTable = new Dictionary<ScoreKey, int>();
                _scoreTable.Add(ScoreKey.EmptyMug, 100);
                _scoreTable.Add(ScoreKey.Tip, 1500);
                _scoreTable.Add(ScoreKey.Customer, 50);
                _scoreTable.Add(ScoreKey.HardCustomer, 75);
                _scoreTable.Add(ScoreKey.HarderCustomer, 100);
                _scoreTable.Add(ScoreKey.LevelFinish, 1000);
            }

            return _scoreTable;
        }
    }
    public int HappyCustomer;

    public GameMode SelectedGameMode;

    public bool HasLevelStarted;
    public bool Oops;
    public bool OopsC;
    
    public int CurrentPlayer = 1;

    public int PlayerOneScore;
    public int PlayerOneCurrentLevel;
    public int PlayerOneLives;


    public int PlayerTwoScore;
    public int PlayerTwoCurrentLevel;
    public int PlayerTwoLives;
    void OnEnable()
    {
        //Tell our 'OnLevelFinishedLoading' function to start listening for a scene change as soon as this script is enabled.
        SceneManager.sceneLoaded += OnLevelFinishedLoading;
    }

    void OnDisable()
    {
        //Tell our 'OnLevelFinishedLoading' function to stop listening for a scene change as soon as this script is disabled.
        SceneManager.sceneLoaded -= OnLevelFinishedLoading;
    }

    void OnLevelFinishedLoading(Scene scene, LoadSceneMode mode)
    {
        if (scene.name != "MainMenuScene")
        {
            HasLevelStarted = false;

            // when a level finishes loading then display the 'get ready to serve' screen for few seconds
            levelUIManager.ToggleReadyToServeImage(displayImage: true);

            StartCoroutine(HideReadyToServeImageAndStartLevel());
        }
    }

    protected IEnumerator HideReadyToServeImageAndStartLevel()
    {
        float displayTime = 2.5f;

        yield return new WaitForSeconds(displayTime);

        levelUIManager.ToggleReadyToServeImage(displayImage: false);
        HasLevelStarted = true;
    }
    
    // Awake is called before the first frame update
    void Awake()
    {
        if (instance == null) 
        {
            instance = this;
        } 
        else if (instance != this) 
        {
            Destroy(gameObject);
        }

        levelManager = GetComponent<LevelManager>();
        levelUIManager = GetComponent<LevelUIManager>();

        DontDestroyOnLoad(gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void StartGame(bool isTwoPlayer)
    {
        if (isTwoPlayer)
        {
            SelectedGameMode = GameMode.TwoPlayer;
        }
        else
        {
            SelectedGameMode = GameMode.SinglePlayer;
        }

        Oops = false;
        OopsC = false;
        
        HappyCustomer = 0;
        
        CurrentPlayer = 1;
        
        levelUIManager.SetPlayerOneScoreText(PlayerOneScore);
        
        levelUIManager.SetPlayerTwoScoreText(PlayerTwoScore);

        PlayerOneCurrentLevel = levelManager.level;
        PlayerTwoCurrentLevel = levelManager.level;
        levelUIManager.SetCurrentLevelText(PlayerOneCurrentLevel);

        PlayerOneLives = 3;
        PlayerTwoLives = 3;

        levelManager.CurrentLevel = levelManager.AllLevels[levelManager.level - 1];

        SceneManager.LoadScene("Level" + levelManager.level);
    }

    internal void AddToCurrentPlayerScore(ScoreKey toAdd)
    {
        if (CurrentPlayer == 1)
        {
            AddToPlayerOneScore(toAdd);
        }
        else if (CurrentPlayer == 2)
        {
            AddToPlayerTwoScore(toAdd);
        }
        if (HappyCustomer >= 20)
        {
            levelManager.level++;
            if (SelectedGameMode == GameMode.TwoPlayer)
            {
                StartGame(isTwoPlayer: true);    
            }
                
            else if (GameManager.instance.SelectedGameMode != GameMode.TwoPlayer)
            {
                StartGame(isTwoPlayer: false);    
            }
        }
    }

    internal void AddToPlayerOneScore(ScoreKey toAdd)
    {
        PlayerOneScore += ScoreTable[toAdd];
        levelUIManager.SetPlayerOneScoreText(PlayerOneScore);
    }

    internal void AddToPlayerTwoScore(ScoreKey toAdd)
    {
        PlayerTwoScore += ScoreTable[toAdd];
        levelUIManager.SetPlayerTwoScoreText(PlayerTwoScore);
    }

    
    public void PlayerLost()
    {
        // Lose a life
        if (CurrentPlayer == 1)
        {
            PlayerOneLives--;
            Debug.Log(string.Format("Player 1 Lives: {0}", PlayerOneLives));
        }
        else
        {
            PlayerTwoLives--;
            Debug.Log(string.Format("Player 2 Lives: {0}", PlayerTwoLives));
        }

        // switch players if two player
        if (SelectedGameMode == GameMode.TwoPlayer)
        {
            SwitchPlayers();
        }

        levelManager.PlayerMissedCustomer = false;
        levelManager.PlayerMissedEmptyMug = false;
        levelManager.PlayerThrewExtraMug = false;

        RestartLevelScene();
    }

    private void SwitchPlayers()
    {
        if (CurrentPlayer == 1)
        {
            CurrentPlayer = 2;
            levelManager.CurrentLevel = levelManager.AllLevels[PlayerTwoCurrentLevel - 1];
        }
        else
        {
            CurrentPlayer = 1;
            levelManager.CurrentLevel = levelManager.AllLevels[PlayerOneCurrentLevel - 1];
        }
    }

    private void RestartLevelScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
