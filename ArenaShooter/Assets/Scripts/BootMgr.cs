using UnityEngine;
using System.Collections.Generic;

public class BootMgr : MonoBehaviour {

    #region Members

    [SerializeField]
    List<GameObject> MgrPrefabList = null;

    static bool isInstantiated = false;

    #endregion

    #region Awake

    void Awake()
    {
        if (isInstantiated == false)
        {
            foreach(GameObject prefab in MgrPrefabList)
            {
                GameObject mgrInst = Instantiate(prefab);
                mgrInst.name = mgrInst.name.Replace("(Clone)", "");
                DontDestroyOnLoad(mgrInst);
            }
            isInstantiated = true;
        }
        Destroy(gameObject);
    }

    #endregion

}
