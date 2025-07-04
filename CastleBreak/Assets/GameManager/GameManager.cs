using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{

    public float timer = 300f;
    public string nextScene = "video scene";

    void Start()
    {

    }

    void Update()
    {
        timer -= Time.deltaTime;

        if (timer < 0)
        {
            SceneManager.LoadScene(nextScene);
        }
    }
}
