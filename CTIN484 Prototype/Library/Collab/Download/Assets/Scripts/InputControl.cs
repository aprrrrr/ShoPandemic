using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputControl : MonoBehaviour {
    //use controller in the future
    //private float force = 10.0f;
    //private float torque = 10.0f;

    //audiosource channels
    public static int PickDrop = 0;

    public bool isPicking;
    public bool hasItem;
    public int playerIndex;

    public Vector3 itemPositionPlayer;
    public Vector3 itemPositionGround;

    public GameObject cart;
    public GameObject cart2;
    public GameObject cartSpace;
    public GameObject world;
    public GameObject groceryManager;

    public AudioSource[] audioSources;

    public float cartRange;
    public float force = 20.0f;

    public float throwForce = 500.0f;

    private KeyCode forwardKey;
    private KeyCode leftKey;
    private KeyCode rightKey;
    private KeyCode backKey;
    private KeyCode pickupKey;
    private KeyCode throwKey;

    private void Start () {
        audioSources = GetComponents<AudioSource> ();

        //setup key pairing
        if (playerIndex == 1) {
            forwardKey = KeyCode.W;
            leftKey = KeyCode.A;
            rightKey = KeyCode.D;
            backKey = KeyCode.S;
            pickupKey = KeyCode.LeftShift;
            throwKey = KeyCode.Z;
        } else {
            forwardKey = KeyCode.UpArrow;
            leftKey = KeyCode.LeftArrow;
            rightKey = KeyCode.RightArrow;
            backKey = KeyCode.DownArrow;
            pickupKey = KeyCode.Space;
            throwKey = KeyCode.RightAlt;
        }
        isPicking = false;
        hasItem = false;
    }

    void Update () {
        //controller code
        //float forward = Input.GetAxis("Vertical");
        //float turn = Input.GetAxis("Horizontal");
        //gameObject.GetComponent<Rigidbody>().AddForce(transform.forward * force);
        //gameObject.GetComponent<Rigidbody>().AddTorque(transform.up * turn * torque);

        //movement input
        if (Input.GetKey (forwardKey)) {
            gameObject.GetComponent<Rigidbody> ().AddForce (Vector3.forward * force * Time.deltaTime);
            transform.rotation = Quaternion.Slerp (transform.rotation, Quaternion.LookRotation (Vector3.forward), 0.15F);

        } else if (Input.GetKey (backKey)) {
            gameObject.GetComponent<Rigidbody> ().AddForce (Vector3.back * force * Time.deltaTime);
            transform.rotation = Quaternion.Slerp (transform.rotation, Quaternion.LookRotation (Vector3.back), 0.15F);

        }

        if (Input.GetKey (leftKey)) {
            gameObject.GetComponent<Rigidbody> ().AddForce (Vector3.left * force * Time.deltaTime);
            transform.rotation = Quaternion.Slerp (transform.rotation, Quaternion.LookRotation (Vector3.left), 0.15F);

        } else if (Input.GetKey (rightKey)) {
            gameObject.GetComponent<Rigidbody> ().AddForce (Vector3.right * force * Time.deltaTime);
            transform.rotation = Quaternion.Slerp (transform.rotation, Quaternion.LookRotation (Vector3.right), 0.15F);

        }

        if (Input.GetKeyDown (pickupKey)) {
            if (hasItem && !isPicking) {
                PutDownItems ();
                hasItem = false;
            } else {
                isPicking = true;
            }
        }

        if (Input.GetKeyDown (throwKey)) {
            if (hasItem && !isPicking) {
                ThrowItem ();
                hasItem = false;
            }
        }
    }
    private void OnTriggerStay (Collider other) {
        if (isPicking && !hasItem) {
            //play sound
            audioSources[PickDrop].Play ();

            //pick up item from ground
            if (other.gameObject.CompareTag ("Grocery")) {
                //Debug.Log ("Triggered grocery");
                if (isPicking && other.gameObject.transform.parent != cart.transform) {
                    //check cart2
                    if(cart2!=null)
                    {
                        if(other.gameObject.transform.parent != cart2.transform)
                        {
                            //pickup item
                            other.gameObject.transform.SetParent (transform);
                            //set item's position relative to player(parent)
                            other.gameObject.transform.localPosition = itemPositionPlayer;
                            other.gameObject.GetComponent<Rigidbody> ().isKinematic = true;
                        }
                    }
                    else
                    {
                        //pickup item
                        other.gameObject.transform.SetParent(transform);
                        //set item's position relative to player(parent)
                        other.gameObject.transform.localPosition = itemPositionPlayer;
                        other.gameObject.GetComponent<Rigidbody>().isKinematic = true;
                    }
                    
                }
            }
            //pick up item from shelf
            else if (other.gameObject.CompareTag ("Shelf")) {
                //Debug.Log ("Triggered shelf");
                Transform item = other.GetComponent<ShelfScript> ().GetItemFromShelf ();
                item.SetParent (transform);
                item.localPosition = itemPositionPlayer;
                //item.GetComponent<ItemScript> ().groceryManager = groceryManager;
            }
            isPicking = false;
            hasItem = true;
        }
    }

    void PutDownItems () {
        //play sound
        audioSources[PickDrop].Play ();

        for (int i = 0; i < transform.childCount; i++) {
            Transform putDownItem = transform.GetChild (i);
            if (putDownItem.gameObject.CompareTag ("Grocery")) {
                //if player is standing near cart
                if (Vector3.Distance (cart.transform.position, transform.position) < cartRange) {
                    putDownItem.SetParent (cart.transform);
                    //set item's position relative to cart(parent)
                    putDownItem.position = cart.GetComponent<CartScript> ().GetRandomPositionInCart ();
                    //add to cart and list
                    putDownItem.gameObject.GetComponent<ItemScript> ().AddSelfToCart (groceryManager);
                } 
                else if(cart2!=null && Vector3.Distance(cart2.transform.position, transform.position) < cartRange)
                {
                    putDownItem.SetParent(cart2.transform);
                    //set item's position relative to cart(parent)
                    putDownItem.position = cart2.GetComponent<CartScript>().GetRandomPositionInCart();
                    //add to cart and list
                    putDownItem.gameObject.GetComponent<ItemScript>().AddSelfToCart(groceryManager);
                }
                else {
                    putDownItem.SetParent (world.transform);
                    //set item's position back to the ground
                    putDownItem.position -= itemPositionPlayer;
                    putDownItem.position += itemPositionGround;
                }
            }
        }

    }

    void ThrowItem () {
        for (int i = 0; i < transform.childCount; i++) {
            Transform throwItem = transform.GetChild (i);
            if (throwItem.gameObject.CompareTag ("Grocery")) {
                throwItem.GetComponent<Rigidbody> ().isKinematic = false;
                throwItem.GetComponent<Rigidbody> ().useGravity = true;
                throwItem.GetComponent<BoxCollider> ().isTrigger = false;
                throwItem.GetComponent<Rigidbody> ().AddForce (transform.forward * throwForce);
                throwItem.SetParent (world.transform);
            }
        }
    }
}