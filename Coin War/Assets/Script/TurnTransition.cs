using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnTransition : MonoBehaviour
{
    //This function will be called in the Turn Transition animation
   public void DeactivateItself()
    {
        this.gameObject.SetActive(false);
    }
}
