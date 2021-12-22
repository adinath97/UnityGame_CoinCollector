using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [SerializeField] List<Image> lifeImages = new List<Image>();
    [SerializeField] Sprite lifeGoneSprite;

    [SerializeField] GameObject endFade;
    [SerializeField] GameObject startFade;

    [SerializeField] TextMeshProUGUI coinTotalBox;
    [SerializeField] TextMeshProUGUI gemTotalBox;

    [SerializeField] Transform allCoins;
    [SerializeField] Transform allGems;

    [SerializeField] AudioClip[] myAudioClips;

    public static Transform[,] coinsGrid = new Transform[9,9];
    public static string[,] coinNamesGrid = new string[9,9];
    public static bool[,] coinsTakenGrid = new bool[9,9];

    private Transform[,] gemsGrid = new Transform[9,9];
    public static string[,] gemNamesGrid = new string[9,9];
    public static bool[,] gemsTakenGrid = new bool[9,9];

    private static int livesGone, coinTotal, gemTotal;

    private AudioSource myAudioSource;

    private Player myPlayer;
    private MoleMovement myMole;

    private int totalCoins = 76, totalGems = 4;

    private void Start() {
        SetUp();
    }

    private void SetUp() {
        myAudioSource = this.GetComponent<AudioSource>();
        myAudioSource.PlayOneShot(myAudioClips[0]);
        if(livesGone == 0) {
            startFade.SetActive(true);
        }
        endFade.SetActive(false);
        myPlayer = GameObject.FindObjectOfType<Player>();
        myMole = GameObject.FindObjectOfType<MoleMovement>();
        PopulateCoinsGrid();
        PopulateGemsGrid();
        if(livesGone > 0) {
            for(int i = 0; i < livesGone; i++) {
                lifeImages[2 - i].sprite = lifeGoneSprite;
            }
        }
        else {
            PopulateInitialCoinGrid();
            PopulateInitialGemGrid();
        }
        DisplayCollectibles();
        coinTotalBox.text = coinTotal.ToString();
        gemTotalBox.text = gemTotal.ToString();
    }

    public void EliminateCoin(Vector2 position) {
        //find where coin is in the grid
        coinsTakenGrid[Mathf.RoundToInt(position.x) + 4, Mathf.RoundToInt(position.y) + 4] = true;
    }

    public void EliminateGem(Vector2 position) {
        //find where gem is in the grid
        gemsTakenGrid[Mathf.RoundToInt(position.x) + 4, Mathf.RoundToInt(position.y) + 4] = true;
    }

    private void PopulateCoinsGrid() {
        int rowIndex = 0, columnIndex = 8;
        for(int coinIndex = 0; coinIndex < allCoins.transform.childCount; coinIndex++) {
            coinsGrid[rowIndex,columnIndex] = allCoins.GetChild(coinIndex).gameObject.transform;
            rowIndex++;
            if(rowIndex > 8) {
                columnIndex--;
                rowIndex = 0;
            }
        }
    }

    private void PopulateGemsGrid() {
        gemsGrid[8,0] = allGems.GetChild(0).gameObject.transform;
        gemsGrid[8,8] = allGems.GetChild(1).gameObject.transform;
        gemsGrid[0,0] = allGems.GetChild(2).gameObject.transform;
        gemsGrid[0,8] = allGems.GetChild(3).gameObject.transform;
    }

    private void DisplayCollectibles() {
        for(int i = 0; i < 9; i++) {
            for(int j = 0; j < 9; j++) {
                //display coins
                if(coinsTakenGrid[i,j]) {
                    coinsGrid[i,j].gameObject.GetComponent<SpriteRenderer>().enabled = false;
                    coinsGrid[i,j].gameObject.GetComponent<CircleCollider2D>().enabled = false;
                }
                //display gems
                if(gemsGrid[i,j] == null) { continue; }
                if(gemsTakenGrid[i,j]) {
                    gemsGrid[i,j].gameObject.GetComponent<SpriteRenderer>().enabled = false;
                    gemsGrid[i,j].gameObject.GetComponent<CircleCollider2D>().enabled = false;
                }
            }
        }
    }

    private void PopulateInitialCoinGrid() {
        //populate coinGrid
        //add each child coin to the coinGrid as appropriate
        int rowIndex = 0, columnIndex = 8;
        for(int coinIndex = 0; coinIndex < allCoins.transform.childCount; coinIndex++) {
            coinNamesGrid[rowIndex,columnIndex] = allCoins.GetChild(coinIndex).gameObject.name;
            coinsTakenGrid[rowIndex,columnIndex] = false;
            rowIndex++;
            if(rowIndex > 8) {
                columnIndex--;
                rowIndex = 0;
            }
        }
    }

    private void PopulateInitialGemGrid() {
        int rowIndex1 = 0, columnIndex1 = 8, gemIndex0 = 0;
        for(int gemIndex = 0; gemIndex < 81; gemIndex++) {
            if(gemsGrid[rowIndex1,columnIndex1] != null) {
                gemNamesGrid[rowIndex1,columnIndex1] = allGems.GetChild(gemIndex0).gameObject.name;
                gemIndex0++;
            }
            gemsTakenGrid[rowIndex1,columnIndex1] = false;
            rowIndex1++;
            if(rowIndex1 > 8) {
                columnIndex1--;
                rowIndex1 = 0;
            }
        }
    }
    
    public void RemoveALife() {
        livesGone++;
        lifeImages[3 - livesGone].sprite = lifeGoneSprite;
        if(livesGone == 3) {
            StartCoroutine(GameOverRoutine());
        }
        else {
            StartCoroutine(LoadGameSceneAgainRoutine());
        }
    }

    private IEnumerator LoadGameSceneAgainRoutine() {
        myPlayer.PlayerDead();
        yield return new WaitForSeconds(1f);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void AddCoinPoints() {
        coinTotal++;
        coinTotalBox.text = coinTotal.ToString();
        CheckIfGameOver();
    }

    public void AddGemPoints() {
        gemTotal++;
        gemTotalBox.text = gemTotal.ToString();
        CheckIfGameOver();
    }

    private void CheckIfGameOver() {
        if(coinTotal == totalCoins && gemTotal == totalGems) {
            StartCoroutine(GameOverRoutine());
        } 
    }

    private IEnumerator GameOverRoutine() {
        if(coinTotal == totalCoins && gemTotal == totalGems) {
            myAudioSource.PlayOneShot(myAudioClips[1]);
        }
        else {
            myPlayer.PlayerDead();
        }
        yield return new WaitForSeconds(.5f);
        endFade.SetActive(true);
        yield return new WaitForSeconds(.5f);
        GameOverManager.SetTotals(coinTotal,gemTotal);
        livesGone = 0;
        coinTotal = 0;
        gemTotal = 0;
        SceneManager.LoadScene("GameOverScene");
    }
}
