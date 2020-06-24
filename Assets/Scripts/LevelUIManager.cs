using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelUIManager : MonoBehaviour
{
    // Connects to Text Objects
    public Text PlayerOneScoreText;

    public Text PlayerTwoScoreText;

    public Text CurrentLevelText;

    // Sets Transform
    public RectTransform CurrentLevelTextBackground;

    // Get UI Libary
    public UnityEngine.UI.Image ReadyToServeImage;


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

    // When the Level is finished loading set the text values
    void OnLevelFinishedLoading(Scene scene, LoadSceneMode mode)
    {
        GetUIComponents();
        SetPlayerOneScoreText(GameManager.instance.PlayerOneScore);
        SetPlayerTwoScoreText(GameManager.instance.PlayerTwoScore);

        if (GameManager.instance.CurrentPlayer == Constants.PLAYER_ONE)
        {
            SetCurrentLevelText(GameManager.instance.PlayerOneCurrentLevel);
        }
        else
        {
            SetCurrentLevelText(GameManager.instance.PlayerTwoCurrentLevel);
        }
    }

    
    // Get the UI Components
    public void GetUIComponents()
    {
        GameObject textObj = GameObject.Find("Player1Score_Text");

        if (textObj != null)
        {
            PlayerOneScoreText = textObj.GetComponent<Text>();
        }

        GameObject player2TextObj = GameObject.Find("Player2Score_Text");

        if (player2TextObj != null)
        {
            PlayerTwoScoreText = player2TextObj.GetComponent<Text>();
        }

        GameObject lvlTextObj = GameObject.Find("Level_Text");

        if (lvlTextObj != null)
        {
            CurrentLevelText = lvlTextObj.GetComponent<Text>();
        }

        GameObject lvlPanelObj = GameObject.Find("LevelText_Panel");

        if (lvlPanelObj != null)
        {
            CurrentLevelTextBackground = lvlPanelObj.GetComponent<RectTransform>();
        }

        GameObject readyToServePanelObj = GameObject.Find("ReadyToServePanel");

        if (readyToServePanelObj != null)
        {
            ReadyToServeImage = readyToServePanelObj.GetComponent<Image>();
        }
    }
    
    // Set the New Score Values in the Player One Text Object
    public void SetPlayerOneScoreText(int newScore)
    {
        if (PlayerOneScoreText != null)
        {
            PlayerOneScoreText.text = newScore.ToString();
        }
    }

    // Set the New Score Values in the Player Two Text Object
    public void SetPlayerTwoScoreText(int newScore)
    {
        if (PlayerTwoScoreText != null)
        {
            PlayerTwoScoreText.text = newScore.ToString();
        }
    }

    // Set Current Level Text and set Correct Background Width 
    public void SetCurrentLevelText(int newLevel)
    {
        if (CurrentLevelText == null || CurrentLevelTextBackground == null)
        {
            return;
        }

        CurrentLevelText.text = newLevel.ToString();

        // adjust the width of the text background to fit text width (Can be used if you want to implement more levels later)
        Vector2 newSize = CurrentLevelTextBackground.sizeDelta;

        if (newLevel < Constants.LVL_2_NUMB)
        {
            newSize.x = Constants.FONT_BG_1;
        }
        else if (newLevel < Constants.LVL_3_NUMB)
        {
            newSize.x = Constants.FONT_BG_2;
        }
        else
        {
            newSize.x = Constants.FONT_BG_3;
        }

        CurrentLevelTextBackground.sizeDelta = newSize;
    }

    // Set the Get Ready to Serve Image to Active
    public void ToggleReadyToServeImage(bool displayImage)
    {
        if (ReadyToServeImage != null)
        {
            ReadyToServeImage.enabled = displayImage;
        }
    }
}
