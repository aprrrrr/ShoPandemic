using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuScript : MonoBehaviour {
    // Start is called before the first frame update
    void Start () {

    }

    // Update is called once per frame
    void Update () {

    }

    public void PlayGame () {
        SceneManager.LoadScene ("RealTutorial");
    }

    public void PlayScene2 () {
        SceneManager.LoadScene ("Level2");
    }

    public void BackToMenu () {
        SceneManager.LoadScene ("StartMenu");
    }

    public void Replay () {
        SceneManager.LoadScene (SceneManager.GetActiveScene ().name);
    }

    public void QuitGame () {
        Application.Quit ();
    }

}