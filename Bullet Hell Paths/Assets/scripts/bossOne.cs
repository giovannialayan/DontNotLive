using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class bossOne : MonoBehaviour
{
    //boss variables
    SpriteRenderer bossSprite;
    Transform bossTransform;

    //health varaibles
    public float bossHealth = 100;
    public Transform healthBar;
    public Text healthText;
    private bool isInvincible = true;

    //laser variables
    public GameObject laserBar;
    private float timeSinceStart = 0;
    private GameObject[] lasers;
    //private float laserSpeed = 30f;
    private float laserSize = .5f;
    private bool clockwise = false;
    private bool counterClockwise = false;

    //knee laser variables
    public Transform kneeLaserParent;
    public GameObject kneeLaser;
    private GameObject[] kneeLasers;
    private Vector3 kneeLaserStartPos = new Vector3(10, -3.5f);
    private Vector3 kneeLaserEndPos = new Vector3(-30, -3.5f);
    private float kneeLaserSpeed = 7f;
    private float kneeLaserSize = 2.5f;

    //pellet pulse variables
    public GameObject pellet;
    public GameObject pelletParent;
    private GameObject[] pelletParents;
    private Transform[] pelletRings = new Transform[8];
    private Vector3[] pelletPos = {
        //          6 o clock                6:45                     7:30                     8:15                     9:00                   9:45
        new Vector3(0,-.5f),     new Vector3(-.75f,-.5f), new Vector3(-1.5f,-.5f), new Vector3(-1.5f,.25f), new Vector3(-1.5f,1),  new Vector3(-1.5f,1.75f),
        //          10:30                    11:15                    12:00                    12:45                    1:30                    2:15
        new Vector3(-1.5f,2.5f), new Vector3(-.75f,2.5f), new Vector3(0,2.5f),     new Vector3(.75f,2.5f),  new Vector3(1.5f,2.5f), new Vector3(1.5f,1.75f),
        //          3:00                     3:45                     4:30                     5:15
        new Vector3(1.5f,1),     new Vector3(1.5f,.25f),  new Vector3(1.5f,-.5f),  new Vector3(.75f,-.5f) };
    private Vector3 ringTargetSize = new Vector3(7, 7);
    private float pelletExpandSpeed = .01f;
    private float pelletRotSpeed = 50;

    //boulder variables
    public GameObject boulder;
    public Transform boulderParent;
    private Vector3 boulderLeftPos = new Vector3(-8, 0);
    private Vector3 boulderRightPos = new Vector3(14, 0);
    private float boulderSpeed = 10;
    private float boulderSize = 4;
    private bool reachedLeft = false;
    private bool reachedRight = false;

    //pattern variables
    private bool isPatternOne = false;
    private bool isPatternTwo = false;
    private bool isPatternThree = false;
    private bool isPatternFour = false;
    private float coolDown = 1.1f;
    private float pattern;

    //telegraph variables
    public SpriteRenderer laserTelegraph;
    public SpriteRenderer kneeLaserTelegraph;
    public SpriteRenderer pelletTelegraph;
    public SpriteRenderer boulderTelegraph;

    //win variables
    public Image gameOverScreen;
    public Text gameOverText;
    public Text timeText;
    public playerController lizy;

    void Start()
    {
        bossSprite = GetComponent<SpriteRenderer>();
        bossTransform = GetComponent<Transform>();
        pattern = Random.Range(0.0f, 4.0f);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if(Time.timeSinceLevelLoad > 1.8 && Time.timeSinceLevelLoad < 4)
        {
            isInvincible = false;
        }
        
        //choose attack, use attack and handle cooldown
        if (!isPatternOne && !isPatternTwo && !isPatternThree && !isPatternFour)
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
                if (bossHealth > 75)
                {
                    if(pattern < 4 / 3)
                    {
                        patternTwo();
                    }
                    else if(pattern < (4 / 3) * 2)
                    {
                        patternOne();
                    }
                    else if (pattern < 4)
                    {
                        patternFour();
                    }
                }
                else
                {
                    //35% chance of getting pattern 3
                    if (pattern < 1.5)
                    {
                        patternThree();
                    }
                    //30% chance for pattern 1
                    else if (pattern < 2.75)
                    {
                        patternOne();
                    }
                    //20% chance for pattern 2
                    else if (pattern < 3.5)
                    {
                        patternTwo();
                    }
                    //15% chance for pattern 4
                    else if (pattern < 4)
                    {
                        patternFour();
                    }
                }
                pattern = Random.Range(0.0f, 4.0f);
            }
        }

        //boss attack telegraphs
        if(coolDown <= 1 && coolDown > 0)
        {
            if(bossHealth > 75)
            {
                if (pattern < 4 / 3)
                {
                    patternTwoTelegraph();
                }
                else if (pattern < (4 / 3) * 2)
                {
                    patternOneTelegraph();
                }
                else if (pattern < 4)
                {
                    patternFourTelegraph();
                }
            }
            else
            {
                if (pattern < 1.5)
                {
                    patternThreeTelegraph();
                }
                else if (pattern < 2.75)
                {
                    patternOneTelegraph();
                }
                else if (pattern < 3.5)
                {
                    patternTwoTelegraph();
                }
                else if (pattern < 4)
                {
                    patternFourTelegraph();
                }
            }
            
        }

        //be able to test patterns
        if (Input.GetKeyDown("1") && !isPatternOne && !isPatternTwo && !isPatternThree && !isPatternFour)
        {
            timeSinceStart = 0;
            patternOne();
        }
        if (Input.GetKeyDown("2") && !isPatternOne && !isPatternTwo && !isPatternThree && !isPatternFour)
        {
            timeSinceStart = 0;
            patternTwo();
        }
        if (Input.GetKeyDown("3") && !isPatternOne && !isPatternTwo && !isPatternThree && !isPatternFour)
        {
            timeSinceStart = 0;
            patternThree();
        }
        if (Input.GetKeyDown("4") && !isPatternOne && !isPatternTwo && !isPatternThree && !isPatternFour)
        {
            timeSinceStart = 0;
            patternFour();
        }

        //make laser pattern work
        if (isPatternOne && !isPatternTwo && !isPatternThree && !isPatternFour)
        {
            //rotate counterclockwise to 45 in .8 sec
            if (!clockwise && !counterClockwise)
            {
                bossTransform.rotation = Quaternion.Slerp(Quaternion.Euler(0, 0, 0), Quaternion.Euler(0, 0, 45), timeSinceStart * 1.25f);
                if (bossTransform.rotation.z >= .382)
                {
                    //timeSinceStart == .8 here
                    clockwise = true;
                    timeSinceStart = 0;
                }
            }
            //rotate clockwise to -90 in (.8 * 3) sec
            else if (clockwise && !counterClockwise)
            {
                bossTransform.rotation = Quaternion.Slerp(Quaternion.Euler(0, 0, 45), Quaternion.Euler(0, 0, -90), timeSinceStart / 3f);
                if (bossTransform.rotation.z <= -.6)
                {
                    counterClockwise = true;
                    timeSinceStart = 0;
                }
            }
            //rotate counterclockwise to 0 in less than 1 sec
            else if (counterClockwise && clockwise)
            {
                bossTransform.rotation = Quaternion.Slerp(Quaternion.Euler(0, 0, -73.8f), Quaternion.Euler(0, 0, 0), timeSinceStart / 2f);
                if(bossTransform.rotation == Quaternion.identity)
                {
                    clockwise = false;
                    Debug.Log(timeSinceStart);
                }
            }
            else
            {
                isPatternOne = false;
                lasers = GameObject.FindGameObjectsWithTag("laser");
                foreach (GameObject newLaser in lasers)
                {
                    Destroy(newLaser);
                }
                bossTransform.rotation = Quaternion.identity;
                counterClockwise = false;
            }
            timeSinceStart += Time.fixedDeltaTime;
        }

        //make knee cap pattern work
        if (isPatternTwo && !isPatternOne && !isPatternThree && !isPatternFour)
        {
            //move lasers to left side of screen
            if (kneeLaserParent.position.x > kneeLaserEndPos.x)
            {
                kneeLaserParent.position = Vector3.MoveTowards(kneeLaserParent.position, kneeLaserEndPos, kneeLaserSpeed * Time.fixedDeltaTime);
            }
            //stop abruptly and move them downward
            else if (kneeLaserParent.position.x <= kneeLaserEndPos.x)
            {
                kneeLaserParent.position = Vector3.MoveTowards(kneeLaserParent.position, new Vector3(-30, -5), 1 * Time.fixedDeltaTime);
                //reset
                if (kneeLaserParent.position.y <= -5)
                {
                    isPatternTwo = false;
                    kneeLasers = GameObject.FindGameObjectsWithTag("kneeLaser");
                    foreach (GameObject newkneeLaser in kneeLasers)
                    {
                        Destroy(newkneeLaser);
                    }
                    kneeLaserParent.position = new Vector3(10, -3.5f);
                    //Debug.Log("pattern two end");
                }
            }
        }

        //make pellet pattern work
        if (isPatternThree && !isPatternOne && !isPatternTwo && !isPatternFour)
        {
            pelletParents = GameObject.FindGameObjectsWithTag("pelletParent");
            for (int i = 0; i < pelletParents.Length; i++)
            {
                pelletRings[i] = pelletParents[i].GetComponent<Transform>();
            }

            //rotate rings, every other ring in the opposite direction
            for (int i = 0; i < pelletParents.Length; i++)
            {
                if (i % 2 == 0)
                {
                    pelletRings[i].RotateAround(pelletRings[i].position, Vector3.back, pelletRotSpeed * Time.fixedDeltaTime);
                }
                else
                {
                    pelletRings[i].RotateAround(pelletRings[i].position, Vector3.back, -pelletRotSpeed * Time.fixedDeltaTime);
                }
            }

            //scale rings up to expand the pellets
            for (int i = 0; i < pelletParents.Length; i++)
            {
                pelletRings[i].localScale = Vector3.Lerp(pelletRings[i].localScale, ringTargetSize, pelletExpandSpeed);
                if (Vector3.Distance(pelletRings[i].localScale, ringTargetSize) < 1)
                {
                    pelletRings[i].localScale = ringTargetSize;
                }
                GameObject[] pelletChildren = GameObject.FindGameObjectsWithTag("pellet_" + i);
                foreach (GameObject pellet in pelletChildren)
                {
                    Vector3 pelletScale = pellet.GetComponent<Transform>().localScale;
                    pelletScale = new Vector3(pelletScale.x / pelletRings[i].localScale.x, pelletScale.y / pelletRings[i].localScale.y);
                }
            }

            if (pelletParents.Length < 8)
            {
                if (isGoodTime(1))
                {
                    patternThree();
                }
            }
            else if (pelletRings[7].localScale == ringTargetSize)
            {
                foreach (GameObject pelletParent in pelletParents)
                {
                    Destroy(pelletParent);
                }
                isPatternThree = false;
                pelletParents = null;
                pelletRings = new Transform[8];
                //Debug.Log("pattern three end");
            }

            timeSinceStart += Time.fixedDeltaTime;
        }

        //make boulder pattern work
        if (isPatternFour && !isPatternOne && !isPatternTwo && !isPatternThree)
        {
            //move boulders toward left side of screen
            if (!reachedLeft && !reachedRight)
            {
                boulderParent.position = Vector3.MoveTowards(boulderParent.position, boulderLeftPos, boulderSpeed * Time.fixedDeltaTime);
                if(Vector3.Distance(boulderParent.position, boulderLeftPos) < .1)
                {
                    reachedLeft = true;
                }
            }
            //move boulders toward right of screen and go off screen
            else if (reachedLeft && !reachedRight)
            {
                boulderParent.position = Vector3.MoveTowards(boulderParent.position, boulderRightPos, boulderSpeed * Time.fixedDeltaTime);
                if(Vector3.Distance(boulderParent.position, boulderRightPos) < .1)
                {
                    reachedRight = true;
                }
            }
            //move boulders back to the left side of the screen at higher speed
            else if (reachedRight && reachedLeft)
            {
                boulderParent.position = Vector3.MoveTowards(boulderParent.position, boulderLeftPos * 2, 3.5f * boulderSpeed * Time.fixedDeltaTime);
                if (Vector3.Distance(boulderParent.position, boulderLeftPos * 2) < 1)
                {
                    reachedLeft = false;
                }
            }
            //reset
            else
            {
                GameObject[] boulders = GameObject.FindGameObjectsWithTag("boulder");
                foreach (GameObject newBoulder in boulders)
                {
                    Destroy(newBoulder);
                }
                boulderParent.position = new Vector3(11, 0);
                isPatternFour = false;
                reachedRight = false;
            }
            //make boulders rotate as they move
            if (!reachedRight)
            {
                for (int i = 0; i < boulderParent.childCount; i++)
                {
                    Transform currentBoulder = boulderParent.GetChild(i);
                    currentBoulder.Rotate(0, 0, 15, Space.Self);
                }
            }
            else
            {
                for (int i = 0; i < boulderParent.childCount; i++)
                {
                    Transform currentBoulder = boulderParent.GetChild(i);
                    currentBoulder.Rotate(0, 0, 30, Space.Self);
                }
            }
            
            timeSinceStart += Time.fixedDeltaTime;
        }

        //make lizy invincible after you win
        if(bossHealth <= 0)
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
        healthBar.localScale = new Vector3(health / 150, 1);
        healthText.text = health.ToString();
    }

    //lasers that go in 8 directions and then oscillate clockwise and counterclockwise
    private void patternOne()
    {
        //set up the lasers
        for (int i = 0; i < 8; i++)
        {
            Quaternion rotation = Quaternion.identity;
            if (i == 0) { rotation = Quaternion.identity; } //6 o clock
            else if (i == 1) { rotation = Quaternion.Euler(0, 0, -45); } //7:30
            else if (i == 2) { rotation = Quaternion.Euler(0, 0, -90); }  //9 o clock
            else if (i == 3) { rotation = Quaternion.Euler(0, 0, -135); } //10:30
            else if (i == 4) { rotation = Quaternion.Euler(0, 0, 180); }  //12 o clock
            else if (i == 5) { rotation = Quaternion.Euler(0, 0, 135); }  //1:30
            else if (i == 6) { rotation = Quaternion.Euler(0, 0, 90); }   //3 o clock
            else if (i == 7) { rotation = Quaternion.Euler(0, 0, 45); }  //4:30

            GameObject newLaser = Instantiate(laserBar, new Vector3(0, 1), rotation, bossTransform);

            newLaser.transform.localScale = new Vector3(laserSize, 15);
            newLaser.gameObject.tag = "laser";
        }
        isPatternOne = true;
        //Debug.Log("pattern one start");
    }

    //he tries to take out your knee caps (vulnerable)
    private void patternTwo()
    {
        for (int i = 0; i < 10; i++)
        {
            GameObject newKneeLaser = Instantiate(kneeLaser, new Vector3(12 + i * 5, -3), Quaternion.identity, kneeLaserParent);
            newKneeLaser.transform.localScale = new Vector3(8f, kneeLaserSize);
            newKneeLaser.gameObject.tag = "kneeLaser";
        }
        isPatternTwo = true;
        //Debug.Log("pattern two start");
    }

    //pulses of pellets
    private void patternThree()
    {
        GameObject newPelletParent = Instantiate(pelletParent, new Vector3(0, 1), Quaternion.identity);
        newPelletParent.gameObject.tag = "pelletParent";
        for (int i = 0; i < pelletPos.Length; i++)
        {
            GameObject newPellet = Instantiate(pellet, pelletPos[i], Quaternion.identity, newPelletParent.GetComponent<Transform>());
            newPellet.transform.localScale = new Vector3(.5f, .5f);
            newPellet.gameObject.tag = "pellet_" + i;
        }
        isPatternThree = true;
        //Debug.Log("pattern three start");
    }

    //boulders from one side of the screen to the other (vulnerable)
    private void patternFour()
    {
        for (int i = 0; i < 3; i++)
        {
            GameObject newBoulder = Instantiate(boulder, new Vector3(9, 3f - i * 3), Quaternion.identity, boulderParent);
            newBoulder.transform.localScale = new Vector3(boulderSize, boulderSize);
            newBoulder.gameObject.tag = "boulder";
        }
        isPatternFour = true;
        //Debug.Log("patter four start");
    }

    private void patternOneTelegraph()
    {
        Color c = laserTelegraph.color;
        if(c.a > 0)
        {
            c.a = coolDown;
        }
        else
        {
            c.a = 1;
        }
        laserTelegraph.color = c;
    }

    private void patternTwoTelegraph()
    {
        Color c = kneeLaserTelegraph.color;
        if (c.a > 0)
        {
            c.a = coolDown;
        }
        else
        {
            c.a = 1;
        }
        kneeLaserTelegraph.color = c;
    }

    private void patternThreeTelegraph()
    {
        Color c = pelletTelegraph.color;
        if (c.a > 0)
        {
            c.a = coolDown;
        }
        else
        {
            c.a = 1;
        }
        pelletTelegraph.color = c;
    }

    private void patternFourTelegraph()
    {
        Color c = boulderTelegraph.color;
        if (c.a > 0)
        {
            c.a = coolDown;
        }
        else
        {
            c.a = 1;
        }
        boulderTelegraph.color = c;
    }

    private void stopTelegraph()
    {
        laserTelegraph.color = new Color(1, .4f, .985f, 0);
        kneeLaserTelegraph.color = new Color(1, .4f, .985f, 0);
        pelletTelegraph.color = new Color(1, .4f, .985f, 0);
        boulderTelegraph.color = new Color(1, .4f, .985f, 0);
    }

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
        timeText.text = "your time was\n" + "from start of fight: " + (Time.timeSinceLevelLoad/60).ToString("F0") + ":" + (Time.timeSinceLevelLoad % 60).ToString("F2") + "\n from startup of game: " + (Time.time / 60).ToString("F0") + ":" + (Time.time % 60).ToString("F2");
        timeText.color = new Color(1, 1, 1, 1);
    }
}
