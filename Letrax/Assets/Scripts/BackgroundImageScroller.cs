using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BackgroundImageScroller : MonoBehaviour
{
    [SerializeField] private RawImage img;
    [SerializeField] private float xSpeed, ySpeed;

    void Update()
    {
        img.uvRect = new Rect(img.uvRect.position + new Vector2(xSpeed, ySpeed) * Time.deltaTime, img.uvRect.size);    
    }
}
