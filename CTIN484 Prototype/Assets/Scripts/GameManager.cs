using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour {
    //System AudioSources

    public GameObject groceryManager;
    public GameObject groceryManager2;

    [Header ("Sound")]
    public AudioClip[] soundEffects;
    private AudioSource[] audioSource;

    [Header ("GameObjects")]
    public GameObject player1;
    public GameObject player2;
    public GameObject player1Radius;
    public GameObject player2Radius;
    public GameObject cart;
    public GameObject cart2;
    public GameObject finishLine;

    [Header ("UI")]
    public Text alert;
    public Text timesupResult;
    public Text gameOverResult;
    public Text timer;

    public GameObject instructionPanel;
    public Text countDownText;
    public GameObject gameOverPanel;
    public GameObject timesUpPanel;
    public GameObject Star1;
    public GameObject Star2;
    public GameObject Star3;

    [Header ("HP & Timer")]
    public Slider HPSlider;
    public float maxHP = 100.0f;
    public float hp = 100.0f;
    public float gameTimer = 60.0f;

    //NOT IN INSPECTOR
    public static string tutorialSceneName = "RealTutorial";
    public static string scene2Name = "Level2";

    public static int EndSound = 0;
    public static int AlertBGM = 1;
    public static int BGM = 2;
    public static int SFX = 3;
    public static float socialDistance = 6.0f;

    private bool endSoundPlaying = false;
    private bool winScene = false;
    private bool gameStart = false;
    private bool circleAppeared = false;

    // Start is called before the first frame update
    void Start () {
        HPSlider.minValue = 0f;
        HPSlider.maxValue = maxHP;
        HPSlider.value = hp;

        //Play AlertBGM and regular BGM at the same time
        //set AlertBGM's volume to 0
        audioSource = GetComponents<AudioSource> ();
        audioSource[AlertBGM].clip = soundEffects[2];
        audioSource[AlertBGM].volume = 0.0f;
        audioSource[AlertBGM].Play ();
        audioSource[AlertBGM].loop = true;

        audioSource[BGM].clip = soundEffects[3];
        audioSource[BGM].volume = 0.5f;
        audioSource[BGM].Play ();
        audioSource[BGM].loop = true;

        countDownText.gameObject.SetActive (false);
        instructionPanel.SetActive (true);
        gameOverPanel.SetActive (false);
        timesUpPanel.SetActive (false);

        player1Radius.SetActive (false);
        player2Radius.SetActive (false);

        Time.timeScale = 0.0f;
    }

    // Update is called once per frame
    void Update () {
        //Press any key to continue
        if (!gameStart && Input.anyKey) {
            instructionPanel.SetActive (false);
            StartCoroutine (CountDown ());
            gameStart = true;
        }

        //restart game
        if (Input.GetKey (KeyCode.R)) {
            SceneManager.LoadScene (SceneManager.GetActiveScene ().name);
        }

        //handle timer
        gameTimer -= Time.deltaTime;
        timer.text = ((int) gameTimer).ToString ();
        if (gameTimer < 0 && !winScene) {
            Win ();
        }

        //if cart hits finish line
        if ((cart.transform.position.z + 1.0f >= finishLine.transform.position.z && !winScene) ||
            (cart2 != null && cart2.transform.position.z + 1.0f >= finishLine.transform.position.z && !winScene)) {
            Debug.Log ("Cart hits finish line");
            Win ();
        }

        HPSlider.value = hp;
        float distance = Vector3.Distance (player1.transform.position, player2.transform.position);

        if (distance < socialDistance + 1.0f) { //when players are almost too close
            //set the circles to active
            player1Radius.SetActive (true);
            player2Radius.SetActive (true);

            //play circle appear sound
            audioSource[SFX].clip = soundEffects[4];
            if (!audioSource[SFX].isPlaying && !circleAppeared) {
                audioSource[SFX].Play ();
            }

            circleAppeared = true;

            //set the circles' color to green
            player1Radius.GetComponent<SpriteRenderer> ().color = new Color (0, 255.0f, 0, 0.7f);
            player2Radius.GetComponent<SpriteRenderer> ().color = new Color (0, 255.0f, 0, 0.7f);
            //clear alert text
            alert.text = "";

            //switch to alert BGM
            if (audioSource[AlertBGM].volume == 0 && !endSoundPlaying) {
                audioSource[AlertBGM].volume = 0.5f;
                audioSource[BGM].volume = 0.0f;
            }

            if (distance < socialDistance) {
                alert.text = "TOO CLOSE!!";
                hp -= 20 * Time.deltaTime;
                if (hp < 0) {
                    Lose ();
                }
                //change the circles' color to red
                player1Radius.GetComponent<SpriteRenderer> ().color = new Color (255.0f, 0, 0, 0.7f);
                player2Radius.GetComponent<SpriteRenderer> ().color = new Color (255.0f, 0, 0, 0.7f);

                //metric
                MetricManager.AddToTooCloseTimer (Time.deltaTime);
            }
        } else {
            alert.text = "";

            if (!audioSource[SFX].isPlaying && circleAppeared) {
                //play circle disappear sound
                audioSource[SFX].clip = soundEffects[5];
                audioSource[SFX].Play ();
                circleAppeared = false;
            }

            player1Radius.SetActive (false);
            player2Radius.SetActive (false);
            //switch back to regular BGM
            audioSource[AlertBGM].volume = 0.0f;
            audioSource[BGM].volume = 0.5f;
        }
    }

    void Win () {
        winScene = true;

        if (!endSoundPlaying) {
            foreach (AudioSource source in audioSource) {
                if (source != audioSource[EndSound]) {
                    source.Stop ();
                }
            }
            audioSource[EndSound].clip = soundEffects[0];
            audioSource[EndSound].Play ();
            endSoundPlaying = true;
        }

        Time.timeScale = 0;
        int totalCollected = groceryManager.GetComponent<GroceryManager> ().GetTotalItemsCollected ();
        int totalNeeded = groceryManager.GetComponent<GroceryManager> ().GetTotalItemsNeeded ();

        if (groceryManager2 != null) {
            totalCollected += groceryManager2.GetComponent<GroceryManager> ().GetTotalItemsCollected ();
            totalNeeded += groceryManager2.GetComponent<GroceryManager> ().GetTotalItemsNeeded ();
        }

        timesUpPanel.SetActive (true);
        timesupResult.text += totalCollected.ToString () + " purchased / " +
            totalNeeded.ToString () + " needed. \n";
        timesupResult.text += "Final HP bonus: " + ((int) hp).ToString ();

        //stars
        double groceryScore = (double) totalCollected / (double) totalNeeded;
        double hpScore = (hp / 100.0) * 0.2;
        if (groceryScore + hpScore >= 0.6 && groceryScore + hpScore <= 0.85) {
            Star2.SetActive (true);

            Star1.SetActive (false);
            Star3.SetActive (false);
        } else if (groceryScore + hpScore > 0.9) {
            Star3.SetActive (true);
            Star1.SetActive (false);
            Star2.SetActive (false);
        }

    }

    void Lose () {
        if (!endSoundPlaying) {
            foreach (AudioSource source in audioSource) {
                source.Stop ();
            }
            audioSource[EndSound].clip = soundEffects[1];
            audioSource[EndSound].Play ();
            endSoundPlaying = true;
        }

        Time.timeScale = 0;
        gameOverPanel.SetActive (true);

        gameOverResult.text =
            "You failed at social distancing.\n" +
            "You are arrested by the CDC for mandatory testing...";
    }

    IEnumerator CountDown () {
        countDownText.gameObject.SetActive (true);
        countDownText.text = "3";
        yield return new WaitForSecondsRealtime (1.0f);
        countDownText.text = "2";
        yield return new WaitForSecondsRealtime (1.0f);
        countDownText.text = "1";
        yield return new WaitForSecondsRealtime (1.0f);
        countDownText.gameObject.SetActive (false);
        Time.timeScale = 1.0f;
    }

}