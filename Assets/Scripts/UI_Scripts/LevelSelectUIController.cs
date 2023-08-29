using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelSelectUIController : MonoBehaviour
{
    [SerializeField] private GameObject SceneManager;
    private ButtonManager buttonManager = new ButtonManager();

    // Start is called before the first frame update
    void Start()
    {
        buttonManager.InitializeCanvasButtons(transform.gameObject);
        buttonManager.GetButton("TestLevel").onClick.AddListener(TestLevelOnClick);
        buttonManager.GetButton("EqualLevel").onClick.AddListener(EqualLevelOnClick);
    }

    private void TestLevelOnClick()
    {
        SceneManager.GetComponent<SceneLoader>().LoadScene("TestLevel");
    }

    private void EqualLevelOnClick()
    {
        SceneManager.GetComponent<SceneLoader>().LoadScene("EqualLevel");
    }
}
