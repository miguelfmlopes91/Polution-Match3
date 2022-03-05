using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class StarManager : MonoBehaviour
{

    public int index;



    void Start()
    {
        UpdateImage();
    }

    public void UpdateImage()
    {
        Image StarImage = GetComponent<Image>();
        StarImage.sprite = GameData.gameData.starsImages[GameData.CalStars(index)];
    }
}
