using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class GameController : MonoBehaviour
{
    bool countdownEnd;

    [SerializeField] GameObject player;
    [SerializeField] GameObject enemy;
    [SerializeField] Canvas gameClearUI;
    [SerializeField] Canvas gameOverUI;
    [SerializeField] Canvas mainUI;
    [SerializeField] Canvas pauseUI;
    [SerializeField] TextMeshProUGUI countdownText;
    [SerializeField] TextMeshProUGUI timerText;
    [SerializeField] TextMeshProUGUI gameClearTimeText;
    [SerializeField] TextMeshProUGUI gameOverText;
    [SerializeField] TextMeshProUGUI distanceMeasureText;

    [SerializeField] GameObject coin;

    AudioSource audioSource;
    [SerializeField] AudioClip gameClearSound;
    [SerializeField] AudioClip gameOverSound;
    [SerializeField] AudioClip countdownSound;
    bool onClearSound;
    bool onOverSound;
    bool onCountdownSound;

    float timer = 5f;
    float countdownTimer = 3;

    bool blink = false;
    public bool onGame;
    public bool gameStarted;
    public bool gameClear;
    public bool gameFailed;
    private bool coinDropCoolTime;
    private bool saved = false;
    private bool isPause;

    private string jsonPath;

    int[] playerLocation = new int[2];
    int[] enemyLocation = new int[2];
    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource)
        {
            audioSource.playOnAwake = false;
            audioSource.loop = false;
        }
        mainUI.gameObject.SetActive(false);
        gameStarted = false;
        gameFailed = false;

        //jsonPath = Resources.Load<TextAsset>("ClearRecords").ToString();
        pauseUI.gameObject.SetActive(false);
        gameClearUI.gameObject.SetActive(false);
        gameOverUI.gameObject.SetActive(false);
        onGame = false;
    }
    // Start is called before the first frame update
    void Start()
    {
        onClearSound = false;
        onOverSound = false;
        onCountdownSound = false;
        StartCoroutine(CoinDrop(coin));
        StartCoroutine(GameStart());
    }
    private void Update()
    {
        OnCountdown();
        GetDistance();
        GameTimerOn();
        CheckGameClearStatus();
        GamePause();
    }

    // Update is called once per frame
    void FixedUpdate()
    {

    }

    void GamePause()
    {
        if (Input.GetKeyDown(KeyCode.Escape)) 
        {
            if (onGame && gameStarted) 
            {
                isPause = !isPause;
                if (isPause)
                {
                    Time.timeScale = 0;
                    pauseUI.gameObject.SetActive(true);
                }
                else 
                {
                    pauseUI.gameObject.SetActive(false);
                    Time.timeScale = 1;
                }
            }
        }
        if (isPause) 
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                Time.timeScale = 1;
                SceneManager.LoadScene("Title");
            }
        }
    }

    void OnCountdown()
    {
        if (onGame || gameStarted)
        {
            return;
        }
        if (!onCountdownSound) 
        {
            onCountdownSound = true;
            audioSource.PlayOneShot(countdownSound);
        }
        if (countdownTimer > 2f)
        {
            countdownText.text = "三";
        }
        else if (countdownTimer > 1f) 
        {
            countdownText.text = "二";
        }
        else if (countdownTimer > 0.1f)
        {
            countdownText.text = "一";
        }
        else
        {
            countdownEnd = true;
        }

        countdownTimer -= Time.deltaTime;

    }
    IEnumerator GameStart() 
    {
        yield return new WaitUntil(() => !onGame && !gameStarted && countdownEnd);
        countdownText.text = "いざ参る!";
        yield return new WaitForSeconds(1f);
        audioSource.Play();
        mainUI.gameObject.SetActive(true);
        countdownText.gameObject.SetActive(false);
        onGame = true;
        gameStarted = true;
        countdownEnd = false;
        StopCoroutine(GameStart());
        
    }
    IEnumerator Blinked()
    {
        while (blink) 
        {
            timerText.enabled = !timerText.enabled;
            yield return new WaitForSeconds(0.1f);
        }
    }

    void GameTimerOn() 
    {
        if (onGame && gameStarted)
        {
            timerText.text = timer.ToString("0.00").Replace(".", ":");
            timer -= Time.deltaTime;
            if (timer > 0 && timer <= 2 && !blink)
            {
                blink = true;
                StartCoroutine(Blinked());
            }
            else if (timer <= 0)
            {
                blink = false;
                if (!timerText.enabled)
                {
                    timerText.gameObject.SetActive(true);
                }
                timer = 0;
                onGame = false;
                gameFailed = true;
            }
        }
    }

    private void GetDistance()
    {
        playerLocation[0] = (int)player.transform.position.x;
        playerLocation[1] = (int)player.transform.position.y;

        enemyLocation[0] = (int)enemy.transform.position.x;
        enemyLocation[1] = (int)enemy.transform.position.y;

        if (gameClear)
        {
            distanceMeasureText.text = "0";
        }
        else 
        {
            distanceMeasureText.text = (Math.Abs(enemyLocation[0] - playerLocation[0]) + Math.Abs(enemyLocation[1] - playerLocation[1])).ToString();

        }
    }
    IEnumerator RecordClearTime() 
    {
        ClearRecords.inst.AddClearTime(float.Parse(timer.ToString("0.00")));
        saved = true;
        yield return null;
    }
    
    private void CheckGameClearStatus()
    {
        if (!onGame && gameStarted)
        {
            if (gameClear)
            {
                audioSource.clip = null;
                GameOverSoundPlay(gameClearSound, ref onClearSound);
                gameClearTimeText.text = $"クリア時間: {timer.ToString("0.00")}\nEnterキーで次のステージへ";
                gameClearUI.gameObject.SetActive(true);
                if (saved == false)
                {
                    StartCoroutine(RecordClearTime());
                }
                if (Input.GetKeyDown(KeyCode.Return))
                {
                    if (SceneManager.GetActiveScene().name == "Stage5")
                    {
                        SceneManager.LoadScene("Continue");
                    }
                    else
                    {
                        GoNextStage();
                    }
                }
            }
            else if (!gameClear)
            {
                gameFailed = true;
            }
        }
        if (gameFailed)
        {
            if (onGame) 
            {
                onGame = false;
            }
            audioSource.clip = null;
            GameOverSoundPlay(gameOverSound, ref onOverSound);
            gameOverUI.gameObject.SetActive(true);
            gameOverText.text = "Game Over \n\n<size=100>Enterキーで\nリスタート</size>";
            if (Input.GetKeyDown(KeyCode.Return))
            {
                RestartStage();
            }
        }
    }

    public void RestartStage() 
    {
       UnityEngine.SceneManagement.Scene thisScene = SceneManager.GetActiveScene();
       SceneManager.LoadScene(thisScene.name);
    }
    public void GoNextStage()
    {
        string nowStage = SceneManager.GetActiveScene().name;
        int stageNumber = int.Parse(nowStage.Substring(nowStage.Length - 1)[0].ToString());
        SceneManager.LoadScene("Stage" + (stageNumber + 1));
    }
    void GameOverSoundPlay(AudioClip audioClip, ref bool on)
    {
        if (!on)
        {
            on = true;
            audioSource.Stop();
            audioSource.PlayOneShot(audioClip);
        }
    }


    IEnumerator CoinDrop(GameObject spawnObject)
    {
        while (true) 
        {
            if (gameClear && !coinDropCoolTime)
            {
                coinDropCoolTime = true;
                List<Vector3> positions = PositionLists();
                Shuffle(positions);
                int dropCount = UnityEngine.Random.Range(3, 6);
                Debug.Log($"{positions[0].x}, {positions[1].x}, {positions[2].x}, {positions[3].x}, {positions[4].x}, {positions[5].x}");
                for (int i = 0; i < dropCount; i++)
                {
                    float xPos = UnityEngine.Random.Range(-21f, 21f);
                    float randomRotation = RotationList()[UnityEngine.Random.Range(0, 3)];
                    GameObject spawnObj = Instantiate(spawnObject, positions[i], Quaternion.identity);
                    spawnObj.AddComponent<Rigidbody2D>();
                    spawnObj.GetComponentInChildren<Transform>().rotation = Quaternion.Euler(0f,0f,randomRotation);
                }
            }
            yield return new WaitForSeconds(0.2f);
            coinDropCoolTime = false;
        }
        List<float> RotationList() 
        {
            List<float> rotationZ = new List<float>();
            rotationZ.Add(-45f);
            rotationZ.Add(0f);
            rotationZ.Add(45f);
            return rotationZ;
        }

        List<Vector3> PositionLists()
        {
            List<Vector3> positions = new List<Vector3>();
            float distance = 0f;

            for (int i = 0; i < 9; i++)
            {
                Vector3 position = new Vector3(spawnObject.transform.position.x + distance, spawnObject.transform.position.y, 0);
                positions.Add(position);
                distance += 6f;
            }
            return positions;
        }

        void Shuffle<T>(List<T> list)
        {
            int n = list.Count;
            for (int i = 0; i < n; i++)
            {
                int k = UnityEngine.Random.Range(i, n - 1);
                T value = list[i];
                list[i] = list[k];
                list[k] = value;
            }
        }
    }
}
