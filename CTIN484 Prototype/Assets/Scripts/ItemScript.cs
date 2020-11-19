using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemScript : MonoBehaviour {
    public GroceryType type;
    //public GameObject groceryManager;
    public AudioSource audioSource;
    public AudioClip[] itemSoundEffects = new AudioClip[2];
    public static int floorDropSound = 0;
    public static int cartDropSound = 1;

    // Start is called before the first frame update
    void Start () {

    }

    // Update is called once per frame
    void Update () {

    }

    public void AddSelfToCart (GameObject groceryManager) {
        groceryManager.GetComponent<GroceryManager> ().AddToCart (type, this.gameObject);
    }

    private void OnCollisionEnter (Collision other) {
        Debug.Log (other);
        if (other.gameObject.CompareTag ("Floor")) {
            //play floor drop sound
            audioSource.clip = itemSoundEffects[floorDropSound];
            audioSource.Play ();

            gameObject.GetComponent<BoxCollider> ().isTrigger = true;
            gameObject.GetComponent<Rigidbody> ().useGravity = false;
            gameObject.GetComponent<Rigidbody> ().velocity = Vector3.zero;
            gameObject.GetComponent<Rigidbody> ().angularVelocity = Vector3.zero;
        } else if (other.gameObject.CompareTag ("Cart")) {
            //play cart drop sound
            audioSource.clip = itemSoundEffects[cartDropSound];
            audioSource.Play ();

            gameObject.GetComponent<BoxCollider> ().isTrigger = true;
            gameObject.GetComponent<Rigidbody> ().useGravity = false;
            gameObject.GetComponent<Rigidbody> ().velocity = Vector3.zero;
            gameObject.GetComponent<Rigidbody> ().angularVelocity = Vector3.zero;
            transform.SetParent (other.transform);
            //set item's position relative to cart(parent)
            transform.position = other.gameObject.GetComponent<CartScript> ().GetRandomPositionInCart ();
            gameObject.GetComponent<Rigidbody> ().isKinematic = true;
            //add to cart and list
            AddSelfToCart (other.gameObject.GetComponent<CartScript> ().groceryManager);

            //metric
            MetricManager.AddToThrowInCartCt();
        }
    }
}