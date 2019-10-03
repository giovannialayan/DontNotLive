using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class bossTwo : MonoBehaviour
{
    //boss variables
    SpriteRenderer bossSprite;

    //health variables
    private float bossHealth = 200;
    public Transform healthBar;
    public Text healthText;
    private bool isInvincible = true;

    //rain laser pattern
    public GameObject rainLaser;
    public Transform rainParent;
    private float rainSpeed = 6;
    private float rainSize = .7f;
    private Vector3 rainEndPos = new Vector3(0, -40);
    private int emptyColumn;

    //machine gun pattern
    public GameObject followBullet;
    public Transform gunParent;
    private float gunSize = 1.5f;
    private float bulletSpeed = 20f;
    private Vector3 lizyLastPos;

    //circle pattern
    public GameObject attractBullet;
    public Transform circleParent;
    private float bulletAttractSpeed = 8;

    //pattern variables
    private bool isRainPattern = false;
    private bool isGunPattern = false;
    private bool isAttractPattern = false;
    private float timeSinceStart = 0;
    public Transform lizyTransform;

    //win variables
    public Image gameOverScreen;
    public Text gameOverText;
    public Text timeText;
    public playerController lizy;
    private bool bossBeaten = false;

    void Start()
    {
        bossSprite = GetComponent<SpriteRenderer>();
        circlePattern();
    }
    
    void FixedUpdate()
    {
        if (Time.timeSinceLevelLoad > 1.8 && Time.timeSinceLevelLoad < 4)
        {
            isInvincible = false;
        }

        //test patterns
        if (Input.GetKeyDown("1") && !isRainPattern && !isGunPattern)
        {
            timeSinceStart = 0;
            rainPattern();
        }
        if(Input.GetKeyDown("2") && !isRainPattern && !isGunPattern)
        {
            timeSinceStart = 0;
            gunPattern();
        }

        //make rain pattern work
        if (isRainPattern && !isGunPattern && !isAttractPattern)
        {
            rainParent.position = Vector3.MoveTowards(rainParent.position, rainEndPos, rainSpeed * Time.fixedDeltaTime);
            if (isGoodTime(.54f))
            {
                if (Random.Range(0f,1f) < .5 && emptyColumn < 8)
                {
                    emptyColumn++;
                }
                else if(emptyColumn > -8)
                {
                    emptyColumn--;
                }
                else
                {
                    emptyColumn++;
                }
                rainPattern();
            }

            if (Vector3.Distance(rainParent.position, rainEndPos) < 2)
            {
                GameObject[] rainObjects = GameObject.FindGameObjectsWithTag("rain");
                foreach(GameObject newRain in rainObjects)
                {
                    Destroy(newRain);
                }
                isRainPattern = false;
                rainParent.position = new Vector3(0, 6);
            }
            timeSinceStart += Time.fixedDeltaTime;
        }

        //make machine gun pattern work
        if(isGunPattern && !isRainPattern && !isAttractPattern)
        {
            gunParent.position = Vector3.MoveTowards(gunParent.position, lizyLastPos * 4, bulletSpeed * Time.fixedDeltaTime);
            gunPattern();
            
            timeSinceStart += Time.fixedDeltaTime;
            if (isGoodTime(1))
            {
                GameObject[] bullets = GameObject.FindGameObjectsWithTag("bullet");
                foreach (GameObject newBullet in bullets)
                {
                    Destroy(newBullet);
                }
                gunParent.position = new Vector3(0, 2);
                if(timeSinceStart >= 8)
                {
                    isGunPattern = false;
                }
            }
        }

        //make attract pattern work
        if (isAttractPattern && !isRainPattern && !isGunPattern)
        {
            foreach (Transform newAttractBullet in circleParent.GetComponentsInChildren<Transform>())
            {
                newAttractBullet.position = Vector3.MoveTowards(newAttractBullet.position, circleParent.position, bulletAttractSpeed * Time.fixedDeltaTime);
            }
        }

        //make lizy invincible after you win
        if (bossBeaten)
        {
            lizy.health = 0;
        }
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
        healthBar.localScale = new Vector3(health / 200, 1);
        healthText.text = health.ToString();
    }

    //build rain pattern
    private void rainPattern()
    {
        //move the first empty column chooser to the telegraph later
        if(timeSinceStart == 0)
        {
            if(Random.Range(0f,1f) < .5f)
            {
                emptyColumn = Random.Range(1, 5);
            }
            else
            {
                emptyColumn = Random.Range(-5, -1);
            }
        }
        for (int i = 0; i < 10; i++)
        {
            if(i != emptyColumn)
            {
                GameObject newRainR = GameObject.Instantiate(rainLaser, new Vector3(i, 6), rainParent.rotation, rainParent);
                newRainR.transform.localScale = new Vector3(rainSize, 1.5f);
                newRainR.tag = "rain";
            }
            if(-i != emptyColumn)
            {
                GameObject newRainL = GameObject.Instantiate(rainLaser, new Vector3(-i, 6), rainParent.rotation, rainParent);
                newRainL.transform.localScale = new Vector3(rainSize, 1.5f);
                newRainL.tag = "rain";
            }
        }
        isRainPattern = true;
    }

    //mahcine gun pattern
    private void gunPattern()
    {
        if (isGoodTime(1) || timeSinceStart == 0)
        {
            lizyLastPos = lizyTransform.position;
            Vector3 direction = lizyLastPos - gunParent.position;
            gunParent.rotation = Quaternion.AngleAxis(Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg, Vector3.forward);
        }
        
        GameObject newBullet = Instantiate(followBullet, new Vector3(0,2), gunParent.rotation, gunParent);
        newBullet.tag = "bullet";
        newBullet.transform.localScale = new Vector3(1, gunSize);
        isGunPattern = true;
    }

    //terraform pattern
    private void terraformPattern()
    {

    }

    //bullets from one corner to another with a space somewhere to jump through. after the space passes make the side of the screen where the player came from rain down with bullets
    private void crossPattern()
    {

    }

    //half circle of randomly placed bullets off screen that slowly move toward frank
    private void circlePattern()
    {
        for(int i=0; i<9; i++)
        {
            GameObject newAttractBullet = Instantiate(attractBullet, new Vector3(11,2), Quaternion.identity, circleParent);
            Vector3 direction = transform.position - newAttractBullet.transform.position;
            newAttractBullet.transform.rotation = Quaternion.AngleAxis(Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg, Vector3.forward);
            newAttractBullet.tag = "bullet";
            newAttractBullet.transform.localScale = new Vector3(1,.5f);
            if(i < 8)
            {
                circleParent.Rotate(0, 0, -22.5f, Space.Self);
            }
        }
        isAttractPattern = true;
    }
    
    //last pattern

    //check of the current timeSinceStart is an iteration of x sec
    private bool isGoodTime(float x)
    {
        if ((timeSinceStart >= x - .01 && timeSinceStart <= x + .01) || timeSinceStart == 1.5)
        {
            return true;
        }
        else if (timeSinceStart >= x * 2 - .01 && timeSinceStart <= x * 2 + .01)
        {
            return true;
        }
        else if (timeSinceStart >= x * 3 - .01 && timeSinceStart <= x * 3 + .01)
        {
            return true;
        }
        else if (timeSinceStart >= x * 4 - .01 && timeSinceStart <= x * 4 + .01)
        {
            return true;
        }
        else if (timeSinceStart >= x * 5 - .01 && timeSinceStart <= x * 5 + .01)
        {
            return true;
        }
        else if (timeSinceStart >= x * 6 - .01 && timeSinceStart <= x * 6 + .01)
        {
            return true;
        }
        else if (timeSinceStart >= x * 7 - .01 && timeSinceStart <= x * 7 + .01)
        {
            return true;
        }
        else if (timeSinceStart >= x * 8 - .01 && timeSinceStart <= x * 8 + .01)
        {
            return true;
        }
        else if (timeSinceStart >= x * 9 - .01 && timeSinceStart <= x * 9 + .01)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    private void playerWin()
    {
        gameOverScreen.color = new Color(0, 0, 0, 1);
        gameOverText.text = "You Win";
        gameOverText.color = new Color(1, 1, 1, 1);
        timeText.text = "your time was\n" + "from start of fight: ";
        string minLevel = (Time.timeSinceLevelLoad / 60).ToString("F0");
        string secLevel = (Time.timeSinceLevelLoad % 60).ToString("F2");
        string minFull = (Time.time / 60).ToString("F0");
        string secFull = (Time.time % 60).ToString("F2");
        if (Time.timeSinceLevelLoad / 60 < 10)
        {
            minLevel = "0" + minLevel;
        }
        if (Time.timeSinceLevelLoad % 60 < 10)
        {
            secLevel = "0" + secLevel;
        }
        timeText.text += minLevel + ":" + secLevel + "\n from startup of game: ";
        if (Time.time / 60 < 10)
        {
            minFull = "0" + minFull;
        }
        if (Time.time % 60 < 10)
        {
            secFull = "0" + secFull;
        }
        timeText.text += minFull + ":" + secFull;
        timeText.color = new Color(1, 1, 1, 1);
        bossBeaten = true;
    }

    //rain pattern

    //machine gun that follows you

    //terraforming
    //closes walls in and stuff
    //raises different parts of the ground
}
