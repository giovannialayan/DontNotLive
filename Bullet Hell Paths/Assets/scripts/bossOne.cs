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
    public float health = 100;
    public Transform healthBar;
    public Text healthText;

    //laser variables
    public GameObject laserBar;
    private float timeSinceStart = 0;
    public float laserSpeed = 25f;
    public float laserSize = .2f;
    private GameObject[] lasers;

    //pattern variables
    private bool isPatternOne = false;
    private bool isPatternTwo = false;

    void Start()
    {
        bossSprite = GetComponent<SpriteRenderer>();
        bossTransform = GetComponent<Transform>();
        patternOne();
    }

    // Update is called once per frame
    void Update()
    {
        //make knee cap pattern work
        if (isPatternTwo)
        {

        }

        //make laser pattern work
        if (isPatternOne)
        {
            //rotate counterclockwise
            if (timeSinceStart >= 0 && timeSinceStart < 3.62)
            {
                bossTransform.rotation = Quaternion.Slerp(Quaternion.Euler(0, 0, 0), Quaternion.Euler(0, 0, Mathf.PingPong(Time.time * laserSpeed, 45)), timeSinceStart);
            }
            //rotate clockwise
            else if (timeSinceStart >= 3.62 && timeSinceStart < 7.22)
            {
                bossTransform.rotation = Quaternion.Slerp(Quaternion.Euler(0, 0, 0), Quaternion.Euler(0, 0, Mathf.PingPong(Time.time * laserSpeed, 45) * -1f), timeSinceStart);
            }
            //stop rotating and destroy lasers and reset rotation of bossOne
            else if (timeSinceStart >= 7.22)
            {
                isPatternOne = false;
                lasers = GameObject.FindGameObjectsWithTag("laser");
                foreach (GameObject newLaser in lasers)
                {
                    Destroy(newLaser);
                }
                bossTransform.rotation = Quaternion.identity;
            }
            timeSinceStart += Time.deltaTime;
        }
    }

    public void takeDamage(int damage)
    {
        health -= damage;
        decreaseHealthBar(health);
        if (health <= 0)
        {
            bossSprite.color = Color.red;
        }
    }

    private void decreaseHealthBar(float health)
    {
        healthBar.localScale = new Vector3(health / 100, 1);
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

            GameObject newLaser = Instantiate(laserBar, new Vector3(0,1), rotation, bossTransform);

            newLaser.transform.localScale = new Vector3(laserSize, 15);
            newLaser.gameObject.tag = "laser";
            isPatternOne = true;
        }
    }

    //he tries to take out your knee caps (vulnerable)
    private void patternTwo()
    {

    }

    //pulses of pellets
    private void patternThree()
    {

    }

    //boulders from one side of the screen tot he other (vulnerable)
    private void patternFour()
    {

    }
}
