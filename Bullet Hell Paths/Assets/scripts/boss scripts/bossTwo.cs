using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class bossTwo : MonoBehaviour
{
    //boss variables
    SpriteRenderer bossSprite;

    //health variables
    private float bossHealth = 300;
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

    //attract pattern
    public GameObject attractBullet;
    public Transform circleParent;
    private float bulletAttractSpeed = 8;

    //cross pattern
    public GameObject crossBullet;
    public Transform leftCrossParent;
    public Transform rightCrossParent;
    private Vector3 leftCrossEndPos = new Vector3(20, -26);
    private Vector3 rightCrossEndPos = new Vector3(-20, -26);
    private float crossSpeed = 5;
    private float rightEmptyBullet;
    private float leftEmptyBullet;
    private int maxNumberOfBullets = 12;
    private int numberOfBullets = 0;
    private string rainZone;

    //grenade variables
    public GameObject grenade;
    public GameObject grenadeBullet;
    private GameObject[] grenadeArr;
    private float grenadeSpeed = 15;
    private float grenadeBulletSpeed = 10;
    private int grenadePos;
    private int maxGrenades = 10;
    private float whenGrenadesEnd;
    private Vector3[] grenadeBulletEndPos = {
        //          6 o clock            7:30                  9:00
        new Vector3(0,-40),  new Vector3(-40,-40), new Vector3(-40,0),
        //          10:30                12:00                 1:30
        new Vector3(-40,40), new Vector3(0,40),    new Vector3(40,40),
        //          3:00                 4:30
        new Vector3(40,0),   new Vector3(40,-40) };

    //walls variable
    private bool wallsActive = false;
    private bool wallsGenades = false;

    //kill rain variable
    public GameObject killRainObj;

    //pattern variables
    private bool isRainPattern = false;
    private bool isGunPattern = false;
    private bool isAttractPattern = false;
    private bool isCrossPattern = false;
    private bool isGrenadePattern = false;
    private bool isWallsPattern = false;
    private float timeSinceStart = 0;
    public Transform lizyTransform;
    private float pattern;
    private float gunChance = 1f / 4f;
    private float attractChance = 1f / 4f * 2;
    private float crossChance = 1f / 4f * 3;
    private float grenadeChance = 1f / 4f * 4;
    private float rainChance = .3f;
    private float wallsChance = .6f;

    //telegraph variables
    public Transform rainTelegraphParent;
    public GameObject rainTelegraphObject;
    public Transform gunTelegraphParent;
    public SpriteRenderer gunTelegraphSprite;
    public GameObject attractTelegraphParent;
    public GameObject crossTelegraphParent;
    public SpriteRenderer grenadeTelegraphSprite;
    public GameObject wallsTelegraphParent;
    private float coolDown = 1.1f;

    //win variables
    public Image gameOverScreen;
    public Text gameOverText;
    public Text timeText;
    public playerController lizy;
    private bool bossBeaten = false;

    void Start()
    {
        bossSprite = GetComponent<SpriteRenderer>();
        grenadePos = Random.Range(-9, 10);
        pattern = Random.Range(0f,1f);
    }

    void FixedUpdate()
    {
        if (Time.timeSinceLevelLoad > 1.8 && Time.timeSinceLevelLoad < 4)
        {
            isInvincible = false;
        }

        //choose attack, use attack and handle cooldown
        if (!isRainPattern && !isGunPattern && !isAttractPattern && !isCrossPattern && !isGrenadePattern && !isWallsPattern)
        {
            if (coolDown > 0)
            {
                coolDown -= Time.fixedDeltaTime;
            }
            else if (coolDown <= 0)
            {
                stopTelegraph();
                coolDown = 1.1f;
                timeSinceStart = 0;
                if (bossHealth > 150)
                {
                    if (pattern < gunChance)
                    {
                        gunPattern();
                    }
                    else if (pattern < attractChance)
                    {
                        attractPattern();
                    }
                    else if (pattern < crossChance)
                    {
                        crossPattern();
                    }
                    else if (pattern < grenadeChance)
                    {
                        grenadePattern();
                    }
                }
                else
                {
                    if (pattern < wallsChance)
                    {
                        wallsPattern();
                    }
                    else if (pattern < rainChance)
                    {
                        rainPattern();
                    }
                    else if (pattern < gunChance)
                    {
                        gunPattern();
                    }
                    else if (pattern < attractChance)
                    {
                        attractPattern();
                    }
                    else if (pattern < crossChance)
                    {
                        crossPattern();
                    }
                }
                choosePatternRange();
                pattern = Random.Range(0.0f, 1.0f);
            }
        }

        //make telegraphs happen
        if (coolDown <= 1 && coolDown > 0)
        {
            if (bossHealth > 150)
            {
                if (pattern < gunChance)
                {
                    gunTelegraph();
                }
                else if (pattern < attractChance)
                {
                    attractTelegraph();
                }
                else if (pattern < crossChance)
                {
                    crossTelegraph();
                }
                else if (pattern < grenadeChance)
                {
                    grenadeTelegraph();
                }
            }
            else
            {
                if (pattern < wallsChance)
                {
                    if (Random.Range(0f, 1f) < .5f && coolDown == 1)
                    {
                        wallsGenades = true;
                    }
                    else if(coolDown == 1)
                    {
                        wallsGenades = false;
                    }
                    if (wallsGenades)
                    {
                        grenadeTelegraph();
                    }
                    else
                    {
                        gunTelegraph();
                    }
                    wallsTelegraph();
                }
                else if (pattern < rainChance)
                {
                    rainTelegraph();
                }
                else if (pattern < gunChance)
                {
                    gunTelegraph();
                }
                else if (pattern < attractChance)
                {
                    attractTelegraph();
                }
                else if (pattern < crossChance)
                {
                    crossTelegraph();
                }
            }
        }

        //test patterns
        if (Input.GetKeyDown("1") && !isRainPattern && !isGunPattern && !isAttractPattern && !isCrossPattern)
        {
            timeSinceStart = 0;
            rainPattern();
        }
        if (Input.GetKeyDown("2") && !isRainPattern && !isGunPattern && !isAttractPattern && !isCrossPattern && !isGrenadePattern && !isWallsPattern)
        {
            timeSinceStart = 0;
            gunPattern();
        }
        if (Input.GetKeyDown("3") && !isRainPattern && !isGunPattern && !isAttractPattern && !isCrossPattern && !isGrenadePattern && !isWallsPattern)
        {
            timeSinceStart = 0;
            attractPattern();
        }
        if (Input.GetKeyDown("4") && !isRainPattern && !isGunPattern && !isAttractPattern && !isCrossPattern && !isGrenadePattern && !isWallsPattern)
        {
            timeSinceStart = 0;
            crossPattern();
        }
        if (Input.GetKeyDown("5") && !isRainPattern && !isGunPattern && !isAttractPattern && !isCrossPattern && !isGrenadePattern && !isWallsPattern)
        {
            timeSinceStart = 0;
            grenadePattern();
        }
        if (Input.GetKeyDown("6") && !isRainPattern && !isGunPattern && !isAttractPattern && !isCrossPattern && !isGrenadePattern && !isWallsPattern)
        {
            timeSinceStart = 0;
            wallsPattern();
        }

        //make rain pattern work
        if (isRainPattern && !isGunPattern && !isAttractPattern && !isCrossPattern && !isGrenadePattern)
        {
            rainParent.position = Vector3.MoveTowards(rainParent.position, rainEndPos, rainSpeed * Time.fixedDeltaTime);
            if (isGoodTime(.54f, 10))
            {
                if (Random.Range(0f, 1f) < .5 && emptyColumn < 8)
                {
                    emptyColumn++;
                }
                else if (emptyColumn > -8)
                {
                    emptyColumn--;
                }
                else
                {
                    emptyColumn++;
                }
                rainPattern();
            }

            GameObject[] rainBullets = GameObject.FindGameObjectsWithTag("rain");
            foreach (GameObject newRain in rainBullets)
            {
                if (newRain.transform.position.y < -5.2)
                {
                    Destroy(newRain);
                }
            }

            if (Vector3.Distance(rainParent.position, rainEndPos) < 2)
            {
                GameObject[] rainObjects = GameObject.FindGameObjectsWithTag("rain");
                foreach (GameObject newRain in rainObjects)
                {
                    Destroy(newRain);
                }
                isRainPattern = false;
                rainParent.position = new Vector3(0, 6);
            }
            timeSinceStart += Time.fixedDeltaTime;
        }

        //make machine gun pattern work
        if (isGunPattern && !isRainPattern && !isAttractPattern && !isCrossPattern && !isGrenadePattern)
        {
            gunParent.position = Vector3.MoveTowards(gunParent.position, lizyLastPos * 4, bulletSpeed * Time.fixedDeltaTime);
            gunPattern();

            timeSinceStart += Time.fixedDeltaTime;
            if (isGoodTime(1, 10))
            {
                GameObject[] bullets = GameObject.FindGameObjectsWithTag("bossTwoAttack");
                foreach (GameObject newBullet in bullets)
                {
                    Destroy(newBullet);
                }
                gunParent.position = new Vector3(0, 2);
                if (timeSinceStart >= 8)
                {
                    isGunPattern = false;
                    wallsActive = false;
                }
            }
        }

        //make attract pattern work
        if (isAttractPattern && !isRainPattern && !isGunPattern && !isCrossPattern && !isGrenadePattern)
        {
            bool endPattern = false;
            int loopCount = 0;
            foreach (Transform newAttractBullet in circleParent.GetComponentsInChildren<Transform>())
            {
                newAttractBullet.position = Vector3.MoveTowards(newAttractBullet.position, circleParent.position, bulletAttractSpeed * Time.fixedDeltaTime);
                if (Vector3.Distance(newAttractBullet.position, circleParent.position) <= .01f && loopCount > 0)
                {
                    endPattern = true;
                }
                loopCount++;
            }
            if (endPattern)
            {
                GameObject[] newAttractBullets = GameObject.FindGameObjectsWithTag("bullet");
                foreach (GameObject newAttractBullet in newAttractBullets)
                {
                    Destroy(newAttractBullet);
                }
                isAttractPattern = false;
            }
        }

        //make cross pattern work
        if (isCrossPattern && !isRainPattern && !isGunPattern && !isAttractPattern && !isGrenadePattern)
        {
            //move bullets across screen at normal speed
            if (numberOfBullets != maxNumberOfBullets)
            {
                leftCrossParent.position = Vector3.MoveTowards(leftCrossParent.position, leftCrossEndPos, crossSpeed * Time.fixedDeltaTime);
                rightCrossParent.position = Vector3.MoveTowards(rightCrossParent.position, rightCrossEndPos, crossSpeed * Time.fixedDeltaTime);
            }
            //bullets fuck off at hyper speed and rain zone activates
            else
            {
                leftCrossParent.position = Vector3.MoveTowards(leftCrossParent.position, leftCrossEndPos, crossSpeed * 5 * Time.fixedDeltaTime);
                rightCrossParent.position = Vector3.MoveTowards(rightCrossParent.position, rightCrossEndPos, crossSpeed * 5 * Time.fixedDeltaTime);

                if (rainZone == "left")
                {
                    //call rainPattern, set empty column outside of i so it doesnt happen
                    //delete part of rain pattern that i dont want
                    //copy the code for the rain moving down, make it really fast
                    killRain(-9, -2);
                }
                else if (rainZone == "middle")
                {
                    killRain(-3, 4);
                }
                else
                {
                    killRain(3, 10);
                }

                rainParent.position = Vector3.MoveTowards(rainParent.position, rainEndPos, rainSpeed * 5 * Time.fixedDeltaTime);
            }
            //create a bullet on either side off screen every .5 seconds
            if (isGoodTime(.5f, maxNumberOfBullets - 1))
            {
                crossPattern();
            }
            //figure out where the player ended up after the cross came down
            if (rightCrossParent.position.y <= -3 && rightCrossParent.position.y >= -3.2)
            {
                if (lizyTransform.position.x < -2)
                {
                    rainZone = "left";
                }
                else if (lizyTransform.position.x < 2)
                {
                    rainZone = "middle";
                }
                else
                {
                    rainZone = "right";
                }
            }
            //delete the objects after pattern is over
            if (Vector3.Distance(rainParent.position, rainEndPos) < 20)
            {
                isCrossPattern = false;
                GameObject[] crossBulletObjects = GameObject.FindGameObjectsWithTag("bullet");
                foreach (GameObject newCrossBullet in crossBulletObjects)
                {
                    Destroy(newCrossBullet);
                }
                rainParent.position = new Vector3(0, 6);
                leftCrossParent.position = new Vector3(-13, 7);
                rightCrossParent.position = new Vector3(13, 7);
                numberOfBullets = 0;
            }
            timeSinceStart += Time.fixedDeltaTime;
        }

        //make grenade pattern work
        if (isGrenadePattern && !isRainPattern && !isGunPattern && !isAttractPattern && !isCrossPattern)
        {
            grenadeArr = GameObject.FindGameObjectsWithTag("bossTwoAttack");
            foreach (GameObject grenade in grenadeArr)
            {
                if (grenade.transform.position.y > -3)
                {
                    grenade.transform.position = Vector3.MoveTowards(grenade.transform.position, new Vector3(grenade.transform.position.x, -3), grenadeSpeed * Time.fixedDeltaTime);
                }
                else
                {
                    Transform[] bullets = grenade.GetComponentsInChildren<Transform>();
                    for (int i = 0; i < 8; i++)
                    {
                        bullets[i + 1].localPosition = Vector3.MoveTowards(bullets[i + 1].localPosition, grenadeBulletEndPos[i], grenadeBulletSpeed * Time.fixedDeltaTime);
                        Vector3 direction = grenadeBulletEndPos[i] - bullets[i + 1].position;
                        bullets[i + 1].localRotation = Quaternion.AngleAxis(Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg, Vector3.forward);
                    }
                    //make grenade opacity 0, disable collider so that children can continue to move locally and delete parent later to kill children too
                    grenade.GetComponent<SpriteRenderer>().color = new Color(0, 0, 0, 0);
                    grenade.GetComponent<CircleCollider2D>().enabled = false;
                }
            }
            if (isGoodTime(1, maxGrenades))
            {
                grenadePattern();
            }
            if (timeSinceStart >= 14)
            {
                foreach (GameObject grenade in grenadeArr)
                {
                    Destroy(grenade);
                }
                isGrenadePattern = false;
                wallsActive = false;
            }
            timeSinceStart += Time.fixedDeltaTime;
        }

        //make walls pattern work
        if (isWallsPattern)
        {
            //make rain appear but it's like jail bars, y'know?
            if (rainParent.position != rainEndPos)
            {
                killRain(-9, -1);
                killRain(2, 9);
                rainParent.position = Vector3.MoveTowards(rainParent.position, rainEndPos, rainSpeed * 5 * Time.fixedDeltaTime);
            }
            //when active pattern ends destroy rain
            if (!wallsActive)
            {
                GameObject[] bullets = GameObject.FindGameObjectsWithTag("bullet");
                foreach (GameObject bullet in bullets)
                {
                    Destroy(bullet);
                }
                isWallsPattern = false;
                rainParent.position = new Vector3(0, 6);
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
        healthBar.localScale = new Vector3(health / 300, 1);
        healthText.text = health.ToString();
    }

    //build rain pattern
    private void rainPattern()
    {
        for (int i = 0; i < 10; i++)
        {
            if (i != emptyColumn)
            {
                GameObject newRainR = GameObject.Instantiate(rainLaser, new Vector3(i, 6), rainParent.rotation, rainParent);
                //newRainR.transform.localScale = new Vector3(rainSize, 1.5f);
                newRainR.tag = "rain";
            }
            if (-i != emptyColumn)
            {
                GameObject newRainL = GameObject.Instantiate(rainLaser, new Vector3(-i, 6), rainParent.rotation, rainParent);
                //newRainL.transform.localScale = new Vector3(rainSize, 1.5f);
                newRainL.tag = "rain";
            }
        }
        isRainPattern = true;
    }

    //mahcine gun pattern
    private void gunPattern()
    {
        if (isGoodTime(1, 10) || timeSinceStart == 0)
        {
            lizyLastPos = lizyTransform.position;
            Vector3 direction = lizyLastPos - gunParent.position;
            gunParent.rotation = Quaternion.AngleAxis(Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg, Vector3.forward);
        }

        if (!wallsActive)
        {
            gunSize = 1.5f;
        }

        GameObject newBullet = Instantiate(followBullet, new Vector3(0, 2), gunParent.rotation, gunParent);
        newBullet.tag = "bossTwoAttack";
        newBullet.transform.localScale = new Vector3(1, gunSize);
        isGunPattern = true;
    }

    //half circle of randomly placed bullets off screen that slowly move toward frank
    private void attractPattern()
    {
        for (int i = 0; i < 9; i++)
        {
            GameObject newAttractBullet = Instantiate(attractBullet, new Vector3(11, 2), Quaternion.identity, circleParent);
            Vector3 direction = transform.position - newAttractBullet.transform.position;
            newAttractBullet.transform.rotation = Quaternion.AngleAxis(Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg, Vector3.forward);
            newAttractBullet.tag = "bullet";
            //newAttractBullet.transform.localScale = new Vector3(1, .5f);
            if (i < 8)
            {
                circleParent.Rotate(0, 0, -22.5f, Space.Self);
            }
        }
        isAttractPattern = true;
    }

    //bullets from one corner to another with a space somewhere to jump through. after the space passes make the side of the screen where the player came from rain down with bullets
    private void crossPattern()
    {
        if (timeSinceStart == 0)
        {
            leftEmptyBullet = Random.Range(2, 6);
            rightEmptyBullet = Random.Range(-5, -1);
        }
        if (leftEmptyBullet != numberOfBullets)
        {
            GameObject newLeftBullet = Instantiate(crossBullet, new Vector3(-13, 7), Quaternion.identity, leftCrossParent);
            newLeftBullet.transform.rotation = Quaternion.Euler(0, 0, 45);
            newLeftBullet.tag = "bullet";

        }
        if (rightEmptyBullet != numberOfBullets * -1)
        {
            GameObject newRightBullet = Instantiate(crossBullet, new Vector3(13, 7), Quaternion.identity, rightCrossParent);
            newRightBullet.transform.rotation = Quaternion.Euler(0, 0, -45);
            newRightBullet.tag = "bullet";
        }

        numberOfBullets++;
        isCrossPattern = true;
    }

    //bullet rain that envelopes a chosen area of the screen
    private void killRain(int startX, int endX)
    {
        for (int i = startX; i < endX; i++)
        {
            GameObject newRain = GameObject.Instantiate(killRainObj, new Vector3(i, 6), rainParent.rotation, rainParent);
            newRain.transform.localScale = new Vector3(rainSize, 1.5f);
            newRain.tag = "bullet";
        }
    }

    //throws grenades at you that explode and (create a hitbox in a radius around them) or (shoot bullets out in 8 directions)
    private void grenadePattern()
    {
        if (wallsActive)
        {
            if (grenadePos > 2)
            {
                grenadePos = Random.Range(-5, -2);
            }
            else if (grenadePos < -2)
            {
                grenadePos = Random.Range(3, 6);
            }
            else
            {
                grenadePos = Random.Range(-5, -2);
            }
        }
        else
        {
            if (grenadePos > 2)
            {
                grenadePos = Random.Range(-9, -1);
            }
            else if (grenadePos < -2)
            {
                grenadePos = Random.Range(2, 10);
            }
            else
            {
                grenadePos = Random.Range(-9, -1);
            }
        }

        GameObject newGrenade = Instantiate(grenade, new Vector3(grenadePos, 6), Quaternion.identity);
        newGrenade.tag = "bossTwoAttack";
        for (int i = 0; i < 8; i++)
        {
            GameObject newGrenadeBullet = Instantiate(grenadeBullet, newGrenade.transform.position, Quaternion.identity, newGrenade.transform);
            newGrenadeBullet.transform.localScale = new Vector3(.5f, .5f);
        }
        isGrenadePattern = true;
    }

    //close walls in and then 
    private void wallsPattern()
    {
        //kill rain on either side of the boss to restrict area and then do either grenade or gun pattern
        wallsActive = true;
        if (wallsGenades)
        {
            grenadePattern();
        }
        else
        {
            gunSize = .75f;
            gunPattern();
        }
        isWallsPattern = true;
    }

    //check of the current timeSinceStart is an iteration of x sec
    private bool isGoodTime(float x, int iterations)
    {
        for (int i = 1; i <= iterations; i++)
        {
            if (timeSinceStart >= x * i - .01 && timeSinceStart <= x * i + .01)
            {
                return true;
            }
        }

        return false;
    }

    private void rainTelegraph()
    {
        if(coolDown == 1)
        {
            if (Random.Range(0f, 1f) < .5f)
            {
                emptyColumn = Random.Range(1, 6);
            }
            else
            {
                emptyColumn = Random.Range(-5, -1);
            }
        }

        for(int i = 0; i < 17; i++)
        {
            if (i != emptyColumn + 8)
            {
                Color c = rainTelegraphParent.GetComponentsInChildren<SpriteRenderer>()[i].color;
                if (c.a > 0)
                {
                    c.a = coolDown;
                }
                else
                {
                    c.a = 1;
                }
                rainTelegraphParent.GetComponentsInChildren<SpriteRenderer>()[i].color = c;
            }
        }
    }

    private void gunTelegraph()
    {
        if(coolDown == 1)
        {
            lizyLastPos = lizyTransform.position;
            Vector3 direction = lizyLastPos - gunTelegraphParent.position;
            gunTelegraphParent.rotation = Quaternion.AngleAxis(Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg, Vector3.forward);
        }

        Color c = gunTelegraphSprite.color;
        if (c.a > 0)
        {
            c.a = coolDown;
        }
        else
        {
            c.a = 1;
        }
        gunTelegraphSprite.color = c;
    }

    private void attractTelegraph()
    {
        foreach(SpriteRenderer attractTelegraphSprite in attractTelegraphParent.GetComponentsInChildren<SpriteRenderer>())
        {
            Color c = attractTelegraphSprite.color;
            if (c.a > 0)
            {
                c.a = coolDown;
            }
            else
            {
                c.a = 1;
            }
            attractTelegraphSprite.color = c;
        }
    }

    private void crossTelegraph()
    {
        foreach (SpriteRenderer crossTelegraphSprite in crossTelegraphParent.GetComponentsInChildren<SpriteRenderer>())
        {
            Color c = crossTelegraphSprite.color;
            if (c.a > 0)
            {
                c.a = coolDown;
            }
            else
            {
                c.a = 1;
            }
            crossTelegraphSprite.color = c;
        }
    }

    private void grenadeTelegraph()
    {
        Color c = grenadeTelegraphSprite.color;
        if (c.a > 0)
        {
            c.a = coolDown;
        }
        else
        {
            c.a = 1;
        }
        grenadeTelegraphSprite.color = c;
    }

    private void wallsTelegraph()
    {
        foreach (SpriteRenderer wallsTelegraphSprite in wallsTelegraphParent.GetComponentsInChildren<SpriteRenderer>())
        {
            Color c = wallsTelegraphSprite.color;
            if (c.a > 0)
            {
                c.a = coolDown;
            }
            else
            {
                c.a = 1;
            }
            wallsTelegraphSprite.color = c;
        }
    }

    private void stopTelegraph()
    {
        Color c = new Color(0,.78f,1,0);
        foreach (SpriteRenderer rainTelegraphSprite in rainTelegraphParent.GetComponentsInChildren<SpriteRenderer>())
        {
            rainTelegraphSprite.color = c;
        }
        foreach (SpriteRenderer gunTelegraphSprite in gunTelegraphParent.GetComponentsInChildren<SpriteRenderer>())
        {
            gunTelegraphSprite.color = c;
        }
        foreach (SpriteRenderer crossTelegraphSprite in crossTelegraphParent.GetComponentsInChildren<SpriteRenderer>())
        {
            crossTelegraphSprite.color = c;
        }
        foreach (SpriteRenderer attractTelegraphSprite in attractTelegraphParent.GetComponentsInChildren<SpriteRenderer>())
        {
            attractTelegraphSprite.color = c;
        }
        grenadeTelegraphSprite.color = c;
        foreach (SpriteRenderer wallsTelegraphSprite in wallsTelegraphParent.GetComponentsInChildren<SpriteRenderer>())
        {
            wallsTelegraphSprite.color = c;
        }
    }

    private void choosePatternRange()
    {
        if (bossHealth > 150)
        {
            //original values
            //gunChance = 1f / 4f;
            //attractChance = 1f / 4f * 2;
            //crossChance = 1f / 4f * 3;
            //grenadeChance = 1f / 4f * 4;

            //left side of screen = cross
            if (lizyTransform.position.y < -1 && lizyTransform.position.x < -2)
            {
                gunChance = 0;
                attractChance = .1f;
                crossChance = .9f;
                grenadeChance = 1;
            }
            //middle of screen = attract
            else if (lizyTransform.position.y < -1 && lizyTransform.position.x < 2)
            {
                gunChance = .1f;
                attractChance = .8f;
                crossChance = .9f;
                grenadeChance = 1;
            }
            //right side of screen = grenade
            else if (lizyTransform.position.y < -1 && lizyTransform.position.x >= 2)
            {
                gunChance = .1f;
                attractChance = .2f;
                crossChance = .3f;
                grenadeChance = 1;
            }
            //in the air = gun
            else
            {
                gunChance = .7f;
                attractChance = .8f;
                crossChance = .9f;
                grenadeChance = 1;
            }
        }
        else
        {
            //original values
            //wallsChance = .3f;
            //rainChance = .6f;
            //gunChance = .75f;
            //attractChance = .9f;
            //crossChance = 1;

            //left side of screen = attract
            if (lizyTransform.position.y < -1 && lizyTransform.position.x < -2)
            {
                wallsChance = .075f;
                rainChance = .15f;
                gunChance = .225f;
                attractChance = .925f;
                crossChance = 1;
            }
            //middle of screen = walls
            else if (lizyTransform.position.y < -1 && lizyTransform.position.x < 2)
            {
                wallsChance = .7f;
                rainChance = .775f;
                gunChance = .85f;
                attractChance = .925f;
                crossChance = 1;
            }
            //right side of screen = gun
            else if (lizyTransform.position.y < -1 && lizyTransform.position.x >= 2)
            {
                wallsChance = .075f;
                rainChance = .15f;
                gunChance = .85f;
                attractChance = .925f;
                crossChance = 1;
            }
            //in the air = rain
            else
            {
                wallsChance = .075f;
                rainChance = .775f;
                gunChance = .85f;
                attractChance = .925f;
                crossChance = 1;
            }
        }
    }

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
