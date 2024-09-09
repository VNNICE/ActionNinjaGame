using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActiveScaffold : MonoBehaviour
{
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.isTrigger && collision.gameObject.CompareTag("Scaffold"))
        {
            Debug.Log("Scaffold On!");
            collision.GetComponent<BoxCollider2D>().isTrigger = false;
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (!collision.isTrigger && collision.gameObject.CompareTag("Scaffold")) 
        {
            Debug.Log("Scaffold Off!");
            collision.GetComponent<BoxCollider2D>().isTrigger = true;
        }
    }
}
