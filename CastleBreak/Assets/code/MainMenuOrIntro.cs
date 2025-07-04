using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuOrIntro : MonoBehaviour
{

    public string newGameScene;
    public string startGameScene;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void ToIntro()
    {
        SceneManager.LoadScene(newGameScene);
    }

    public void StartGame()
    {
        SceneManager.LoadScene(startGameScene);
    }
}
