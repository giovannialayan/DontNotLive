using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class bossThree : MonoBehaviour
{
    //boss variables
    SpriteRenderer bossSprite;

    //lizy variables
    private Vector3 lizyLastPos;
    public Transform lizyTransform;

    //health variables
    private float bossHealth = 600;
    public Transform healthBar;
    public Text healthText;
    private bool isInvincible = true;

    //floor fire variables
    public GameObject floorFire;

    //shotgun fire variables
    public Transform shotgunParent;

    //pattern variables
    private float timeSinceStart = 0;
    private bool isFloorPattern = false;
    private bool isShotgunPattern = false;

    //win variables
    public Image gameOverScreen;
    public Text gameOverText;
    public Text timeText;
    public playerController lizy;
    private bool bossBeaten = false;

    void Start()
    {
        bossSprite = GetComponent<SpriteRenderer>();
    }

    void FixedUpdate()
    {
        if (Time.timeSinceLevelLoad > 1.8 && Time.timeSinceLevelLoad < 4)
        {
            isInvincible = false;
        }

        if (Input.GetKeyDown("1") && !isFloorPattern && !isShotgunPattern) { floorFirePattern(); }
        if (Input.GetKeyDown("2") && !isFloorPattern && !isShotgunPattern) { shotgunFirePattern(); }

        if (isFloorPattern)
        {
            timeSinceStart += Time.fixedDeltaTime;

            if(timeSinceStart > 2)
            {
                destroyAttack();
                timeSinceStart = 0;
                isFloorPattern = false;
            }
        }

        if (isShotgunPattern)
        {
            //make center fireball face player then instantiate a few fireballs in arc based on the first fireball's position
            lizyLastPos = lizyTransform.position;
            Vector3 direction = lizyLastPos - shotgunParent.position;
            shotgunParent.rotation = Quaternion.AngleAxis(Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg, Vector3.forward);
        }

        //make lizy invincible after you win
        if (bossBeaten)
        {
            lizy.health = 0;
        }
    }

    //fire on the floor pattern
    private void floorFirePattern()
    {
        GameObject newFloorFire = Instantiate(floorFire, new Vector3(0,-4), Quaternion.identity);
        newFloorFire.tag = "bossThreeAttack";
        isFloorPattern = true;
    }

    //shotgun fire pattern
    private void shotgunFirePattern()
    {
        GameObject newFireBall;
    }

    //reduce boss health when hit and trigger boss death when health reaches 0
    public void takeDamage(int damage)
    {
        if (bossHealth > 0 && !isInvincible)
        {
            bossHealth -= damage;
            decreaseHealthBar(bossHealth);
        }
        if (bossHealth <= 0)
        {
            bossSprite.color = Color.red;
            playerWin();
        }
    }

    //update healthbar
    private void decreaseHealthBar(float health)
    {
        healthBar.localScale = new Vector3(health / 300, 1);
        healthText.text = health.ToString();
    }

    private void destroyAttack()
    {
        GameObject[] bossThreeAttacks = GameObject.FindGameObjectsWithTag("bossThreeAttack");
        foreach (GameObject attack in bossThreeAttacks)
        {
            Destroy(attack);
        }
    }

    //player winning
    private void playerWin()
    {
        gameOverScreen.color = new Color(0, 0, 0, 1);
        gameOverText.text = "You Win";
        gameOverText.color = new Color(1, 1, 1, 1);
        timeText.text = "your time was\n" + "from start of fight: ";
        timeText.text += Mathf.Floor(Time.timeSinceLevelLoad / 60).ToString("00") + ":" + (Time.timeSinceLevelLoad % 60).ToString("00") + (Time.timeSinceLevelLoad % 60).ToString("F2").Substring((Time.timeSinceLevelLoad % 60).ToString("F2").IndexOf("."), 3) + "\n from startup of game: ";
        timeText.text += Mathf.Floor(Time.time / 60).ToString("00") + ":" + (Time.time % 60).ToString("00") + (Time.time % 60).ToString("F2").Substring((Time.time % 60).ToString("F2").IndexOf("."), 3);
        timeText.text += "\n deaths: " + playerController.deaths;
        timeText.color = new Color(1, 1, 1, 1);
        bossBeaten = true;
    }
}
