using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Analytics;
using UnityEngine.Experimental.XR.Interaction;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

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

    public bool NotDone = false;
    
    public bool IsGameWon;
    public bool LevelWon;
    
    public bool NewHighScoreP1;
    public bool NewHighScoreP2;
    public bool NewHighScoreP1N1;
    public bool NewHighScoreP1N2;
    public bool NewHighScoreP1N3;
    public bool NewHighScoreP2N1;
    public bool NewHighScoreP2N2;
    public bool NewHighScoreP2N3;
    
    public int CurrentPlayer = 1;

    public int PlayerOneScore;
    public int PlayerOneCurrentLevel;
    public int PlayerOneLives;

    private GameObject Player;

    public Text HighScore1;
    public Text HighScore2;
    public Text HighScore3;

    public Text P1S;
    public Text P2S;

    public Text Game;

    public Image BG;

    public InputField P1;
    public InputField P2;
    
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
        CurrentPlayer = 1;
        
        PlayerOneLives = 3;
        PlayerTwoLives = 3;
        
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
        NotDone = false;

        LevelWon = false;
        
        Oops = false;
        OopsC = false;
        
        HappyCustomer = 0;

        levelUIManager.SetPlayerOneScoreText(PlayerOneScore);
        
        levelUIManager.SetPlayerTwoScoreText(PlayerTwoScore);

        PlayerOneCurrentLevel = levelManager.level;
        PlayerTwoCurrentLevel = levelManager.level;
        levelUIManager.SetCurrentLevelText(PlayerOneCurrentLevel);
        

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
        if (HappyCustomer >= 20 && !LevelWon)
        {
            LevelWon = true;
            levelManager.level++;
            AddToCurrentPlayerScore(ScoreKey.LevelFinish);
            StartCoroutine("NextLevel");
        }
    }

    IEnumerator NextLevel ()
    {
        Player = GameObject.Find("Player");
        Player.GetComponent<Player>().Win.Play();
        if (levelManager.level <= 3)
        {
            Player.GetComponent<Animator>().SetBool("hasWon", true);
            yield return new WaitForSeconds(2);
            Player.GetComponent<Animator>().SetBool("hasWon", false);
            if (SelectedGameMode == GameMode.TwoPlayer)
            {
                StartGame(isTwoPlayer: true);    
            }
                
            else if (GameManager.instance.SelectedGameMode != GameMode.TwoPlayer)
            {
                StartGame(isTwoPlayer: false);    
            }
        }
        else
        {
            IsGameWon = true;
            if (CurrentPlayer == 1 && PlayerTwoLives != 0 && SelectedGameMode == GameMode.TwoPlayer)
            {
                Player.GetComponent<Animator>().SetBool("hasWon", true);
                yield return new WaitForSeconds(2);
                Player.GetComponent<Animator>().SetBool("hasWon", false);
                SwitchPlayers();
            }
            else if (CurrentPlayer == 2 && PlayerOneLives != 0 && SelectedGameMode == GameMode.TwoPlayer)
            {
                Player.GetComponent<Animator>().SetBool("hasWon", true);
                yield return new WaitForSeconds(2);
                Player.GetComponent<Animator>().SetBool("hasWon", false);
                SwitchPlayers();
            }
            else
            {
                GameOver();
            }
        }
        yield return null;
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
        levelManager.PlayerMissedCustomer = false;
        levelManager.PlayerMissedEmptyMug = false;
        levelManager.PlayerThrewExtraMug = false;
        
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
            if ((CurrentPlayer == 1 && PlayerOneLives != 0) || (CurrentPlayer == 2 && PlayerOneLives != 0))
            {
                SwitchPlayers();
            }
            else
            {
                GameOver();
            }
        }
        else
        {
            if (PlayerOneLives == 0)
            {
                GameOver();
            }

            else
            {
                RestartLevelScene();
            }
        }
    }

    private void SwitchPlayers()
    {
        if (CurrentPlayer == 1)
        {
            if (PlayerTwoLives != 0)
            {
                HappyCustomer = 0;
                CurrentPlayer = 2;
                levelManager.CurrentLevel = levelManager.AllLevels[PlayerTwoCurrentLevel - 1];   
                RestartLevelScene();
            }
            else
            {
                CurrentPlayer = 1;  
                RestartLevelScene();
            }
        }
        else if (CurrentPlayer == 2)
        {
            if (PlayerOneLives != 0)
            {
                HappyCustomer = 0;
                CurrentPlayer = 1;
                levelManager.CurrentLevel = levelManager.AllLevels[PlayerOneCurrentLevel - 1];   
                RestartLevelScene();
            }
            else
            {
                CurrentPlayer = 2;  
                RestartLevelScene();
            }
        }
    }

    private void RestartLevelScene()
    {
        Oops = false;
        OopsC = false;
        NotDone = false;
        
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    
    private void GameOver()
    { 
        if (PlayerOneScore > PlayerPrefs.GetInt("HighScore3", 0))
        {
            if (PlayerOneScore > PlayerPrefs.GetInt("HighScore2", 0))
            {
                if (PlayerOneScore > PlayerPrefs.GetInt("HighScore1", 0))
                {
                    NewHighScoreP1 = true;
                    NewHighScoreP1N1 = true;
                    PlayerPrefs.SetInt("HighScore1", PlayerOneScore);
                    SceneManager.LoadScene("EndScore");
                    GetPlayerCurrentScore();
                    GetGameEnd();
                }
                else
                {
                    NewHighScoreP1 = true;
                    NewHighScoreP1N2 = true;
                    PlayerPrefs.SetInt("HighScore2", PlayerOneScore);
                    SceneManager.LoadScene("EndScore");
                    GetPlayerCurrentScore();
                    GetGameEnd();
                }
            }
            else
            {
                NewHighScoreP1 = true;
                NewHighScoreP1N3 = true;
                PlayerPrefs.SetInt("HighScore3", PlayerOneScore);
                SceneManager.LoadScene("EndScore");
                GetPlayerCurrentScore();
                GetGameEnd();
            }
        }
        else
        {
            SceneManager.LoadScene("EndScore");
            GetPlayerCurrentScore();
            GetGameEnd();
        }

        if (PlayerTwoScore > PlayerPrefs.GetInt("HighScore3", 0) && SelectedGameMode == GameMode.TwoPlayer)
        {
            if (PlayerTwoScore > PlayerPrefs.GetInt("HighScore2", 0))
            {
                if (PlayerTwoScore > PlayerPrefs.GetInt("HighScore1", 0))
                {
                    NewHighScoreP2 = true;
                    NewHighScoreP2N1 = true;
                    PlayerPrefs.SetInt("HighScore1", PlayerTwoScore);
                    SceneManager.LoadScene("EndScore");
                    GetPlayerCurrentScore();
                    GetGameEnd();

                }
                else
                {
                    NewHighScoreP2 = true;
                    NewHighScoreP2N2 = true;
                    PlayerPrefs.SetInt("HighScore2", PlayerTwoScore);
                    SceneManager.LoadScene("EndScore");
                    GetPlayerCurrentScore();
                    GetGameEnd();
                }
            }
            else
            {
                NewHighScoreP2 = true;
                NewHighScoreP2N3 = true;
                PlayerPrefs.SetInt("HighScore3", PlayerTwoScore);
                SceneManager.LoadScene("EndScore");
                GetPlayerCurrentScore();
                GetGameEnd();
            }
        }
        else
        {
            SceneManager.LoadScene("EndScore");
            GetPlayerCurrentScore();
            GetGameEnd();
        }
    }

    private void GetPlayerCurrentScore()
    {
        SetCurrentScoreActive();
        P1S.text = PlayerOneScore.ToString();
        P2S.text = PlayerTwoScore.ToString();
    }

    private void GetGameEnd()
    {
        Game = GameObject.Find("Game").GetComponent<Text>();
        if (IsGameWon == true)
        {
            Game.text = "Game Won";
        }
        
        else if (IsGameWon == false)
        {
            Game.text = "Game Over";
        }

        else
        {
            Debug.Log("Error Checking Game End Status");
        }

        if (NewHighScoreP1 == false && NewHighScoreP2 == false)
        {
            Invoke("Back2MainMenu", 10);   
        }
    }

    private void Back2MainMenu()
    {
        SetCurrentScoreInactive();
        SceneManager.LoadScene("MainMenuScene");
    }
    
    public void GetHighScore()
    {
        HighScore1.text = "1. " +  PlayerPrefs.GetString("Nr1", "TMP").ToString() + " Score: " + PlayerPrefs.GetInt("HighScore1", 0).ToString();
        HighScore2.text = "2. " +  PlayerPrefs.GetString("Nr2", "TMP").ToString() + " Score: " + PlayerPrefs.GetInt("HighScore2", 0).ToString();
        HighScore3.text = "3. " +  PlayerPrefs.GetString("Nr3", "TMP").ToString() + " Score: " + PlayerPrefs.GetInt("HighScore3", 0).ToString();
    }

    public void SetHighScoreActive()
    {
        HighScore1.gameObject.SetActive(true);
        HighScore2.gameObject.SetActive(true);
        HighScore3.gameObject.SetActive(true);
    }
    
    public void SetHighScoreInactive()
    {
        HighScore1.gameObject.SetActive(false);
        HighScore2.gameObject.SetActive(false);
        HighScore3.gameObject.SetActive(false);

    }
    
    public void SetCurrentScoreActive()
    {
        if (NewHighScoreP1 == true)
        {
            P1.gameObject.SetActive(true);
        }
        if (NewHighScoreP2 == true)
        {
            P2.gameObject.SetActive(true);
        }
        BG.gameObject.SetActive(true);
        Game.gameObject.SetActive(true);
        P1S.gameObject.SetActive(true);
        P2S.gameObject.SetActive(true);
    }
    
    public void SetCurrentScoreInactive()
    {
        if (NewHighScoreP1 == true)
        {
            NewHighScoreP2 = false;
            NewHighScoreP1N1 = false;
            NewHighScoreP1N2 = false;
            NewHighScoreP1N3 = false;
        }
        if (NewHighScoreP2 == true)
        {
            NewHighScoreP2 = false;
            NewHighScoreP2N1 = false;
            NewHighScoreP2N2 = false;
            NewHighScoreP2N3 = false;
        }
        BG.gameObject.SetActive(false);
        Game.gameObject.SetActive(false);
        P1S.gameObject.SetActive(false);
        P2S.gameObject.SetActive(false);
    }

    public void TriggerInput()
    {
        if (NewHighScoreP1N1 == true)
        {
            PlayerPrefs.SetString("Nr1", P1.text);
            P1.gameObject.SetActive(false);
            IsDone();
        }
        else if (NewHighScoreP1N2 == true)
        {
            PlayerPrefs.SetString("Nr2", P1.text);
            P1.gameObject.SetActive(false);
            IsDone();
        }
        else if (NewHighScoreP1N3 == true)
        {
            PlayerPrefs.SetString("Nr3", P1.text);
            P1.gameObject.SetActive(false);
            IsDone();
        }
        if (NewHighScoreP2N1 == true)
        {
            PlayerPrefs.SetString("Nr1", P2.text);
            P2.gameObject.SetActive(false);
            IsDone();
        }
        else if (NewHighScoreP2N2 == true)
        {
            PlayerPrefs.SetString("Nr2", P2.text);
            P2.gameObject.SetActive(false);
            IsDone();
        }
        else if (NewHighScoreP2N3 == true)
        {
            PlayerPrefs.SetString("Nr3", P2.text);
            P2.gameObject.SetActive(false);
            IsDone();

        }
    }

    private void IsDone()
    {
        if (P1.IsActive() == false && P2.IsActive() == false)
        {
            Back2MainMenu();
        }
    }
    
    public void DeletePlayerPrefsKeys()
    {
        PlayerPrefs.DeleteAll();   
    }
}
