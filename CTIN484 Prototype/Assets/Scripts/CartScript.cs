using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CartScript : MonoBehaviour {
    private float range = 1;
    public GameObject player1;
    public GameObject player2;
    public GameObject groceryManager;
    //public GameObject gameManager;
    public Vector3 spawnPosition; //relative to wall
    public Vector3 hidePosition; //relative to wall

    private bool outsideFrame = false;
    public float respawnTime = 5.0f;

    public Text respawnCountdown;

    private Transform world;
    private AudioSource audioSource;

    private void Start () {
        audioSource = gameObject.GetComponent<AudioSource> ();
    }
    void Update () {
        if ((Mathf.Abs (player1.transform.position.x - transform.position.x) <= range &&
                Mathf.Abs (player1.transform.position.y - transform.position.y) <= range) ||
            (Mathf.Abs (player2.transform.position.x - transform.position.x) <= range &&
                Mathf.Abs (player2.transform.position.y - transform.position.y) <= range)) {
            GetComponent<Rigidbody> ().isKinematic = false;
            Debug.Log (Mathf.Abs (player1.transform.position.z - transform.position.z));
            if ((Mathf.Abs (player1.transform.position.z - transform.position.z) <= 3)) {
                player1.GetComponent<Animator> ().SetBool ("pushCart", true);
            } else {
                player1.GetComponent<Animator> ().SetBool ("pushCart", false);
            }
            if ((Mathf.Abs (player2.transform.position.z - transform.position.z) <= 3)) {
                player2.GetComponent<Animator> ().SetBool ("pushCart", true);
            } else {
                player2.GetComponent<Animator> ().SetBool ("pushCart", false);
            }
        } else {
            GetComponent<Rigidbody> ().isKinematic = true;
            player1.GetComponent<Animator> ().SetBool ("pushCart", false);
            player2.GetComponent<Animator> ().SetBool ("pushCart", false);
        }

        //play sound if cart is moving
        if (GetComponent<Rigidbody> ().velocity.magnitude >= 0.5) {
            if (!audioSource.isPlaying) {
                audioSource.Play ();
            }
        } else {
            audioSource.Stop ();
        }

        //wait for respawn
        if (outsideFrame) {
            respawnCountdown.text = ((int) respawnTime + 1).ToString ();

            if (respawnTime > 0) {
                respawnTime -= Time.deltaTime;
            } else {
                respawnCountdown.gameObject.SetActive (false);
                Respawn ();
                respawnTime = 5.0f;
                outsideFrame = false;
            }
        }
    }

    public Vector3 GetRandomPositionInCart () {
        Transform cartSpace = transform.Find ("CartSpace");
        Bounds bounds = cartSpace.gameObject.GetComponent<Collider> ().bounds;
        float offsetX = Random.Range (-bounds.extents.x, bounds.extents.x);
        float offsetY = Random.Range (-bounds.extents.y, bounds.extents.y);
        float offsetZ = Random.Range (-bounds.extents.z, bounds.extents.z);

        return bounds.center + new Vector3 (offsetX, offsetY, offsetZ);
    }

    private void OnCollisionEnter (Collision other) {
        if (other.gameObject.CompareTag ("Wall")) {
            Debug.Log ("Cart hits wall");
            world = transform.parent;
            //set parent to wall so it moves with the frame
            transform.SetParent (other.transform);
            //hide cart
            transform.localPosition = hidePosition;
            outsideFrame = true;
            respawnCountdown.gameObject.SetActive (true);
        }
    }

    private void Respawn () {
        //unhide cart
        transform.localPosition -= hidePosition;
        transform.position += spawnPosition;
        transform.SetParent (world);
    }

}