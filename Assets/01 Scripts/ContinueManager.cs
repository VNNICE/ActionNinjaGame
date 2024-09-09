using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ContinueManager : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI scoreText;
    AudioSource audioSource;
    [SerializeField] AudioClip goToTitleSound;
    // Start is called before the first frame update
    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }
    private void Start()
    {
        scoreText.text = ClearRecords.inst.LoadClearTIme();
    }
    public string titlesceneName = "Title";
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            StartCoroutine(WaitForSoundEnd());
         }
    }

    IEnumerator WaitForSoundEnd()
    {
        audioSource.PlayOneShot(goToTitleSound);
        yield return new WaitForSeconds(goToTitleSound.length);
        SceneManager.LoadScene(titlesceneName);
    }
}
