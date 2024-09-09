using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectSensor : MonoBehaviour
{
    public bool dectected;

    private void OnTriggerStay2D(Collider2D collision)
    {
        dectected = true;
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        dectected = false;
    }
}
