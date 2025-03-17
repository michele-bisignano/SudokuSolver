using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonsScript : MonoBehaviour
{
    // Method to reload the resolver scene
    public void PlayAgain()
    {
        SceneManager.LoadScene("SResolver");
    }

    // Method to load the home scene
    public void Exit()
    {
        SceneManager.LoadScene("HomeScene");
    }
}
