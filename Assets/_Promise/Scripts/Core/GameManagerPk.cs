using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManagerPk : MonoBehaviour
{
    public static GameManagerPk manager;
    [SerializeField] GameObject _pauseMenu;
    public static bool _isPaused=false;
   


    private void Update()
    {
        if (SceneManager.GetActiveScene().name == "MainMenu")
            return;
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (!_isPaused)
            {
                _pauseMenu.SetActive(true);
                
                _isPaused = true;
                FindAnyObjectByType<MenuController>().index = 0;

            }
            else
            {
                _pauseMenu.SetActive(false);
                _isPaused = false;
            }
        }
    }


    private void Awake()
    {
        if (manager == null)
        {
           // DontDestroyOnLoad(gameObject);
            manager = this;
            _isPaused = false;
        }
        else if(manager!=null)
        {
            Destroy(gameObject);
        }
      
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            FindAnyObjectByType<LevelChanger>().LoadCurrentLevel();
        }
    }

    public void ResumeGame()
    {
        if (_isPaused)
        {
            _pauseMenu.SetActive(false);
            _isPaused = false;
        }
    }

}
