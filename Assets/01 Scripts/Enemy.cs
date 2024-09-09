using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    GameController gameController;
    GameObject player;

    private void Start()
    {
        gameController = FindObjectOfType<GameController>().GetComponent<GameController>();
        player = FindObjectOfType<Player>().gameObject;
        if (gameController == null)
        {
            Debug.Log("Enemy: Cannot Find GameContorller");
        }
        else
        {
            Debug.Log("Enemy: Successfully find GameContorller");
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject ==  player)
        {
            gameController.gameClear = true;
            gameController.onGame = false;
            Debug.Log("Clear");
        }
    }
}
