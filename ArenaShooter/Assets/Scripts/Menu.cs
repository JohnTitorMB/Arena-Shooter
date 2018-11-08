using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections.Generic;
using System.IO;
using System;

public class Menu : MonoBehaviour
{
    #region Members

    EventSystem eventSystem = null;
    InputField inputfiled = null;
    Text text = null;
    Text highScoreText = null;

    #endregion

    #region Start

    // Use this for initialization
    void Start()
    {
        eventSystem = EventSystem.FindObjectOfType<EventSystem>();
        inputfiled = transform.Find("InputField").GetComponent<InputField>();
        text = inputfiled.transform.Find("Text").GetComponent<Text>();
        highScoreText = transform.Find("HighScoreText").GetComponent<Text>();
        GameMgr.Instance.OnUpdateHighScore += HighScore;
        
        GameMgr.Instance.IncreaseHighScore(GameMgr.Instance.HighScore);

    }

    #endregion

    #region Run/Quit

    public void Quit()
    {
        GameMgr.Instance.Quit();
    }

    public void RunGame()
    {
        GameMgr.Instance.OnUpdateHighScore -= HighScore;
        GameMgr.Instance.LoadScene(1);
    }

    #endregion

    #region ButtonSelectioned

    public void Select(Button b)
    {
        eventSystem.SetSelectedGameObject(b.gameObject);
    }

    #endregion

    #region Name

    public void SetName()
    {
        GameMgr.Instance.Name = text.text;
        if (GameMgr.Instance.ScoreEnter.ContainsKey(GameMgr.Instance.Name))
        {
            GameMgr.Instance.HighScore = GameMgr.Instance.ScoreEnter[GameMgr.Instance.Name];


        }
        else
        {
            GameMgr.Instance.HighScore = 0;
        }
    }

    #endregion

    #region HighScore

    private void HighScore(int val)
    {
        if (GameMgr.Instance.ScoreEnter.ContainsKey(GameMgr.Instance.Name))
        {
          
            GameMgr.Instance.ScoreEnter[GameMgr.Instance.Name] = val;
        }
        else
        {
            GameMgr.Instance.ScoreEnter.Add(GameMgr.Instance.Name, val); 
        }

        highScoreText.text = "";
        foreach (KeyValuePair<string, int> kvp in GameMgr.Instance.ScoreEnter)
        {
            highScoreText.text += kvp.Key + " : " + kvp.Value + '\n';  
        }

        GameMgr.Instance.SaveScore();
       
    }

    #endregion
}
