using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Canon : MonoBehaviour {

    #region Members

    [System.Serializable]
    enum Type
    {
        Player,
        Enemy,
    }

    [SerializeField]
    Type type;

    ParticleSystem par = null;


    bool animate = false;

    [SerializeField]
    GameObject AudioPrefab = null;

    #endregion

    #region Awake

    // Use this for initialization
    void Awake()
    {
        par = gameObject.GetComponentInChildren<ParticleSystem>();
        if (par != null)
            par.Stop();
    }

    #endregion

    #region Update

    // Update is called once per frame
    void Update()
    {
        if (GameMgr.Instance.State == GameMgr.GameState.Gameplay)
        {
            if (par != null)
            {
                if (type == Type.Player)
                {
                    if (Input.GetKeyUp(KeyCode.Mouse0))
                    {
                        par.Play();
                        Audio();
                    }
                }
                else
                {
                    if (animate)
                    {
                        par.Play();
                        Audio();
                        animate = false;
                    }
                }
            }
        }
    }

    #endregion

    #region Rotation

    public void rotation(Vector3 position, GameMgr.GameControl control)
    {

        if (par != null)
        {
            if (control == GameMgr.GameControl.Clavier)
            {
                Plane playerPlane = new Plane(Vector3.up, transform.position);

                if (type == Type.Enemy)
                    Debug.Log("Test");

                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

                float hitdist = 0.0f;

                if (playerPlane.Raycast(ray, out hitdist))
                {
                    Vector3 targetPoint = ray.GetPoint(hitdist);
                    //    bullet.transform.LookAt(targetPoint);

                    Quaternion targetRotation = Quaternion.LookRotation(targetPoint - transform.position);

                    // Smoothly rotate towards the target point.

                    transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 20.0f * Time.deltaTime);

                }
            }
            else
            {
                float moveX = Input.GetAxis("ManetteHorizontal");
                float moveZ = Input.GetAxis("ManetteVertical");

                Vector3 test = new Vector3(moveX, 0, moveZ);

                if (test != Vector3.zero)
                {
                    Quaternion targetRotation = Quaternion.LookRotation(test);
                    transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 20.0f * Time.deltaTime);
                }
            }


        }

    }

    public void rotationEnemy(Vector3 pos)
    {
        if (par != null)
        {
            Quaternion targetRotation = Quaternion.LookRotation(new Vector3(pos.x, GameMgr.Instance.Players.transform.position.y + 0.75f, pos.z) - transform.position);

            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 20.0f * Time.deltaTime);
          

        }
    }

    #endregion

    #region Trigger

    void OnTriggerEnter(Collider col)
    {
        if (GameMgr.Instance.State == GameMgr.GameState.Gameplay)
        {
            if (type == Type.Player)
            {
                if (col.gameObject.layer == LayerMask.NameToLayer("BulletEnemy"))
                {
                    Destroy(col.gameObject);
                    // GameMgr.Instance.Players.HP -= 1;
                    GetComponentInParent<Player>().HP -= 2;
                    GameMgr.Instance.IncreaseHp(GameMgr.Instance.Players.HP, GameMgr.Instance.Players.HPMax);
                }

                if (GameMgr.Instance.Players.HP <= 0)
                {
                    GameMgr.Instance.Dead();
                }
            }

            else
            {
                if (col.gameObject.layer == LayerMask.NameToLayer("Bullet"))
                {
                    Destroy(col.gameObject);
                    gameObject.GetComponentInParent<Enemy>().HP--;
                    GameMgr.Instance.Score += 100;
                    GameMgr.Instance.IncreaseScore(GameMgr.Instance.Score);

                }
                if (gameObject.GetComponentInParent<Enemy>().HP <= 0)
                {
                    Destroy(gameObject.GetComponentInParent<Enemy>().gameObject);
                    GameMgr.Instance.Players.EnemyKill++;
                }

                if (GameMgr.Instance.Players.EnemyKill >= GameMgr.Instance.Players.NmbEnemiesTokill)
                {
                    GameMgr.Instance.Victory();
                }
                Debug.Log("Test2");

            }

        }

    }

    #endregion

    #region Animation/Audio

    public void Animate()
    {
        animate = true;
    }

    void Audio()
    {
        Instantiate(AudioPrefab);
    }

    #endregion

}

