using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class Player : MonoBehaviour
{
    GameController gameController;
    [SerializeField] private float jumpForce = 35f;
    [SerializeField] private float moveSpeed = 20f;
    [SerializeField] private float triangleJumpForce = 15f;
    Animator animator;
    SpriteRenderer spriteRenderer;

    private bool detectedLeft;
    private bool detectedRight;
    private bool onMove;
    private bool onTriangleJump;
    private bool isMove;
    private bool onGround;
    private bool canTriangleJump;
    private bool collisionRight;
    private bool collisionLeft;

    private ObjectSensor sensorLT;
    private ObjectSensor sensorLB;
    private ObjectSensor sensorRT;
    private ObjectSensor sensorRB;

    [SerializeField] private Vector2 angleLT = new Vector2(-1, 3);
    [SerializeField] private Vector2 angleRT = new Vector2(1, 3);

    Rigidbody2D rb;

    AudioSource audioSource;
    [SerializeField] AudioClip triangleJumpSound;
    [SerializeField] AudioClip jumpSound;


    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource)
        {
            audioSource.playOnAwake = false;
            audioSource.loop = false;
        }
    }
    void Start()
    {
        Debug.Log(SceneManager.GetActiveScene().name);
        animator = GetComponentInChildren<Animator>();
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        //SensorLeftTop, 左上
        sensorLT = transform.Find("Sensor_LT").GetComponent<ObjectSensor>();
        //LeftBottom, 左下
        sensorLB = transform.Find("Sensor_LB").GetComponent<ObjectSensor>();
        //Right Top, 右上
        sensorRT = transform.Find("Sensor_RT").GetComponent<ObjectSensor>();
        //Right Bottom, 右下
        sensorRB = transform.Find("Sensor_RB").GetComponent<ObjectSensor>();

        gameController = FindObjectOfType<GameController>().GetComponent<GameController>();
        rb = GetComponent<Rigidbody2D>();
        if (gameController == null)
        {
            Debug.Log("Player: Cannot Find GameContorller");
        }
        else
        {
            Debug.Log("Player: Successfully find GameContorller");
        }

        
        onMove = true;
    }

    // Update is called once per frame
    void Update()
    {
        animator.SetBool("IsMove", isMove);
        animator.SetBool("OnGround", onGround);
        animator.SetFloat("VectorCheck", rb.velocity.y);
        animator.SetBool("OnTriangleJump", onTriangleJump);
        if (gameController.onGame)
        {
            DetectedOnRight();
            DetectedOnLeft();
            //Move
            if (onMove && Input.GetKey(KeyCode.A) && !detectedLeft && !collisionLeft)
            {
                //移動を押した時加速を初期化(三角飛び中など)
                rb.velocity = new Vector2(0, rb.velocity.y);
                spriteRenderer.flipX = false;
                transform.Translate(Vector2.left * moveSpeed * Time.deltaTime);
                isMove = true;
            }
            else if (onMove && Input.GetKey(KeyCode.D) && !detectedRight && !collisionRight)
            {
                rb.velocity = new Vector2(0, rb.velocity.y);
                spriteRenderer.flipX = true;
                transform.Translate(Vector2.right * moveSpeed * Time.deltaTime);
                isMove = true;
            }
            else 
            {
                isMove = false;
            }

            //Jump
            if (Input.GetKeyDown(KeyCode.Space) && onGround)
            {
                audioSource.PlayOneShot(jumpSound);
                rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            }//Triangle Jump
            else if (canTriangleJump && Input.GetKeyDown(KeyCode.Space) && !onGround && detectedLeft && !detectedRight && collisionLeft)
            {
                audioSource.PlayOneShot(triangleJumpSound);
                rb.velocity = angleRT * triangleJumpForce;
                StartCoroutine(WaitTriangleJump());
            }
            else if (canTriangleJump && Input.GetKeyDown(KeyCode.Space) && !onGround && !detectedLeft && detectedRight && collisionRight)
            {
                audioSource.PlayOneShot(triangleJumpSound);
                rb.velocity = angleLT * triangleJumpForce;
                StartCoroutine(WaitTriangleJump());
            }
        }
        else if (gameController.gameFailed)
        {
            this.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeAll;
            onMove = false;
            isMove = false;
            GetComponentInChildren<SpriteRenderer>().color = Color.red;
        }
        else if (gameController.gameClear)
        {
            this.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeAll;
            onMove = false;
            isMove = false;
        }

    }

    //三角飛び後すぐの方向転換を防ぐ
    IEnumerator WaitTriangleJump()
    {
        spriteRenderer.flipX = !spriteRenderer.flipX;
        onMove = false; 
        onTriangleJump = true;
        yield return new WaitForSeconds(0.3f);
        onMove = true;
        onTriangleJump = false;
    }

    //左上と左下を感知した時活性化、これにより左が完全に触れた時にだけ三角飛び可
    private void DetectedOnLeft()
    {
        if (sensorLT.dectected && sensorLB.dectected)
        {
            detectedLeft = true;
        }
        else
        {
            detectedLeft = false;
        }
    }
    //右上と右下を感知した時活性化
    private void DetectedOnRight()
    {
        if (sensorRT.dectected && sensorRB.dectected)
        {
            detectedRight = true;
        }
        else
        {
            detectedRight = false;
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        Vector2 normal = collision.contacts[0].normal;
        if (normal.y > 0)
        {
            rb.velocity = new Vector2(0, rb.velocity.y);
        }
        
        if (collision.gameObject.tag == "Ground" || collision.gameObject.CompareTag("Scaffold"))
        {
            onGround = true;
            //rb.velocity = new Vector2(0, rb.velocity.y);
        }

        //左右センサー感知だけではくっついてないのに三脚飛びができてしまうためCollisionを確認
        if (collision.gameObject.tag == "Wall")
        {
            canTriangleJump = true;
            if (normal.x < 0)
            {
                collisionRight = true;
            }else if (normal.x > 0)
            {
                collisionLeft = true;
            }
        }
    }

    private void OnCollisionExit2D(Collision2D collision) 
    {
        if (collision.gameObject.tag == "Ground" || collision.gameObject.CompareTag("Scaffold"))
        {
            onGround = false;
        }
        if (collision.gameObject.tag == "Wall")
        {
            canTriangleJump = false;
            collisionLeft = false;
            collisionRight = false;
        }
    }
}