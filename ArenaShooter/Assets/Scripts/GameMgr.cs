using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;
using System.Collections.Generic;
using System.IO;

public class GameMgr : MonoBehaviour
{

    #region Members

    public enum GameState
    {
        Menu,
        Gameplay,
        Victory,
        Pause,
        GameOver
    }

    public enum GameControl
    {
        Clavier = 0,
        Manette,  
    }

    [SerializeField]
    GameState state = GameState.Menu;
    [SerializeField]
    GameObject playerPrefab = null;

    [SerializeField]
    GameControl control;

    Player player = null;

    int score = 0;
    int highScore = 0;
    string names = "Player";

    static public GameMgr instance = null;
    
    public delegate void DeadEventHandler();
    public event DeadEventHandler OnUpdateDead = null;

    public delegate void VictoryEventHandler();
    public event VictoryEventHandler OnUpdateVictory = null;

    public Dictionary<string, int> ScoreEnter = new Dictionary<string, int>();
  
    public delegate void HPEventHandler(int val, int valmax);
    public event HPEventHandler OnUpdateHP = null;

    public delegate void ScoreEventHandler(int val);
    public event ScoreEventHandler OnUpdateScore = null;

    public delegate void HighScoreEventHandler(int val);
    public event HighScoreEventHandler OnUpdateHighScore = null;

    #endregion

    #region Getter/Setter

    static public GameMgr Instance
    {
        get
        {
            if (instance == null)
                instance = FindObjectOfType<GameMgr>();
            return instance;
        }
    }

    public GameState State
    {
        get { return state; }
        set { state = value; }
    }

    public Player Players
    {
        get { return player; }
    }

    public GameObject PlayerPrefab { set { playerPrefab = value; } }
    public GameControl Control { get { return control; } set { control = value; } }

    public int Score
    {
        get { return score; }
        set { score = value; }
    }

    public int HighScore
    {
        get { return highScore; }
        set { highScore = value; }
    }

 
    public string Name
    {
        get
        {
            return names;
        }

        set
        {
            names = value;
        }
    }

    #endregion

    #region Awake

    void Awake()
    {
        if (state == GameState.Gameplay && player == null)
        {
            InstantiatePlayer();
        }
        GameMgr.Instance.LoadScore();

    }

    #endregion

    #region LoadScene

    public void LoadScene(int index)
    {
        SceneManager.LoadScene(index);
        Score = 0;
        IncreaseScore(Score);
    }

    public void ReloadLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        Score = 0;
        IncreaseScore(Score);
    }


    public void LoadMainMenu(bool firstLoad = false)
    {
        Score = 0;
        state = GameState.Menu;
        SceneManager.LoadScene("Menu");

    }

    #endregion

    #region Quit

    public void Quit()
    {
        Application.Quit();
    }

    #endregion

    #region IncreaseScore/HighScore/HP

    public void IncreaseHp(int Hp, int HPMAX)
    {
        if (OnUpdateHP != null)
            OnUpdateHP(Hp, HPMAX);
    }

    public void IncreaseScore(int val)
    {
        if (OnUpdateScore != null)
            OnUpdateScore(val);

        if (Score >= HighScore)
        {
            HighScore = Score;
        }
    }
    public void IncreaseHighScore(int val)
    {
        if (OnUpdateHighScore != null)
            OnUpdateHighScore(val);

    }

    #endregion

    #region Victory/Dead

    public void Dead()
    {
        if (OnUpdateDead != null)
            OnUpdateDead();
        GameMgr.Instance.state = GameMgr.GameState.GameOver;
    }

    public void Victory()
    {
        if (OnUpdateVictory!= null)
            OnUpdateVictory();
        GameMgr.Instance.state = GameMgr.GameState.GameOver;
    }

    #endregion

    #region InstantiatePlayer

    public void InstantiatePlayer()
    {
        if (player == null)
        {
            GameObject PlayerGo = Instantiate(playerPrefab, new Vector3(0f, 1.08f,0), Quaternion.identity) as GameObject;
            player = PlayerGo.GetComponent<Player>();
            
        }
    }

    #endregion

    #region Load/SaveScore

    public void LoadScore()
    {
        string fileName = "entries.txt";

        if (!File.Exists(fileName))
        {
            new StreamWriter(fileName);
        }
        else
        {

            string[] lines = System.IO.File.ReadAllLines(fileName);

            foreach (string line in lines)
            {
                char[] separators = new char[1];
                separators[0] = ' ';
                string[] entries = line.Split(separators, 2);
                string nameEnter = entries[0];
                int ScoreSave = 0;
                int.TryParse(entries[1], out ScoreSave);
                ScoreEnter.Add(nameEnter, ScoreSave);
                
            }

          

        }


    }

    public void SaveScore()
    {

        File.Delete("entries.txt");

        StreamWriter Endfile = new StreamWriter("entries.txt");

        foreach (KeyValuePair<string, int> kvp in ScoreEnter)
        {
            Endfile.WriteLine(kvp.Key + ' ' + kvp.Value);
        }
        Endfile.Close();
    }

    #endregion

    #region Update

    void Update()
    {

        if (state == GameState.Gameplay)
            InstantiatePlayer();
    }

    #endregion

}
