using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CombatText : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI tmp;
    [SerializeField] Rigidbody2D rb;
    public string text;
    public float timeLeft;
    public bool hasGravity;
    public void Start()
    {
        tmp.text = text;
    }
    private void OnGUI()
    {
        timeLeft -= Time.deltaTime;
        if(timeLeft <= 0) { Destroy(gameObject); return; }
        if (hasGravity)
        {
            rb.isKinematic = false;
            rb.gravityScale = 0.1f;
        }
    }
}
