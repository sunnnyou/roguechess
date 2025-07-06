using System.Collections;
using Assets.Scripts.Game;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneChange : MonoBehaviour
{
    public Button Button;
    public string SceneName;

    public void Start()
    {
        this.Button.onClick.AddListener(() =>
        {
            MusicManager.Instance.PlayClickSound();
            SceneManager.LoadScene(this.SceneName);
        });
    }
}
