using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TitileManager : MonoBehaviour
{
    [SerializeField] GameObject titleEffect;
    [SerializeField] Canvas titleUI;
    [SerializeField] Canvas howToPlay;
    [SerializeField] Canvas credits;
    [SerializeField] GameObject mainCamera;

    Animator animator;
    AudioSource audioSource;
    [SerializeField] AudioClip effectSound;
    [SerializeField] AudioClip buttonSound;
    [SerializeField] AudioClip titleBGM;
    [SerializeField] AudioClip startSound;
    [SerializeField] AudioClip howToPlayToTitleSound;
    [SerializeField] AudioClip howToPlayBGM;

    bool onHowToPlay;
    bool onCredits;
    public bool effectEnd = false;
    public bool canControl = false;
    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        //自動再生及び繰り返しを防止
        if (audioSource) 
        {
            audioSource.playOnAwake = false;
            audioSource.loop = true;
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();

        titleUI.gameObject.SetActive(false);
        onHowToPlay = false;
        onCredits = false;
        howToPlay.gameObject.SetActive(false);
        credits.gameObject.SetActive(false);
        
        animator.gameObject.SetActive(true);
    }
    private void Update()
    {
        animator.SetBool("EffectEnd", effectEnd);
        ControlSettings();
    }

    void ControlSettings()
    {
        if (canControl && Input.GetKeyDown(KeyCode.Space) && !onCredits) 
        {
            onHowToPlay = !onHowToPlay;
            ShowHowToControl();
            Debug.Log("onHowToControl: " + onHowToPlay);
        }

        if (canControl && Input.GetKeyDown(KeyCode.C) && !onHowToPlay && !onCredits)
        {
            onCredits = true;
            ShowCredits();
            Debug.Log("onCredits: " + onCredits);
        }

        if (canControl && Input.GetKeyDown(KeyCode.Space) && !onHowToPlay && onCredits)
        {
            onCredits = false;
            ShowCredits();
            Debug.Log("onCredits: " + onCredits);
        }

        if (canControl && Input.GetKeyDown(KeyCode.Return) && !onCredits && !onHowToPlay)
        {
            StartCoroutine(WaitForStartSoundEnd());
        }
    }

    void ShowHowToControl() 
    {
        
        Debug.Log("ShowHowToControl - titleUI active: " + titleUI.gameObject.activeSelf);
        if (onHowToPlay)
        {
            audioSource.PlayOneShot(buttonSound);
            audioSource.clip = howToPlayBGM;
            audioSource.Play();
            titleUI.gameObject.SetActive(false);
            howToPlay.gameObject.SetActive(true);
            
            

        }
        else if(!onHowToPlay)
        {
            audioSource.PlayOneShot(howToPlayToTitleSound);
            audioSource.clip = titleBGM;
            howToPlay.gameObject.SetActive(false);
            titleUI.gameObject.SetActive(true);
            audioSource.Play();
        }
    }
    void ShowCredits()
    {
        audioSource.PlayOneShot(buttonSound);
        if (onCredits)
        {
            titleUI.gameObject.SetActive(false);
            credits.gameObject.SetActive(true);
        }
        else if(!onCredits)
        {
            credits.gameObject.SetActive(false);
            titleUI.gameObject.SetActive(true);
        }
    }
    IEnumerator WaitForStartSoundEnd()
    {
        audioSource.PlayOneShot(startSound);
        yield return new WaitForSeconds(startSound.length);
        SceneManager.LoadScene("Stage1");
    }

    //アニメーションイベントで起動させています。
    void OnTitleEffectEnd()
    {
        effectEnd = true;
        titleUI.gameObject.SetActive(true);
        audioSource.Play();
        Debug.Log(effectEnd);
    }

    void ActiveControl() 
    {
        canControl = true;
    }

    void EffectSound() 
    {
        audioSource.PlayOneShot(effectSound);
    }

    void TitleBGMOn() 
    {
        audioSource.Play();
    }
}
