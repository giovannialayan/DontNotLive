using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class damageBosses : MonoBehaviour
{
    public bossOne bossOne;
    public bossTwo bossTwo;
    public bossThree bossThree;

    public void dealDamage(int damage)
    {
        if (SceneManager.GetActiveScene().name == "FirstBoss")
        {
            bossOne.takeDamage(damage);
        }
        else if (SceneManager.GetActiveScene().name == "FrankBoss")
        {
            bossTwo.takeDamage(damage);
        }
        else if(SceneManager.GetActiveScene().name == "DominicBoss")
        {
            bossThree.takeDamage(damage);
        }
    }
}
