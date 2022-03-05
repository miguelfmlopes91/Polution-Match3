using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;



public class BackgroundManager : MonoBehaviour
{

    public Image backGround;

    public Sprite[] Sprites;

    public int x;




    void Start()
    {
        x = Random.Range(0, 5);
        backGround.sprite = Sprites[x];
    }

   
    void Update()
    {
        
    }
}
