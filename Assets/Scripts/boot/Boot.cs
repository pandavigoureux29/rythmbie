using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Boot : MonoBehaviour
{
    public void Play()
    {
        SceneManager.LoadScene("GameplayScene");
    }
}
