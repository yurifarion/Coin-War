using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeckSlot : MonoBehaviour
{
    public GameObject attackModeObj;
    public GameObject card;
    private bool attackMode = false;

    public void SetCard(GameObject c)
    {
       
        card = c;
    }
    public GameObject GetCard()
    {
        return card;
    }
    public void ToggleAttackMode()
    {
        attackMode = !attackMode;
        attackModeObj.SetActive(attackMode);
       
    }
    public void ToggleAttackMode(bool b)
    {
        attackMode = b;
        attackModeObj.SetActive(attackMode);
    }
    public bool GetAttackMode()
    {
        return attackMode;
    }
    private void Update()
    {
        

        
    }
    
}
