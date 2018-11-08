

using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour
{

    #region Members

    GameMgr.GameControl Control;
    [SerializeField]
    float shootForce = 20;

    [SerializeField]
    float moveSpeed = 5;

    [SerializeField]
    float RotationSpeed = 5;

    [SerializeField]
    float JumpSpeed = 10;

    [SerializeField]
    float smoothTime = 0.3f;

    [SerializeField]
    GameObject bulletPrefab = null;

    [SerializeField]
    GameObject canonPrefab = null;

    [SerializeField]
    GameObject BonusPrefab = null;

    [SerializeField]
    int hp = 100;

    [SerializeField]
    Vector3 posCam = new Vector3(0, 9, -10);

    [SerializeField]
    int BonusminXSpawn = 0;

    [SerializeField]
    int BonusmaxXSpawn = 0;

    [SerializeField]
    int BonusminZSpawn = 0;

    [SerializeField]
    int BonusmaxZSpawn = 0;

    Vector3 moveDirection = Vector3.zero;
    Vector3 posStartBullet = Vector3.zero;
    CharacterController controller = null;
    int hpMax = 100; 
    Bullet bullet = null;
    Canon canon = null;
    int enemyKill = 0;
    int nmbEnemiesTokill = 0;


    #endregion

    #region getter/Setter

    public int HP { get { return hp; } set { hp = value; } }
    public int HPMax { get { return hpMax; } set { hpMax = value; } }
    public int EnemyKill { get { return enemyKill; } set {enemyKill = value; } }
    public int NmbEnemiesTokill { get { return nmbEnemiesTokill; } set { nmbEnemiesTokill = value; } }
    public float MoveSpeed { get { return moveSpeed; } }

    #endregion

    #region Awake/Start

    void Awake()
    {
        controller = GetComponent<CharacterController>();
        HPMax = HP;
        

    }
    // Use this for initialization

    void Start()
    {

        Control = GameMgr.Instance.Control;
        InstantiateCanon();
        GameMgr.Instance.IncreaseHp(HP, HPMax);

        StartCoroutine(InstantiateBonus());

    }

    #endregion

    #region Updates
    
    // Update is called once per frame
    void Update()
    {
        if (GameMgr.Instance.State == GameMgr.GameState.Gameplay)
        {
            if (HP <= 0)
            {
                GameMgr.Instance.Dead();
            }


            moveDirection.y = 0;
            //Compute Input

            bool isJumping = Input.GetKeyUp(KeyCode.Space);
            bool IsBullet;
            float moveX;
            float moveZ;
            if (Control == GameMgr.GameControl.Clavier)
            {
                 IsBullet = Input.GetKeyUp(KeyCode.Mouse0);

                 moveX = Input.GetAxis("Horizontal");
                 moveZ = Input.GetAxis("Vertical");

            }
            else
            {
                IsBullet = Input.GetKeyUp(KeyCode.Joystick1Button4);
                moveX = Input.GetAxis("ManetteHorizontal2");
                moveZ = Input.GetAxis("ManetteVertical2");
            }

            moveDirection.x = moveX;
            moveDirection.z = moveZ;
            moveDirection.Normalize();

            //MoveDirection

            if (isJumping && controller.isGrounded)
            {
                moveDirection.y = JumpSpeed;
            }

            if (IsBullet)
            {
                if (Control == GameMgr.GameControl.Clavier)
                {
                    Plane plane = new Plane(Vector3.up, canon.transform.position);

                    Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

                    float hitdist = 0.0f;
                    GameObject obj = GameObject.FindGameObjectWithTag("Tire");
                    posStartBullet = obj.transform.position;
                    InstantiateBullet();

                    if (plane.Raycast(ray, out hitdist))
                    {
                        Vector3 targetPoint = ray.GetPoint(hitdist);
                        bullet.transform.LookAt(targetPoint);
                    }

                    bullet.GetComponent<Rigidbody>().velocity = bullet.transform.forward * shootForce;

                }
                else
                {
                    GameObject obj = GameObject.FindGameObjectWithTag("Tire");
                    posStartBullet = obj.transform.position;
                    InstantiateBullet();

                    bullet.transform.LookAt(canon.transform.position + canon.transform.forward);
                    bullet.GetComponent<Rigidbody>().velocity = bullet.transform.forward * shootForce;
                }
            }
            canon.rotation(transform.position, Control);

          
        }

        Vector3 desirePosition = transform.position + new Vector3(0, posCam.y, posCam.z);

        Vector3 velocity = Vector3.zero;

        Camera.main.transform.position = Vector3.SmoothDamp(Camera.main.transform.position, desirePosition, ref velocity, smoothTime);

    }

    void FixedUpdate()
    {
        if (GameMgr.Instance.State == GameMgr.GameState.Gameplay)
        {
            Vector3 lookDir = moveDirection;
            lookDir.y = 0f;
            if (lookDir != Vector3.zero)
            {
                Quaternion targetRot = Quaternion.LookRotation(lookDir, transform.up);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, RotationSpeed * Time.deltaTime);
            }


            moveDirection += Physics.gravity * Time.deltaTime;

            controller.Move(moveDirection * moveSpeed * Time.deltaTime);


        }


    }

    #endregion

    #region InstantiateObject

    IEnumerator InstantiateBonus()
    {
        while (GameMgr.Instance.State == GameMgr.GameState.Gameplay)
        {
            System.Random rnd = new System.Random();
            int randx = rnd.Next(BonusminXSpawn, BonusmaxXSpawn);
            int randz = rnd.Next(BonusminZSpawn, BonusmaxZSpawn);
            Vector3 pos = new Vector3(randx,1,randz);
            Instantiate(BonusPrefab, pos, Quaternion.identity);
            yield return new WaitForSeconds(5.0f);
        }
    }

   
    void InstantiateBullet()
    {
        GameObject BulletGo = GameObject.Instantiate(bulletPrefab, posStartBullet, Quaternion.identity) as GameObject;
        bullet = BulletGo.GetComponent<Bullet>();
    }

    public void InstantiateCanon()
    {
        Vector3 pos = transform.position;
        pos.y = transform.position.y + 0.75f;

        GameObject CanonGo = GameObject.Instantiate(canonPrefab, pos , Quaternion.identity) as GameObject;
        canon = CanonGo.GetComponent<Canon>();
       

        canon.transform.parent = transform;
    }

    #endregion

    #region Trigger

    void OnTriggerEnter(Collider col)
    {
        if (GameMgr.Instance.State == GameMgr.GameState.Gameplay)
        {
            if (col.gameObject.layer == LayerMask.NameToLayer("HealthBonus"))
            {
                HP += 5;
                GameMgr.Instance.IncreaseHp(HP, HPMax);

                Destroy(col.gameObject);
            }
            if (col.gameObject.layer == LayerMask.NameToLayer("Enemy") && HP > 0)
            {

                HP -= 1;
                GameMgr.Instance.IncreaseHp(HP, HPMax);

            }
        }       
    }

    #endregion

}



