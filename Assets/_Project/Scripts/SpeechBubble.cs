using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeechBubble : MonoBehaviour
{
    public SpriteRenderer bubbleSprite;
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Color color = bubbleSprite.color;
            color.a = .2f;
            bubbleSprite.color = color;
        }
    }
    
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Color color = bubbleSprite.color;
            color.a = 1f;
            bubbleSprite.color = color;
        }
    }
}
