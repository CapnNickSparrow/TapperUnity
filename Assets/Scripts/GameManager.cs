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

    // Connects to multiple assets
    private GameObject Player;
    
    public LevelManager levelManager;

    public LevelUIManager levelUIManager;

    // The Score Palette
    private Dictionary<ScoreKey, int> _scoreTable;

    public Dictionary<ScoreKey, int> ScoreTable
    {
        get
        {
            if (_scoreTable == null)
            {
                _scoreTable = new Dictionary<ScoreKey, int>();
                _scoreTable.Add(ScoreKey.EmptyMug, Constants.MUG_HARD);
                _scoreTable.Add(ScoreKey.Tip, Constants.TIP_FINISH);
                _scoreTable.Add(ScoreKey.Customer, Constants.STANDARD);
                _scoreTable.Add(ScoreKey.HardCustomer, Constants.UNCOMMON);
                _scoreTable.Add(ScoreKey.HarderCustomer, Constants.MUG_HARD);
                _scoreTable.Add(ScoreKey.LevelFinish, Constants.TIP_FINISH);
            }

            return _scoreTable;
        }
    }
    
    // Game Mode
    public GameMode SelectedGameMode;
    
    // Ints
    public int CurrentPlayer = Constants.PLAYER_ONE;

    public int PlayerTwoScore;
    public int PlayerTwoCurrentLevel;
    public int PlayerTwoLives;
    
    public int PlayerOneScore;
    public int PlayerOneCurrentLevel;
    public int PlayerOneLives;
    
    public int HappyCustomer;

    // Bools
    public bool HasLevelStarted;
    
    public bool Oops;
    public bool OopsC;
    public bool NotDone;
    
    public bool IsGameWon;
    public bool LevelWon;

    public bool TwoPlayer;
    
    public bool NewHighScoreP1;
    public bool NewHighScoreP2;
    public bool NewHighScoreP1N1;
    public bool NewHighScoreP1N2;
    public bool NewHighScoreP1N3;
    public bool NewHighScoreP2N1;
    public bool NewHighScoreP2N2;
    public bool NewHighScoreP2N3;

    // Canvas Objects
    public GameObject Menu;
    
    public GameObject ScorePal;
    
    public Text HighScore1;
    public Text HighScore2;
    public Text HighScore3;

    public Text P1S;
    public Text P2S;

    public Text Game;

    public Text DefaultServeBind;

    public Text DefaultPourBind;

    public Text MenuRef;
    
    public Image BG;

    public InputField P1;
    public InputField P2;
    
    public InputField Serve;
    public InputField Pour;
    
    // KeyCodes    
    public KeyCode KCP;
    public KeyCode KCS;
    
    void OnEnable()
    {
        // Tell our 'OnLevelFinishedLoading' function to start listening for a scene change as soon as this script is enabled.
        SceneManager.sceneLoaded += OnLevelFinishedLoading;
    }

    void OnDisable()
    {
        // Tell our 'OnLevelFinishedLoading' function to stop listening for a scene change as soon as this script is disabled.
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

    // After 2.5 Seconds start the level and  set the Get Ready To Serve screen on inactive
    protected IEnumerator HideReadyToServeImageAndStartLevel()
    {
        float displayTime = Constants.DISPLAY_TIME;

        yield return new WaitForSeconds(displayTime);

        levelUIManager.ToggleReadyToServeImage(displayImage: false);
        HasLevelStarted = true;
    }
    
    // Awake is called before the first frame update
    void Awake()
    {
        // Sets Standard Current Player and set the lives of each player    
        CurrentPlayer = Constants.PLAYER_ONE;
        
        PlayerOneLives = Constants.STANDARD_LIVES;
        PlayerTwoLives = Constants.STANDARD_LIVES;
        
        // GameManager is the instance
        if (instance == null) 
        {
            instance = this;
        } 
        
        // If something else is the instance destroy it
        else if (instance != this) 
        {
            Destroy(gameObject);
        }

        // Gets the LevelManagement Components
        levelManager = GetComponent<LevelManager>();
        levelUIManager = GetComponent<LevelUIManager>();

        // Gets the last saved KeyBindings
        GetBinds();
        
        // Let's the GameManager not kill itself after switching scenes
        DontDestroyOnLoad(gameObject);
    }
    
    // Starts the game with the following attributes
    public void StartGame(bool isTwoPlayer)
    {
        // If the game has a true Two Player bool make the SelectedGameMode Two Player otherwise make it One Player
        if (isTwoPlayer)
        {
            SelectedGameMode = GameMode.TwoPlayer;
        }
        else
        {
            SelectedGameMode = GameMode.SinglePlayer;
        }
        
        // Sets the made a mistake or won variables on false to prevent problems

        NotDone = false;

        LevelWon = false;
        
        Oops = false;
        OopsC = false;
        
        // Set the current HappyCustomer to 0 every time a new level begins
        HappyCustomer = Constants.ZERO;

        // Displays the current score of the players
        levelUIManager.SetPlayerOneScoreText(PlayerOneScore);
        
        levelUIManager.SetPlayerTwoScoreText(PlayerTwoScore);

        // Sets the Players Current level equal to that of the level they are in
        PlayerOneCurrentLevel = levelManager.level;
        PlayerTwoCurrentLevel = levelManager.level;
        
        // Sets the Levelicon equal to the level the first player is in
        levelUIManager.SetCurrentLevelText(PlayerOneCurrentLevel);
        
        levelManager.CurrentLevel = levelManager.AllLevels[levelManager.level - Constants.ONE];

        // Loads the Level scene with the current number so the levelloader matches
        SceneManager.LoadScene("Level" + levelManager.level);
    }

    // Called from another place, this will add score based on the Palette to the correct player
    internal void AddToCurrentPlayerScore(ScoreKey toAdd)
    {
        // Checks which player is playing to add score to
        if (CurrentPlayer == Constants.PLAYER_ONE)
        {
            AddToPlayerOneScore(toAdd);
        }
        else
        {
            AddToPlayerTwoScore(toAdd);
        }
        
        // If you have served 20 customers but the level hasn't been won yet
        // Set the level to won and set the level to the next level, add level finish bonus and start the next level
        if (HappyCustomer >= Constants.PROMOTION && !LevelWon)
        {
            LevelWon = true;
            levelManager.level++;
            AddToCurrentPlayerScore(ScoreKey.LevelFinish);
            StartCoroutine("NextLevel");
        }
    }

    // Wil prepare for the next level
    IEnumerator NextLevel ()
    {
        // Find the player and let him do the win track
        Player = GameObject.Find("Player");
        Player.GetComponent<Player>().Win.Play();
        
        // Checks if the player isn't at the final level
        if (levelManager.level <= Constants.LVL_3)
        {
            // Let the player does his win animation
            Player.GetComponent<Animator>().SetBool("hasWon", true);
            yield return new WaitForSeconds(Constants.WAIT_2_SEC);
            Player.GetComponent<Animator>().SetBool("hasWon", false);
            
            // If the game mode is two player start new game with two player otherwise with one player
            if (SelectedGameMode == GameMode.TwoPlayer)
            {
                StartGame(isTwoPlayer: true);    
            }
            else
            {
                StartGame(isTwoPlayer: false);    
            }
        }
        // If it is the final level
        else
        {
            // Set the GameWon bool to true
            IsGameWon = true;
            // If the current plaer is one and the second player has lives left and the GameMode is Two Player use the win animation and switch players
            if (CurrentPlayer == Constants.PLAYER_ONE && PlayerTwoLives != Constants.ZERO && SelectedGameMode == GameMode.TwoPlayer)
            {
                Player.GetComponent<Animator>().SetBool("hasWon", true);
                yield return new WaitForSeconds(Constants.WAIT_2_SEC);
                Player.GetComponent<Animator>().SetBool("hasWon", false);
                SwitchPlayers();
            }
            // If the current player is two and the first player has lives left and the GameMode is Two Player use the win animation and switch players
            else if (CurrentPlayer == Constants.PLAYER_TWO && PlayerOneLives != Constants.ZERO && SelectedGameMode == GameMode.TwoPlayer)
            {
                Player.GetComponent<Animator>().SetBool("hasWon", true);
                yield return new WaitForSeconds(Constants.WAIT_2_SEC);
                Player.GetComponent<Animator>().SetBool("hasWon", false);
                SwitchPlayers();
            }
            // Go the the end screen
            else
            {
                GameOver();
            }
        }
        yield return null;
    }
    
    // Adds score to Player One according to the Score Palette
    internal void AddToPlayerOneScore(ScoreKey toAdd)
    {
        PlayerOneScore += ScoreTable[toAdd];
        levelUIManager.SetPlayerOneScoreText(PlayerOneScore);
    }

    // Adds score to Player Two according to the Score Palette
    internal void AddToPlayerTwoScore(ScoreKey toAdd)
    {
        PlayerTwoScore += ScoreTable[toAdd];
        levelUIManager.SetPlayerTwoScoreText(PlayerTwoScore);
    }

    
    // Let's Player Lose
    public void PlayerLost()
    {
        // Set's all mistakes to false
        levelManager.PlayerMissedCustomer = false;
        levelManager.PlayerMissedEmptyMug = false;
        levelManager.PlayerThrewExtraMug = false;
        
        // Player on or Two, depends on the current Player Loses a life 
        if (CurrentPlayer == Constants.PLAYER_ONE)
        {
            PlayerOneLives--;
            Debug.Log(string.Format("Player 1 Lives: {0}", PlayerOneLives));
        }
        
        else
        {
            PlayerTwoLives--;
            Debug.Log(string.Format("Player 2 Lives: {0}", PlayerTwoLives));
        }

        // Switch Players if Two Player
        if (SelectedGameMode == GameMode.TwoPlayer)
        {
            // If there is Health Left Switch Players, otherwise Game Over
            if ((CurrentPlayer == Constants.PLAYER_ONE && PlayerOneLives != Constants.ZERO) || (CurrentPlayer == Constants.PLAYER_TWO && PlayerOneLives != Constants.ZERO))
            {
                SwitchPlayers();
            }
            else
            {
                GameOver();
            }
        }
        // If the GameMode is Singleplayer check if the Player has Health left so the scene can be restarted otherwise Game Over 
        else
        {
            if (PlayerOneLives == Constants.ZERO)
            {
                GameOver();
            }

            else
            {
                RestartLevelScene();
            }
        }
    }

    // Switches the Player if Possible
    private void SwitchPlayers()
    {
        // If Player is 1 and the 2nd Player has lives left, Switch Current Player and Restart the Scene, otherwise Current Player Stays 1 and restarts the scene
        if (CurrentPlayer == Constants.PLAYER_ONE)
        {
            if (PlayerTwoLives != Constants.ZERO)
            {
                HappyCustomer = Constants.ZERO;
                CurrentPlayer = Constants.PLAYER_TWO;
                levelManager.CurrentLevel = levelManager.AllLevels[PlayerTwoCurrentLevel - Constants.ONE];   
                RestartLevelScene();
            }
            else
            {
                CurrentPlayer = Constants.PLAYER_ONE;  
                RestartLevelScene();
            }
        }
        
        // If Player is 2 and the 1st Player has lives left, Switch Current Player and Restart the Scene, otherwise Current Player Stays 2 and restarts the scene
        else if (CurrentPlayer == Constants.PLAYER_TWO)
        {
            if (PlayerOneLives != Constants.ZERO)
            {
                HappyCustomer = Constants.ZERO;
                CurrentPlayer = Constants.PLAYER_ONE;
                levelManager.CurrentLevel = levelManager.AllLevels[PlayerOneCurrentLevel - Constants.ONE];   
                RestartLevelScene();
            }
            else
            {
                CurrentPlayer = Constants.PLAYER_TWO;  
                RestartLevelScene();
            }
        }
    }

    // Restarts the same scene and sets the "Made Mistake" bools to false
    private void RestartLevelScene()
    {
        Oops = false;
        OopsC = false;
        NotDone = false;
        
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    
    // Checks if it defeats a highscore to change the current highscore with the new highscore otherwise leave it as it is. Checks for both player 1 and Player 2
    // It will also sets the correct Bool from which highscore is broken to change the highscore itself but also the name it belongs to in the next function
    // When done it will go the the GetGameEnd function
    private void GameOver()
    { 
        if (PlayerOneScore > PlayerPrefs.GetInt("HighScore3", Constants.ZERO))
        {
            if (PlayerOneScore > PlayerPrefs.GetInt("HighScore2", Constants.ZERO))
            {
                if (PlayerOneScore > PlayerPrefs.GetInt("HighScore1", Constants.ZERO))
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

        if (PlayerTwoScore > PlayerPrefs.GetInt("HighScore3", Constants.ZERO) && SelectedGameMode == GameMode.TwoPlayer)
        {
            if (PlayerTwoScore > PlayerPrefs.GetInt("HighScore2", Constants.ZERO))
            {
                if (PlayerTwoScore > PlayerPrefs.GetInt("HighScore1", Constants.ZERO))
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

    // Sets the Sets the End Players Score equal to the actual Player Score and set the current score items active
    private void GetPlayerCurrentScore()
    {
        SetCurrentScoreActive();
        P1S.text = PlayerOneScore.ToString();
        P2S.text = PlayerTwoScore.ToString();
    }
    
    private void GetGameEnd()
    {   // Finds the Game Text to give it the correct text based on if the Player won the last level
        Game = GameObject.Find("Game").GetComponent<Text>();
        if (IsGameWon == true)
        {
            Game.text = "Game Won";
        }
        
        else if (IsGameWon == false)
        {
            Game.text = "Game Over";
        }

        //
        if (NewHighScoreP1 == false && NewHighScoreP2 == false)
        {
            Invoke("Back2MainMenu", Constants.WAIT_10_SEC);   
        }
    }

    // Sets the Current Score Inactive and bring you back to the Main Menu
    private void Back2MainMenu()
    {
        SetCurrentScoreInactive();
        SceneManager.LoadScene("MainMenuScene");
    }
    
    // Gets the High Score of the Top 3 Players
    public void GetHighScore()
    {
        HighScore1.text = "1. " +  PlayerPrefs.GetString("Nr1", "TMP") + " Score: " + PlayerPrefs.GetInt("HighScore1", Constants.ZERO);
        HighScore2.text = "2. " +  PlayerPrefs.GetString("Nr2", "TMP") + " Score: " + PlayerPrefs.GetInt("HighScore2", Constants.ZERO);
        HighScore3.text = "3. " +  PlayerPrefs.GetString("Nr3", "TMP") + " Score: " + PlayerPrefs.GetInt("HighScore3", Constants.ZERO);
    }

    // Sets all HighScores Active
    public void SetHighScoreActive()
    {
        HighScore1.gameObject.SetActive(true);
        HighScore2.gameObject.SetActive(true);
        HighScore3.gameObject.SetActive(true);
    }
    
    // Sets all HighScores Inactive
    public void SetHighScoreInactive()
    {
        HighScore1.gameObject.SetActive(false);
        HighScore2.gameObject.SetActive(false);
        HighScore3.gameObject.SetActive(false);

    }
    
    // Set Current Ending Object Active depending on if Player 1 or 2 have beaten the previous High Score
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
    
    // Set Current Ending Object Inactive depending on if Player 1 or 2 have beaten the previous High Score
    public void SetCurrentScoreInactive()
    {
        if (NewHighScoreP1 == true)
        {
            NewHighScoreP1 = false;
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

    // If the Set Name Button has been pressed it will check which Highscore part they had beaten to set new Name and the object to inactive
    // Wil go to the IsDone function in the end
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

    // Sets the Input Fields Inactive and sets back the Default Values of certain variables for a fresh start
    private void IsDone()
    {
        if (P1.IsActive() == false && P2.IsActive() == false)
        {
            PlayerOneLives = Constants.STANDARD_LIVES;
            PlayerTwoLives = Constants.STANDARD_LIVES;
            PlayerOneCurrentLevel = Constants.LVL_1;
            PlayerTwoCurrentLevel = Constants.LVL_1;
            PlayerOneScore = Constants.ZERO;
            PlayerTwoScore = Constants.ZERO;
            levelManager.level = Constants.ONE;
            Back2MainMenu();
        }
    }

    // Changes the Binding of Pouring by converting certain aspects so it will be compatible with each other and eventually changing the KeyMap
    public void ChangeBindPour()
    {
        PlayerPrefs.SetString("Pour", Pour.text.ToUpper());
        DefaultPourBind.text = PlayerPrefs.GetString("Pour", "X");
        KCP = (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("Pour", "X"));
        GameInputManager.SetKeyMap("Pour", KCP);
    }
    
    
    // Changes the Binding of Serving by converting certain aspects so it will be compatible with each other and eventually changing the KeyMap
    public void ChangeBindServe()
    {
        PlayerPrefs.SetString("Serve", Serve.text.ToUpper());
        DefaultServeBind.text = PlayerPrefs.GetString("Serve", "Z");
        KCS = (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("Serve", "Z"));
        GameInputManager.SetKeyMap("Serve", KCS);
    }

    // Gets the Bindings of Serving and Pouring via converted PlayerPrefs
    public void GetBinds()
    {
        DefaultPourBind.text = PlayerPrefs.GetString("Pour", "X");
        KCP = (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("Pour", "X"));
        GameInputManager.SetKeyMap("Pour", KCP);
        
        DefaultServeBind.text = PlayerPrefs.GetString("Serve", "Z");
        KCS = (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("Serve", "Z"));
        GameInputManager.SetKeyMap("Serve", KCS);
    }
    
    // A Secret Function for the me to delete PlayerPrefs before Shipping
    public void DeletePlayerPrefsKeys()
    {
        PlayerPrefs.DeleteAll();   
    }
}
