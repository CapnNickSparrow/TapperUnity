using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadLevel : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        GameManager.instance.ScorePal.SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && GameManager.instance.TwoPlayer == false)
        {
            GameManager.instance.ScorePal.SetActive(false);
            GameManager.instance.StartGame(isTwoPlayer: false);    
        }
        else if (Input.GetKeyDown(KeyCode.Space) && GameManager.instance.TwoPlayer == true)
        {
            GameManager.instance.ScorePal.SetActive(false);
            GameManager.instance.StartGame(isTwoPlayer: true); 
        }
    }
}
