using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public enum GroceryType : int {
    Water = 0,
    Eggs = 1,
    ToiletPaper = 2,
    Cereal = 3,
    Ramen = 4,
    Milk = 5,
    Chocolate = 6,
    Apple = 7,
    Spray = 8,
    Banana = 9,
    Chips = 10,
    Soda = 11,
    Beer = 12
}

public enum PlayerNo {
    Player1,
    Player2
}

public class GroceryManager : MonoBehaviour {

    public PlayerNo playerNumber;
    public GameObject listPanel;

    public Dictionary<GroceryType, List<GameObject>> groceryInCart;
    //actual map storing grocery objects by type.

    public Dictionary<GroceryType, int> groceryNeeded;
    //actual map storing grocery needs. (potato, 4) means you need 4 potatoes in total

    public List<KeyValuePair<GroceryType, GameObject>> cards;
    //card list UI
    public AudioClip clearListSound;
    private AudioSource audioSource;
    // Start is called before the first frame update
    void Start () {
        //Initialize audiosource
        audioSource = gameObject.AddComponent<AudioSource> ();
        audioSource.playOnAwake = false;
        audioSource.clip = clearListSound;

        groceryInCart = new Dictionary<GroceryType, List<GameObject>> ();
        groceryNeeded = new Dictionary<GroceryType, int> ();

        //start task
        Scene scene = SceneManager.GetActiveScene ();
        if (scene.name == GameManager.tutorialSceneName) {
            TutorialLevel ();
        } else if (scene.name == GameManager.scene2Name) {
            Level2 ();
        }

        InitializeUI ();
    }

    void TutorialLevel () {
        AddToList (GroceryType.Spray, 1);
        AddToList (GroceryType.Ramen, 2);
        AddToList (GroceryType.Eggs, 3);
        AddToList (GroceryType.Cereal, 2);
        AddToList (GroceryType.ToiletPaper, 3);
        AddToList (GroceryType.Water, 2);
    }

    void Level2 () {
        if (playerNumber == PlayerNo.Player1) {
            AddToList (GroceryType.Beer, 1);
            AddToList (GroceryType.Soda, 3);
            AddToList (GroceryType.Eggs, 1);
            AddToList (GroceryType.Banana, 2);
            AddToList (GroceryType.ToiletPaper, 2);
            AddToList (GroceryType.Apple, 1);
            AddToList (GroceryType.Chips, 3);
            AddToList (GroceryType.Banana, 1);
        } else if (playerNumber == PlayerNo.Player2) {
            AddToList (GroceryType.Water, 2);
            AddToList (GroceryType.Beer, 1);
            AddToList (GroceryType.Apple, 2);
            AddToList (GroceryType.Eggs, 2);
            AddToList (GroceryType.Banana, 2);
            AddToList (GroceryType.Apple, 1);
            AddToList (GroceryType.Chips, 1);
            AddToList (GroceryType.ToiletPaper, 2);
        }
    }

    // Update is called once per frame
    void Update () {
        cards = listPanel.GetComponent<ListPanelScript> ().instantiatedCards;
        ShowListOnUI ();
    }

    public void AddToList (GroceryType type, int num) {
        if (groceryNeeded.ContainsKey (type)) {
            groceryNeeded[type] += num;
        } else {
            groceryNeeded.Add (type, num);
            groceryInCart.Add (type, new List<GameObject> ());
        }
    }

    public void ClearItemInTodoList (GroceryType type) {
        if (groceryNeeded.ContainsKey (type)) {
            groceryNeeded.Remove (type);
        } else {
            Debug.LogError ("This object doesn't exist in list.");
        }
    }

    public void AddToCart (GroceryType type, GameObject grocery) {
        if (groceryInCart.ContainsKey (type) && groceryInCart[type].Count < groceryNeeded[type]) {
            groceryInCart[type].Add (grocery);
        }
        if (groceryInCart[type].Count == groceryNeeded[type]) {
            Debug.Log ("PlayedSound");
            audioSource.Play ();
        }
    }

    public void ClearItemInCart (GroceryType type) {
        if (groceryInCart.ContainsKey (type)) {
            for (int i = 0; i < groceryInCart[type].Count; i++) {
                Destroy (groceryInCart[type][i]);
            }
            groceryInCart.Remove (type);
        } else {
            Debug.LogError ("This object doesn't exist in cart.");
        }
    }

    public void InitializeUI () {
        for (int i = 0; i < groceryNeeded.Count; i++) {
            listPanel.GetComponent<ListPanelScript> ().CreateCard (groceryNeeded.ElementAt (i).Key, groceryNeeded.ElementAt (i).Value);
        }
    }

    public void ShowListOnUI () {
        for (int i = 0; i < groceryNeeded.Count; i++) {
            GroceryType type = groceryNeeded.ElementAt (i).Key;
            int numNeeded = groceryNeeded.ElementAt (i).Value;

            int numInCart;
            if (groceryInCart.ContainsKey (type)) {
                numInCart = groceryInCart[type].Count;
            } else {
                numInCart = 0;
            }

            //find the corresponding card and update UI
            int count = cards.Count;
            for (int j = 0; j < cards.Count; j++) {
                KeyValuePair<GroceryType, GameObject> pair = cards.ElementAt (j);
                if (pair.Key == type) {
                    Text[] childTexts = pair.Value.GetComponentsInChildren<Text> ();
                    childTexts[1].text = numInCart.ToString () + "/" + numNeeded.ToString () + '\n';

                    //clear card
                    if (numInCart >= numNeeded) {
                        cards[j].Value.SetActive (false);
                    }
                }
            }
        }
    }

    public int GetTotalItemsNeeded () {
        int ans = 0;
        for (int i = 0; i < groceryNeeded.Count; i++) {
            ans += groceryNeeded.ElementAt (i).Value;
        }
        return ans;
    }

    public int GetTotalItemsCollected () {
        int ans = 0;
        for (int i = 0; i < groceryInCart.Count; i++) {
            ans += groceryInCart.ElementAt (i).Value.Count;
        }
        return ans;
    }
}