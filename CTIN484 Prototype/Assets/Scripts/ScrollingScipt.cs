using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScrollingScipt : MonoBehaviour
{
    public GameObject[] movingobjects;
    public float movingSpeed;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 moveVec = new Vector3(0, 0, movingSpeed);

        for(int i = 0; i < movingobjects.Length; i++)
        {
            movingobjects[i].transform.position += moveVec * Time.deltaTime;
        }
    }
}
