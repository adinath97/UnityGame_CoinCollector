using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gem : MonoBehaviour
{
    public AudioClip myAudioClip;
    private AudioSource myAudioSource;

    private void Start() {
        myAudioSource = this.GetComponent<AudioSource>();
    }
    
    private void OnTriggerEnter2D(Collider2D other) {
        RespondToPlayer(other);
    }

    private void RespondToPlayer(Collider2D other) {
        if(other.gameObject.tag == "Player") {
            this.GetComponent<CircleCollider2D>().enabled = false;
            this.GetComponent<SpriteRenderer>().enabled = false;
            myAudioSource.PlayOneShot(myAudioClip);
            GameManager myGameManager = GameObject.FindObjectOfType<GameManager>();
            myGameManager.AddGemPoints();
            Vector2 myPosition = new Vector2(this.transform.position.x, this.transform.position.y);
            myGameManager.EliminateGem(myPosition);
        }
    }
}
