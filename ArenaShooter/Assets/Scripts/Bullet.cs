using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour {

    #region Trigger

    void OnTriggerEnter(Collider col)
    {
        if (col.gameObject.layer != LayerMask.NameToLayer("Player") && col.gameObject.layer != LayerMask.NameToLayer("Enemy"))
        {
            Destroy(gameObject);
        }
    }

    #endregion

}
