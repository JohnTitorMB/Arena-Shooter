using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;


public class SelectPlayer : MonoBehaviour
{

    #region Members

    enum PlayerSelectioned
    {
        CubeeRed = 0,
        MajorCubee,
        MonoCubee
    }

    List<GameObject> Player = new List<GameObject>();
    List<Text> PlayerTr = new List<Text>();

    PlayerSelectioned PlayerSeclect = PlayerSelectioned.CubeeRed;
    [SerializeField]
    GameObject CubeeRedPrefab = null;
    [SerializeField]
    GameObject MajorCubeePrefab = null;
    [SerializeField]
    GameObject MonoCubeePrefab = null;

    GameMgr.GameControl controlActualy = GameMgr.GameControl.Clavier;
    Player MayorCubee = null;
    Player CubeeRed = null;
    Player MonoCubee = null;


    Text SpeedTr = null;
    Text HPTr = null;
    Text GeneralTr = null;
    Text controlText = null;

    #endregion

    #region Start 

    // Use this for initialization
    void Start()
    {
        Player.Add(GameObject.FindGameObjectWithTag("CubeeRed"));
        Player.Add(GameObject.FindGameObjectWithTag("MajorCubee"));
        Player.Add(GameObject.FindGameObjectWithTag("MonoCubee"));

        PlayerTr.Add(transform.Find("CubeeRed").GetComponent<Text>());
        PlayerTr.Add(transform.Find("MajorCubee").GetComponent<Text>());
        PlayerTr.Add(transform.Find("MonoCubee").GetComponent<Text>());

        CubeeRed = CubeeRedPrefab.GetComponent<Player>();
        MayorCubee = MajorCubeePrefab.GetComponent<Player>();
        MonoCubee = MonoCubeePrefab.GetComponent<Player>();
        SpeedTr = transform.Find("Cadre").Find("Speed").GetComponent<Text>();
        HPTr = transform.Find("Cadre").Find("HP").GetComponent<Text>();
        GeneralTr = transform.Find("Cadre").Find("General").GetComponent<Text>();
        SetDatainText(CubeeRed);

        controlText = transform.Find("Control").GetComponent<Text>();
        controlText.text = "Control : " + controlActualy.ToString();
    }

    #endregion

    #region Next/Prev

    public void Next()
    {
        Vector3 save = Player[2].transform.position;
        Player[2].transform.position = Player[1].transform.position;
        Player[1].transform.position = Player[0].transform.position;
        Player[0].transform.position = save;

        save = PlayerTr[2].rectTransform.position;
        PlayerTr[2].rectTransform.position = PlayerTr[1].rectTransform.position;
        PlayerTr[1].rectTransform.position = PlayerTr[0].rectTransform.position;
        PlayerTr[0].rectTransform.position = save;

        if (PlayerSeclect == PlayerSelectioned.CubeeRed)
        {
            PlayerSeclect = PlayerSelectioned.MajorCubee;
            SetDatainText(MayorCubee);

        }
        else if (PlayerSeclect == PlayerSelectioned.MajorCubee)
        {
            PlayerSeclect = PlayerSelectioned.MonoCubee;
            SetDatainText(MonoCubee);
        }

        else if (PlayerSeclect == PlayerSelectioned.MonoCubee)
        {
            PlayerSeclect = PlayerSelectioned.CubeeRed;
            SetDatainText(CubeeRed);
        }

    }

    public void Prev()
    {
        Vector3 save = Player[0].transform.position;
        Player[0].transform.position = Player[1].transform.position;
        Player[1].transform.position = Player[2].transform.position;
        Player[2].transform.position = save;

        save = PlayerTr[0].rectTransform.position;
        PlayerTr[0].rectTransform.position = PlayerTr[1].rectTransform.position;
        PlayerTr[1].rectTransform.position = PlayerTr[2].rectTransform.position;
        PlayerTr[2].rectTransform.position = save;

        if (PlayerSeclect == PlayerSelectioned.CubeeRed)
        {
            PlayerSeclect = PlayerSelectioned.MonoCubee;
            SetDatainText(MonoCubee);
        }
        else if (PlayerSeclect == PlayerSelectioned.MajorCubee)
        {
            PlayerSeclect = PlayerSelectioned.CubeeRed;
            SetDatainText(CubeeRed);
        }

        else if (PlayerSeclect == PlayerSelectioned.MonoCubee)
        {
            PlayerSeclect = PlayerSelectioned.MajorCubee;
            SetDatainText(MayorCubee);
        }

    }

    #endregion

    #region Setter

    public void SetDatainText(Player p)
    {
        HPTr.text = "HP :" + p.HP;
        SpeedTr.text = "Speed :" + p.MoveSpeed;
        GeneralTr.text = PlayerSeclect.ToString();

        if (PlayerSeclect == PlayerSelectioned.CubeeRed)
            GeneralTr.text += " est un character équilibré avec une vitesse de déplacement moyenne et des points de vie standards ";
        else if(PlayerSeclect == PlayerSelectioned.MajorCubee)
            GeneralTr.text += " est un character qui aura une vitesse de déplacement élevée mais peu de points de vie ";
        else if (PlayerSeclect == PlayerSelectioned.MonoCubee)
            GeneralTr.text += " un character qui aura une vitesse de déplacement réduite mais beaucoup de points de vie ";
    }

    public void SetPlayer()
    {
        if (PlayerSeclect == PlayerSelectioned.CubeeRed)
            GameMgr.Instance.PlayerPrefab = CubeeRedPrefab;
        else if(PlayerSeclect == PlayerSelectioned.MajorCubee)
            GameMgr.Instance.PlayerPrefab = MajorCubeePrefab;
         else if (PlayerSeclect == PlayerSelectioned.MonoCubee)
            GameMgr.Instance.PlayerPrefab = MonoCubeePrefab;

        GameMgr.Instance.Control = controlActualy;

        GameMgr.Instance.LoadScene(2);
    }

    public void SetClavier()
    {
        controlActualy = GameMgr.GameControl.Clavier;
        controlText.text = "Control : " + controlActualy.ToString();
    }

    public void SetManette()
    {
        controlActualy = GameMgr.GameControl.Manette;
        controlText.text = "Control : " + controlActualy.ToString();
    }

    #endregion

}