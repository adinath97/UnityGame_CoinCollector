using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneManagerScript : MonoBehaviour
{
    [SerializeField] AudioClip clickSFX;
    private AudioSource myAudioSource;

    private void Start() {
        myAudioSource = this.GetComponent<AudioSource>();
    }
    
    public void PlayGame() {
        myAudioSource.PlayOneShot(clickSFX);
        if(SceneManager.GetActiveScene().name == "GameOverScene") {
            GameOverManager myGameOverManager = GameObject.FindObjectOfType<GameOverManager>();
            myGameOverManager.ActivateEndFade();
        }
        if(SceneManager.GetActiveScene().name == "StartMenu") {
            StartMenuManager myStartMenuManager = GameObject.FindObjectOfType<StartMenuManager>();
            myStartMenuManager.ActivateEndFade();
        }
        StartCoroutine(PlayAgainRoutine());
    }

    private IEnumerator PlayAgainRoutine() {
        yield return new WaitForSeconds(.5f);
        SceneManager.LoadScene("GameScene");
    }

    public void StartMenu() {
        myAudioSource.PlayOneShot(clickSFX);
        if(SceneManager.GetActiveScene().name == "GameOverScene") {
            GameOverManager myGameOverManager = GameObject.FindObjectOfType<GameOverManager>();
            myGameOverManager.ActivateEndFade();
        }
        StartCoroutine(StartMenuRoutine());
    }

    private IEnumerator StartMenuRoutine() {
        yield return new WaitForSeconds(.5f);
        SceneManager.LoadScene("StartMenu");
    }

    public void QuitGame() {
        myAudioSource.PlayOneShot(clickSFX);
        Application.Quit();
    }
}
