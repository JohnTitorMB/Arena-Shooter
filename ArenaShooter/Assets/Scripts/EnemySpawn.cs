using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnemySpawn : MonoBehaviour
{

    #region Menbers

    [System.Serializable]
    class enemyWave
    {
        public List<Enemy> enemies;
    }

    enum SpawnState
    {
        Init = 0,
        Open ,
        Spawn,
        CLose,
        End
    }

    [SerializeField]
    List<enemyWave> enemiesWave;

    [SerializeField]
    float speedDoore = 1f;

    float currendistance = 0;
    float angle = 0;

    SpawnState Spawnstate;

    int EnemyActually = 0;
    int enemiesWaveActually = 0;

    #endregion

    #region Start

    // Use this for initialization
    void Start()
    {
        Spawnstate = SpawnState.Init;
        angle = transform.localEulerAngles.y;
  
    }

    #endregion

    #region Update

    // Update is called once per frame
    void Update()
    {
       

        if (GameMgr.Instance.State == GameMgr.GameState.Pause)
        {

        }

        if (GameMgr.Instance.State == GameMgr.GameState.Gameplay)
        {
            if (GameMgr.Instance.Players != null)
            {
                if (Spawnstate == SpawnState.Init)
                {
                        int NumOFenemies = 0;
                        foreach (enemyWave c in enemiesWave)
                        {
                            for (int i = 0; i < c.enemies.Capacity; i++)
                            {
                                NumOFenemies++;
                            }
                        }

                        GameMgr.Instance.Players.NmbEnemiesTokill += NumOFenemies;
                        Spawnstate = SpawnState.Open;
                 }
            }

            if (enemiesWaveActually < enemiesWave.Capacity)
            { 
                if (Spawnstate == SpawnState.Open)
                    OpenTheDoor();
            }

            

        }

    }

    #endregion

    #region Open/CloseDoor

    void OpenTheDoor()
    {
        Vector3 DoorPos = transform.GetChild(0).gameObject.transform.localPosition;
        if (currendistance < transform.GetChild(0).gameObject.transform.localScale.y)
        {
            
            DoorPos.y += speedDoore * Time.deltaTime;
            currendistance += speedDoore * Time.deltaTime;
            transform.GetChild(0).gameObject.transform.localPosition = DoorPos;
        }

        else
        {
            Spawnstate = SpawnState.Spawn;
            StartCoroutine(InstanciateEnemy());
            currendistance = 0;
        }
    }

    IEnumerator Closethedoor()
    {
        
        while (currendistance < transform.GetChild(0).gameObject.transform.localScale.y)
        {
                Vector3 DoorPos = transform.GetChild(0).gameObject.transform.localPosition;
                DoorPos.y -= speedDoore * Time.deltaTime;
                currendistance += speedDoore * Time.deltaTime;
                transform.GetChild(0).gameObject.transform.localPosition = DoorPos;
                yield return new WaitForSeconds(0);        
        }

       
            yield return new WaitForSeconds(10);

            enemiesWaveActually++;
            EnemyActually = 0;
            currendistance = 0;
            Spawnstate = SpawnState.Open;
        
    }

    #endregion

    #region InstanciateEnemy

    IEnumerator InstanciateEnemy()
    {
        while (EnemyActually < enemiesWave[enemiesWaveActually].enemies.Capacity && Spawnstate == SpawnState.Spawn)
        {
            if (GameMgr.Instance.State == GameMgr.GameState.Pause)
            {
                yield return new WaitForSeconds(0.0f);
            }
            else
            {
                Vector3 pos = transform.position;
                pos.y -= 0.50f;
                Instantiate(enemiesWave[enemiesWaveActually].enemies[EnemyActually], pos, Quaternion.Euler(0, angle, 0));
                EnemyActually++;
                yield return new WaitForSeconds(2.0f);
            }
           
        }
        Spawnstate = SpawnState.CLose;
        StartCoroutine(Closethedoor());
    }

    #endregion

}