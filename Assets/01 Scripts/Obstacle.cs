using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Obstacle : MonoBehaviour
{
    GameController gameController;
    GameObject player;
    // Start is called before the first frame update
    void Start()
    {
        gameController = FindObjectOfType<GameController>().GetComponent<GameController>();
        player = FindObjectOfType<Player>().gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.gameObject.tag == "Player")
        {
            gameController.gameFailed = true;
        }
    }
}
