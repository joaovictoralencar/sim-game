using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SpeechBubble : MonoBehaviour
{
    public SpriteRenderer bubbleSprite;
    public TextMeshPro bubbleText;
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Color color = bubbleSprite.color;
            color.a = .2f;
            bubbleSprite.color = color;
            
            Color colorText = bubbleText.color;
            colorText.a = .2f;
            bubbleText.color = colorText;
            
            
        }
    }
    
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Color color = bubbleSprite.color;
            color.a = 1f;
            bubbleSprite.color = color;
             
            Color colorText = bubbleText.color;
            colorText.a = 1f;
            bubbleText.color = colorText;
        }
    }
}
