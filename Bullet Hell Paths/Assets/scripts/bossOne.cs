using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class bossOne : MonoBehaviour
{
    public float health = 100;
    SpriteRenderer bossSprite;
    public Transform healthBar;
    public Text healthText;

    void Start()
    {
        bossSprite = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void takeDamage(int damage)
    {
        health -= damage;
        decreaseHealthBar(health);
        if(health <= 0)
        {
            bossSprite.color = Color.red;
        }
    }

    private void decreaseHealthBar(float health)
    {
        healthBar.localScale = new Vector3(health/100, 1);
        healthText.text = health.ToString();
    }
}
