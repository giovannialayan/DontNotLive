using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class levelSelect : MonoBehaviour
{
    public void goToBruno()
    {
        SceneManager.LoadScene("FirstBoss");
    }

    public void goToFrank()
    {
        SceneManager.LoadScene("FrankBoss");
    }

    public void goToDominic()
    {
        SceneManager.LoadScene("DominicBoss");
    }
}
