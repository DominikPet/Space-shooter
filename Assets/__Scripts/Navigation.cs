using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Navigation : MonoBehaviour
{
    public TextMeshProUGUI highScoreText;
    public void StartGame()
    {
        SceneManager.LoadScene("__Scene_0");

    }
    private void Start()
    {

            if (PlayerPrefs.HasKey("hiScore"))
            {
                highScoreText.text = "High score:\n" + PlayerPrefs.GetInt("hiScore");
            }
    }
}
