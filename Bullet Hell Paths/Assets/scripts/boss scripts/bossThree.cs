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

    //pattern variables
    private float timeSinceStart = 0;
    private bool isFloorPattern = false;
    private bool isShotgunPattern = false;
    private bool isEngulfPattern = false;
    private bool isFlameThrowerPattern = false;
    private bool isRisingFireBallPattern = false;

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

        if (Input.GetKeyDown("1") && !isFloorPattern && !isShotgunPattern && !isEngulfPattern && !isFlameThrowerPattern && !isRisingFireBallPattern)
        {
            timeSinceStart = 0;
            floorFirePattern();
        }
        if (Input.GetKeyDown("2") && !isFloorPattern && !isShotgunPattern && !isEngulfPattern && !isFlameThrowerPattern && !isRisingFireBallPattern)
        {
            timeSinceStart = 0;
            shotgunFirePattern();
        }
        if (Input.GetKeyDown("3") && !isFloorPattern && !isShotgunPattern && !isEngulfPattern && !isFlameThrowerPattern && !isRisingFireBallPattern)
        {
            timeSinceStart = 0;
            engulfPattern();
        }
        if (Input.GetKeyDown("4") && !isFloorPattern && !isShotgunPattern && !isEngulfPattern && !isFlameThrowerPattern && !isRisingFireBallPattern)
        {
            timeSinceStart = 0;
            dualFlamehtrowerPattern();
        }
        if(Input.GetKeyDown("5") && !isFloorPattern && !isShotgunPattern && !isEngulfPattern && !isFlameThrowerPattern && !isRisingFireBallPattern)
        {
            timeSinceStart = 0;
            risingFireBallPattern();
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

            //spawn new fireballs either 1 unit to the left or right of the last one
            if (isGoodTime(1, 10))
            {
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
            }

            timeSinceStart += Time.fixedDeltaTime;
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
        //set initial position of shotgun
        float shotgunPos = ((lizyTransform.position.x - -9) / (9 - -9)) * (1f - -1f) + -1f;
        shotgunParent.position = new Vector3(shotgunPos, 1);

        for (int i = 0; i < 7; i++)
        {
            GameObject newFireBall = Instantiate(fireBall, shotgunParent.position, Quaternion.identity, shotgunParent);
            newFireBall.transform.position += shotgunPositions[i];
            newFireBall.tag = "bossThreeAttack";
        }
        isShotgunPattern = true;
    }    

    //engulfing fireball pattern
    private void engulfPattern()
    {
        GameObject newFireBall = Instantiate(fireBall, engulfParent.position, Quaternion.identity, engulfParent);
        newFireBall.tag = "bossThreeAttack";
        isEngulfPattern = true;
    }

    //rising fireballs pattern
    private void risingFireBallPattern()
    {
        GameObject newFireBall = Instantiate(fireBall, new Vector3(risingFireBallX, -7), Quaternion.identity);
        newFireBall.tag = "bossThreeAttack";
        risingFireBalls.Add(newFireBall);
        newFireBall = Instantiate(fireBall, new Vector3(risingFireBallX - 1.5f, -7), Quaternion.identity);
        newFireBall.tag = "bossThreeAttack";
        risingFireBalls.Add(newFireBall);
        newFireBall = Instantiate(fireBall, new Vector3(risingFireBallX + 1.5f, -7), Quaternion.identity);
        newFireBall.tag = "bossThreeAttack";
        risingFireBalls.Add(newFireBall);
        isRisingFireBallPattern = true;
    }

    //double flamethrower pattern
    private void dualFlamehtrowerPattern()
    {
        GameObject newFlameThrowerLeft = Instantiate(flameThrower, new Vector3(-4, -6f), flameThrowerParentLeft.rotation, flameThrowerParentLeft);
        GameObject newFlameThrowerRight = Instantiate(flameThrower, new Vector3(4, -6f), flameThrowerParentRight.rotation, flameThrowerParentRight);
        newFlameThrowerLeft.tag = "bossThreeAttack";
        newFlameThrowerRight.tag = "bossThreeAttack";
        isFlameThrowerPattern = true;
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
