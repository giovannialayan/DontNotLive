using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class buffPlayer : MonoBehaviour
{
    public playerController lizy;

    private void Start()
    {
        if(SceneManager.GetActiveScene().buildIndex > 1)
        {
            lizy.attackDamage *= 2;
            lizy.activateUpAttack = true;
        }
        if (SceneManager.GetActiveScene().buildIndex > 2)
        {
            lizy.attackDamage *= 2;
            lizy.maxHealth = 3;
            lizy.activateDoubleJump = true;
        }
        if (SceneManager.GetActiveScene().buildIndex > 3)
        {
            lizy.attackDamage *= 2;
            lizy.activateParry = true;
            //dash?
            //roll?
            //screw attack?
            //flurry attack?
            //projectile destroying attack????????
            //parry?????????????
            //ability to stand on projectiles???????
            //before the final boss give the player the ability to change the music instead of an upgrade
        }
    }
}
