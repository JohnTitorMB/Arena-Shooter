using UnityEngine;
using System.Collections;

public class Enemy : MonoBehaviour
{

    #region Members  

    [System.Serializable]
    enum Type
    {
        Armee,
        NotArmee
    }

    [SerializeField]
        Type type = Type.Armee;

    [SerializeField]
        GameObject bulletPrefab = null;

    [SerializeField]
    GameObject canonPrefab = null;

    [SerializeField]
    float hp = 2;
    [SerializeField]
    float speed = 2;
    [SerializeField]
    float porter = 30;

    float time;
    float angle;
    float TimeTire = 0.0f;

    Vector3 posStartBullet = Vector3.zero;
    UnityEngine.AI.NavMeshAgent navigation;
    Transform playerTr;
    Ray ray;
    Canon canon = null;
    Bullet bullet = null;
    
    #endregion

    #region Getter/Setter

    public float HP { get { return hp; } set { hp = value; } }
    public float Speed { get { return speed; } set { speed = value; } }
    public float Porter { get { return porter; } set { porter = value; } }

    #endregion

    #region Awake/Start

    void Awake()
    {

        angle = transform.eulerAngles.y;

        InstantiateCanon();
        navigation = GetComponent<UnityEngine.AI.NavMeshAgent>();
        time = Time.time;

        //  navigation.enabled = false;
    }

    // Use this for initialization
    void Start()
    {
        playerTr = GameObject.FindGameObjectWithTag("Player").transform;
        navigation.speed = Speed;


    }

    #endregion

    #region Update

    // Update is called once per frame
    void Update()
    {
        if (GameMgr.Instance.State == GameMgr.GameState.Gameplay)
        {
            if (!navigation.enabled)
            {
                if (Time.time < time + 1)
                {
                    Vector3 pos = transform.position;
                    pos.x += 1.5f * transform.forward.x * Time.deltaTime;
                    pos.z += 1.5f * transform.forward.z * Time.deltaTime;
                    transform.position = pos;
                }

                else
                {
                    navigation.enabled = true;
                }
            }
            else
            {
                if (type == Type.NotArmee)
                    ActionEnemyNotArmee();
                else
                    ActionEnemyArmee();
            }

            canon.rotationEnemy(GameMgr.Instance.Players.transform.position);
        }

        else
        {
            navigation.enabled = false;
            GetComponent<Rigidbody>().useGravity = true;
        }
    }

    #endregion

    #region Action

    void ActionEnemyArmee()
    {
        Vector3 posPlayer = GameMgr.Instance.Players.transform.position;
        Vector3 posEnemy = transform.position;
        Vector3 target = posPlayer - posEnemy;

        float distance = Mathf.Abs(target.sqrMagnitude);
        if (distance < Porter)
        {
            navigation.enabled = false;
            ActionTire();
        }
        else
        {
            navigation.enabled = true;
            navigation.SetDestination(playerTr.position);
        }
    }

    void ActionTire()
    {
        if (Time.time >= TimeTire + 2.0f)
        {
            canon.Animate();

            Transform[] test = GetComponentsInChildren<Transform>();
            foreach (Transform c in test)
            {
                if (c.CompareTag("Tire2"))
                    posStartBullet = c.transform.position;

            }

            InstantiateBullet();

            bullet.transform.LookAt(new Vector3(GameMgr.Instance.Players.transform.position.x, 1.8f, GameMgr.Instance.Players.transform.position.z));

            bullet.GetComponent<Rigidbody>().velocity = bullet.transform.forward * 10;
            TimeTire = Time.time;
        }
    }

    void ActionEnemyNotArmee()
    {
        navigation.SetDestination(playerTr.position);
    }

    #endregion

    #region Instantiate Canon/Bullet

    void InstantiateCanon()
    {
        Vector3 pos = transform.position;
        pos.y = transform.position.y + 0.75f;

        GameObject CanonGo = GameObject.Instantiate(canonPrefab, pos, Quaternion.Euler(0, angle, 0)) as GameObject;
        canon = CanonGo.GetComponent<Canon>();
        canon.transform.parent = transform;
    }

    void InstantiateBullet()
    {
        GameObject BulletGo = GameObject.Instantiate(bulletPrefab, posStartBullet, Quaternion.identity) as GameObject;
        bullet = BulletGo.GetComponent<Bullet>();
    }

    #endregion

}