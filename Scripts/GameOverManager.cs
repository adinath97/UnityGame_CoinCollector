using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class GameOverManager : MonoBehaviour
{
    [SerializeField] GameObject endFade;
    [SerializeField] GameObject startFade;

    [SerializeField] TextMeshProUGUI coinTotalBox;
    [SerializeField] TextMeshProUGUI gemTotalBox;
    [SerializeField] TextMeshProUGUI coinTotalBestBox;
    [SerializeField] TextMeshProUGUI gemTotalBestBox;

    private static int finalCoinTotal, finalGemTotal;
    
    // Start is called before the first frame update
    void Start()
    {
        SetUp();
    }

    private void SetUp() {
        StartCoroutine(StartRoutine());
        coinTotalBox.text = finalCoinTotal.ToString();
        gemTotalBox.text = finalGemTotal.ToString();
        if(finalCoinTotal > PlayerPrefs.GetInt("CoinHighScore",0)) {
            PlayerPrefs.SetInt("CoinHighScore",finalCoinTotal);
        }
        if(finalGemTotal > PlayerPrefs.GetInt("GemHighScore",0)) {
            PlayerPrefs.SetInt("GemHighScore",finalGemTotal);
        }
        coinTotalBestBox.text = PlayerPrefs.GetInt("CoinHighScore",0).ToString();
        gemTotalBestBox.text = PlayerPrefs.GetInt("GemHighScore",0).ToString();
    }

    public void ActivateEndFade() {
        endFade.SetActive(true);
    }

    private IEnumerator StartRoutine() {
        startFade.SetActive(true);
        endFade.SetActive(false);
        yield return new WaitForSeconds(.5f);
        startFade.SetActive(false);
    }

    public static void SetTotals(int coinTotal, int gemTotal) {
        finalCoinTotal = coinTotal;
        finalGemTotal = gemTotal;
    }
}
