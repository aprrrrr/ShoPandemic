using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialScript : MonoBehaviour {

    //instruction screen
    public GameObject instructions;

    // Start is called before the first frame update
    void Start () {
        instructions.SetActive (true);
        Time.timeScale = 0.0f;
    }

    // Update is called once per frame
    void Update () {
        //Press any key to continue
        if (Input.anyKey) {
            instructions.SetActive (false);
            Time.timeScale = 1.0f;
        }
    }
}