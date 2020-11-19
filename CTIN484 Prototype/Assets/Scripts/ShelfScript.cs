using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShelfScript : MonoBehaviour {
    // Start is called before the first frame update
    public Transform prefab;

    public Transform GetItemFromShelf () {
        //instantiate item
        Transform item = Instantiate (prefab, transform.position, Quaternion.identity);
        //add the rigid body
        item.gameObject.AddComponent<Rigidbody> ().useGravity = false;
        item.gameObject.GetComponent<Rigidbody> ().isKinematic = true;
        //add audio source
        item.gameObject.GetComponent<ItemScript> ().audioSource = item.gameObject.AddComponent<AudioSource> ();
        item.gameObject.GetComponent<ItemScript> ().audioSource.playOnAwake = false;
        return item;
    }
}