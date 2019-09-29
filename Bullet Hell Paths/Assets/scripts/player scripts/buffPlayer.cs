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
            afterBossOne();
        }
    }

    public void afterBossOne()
    {
        lizy.attackDamage = 10;
        lizy.activateUpAttack = true;
    }
}
