using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class StartScene : MonoBehaviour
{
    private Button start;
    private Button exit;
    void Start()
    {
        start = transform.Find("Start").GetComponent<Button>();
        exit = transform.Find("Exit").GetComponent<Button>();
        start.onClick.AddListener(() =>
        {
            SceneManager.LoadScene("MainScene");
        });
        exit.onClick.AddListener(()=>
        {
            Application.Quit();
        });
    }
}
