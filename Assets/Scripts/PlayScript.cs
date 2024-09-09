using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayScript : MonoBehaviour
{
    public void playBtn()
    {
        SceneManager.LoadScene("GameScene2");
    }
    public void playAgainBtn()
    {
        SceneManager.LoadScene("HomeScene");
    }
}
