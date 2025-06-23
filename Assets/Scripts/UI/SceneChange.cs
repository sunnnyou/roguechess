using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneChange : MonoBehaviour
{
    public Button Button;
    public string SceneName;

    public void Start()
    {
        this.Button.onClick.AddListener(() => SceneManager.LoadScene(this.SceneName));
    }
}
