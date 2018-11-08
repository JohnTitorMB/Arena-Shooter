using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;

public class HUD : MonoBehaviour
{
    #region Members

    Text hpText = null;
    Text nameText = null;
    Text scoreText = null;

    Image srollbar = null;
    Transform pause = null;

    [SerializeField]
    Texture text = null;
    [SerializeField]
    Texture text2 = null;

    private bool IsDead = false;
    private bool IsVictory = false;

    #endregion

    #region Awake/Start

    // Use this for initialization

    void Awake()
    {

        hpText = transform.Find("HpText").GetComponent<Text>();
        nameText = transform.Find("NameText").GetComponent<Text>();
        scoreText = transform.Find("ScoreText").GetComponent<Text>();
        GameMgr.Instance.OnUpdateHP += UpdateHPText;
        pause = transform.Find("Pause");
        pause.gameObject.SetActive(false);



        GameMgr.Instance.OnUpdateScore+= Score;
        GameMgr.Instance.State = GameMgr.GameState.Gameplay;
        srollbar = transform.Find("HealthBar").GetComponent<Image>();

        pause.Find("Character").Find("Button").Find("Name").GetComponent<Text>().text = GameMgr.Instance.Name;
        

    }

    void Start()
    {
        GameMgr.Instance.OnUpdateVictory += Victory;
        GameMgr.Instance.OnUpdateDead += Dead;

    }

    #endregion

    #region LoaderLevel

    public void ReloadLevel()
    {
        GameMgr.Instance.OnUpdateHP -= UpdateHPText;
        GameMgr.Instance.OnUpdateScore -= Score;
        GameMgr.Instance.ReloadLevel();
    }

    public void LoadLevel()
    {
        GameMgr.Instance.OnUpdateHP -= UpdateHPText;
        GameMgr.Instance.OnUpdateScore -= Score;
        GameMgr.Instance.LoadScene(3);
    }

    public void LoadMainMenu()
    {
        GameMgr.Instance.OnUpdateHP -= UpdateHPText;
        GameMgr.Instance.OnUpdateScore -= Score;

        GameMgr.Instance.LoadMainMenu();
    }

    #endregion

    #region Play/Pause

    public void BackGame()
    {
        GameMgr.Instance.State = GameMgr.GameState.Gameplay;
        pause.gameObject.SetActive(false);
        transform.Find("PauseButton").gameObject.SetActive(true);
    }

    public void Pause()
    {
        GameMgr.Instance.State = GameMgr.GameState.Pause;
        pause.gameObject.SetActive(true);
        transform.Find("PauseButton").gameObject.SetActive(false);
    }

    #endregion

    #region Setter

    private void UpdateHPText(int val, int valmax)
    {
        float i = 0;
        i = Mathf.Clamp(val, 0, valmax);
        float amount = (float)i / valmax;
        srollbar.fillAmount = amount;
        
        hpText.text = "  " + val.ToString() + " /   " + valmax;
    }

    private void Score(int val)
    {
        scoreText.text = "Score : " + val.ToString();
    }


    private void SetNameText()
    {

        nameText.text = GameMgr.Instance.Players.name;

    }

    void Dead()
    {
        IsDead = true;
       
            
        
    }

    void Victory()
    {
        IsVictory = true;
      
          
        
    }

    #endregion

    #region Update

    void Update()
    {
        if (GameMgr.Instance.Players != null)
        {
            GameMgr.Instance.Players.name = GameMgr.Instance.Name;
            SetNameText();
        }
    }

    #endregion

    #region Event Dead/Victory

    void OnGUI()
    {
        if (IsDead)
        {
            transform.Find("PauseButton").gameObject.SetActive(false);
            transform.Find("MenuDead").gameObject.SetActive(true);

            
            GUI.DrawTexture(new Rect(((Screen.width / 2) - (500 / 2)), ((Screen.height / 2) - (212 / 2)), 500, 212), text, ScaleMode.ScaleToFit, true, 0.0f);
        }

        if(IsVictory)
        {
            transform.Find("PauseButton").gameObject.SetActive(false);
            if (SceneManager.GetActiveScene().buildIndex == 3)
            {
                transform.Find("MenuDead").gameObject.SetActive(true);
            }
            else
            {
                transform.Find("MenuVictory").gameObject.SetActive(true);
            }
          
            GUI.DrawTexture(new Rect(((Screen.width / 2) - (500 / 2)), ((Screen.height / 2) - (212 / 2)), 500, 212), text2, ScaleMode.ScaleToFit, true, 0.0f);
        }
    }

    #endregion
}
