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
    private float bossHealth = 500;
    private const float maxHealth = 500;
    public Transform healthBar;
    public Text healthText;
    private bool isInvincible = true;

    //fireball used by most attacks
    public GameObject fireBall;

    //floor fire variables
    public GameObject floorFire;

    //shotgun fire variables
    public Transform shotgunParent;
    private Vector3[] shotgunPositions = {
        //middle one
        new Vector3(0,-1), new Vector3(-1.5f, -.25f), new Vector3(-1f, -.5f), new Vector3(-.5f, -.75f), new Vector3(.5f, -.75f), new Vector3(1, -.5f), new Vector3(1.5f, -.25f)
    };
    private float shotgunSpeed = 4;

    //engulfing fireball variables
    public Transform engulfParent;
    private float engulfSpeed = 10;
    private Vector3 engulfTargetSize = new Vector3(12, 12);
    private bool expanding = true;

    //flamethrower variables
    public GameObject flameThrower;
    public Transform flameThrowerParentLeft;
    public Transform flameThrowerParentRight;
    private float flameThrowerSpeed = 30;
    private bool goingLeft = true;

    //rising fireball variables
    private List<GameObject> risingFireBalls = new List<GameObject>();
    private float risingSpeed = 5;
    private float risingFireBallX = 0;

    //flame geyser variables
    public GameObject fireGeyser;
    private List<GameObject> fireGeysers = new List<GameObject>();
    private float[] geyserPositions = { 0, 3, 6, 9 };
    private float geyserSpeed = 8;
    private int numGeysers = 0;

    //fire sword variables
    public GameObject fireSword;
    private int swordSequence;
    private int swordIteration = 0;
    private float swordDelay = 0;

    //meteor variables
    private GameObject meteor;
    private float meteorSpeed = 10;
    private List<GameObject> meteorRubble = new List<GameObject>();
    private Vector3[] rubbleOriginPos = new Vector3[4]; 
    private Vector3[] rubbleTargetPos = new Vector3[4];
    private float rubbleSpeed = 1;
    private float bezierControl = 0;

    //pattern variables
    private float timeSinceStart = 0;
    private bool isFloorPattern = false;
    private bool isShotgunPattern = false;
    private bool isEngulfPattern = false;
    private bool isFlameThrowerPattern = false;
    private bool isRisingFireBallPattern = false;
    private bool isFireGeyserPattern = false;
    private bool isFireSwordPattern = false;
    private bool isMeteorPattern = false;
    private float pattern;
    private float floorFireChance = 1f / 8f;
    private float shotgunChance = 2f / 8f;
    private float engulfChance = 3f / 8f;
    private float risingFireChance = 4f / 8f;
    private float flamethrowerChance = 5f / 8f;
    private float geyserChance = 6f / 8f;
    private float swordChance = 7f / 8f;
    private float meteorChance = 1;

    //telegraph variables
    public SpriteRenderer floorFireTelegraphSprite;
    public SpriteRenderer shotgunTelegraphSprite;
    public SpriteRenderer engulfTelegraphSprite;
    public GameObject risingFireTelegraphObj;
    public SpriteRenderer flamethrowerTelegraphSprite;
    public GameObject geyserTelegraphParent;
    public GameObject swordTelegraphObj;
    public GameObject meteorTelegraphObj;
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
        swordSequence = Random.Range(1, 4);
        pattern = Random.Range(0f, 1f);
    }

    void FixedUpdate()
    {
        //invincible at the beginning of the fight
        if (Time.timeSinceLevelLoad > 1.8 && Time.timeSinceLevelLoad < 4)
        {
            isInvincible = false;
        }

        if (!isFloorPattern && !isShotgunPattern && !isEngulfPattern && !isFlameThrowerPattern && !isRisingFireBallPattern && !isFireGeyserPattern && !isFireSwordPattern && !isMeteorPattern)
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

                if (pattern < floorFireChance)
                {
                    floorFirePattern();
                }
                else if (pattern < shotgunChance)
                {
                    shotgunFirePattern();
                }
                else if (pattern < engulfChance)
                {
                    engulfPattern();
                }
                else if (pattern < risingFireChance)
                {
                    risingFireBallPattern();
                }
                else if (pattern < flamethrowerChance)
                {
                    dualFlamehtrowerPattern();
                }
                else if (pattern < geyserChance)
                {
                    flameGeyserPattern(numGeysers);
                }
                else if (pattern < swordChance)
                {
                    fireSwordPattern(swordSequence, swordIteration);
                }
                else if (pattern < meteorChance)
                {
                    meteorPattern();
                }
                choosePatternRange();
                pattern = Random.Range(0.0f, 1.0f);
            }
        }

        //make telegraphs happen
        if (coolDown <= 1 && coolDown > 0)
        {
            if (pattern < floorFireChance)
            {
                floorFireTelegraph();
            }
            else if (pattern < shotgunChance)
            {
                shotgunFireTelegraph();
            }
            else if (pattern < engulfChance)
            {
                engulfTelegraph();
            }
            else if (pattern < risingFireChance)
            {
                risingFireBallTelegraph();
            }
            else if (pattern < flamethrowerChance)
            {
                dualFlamethrowerTelegraph();
            }
            else if (pattern < geyserChance)
            {
                flameGeyserTelegraph();
            }
            else if (pattern < swordChance)
            {
                fireSwordTelegraph();
            }
            else if (pattern < meteorChance)
            {
                meteorTelegraph();
            }
        }

        //pattern testing
        {
            if (Input.GetKeyDown("1") && !isFloorPattern && !isShotgunPattern && !isEngulfPattern && !isFlameThrowerPattern && !isRisingFireBallPattern && !isFireGeyserPattern && !isFireSwordPattern && !isMeteorPattern)
            {
                timeSinceStart = 0;
                floorFirePattern();
            }
            if (Input.GetKeyDown("2") && !isFloorPattern && !isShotgunPattern && !isEngulfPattern && !isFlameThrowerPattern && !isRisingFireBallPattern && !isFireGeyserPattern && !isFireSwordPattern && !isMeteorPattern)
            {
                timeSinceStart = 0;
                shotgunFirePattern();
            }
            if (Input.GetKeyDown("3") && !isFloorPattern && !isShotgunPattern && !isEngulfPattern && !isFlameThrowerPattern && !isRisingFireBallPattern && !isFireGeyserPattern && !isFireSwordPattern && !isMeteorPattern)
            {
                timeSinceStart = 0;
                engulfPattern();
            }
            if (Input.GetKeyDown("4") && !isFloorPattern && !isShotgunPattern && !isEngulfPattern && !isFlameThrowerPattern && !isRisingFireBallPattern && !isFireGeyserPattern && !isFireSwordPattern && !isMeteorPattern)
            {
                timeSinceStart = 0;
                dualFlamehtrowerPattern();
            }
            if (Input.GetKeyDown("5") && !isFloorPattern && !isShotgunPattern && !isEngulfPattern && !isFlameThrowerPattern && !isRisingFireBallPattern && !isFireGeyserPattern && !isFireSwordPattern && !isMeteorPattern)
            {
                timeSinceStart = 0;
                risingFireBallPattern();
            }
            if (Input.GetKeyDown("6") && !isFloorPattern && !isShotgunPattern && !isEngulfPattern && !isFlameThrowerPattern && !isRisingFireBallPattern && !isFireGeyserPattern && !isFireSwordPattern && !isMeteorPattern)
            {
                timeSinceStart = 0;
                flameGeyserPattern(numGeysers);
            }
            if (Input.GetKeyDown("7") && !isFloorPattern && !isShotgunPattern && !isEngulfPattern && !isFlameThrowerPattern && !isRisingFireBallPattern && !isFireGeyserPattern && !isFireSwordPattern && !isMeteorPattern)
            {
                timeSinceStart = 0;
                fireSwordPattern(swordSequence, swordIteration);
            }
            if (Input.GetKeyDown("8") && !isFloorPattern && !isShotgunPattern && !isEngulfPattern && !isFlameThrowerPattern && !isRisingFireBallPattern && !isFireGeyserPattern && !isFireSwordPattern && !isMeteorPattern)
            {
                timeSinceStart = 0;
                meteorPattern();
            }
        }

        //floor pattern
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

        //shotgun pattern
        if (isShotgunPattern)
        {
            timeSinceStart += Time.fixedDeltaTime;

            if(timeSinceStart < 2.5)
            {
                //make center fireball face player
                lizyLastPos = lizyTransform.position;
                Vector3 direction = lizyLastPos - shotgunParent.position;
                shotgunParent.rotation = Quaternion.AngleAxis(Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg, Vector3.forward);

                //move the shotgun slightly to follow the player as they move away from the center of the screen
                float shotgunPos = ((lizyTransform.position.x - -9) / (9 - -9)) * (1f - -1f) + -1f;
                shotgunParent.position = Vector3.MoveTowards(shotgunParent.position, new Vector3(shotgunPos, 1), .8f * Time.fixedDeltaTime);
            }
            else if (timeSinceStart > 2.75)
            {
                //shoot fire balls outward by expanding their parent object
                shotgunParent.localScale = Vector3.Lerp(shotgunParent.localScale, new Vector3(12, 12), shotgunSpeed * Time.fixedDeltaTime);

                foreach (Transform newFireBall in shotgunParent.GetComponentInChildren<Transform>())
                {
                    Vector3 fireBallScale = newFireBall.localScale;
                    fireBallScale = new Vector3(4 / shotgunParent.localScale.x, 4 / shotgunParent.localScale.y, 1);
                    newFireBall.localScale = fireBallScale;
                }
            }
            //reset attack
            if (Vector3.Distance(shotgunParent.localScale, new Vector3(12, 12)) < 1)
            {
                destroyAttack();
                shotgunParent.localScale = new Vector3(1, 1, 1);
                shotgunParent.localRotation = Quaternion.Euler(0, 0, -90);
                isShotgunPattern = false;
            }
        }

        //engulfing fireball pattern
        if (isEngulfPattern)
        {
            //expanding or shrinking
            if (expanding)
            {
                engulfParent.localScale = Vector3.MoveTowards(engulfParent.localScale, engulfTargetSize, engulfSpeed * Time.fixedDeltaTime);
            }
            else
            {
                engulfParent.localScale = Vector3.MoveTowards(engulfParent.localScale, new Vector3(0, 0), engulfSpeed * Time.fixedDeltaTime);
            }

            //switch to shrinking
            if (Vector3.Distance(engulfParent.localScale, engulfTargetSize) < .1f)
            {
                timeSinceStart += Time.fixedDeltaTime;
                if (timeSinceStart >= 1)
                {
                    expanding = false;
                }
            }

            //reset attack
            if(Vector3.Distance(engulfParent.localScale, new Vector3(0,0)) <= .01f)
            {
                destroyAttack();
                engulfParent.localScale = new Vector3(1, 1);
                expanding = true;
                isEngulfPattern = false;
            }
        }

        //flamethrower pattern
        if (isFlameThrowerPattern)
        {   
            if ((flameThrowerParentLeft.eulerAngles.z == 0 || flameThrowerParentLeft.eulerAngles.z > 315) && goingLeft)
            {
                flameThrowerParentLeft.rotation = Quaternion.RotateTowards(flameThrowerParentLeft.rotation, Quaternion.Euler(0, 0, -45), flameThrowerSpeed * Time.fixedDeltaTime);
                flameThrowerParentRight.rotation = Quaternion.RotateTowards(flameThrowerParentRight.rotation, Quaternion.Euler(0, 0, -45), flameThrowerSpeed * Time.fixedDeltaTime);
            }
            else
            {
                goingLeft = false;
                flameThrowerSpeed = 40;
                flameThrowerParentLeft.rotation = Quaternion.RotateTowards(flameThrowerParentLeft.rotation, Quaternion.Euler(0, 0, 70), flameThrowerSpeed * Time.fixedDeltaTime);
                flameThrowerParentRight.rotation = Quaternion.RotateTowards(flameThrowerParentRight.rotation, Quaternion.Euler(0, 0, 70), flameThrowerSpeed * Time.fixedDeltaTime);
                if (flameThrowerParentRight.eulerAngles.z >= 50 && flameThrowerParentRight.eulerAngles.z < 70 && timeSinceStart == 0)
                {
                    Destroy(flameThrowerParentRight.GetChild(0).gameObject);
                    goingLeft = true;
                    timeSinceStart += Time.fixedDeltaTime;
                }
                else if (timeSinceStart > 0)
                {
                    timeSinceStart += Time.fixedDeltaTime;
                }
            }
            if(timeSinceStart >= 1)
            {
                destroyAttack();
                flameThrowerParentLeft.eulerAngles = new Vector3(0, 0, 0);
                flameThrowerParentRight.eulerAngles = new Vector3(0, 0, 0);
                goingLeft = true;
                isFlameThrowerPattern = false;
            }
        }

        //fireballs rising from the floor pattern
        if (isRisingFireBallPattern)
        {
            //move all fire balls upwards
            foreach (GameObject newFireBall in risingFireBalls)
            {
                Vector3 fireBallPos = newFireBall.transform.position;
                newFireBall.transform.position = Vector3.MoveTowards(newFireBall.transform.position, new Vector3(fireBallPos.x, 20), risingSpeed * Time.fixedDeltaTime);
            }

            //spawn new fireballs below the player
            if (isGoodTime(1, 10))
            {
                //spawn new fireballs either 1 unit to the left or right of the last one
                //if (lizyTransform.position.x < risingFireBallX)
                //{
                //    risingFireBallX -= 2;
                //}
                //else
                //{
                //    risingFireBallX += 2;
                //}
                //if (risingFireBallX > 8)
                //{
                //    risingFireBallX = 7;
                //}
                //else if (risingFireBallX < -8)
                //{
                //    risingFireBallX = -7;
                //}
                risingFireBallX = lizyTransform.position.x;
                risingFireBallPattern();
            }

            if (risingFireBalls[risingFireBalls.Count-1].transform.position.y >= 15)
            {
                risingFireBallX = 0;
                destroyAttack();
                isRisingFireBallPattern = false;
                risingFireBalls.Clear();
            }

            timeSinceStart += Time.fixedDeltaTime;
        }

        //fire geysers rising from floor
        if (isFireGeyserPattern)
        {
            foreach (GameObject geyser in fireGeysers)
            {
                Vector3 geyserPos = geyser.transform.position;
                geyser.transform.position = Vector3.MoveTowards(geyserPos, new Vector3(geyserPos.x, 0), geyserSpeed * Time.fixedDeltaTime);
            }

            if (isGoodTime(1f, 3))
            {
                numGeysers++;
                flameGeyserPattern(numGeysers);
            }

            if(fireGeysers[fireGeysers.Count - 1].transform.position.y >= 0)
            {
                numGeysers = 0;
                destroyAttack();
                isFireGeyserPattern = false;
                fireGeysers.Clear();
            }

            timeSinceStart += Time.fixedDeltaTime;
        }

        //fire swords slashing parts of the screen
        if (isFireSwordPattern)
        {
            if (swordDelay >= .7f)
            {
                destroyAttack();
            }

            swordDelay += Time.fixedDeltaTime;

            if (isGoodTime(1, 3))
            {
                swordIteration++;
                if (swordIteration < 3)
                {
                    fireSwordPattern(swordSequence, swordIteration);
                }
                else
                {
                    destroyAttack();
                    isFireSwordPattern = false;
                    swordIteration = 0;
                }
                swordDelay = 0;
            }

            timeSinceStart += Time.fixedDeltaTime;
        }

        //metoer pattern
        if (isMeteorPattern)
        {
            //if the meteor exists move it
            if (meteor != null)
            {
                Vector3 meteorPos = meteor.transform.position;
                meteor.transform.position = Vector3.MoveTowards(meteorPos, new Vector3(meteorPos.x, -4), meteorSpeed * Time.fixedDeltaTime);
                
                //if the meteor has crashed make some rubble
                if (meteor.transform.position.y <= -4)
                {
                    for (int i = 0; i < 4; i++)
                    {
                        meteorRubble.Add(Instantiate(fireBall, meteor.transform.position, Quaternion.identity));
                        rubbleOriginPos[i] = meteorRubble[i].transform.position;
                    }
                    rubbleTargetPos[0] = new Vector3(meteorRubble[0].transform.position.x - 8, -5);
                    rubbleTargetPos[1] = new Vector3(meteorRubble[1].transform.position.x - 4, -5);
                    rubbleTargetPos[2] = new Vector3(meteorRubble[2].transform.position.x + 4, -5);
                    rubbleTargetPos[3] = new Vector3(meteorRubble[3].transform.position.x + 8, -5);

                    Destroy(meteor);
                }
            }

            //if the rubble exists move it
            if (meteorRubble.Count > 0)
            {
                for (int i = 0; i < 4; i++)
                {
                    //get the middle control point for the bezier curve
                    float xOffset = Mathf.Abs(rubbleOriginPos[i].x - rubbleTargetPos[i].x) / 2f;
                    float midPointX;
                    if (rubbleOriginPos[i].x > rubbleTargetPos[i].x)
                    {
                        midPointX = rubbleOriginPos[i].x - xOffset;
                    }
                    else
                    {
                        midPointX = rubbleOriginPos[i].x + xOffset;
                    }
                    Vector3 midPoint = new Vector3(midPointX, rubbleOriginPos[i].y + 6);

                    //interpolate rubble between control points
                    Vector3 m1 = Vector3.Lerp(rubbleOriginPos[i], midPoint, rubbleSpeed * bezierControl);
                    Vector3 m2 = Vector3.Lerp(midPoint, rubbleTargetPos[i], rubbleSpeed * bezierControl);
                    meteorRubble[i].transform.position = Vector3.Lerp(m1, m2, rubbleSpeed * bezierControl);
                }

                bezierControl += Time.fixedDeltaTime;

                //if all rubble is below the floor stop attack
                if (bezierControl >= 1)
                {
                    destroyAttack();
                    meteorRubble.Clear();
                    isMeteorPattern = false;
                    bezierControl = 0;
                }
            }
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
        isFloorPattern = true;
    }

    //shotgun fire pattern
    private void shotgunFirePattern()
    {
        //set initial position of shotgun
        float shotgunPos = ((lizyTransform.position.x - -9) / (9 - -9)) * (1f - -1f) + -1f;
        shotgunParent.position = new Vector3(shotgunPos, 1);

        for (int i = 0; i < 7; i++)
        {
            GameObject newFireBall = Instantiate(fireBall, shotgunParent.position, Quaternion.identity, shotgunParent);
            newFireBall.transform.position += shotgunPositions[i];
        }
        isShotgunPattern = true;
    }    

    //engulfing fireball pattern
    private void engulfPattern()
    {
        GameObject newFireBall = Instantiate(fireBall, engulfParent.position, Quaternion.identity, engulfParent);
        isEngulfPattern = true;
    }

    //rising fireballs pattern
    private void risingFireBallPattern()
    {
        GameObject newFireBall = Instantiate(fireBall, new Vector3(risingFireBallX, -7), Quaternion.identity);
        risingFireBalls.Add(newFireBall);
        newFireBall = Instantiate(fireBall, new Vector3(risingFireBallX - 1.5f, -7), Quaternion.identity);
        risingFireBalls.Add(newFireBall);
        newFireBall = Instantiate(fireBall, new Vector3(risingFireBallX + 1.5f, -7), Quaternion.identity);
        risingFireBalls.Add(newFireBall);
        isRisingFireBallPattern = true;
    }

    //double flamethrower pattern
    private void dualFlamehtrowerPattern()
    {
        GameObject newFlameThrowerLeft = Instantiate(flameThrower, new Vector3(-4, -6f), flameThrowerParentLeft.rotation, flameThrowerParentLeft);
        GameObject newFlameThrowerRight = Instantiate(flameThrower, new Vector3(4, -6f), flameThrowerParentRight.rotation, flameThrowerParentRight);
        isFlameThrowerPattern = true;
    }

    //fire geysers pattern
    private void flameGeyserPattern(int nextGeyser)
    {
        //only instantiate one geyser when it is at the center of the screen otherwise mirror the instantiations
        if (nextGeyser == 0)
        {
            GameObject newGeyser = Instantiate(fireGeyser, new Vector3(0,-10), Quaternion.identity);
            fireGeysers.Add(newGeyser);
        }
        else
        {
            GameObject newGeyser1 = Instantiate(fireGeyser, new Vector3(geyserPositions[nextGeyser], -10), Quaternion.identity);
            GameObject newGeyser2 = Instantiate(fireGeyser, new Vector3(geyserPositions[nextGeyser] * -1, -10), Quaternion.identity);
            fireGeysers.Add(newGeyser1);
            fireGeysers.Add(newGeyser2);
        }
        isFireGeyserPattern = true;
    }

    //fire swords pattern
    private void fireSwordPattern(int sequence, int iteration)
    {
        GameObject newSword;

        //top --> right --> left
        if (sequence == 1)
        {
            if(iteration == 0)
            {
                newSword = Instantiate(fireSword, new Vector3(0, 6), Quaternion.Euler(0, 0, 45));
            }
            else if(iteration == 1)
            {
                newSword = Instantiate(fireSword, new Vector3(7, 0), Quaternion.identity);
            }
            else
            {
                newSword = Instantiate(fireSword, new Vector3(-7, 0), Quaternion.identity);
            }
        }
        //right --> left --> top
        else if(sequence == 2)
        {
            if (iteration == 0)
            {
                newSword = Instantiate(fireSword, new Vector3(7, 0), Quaternion.identity);
            }
            else if (iteration == 1)
            {
                newSword = Instantiate(fireSword, new Vector3(-7, 0), Quaternion.identity);
            }
            else
            {
                newSword = Instantiate(fireSword, new Vector3(0, 6), Quaternion.Euler(0, 0, 45));
            }
        }
        //left --> right --> top
        else
        {
            if (iteration == 0)
            {
                newSword = Instantiate(fireSword, new Vector3(-7, 0), Quaternion.identity);
            }
            else if (iteration == 1)
            {
                newSword = Instantiate(fireSword, new Vector3(7, 0), Quaternion.identity);
            }
            else
            {
                newSword = Instantiate(fireSword, new Vector3(0, 6), Quaternion.Euler(0, 0, 45));
            }
        }

        isFireSwordPattern = true;
    }

    //meteor pattern
    private void meteorPattern()
    {
        meteor = Instantiate(fireBall, new Vector3(lizyTransform.position.x, 10), Quaternion.identity);
        meteor.transform.localScale = new Vector3(10, 10);
        isMeteorPattern = true;
    }

    //floor fire telegraph
    private void floorFireTelegraph()
    {
        Color c = floorFireTelegraphSprite.color;
        if (c.a > 0)
        {
            c.a = coolDown;
        }
        else
        {
            c.a = 1;
        }
        floorFireTelegraphSprite.color = c;
    }

    //shotgun fire telegraph
    private void shotgunFireTelegraph()
    {
        Color c = shotgunTelegraphSprite.color;
        if (c.a > 0)
        {
            c.a = coolDown;
        }
        else
        {
            c.a = 1;
        }
        shotgunTelegraphSprite.color = c;
    }

    //engulfing fireball telegraph
    private void engulfTelegraph()
    {
        Color c = engulfTelegraphSprite.color;
        if (c.a > 0)
        {
            c.a = coolDown;
        }
        else
        {
            c.a = 1;
        }
        engulfTelegraphSprite.color = c;
    }

    //rising fireballs telegraph
    private void risingFireBallTelegraph()
    {
        if(coolDown > .98f)
        {
            risingFireTelegraphObj.transform.position = new Vector3(lizyTransform.position.x, risingFireTelegraphObj.transform.position.y);
        }

        Color c = risingFireTelegraphObj.GetComponent<SpriteRenderer>().color;
        if (c.a > 0)
        {
            c.a = coolDown;
        }
        else
        {
            c.a = 1;
        }
        risingFireTelegraphObj.GetComponent<SpriteRenderer>().color = c;
    }

    //double flamethrower telegraph
    private void dualFlamethrowerTelegraph()
    {
        Color c = flamethrowerTelegraphSprite.color;
        if (c.a > 0)
        {
            c.a = coolDown;
        }
        else
        {
            c.a = 1;
        }
        flamethrowerTelegraphSprite.color = c;
    }
    
    //fire geysers telegraph
    private void flameGeyserTelegraph()
    {
        SpriteRenderer[] geyserSprites = geyserTelegraphParent.GetComponentsInChildren<SpriteRenderer>();
        foreach (SpriteRenderer geyser in geyserSprites)
        {
            Color c = geyser.color;
            if (c.a > 0)
            {
                c.a = coolDown;
            }
            else
            {
                c.a = 1;
            }
            geyser.color = c;
        }
    }

    //fire swords telegraph
    private void fireSwordTelegraph()
    {
        if(coolDown > .98f)
        {
            swordSequence = Random.Range(1, 4);
            if (swordSequence == 1)
            {
                swordTelegraphObj.transform.position = new Vector3(0, 6);
                swordTelegraphObj.transform.rotation = Quaternion.Euler(0, 0, 45);
            }
            else if (swordSequence == 2)
            {
                swordTelegraphObj.transform.position = new Vector3(7, 0);
            }
            else
            {
                swordTelegraphObj.transform.position = new Vector3(-7, 0);
            }
        }

        Color c = swordTelegraphObj.GetComponent<SpriteRenderer>().color;
        if (c.a > 0)
        {
            c.a = coolDown;
        }
        else
        {
            c.a = 1;
        }
        swordTelegraphObj.GetComponent<SpriteRenderer>().color = c;
    }

    //meteor telegraph
    private void meteorTelegraph()
    {
        if (coolDown > .98f)
        {
            meteorTelegraphObj.transform.position = new Vector3(lizyTransform.position.x, meteorTelegraphObj.transform.position.y);
        }

        Color c = meteorTelegraphObj.GetComponent<SpriteRenderer>().color;
        if (c.a > 0)
        {
            c.a = coolDown;
        }
        else
        {
            c.a = 1;
        }
        meteorTelegraphObj.GetComponent<SpriteRenderer>().color = c;
    }

    //stop telegraphs
    private void stopTelegraph()
    {
        Color c = new Color(1, .313f, .3f, 0);

        floorFireTelegraphSprite.color = c;
        shotgunTelegraphSprite.color = c;
        engulfTelegraphSprite.color = c;
        risingFireTelegraphObj.GetComponent<SpriteRenderer>().color = c;
        flamethrowerTelegraphSprite.color = c;
        foreach (SpriteRenderer geyser in geyserTelegraphParent.GetComponentsInChildren<SpriteRenderer>())
        {
            geyser.color = c;
        }
        swordTelegraphObj.GetComponent<SpriteRenderer>().color = c;
        meteorTelegraphObj.GetComponent<SpriteRenderer>().color = c;
    }

    //TODO
    //decide which patterns have a higher chance of occuring
    private void choosePatternRange()
    {

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
        healthBar.localScale = new Vector3(health / maxHealth, 1);
        healthText.text = health.ToString();
    }

    //destroy all attacks in the scene
    private void destroyAttack()
    {
        GameObject[] bossThreeAttacks = GameObject.FindGameObjectsWithTag("bossThreeAttack");
        foreach (GameObject attack in bossThreeAttacks)
        {
            Destroy(attack);
        }
    }

    //check if the current timeSinceStart is an iteration of x sec
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
