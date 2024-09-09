using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyCoin : MonoBehaviour
{
    void Update()
    {
        if (this.transform.position.y < -17f) 
        {
            Destroy(this.gameObject);
        }
    }
}
