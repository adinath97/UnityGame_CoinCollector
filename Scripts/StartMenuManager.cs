using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartMenuManager : MonoBehaviour
{
    [SerializeField] GameObject startFade;
    [SerializeField] GameObject endFade;
    
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(StartRoutine());
    }

    private IEnumerator StartRoutine() {
        startFade.SetActive(true);
        endFade.SetActive(false);
        yield return new WaitForSeconds(.5f);
        startFade.SetActive(false);
    }

    public void ActivateEndFade() {
        endFade.SetActive(true);
    }
}
