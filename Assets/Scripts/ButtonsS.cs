using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class ButtonsS : MonoBehaviour
{
    public void PlayAgain()
    {
        SceneManager.LoadScene("SResolver");
    }

    public void Exit()
    {
        SceneManager.LoadScene("HomeScene");
    }

}
