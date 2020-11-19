using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ListPanelScript : MonoBehaviour
{
    public GameObject[] cardPrefabs;

    public List<KeyValuePair<GroceryType, GameObject>> instantiatedCards = new List<KeyValuePair<GroceryType, GameObject>>();
    //move this in start and it's null. Can's move this in start?????

    private void Update()
    {
    }

    public void CreateCard(GroceryType type, int amount)
    {
        GameObject newCard = Instantiate(cardPrefabs[(int)type], new Vector3(0, 0, 0), Quaternion.identity) as GameObject;
        newCard.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
        newCard.transform.SetParent(transform);

        //store in data structure
        KeyValuePair<GroceryType, GameObject> newPair = new KeyValuePair<GroceryType, GameObject>(type, newCard);
        instantiatedCards.Add(newPair);
        
        //Text[0] is product name. Text[1] is amount
        Text[] childTexts = newCard.GetComponentsInChildren<Text>();
        childTexts[1].text = "0/" + amount.ToString();
    }
}
