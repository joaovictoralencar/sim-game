using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShowBubble : MonoBehaviour
{
    public GameObject bubble; 
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            bubble.SetActive(true);
        }
    }
    
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            bubble.SetActive(false);

        }
    }
}
