using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleCard : MonoBehaviour
{
    private BoardManager _boardManager;
    // Start is called before the first frame update
    void Start()
    {
        _boardManager = GameObject.FindGameObjectWithTag("BoardManager").GetComponent<BoardManager>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
