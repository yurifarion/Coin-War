using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeckSlot : MonoBehaviour
{
    private GameObject card;


    public void SetCard(GameObject c)
    {
        card = c;
    }
    public GameObject GetCard()
    {
        return card;
    }
    private void Update()
    {
        
    }
    
}
