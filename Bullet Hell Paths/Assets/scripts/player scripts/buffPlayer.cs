using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class buffPlayer : MonoBehaviour
{
    public playerController lizy;

    private void Start()
    {
        if(SceneManager.GetActiveScene().name == "FrankBoss")
        {
            lizy.attackDamage = 10;
            lizy.activateUpAttack = true;
        }
        else if (SceneManager.GetActiveScene().name == "DominicBoss")
        {
            lizy.attackDamage = 30;
            lizy.activateUpAttack = true;
            //lizy.activateDoubleJump = true;
        }
    }
}
