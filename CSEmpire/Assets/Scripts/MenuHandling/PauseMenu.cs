using UnityEngine;
using UnityEngine.SceneManagement;
namespace MenuHandling
{
    ///<author> Salma </author>
    /// <summary>
    /// Handling the different states of the game when it paused or resumed
    /// </summary>
    public class PauseMenu : MonoBehaviour
    {

        public GameObject pauseMenu;
        public static bool IsPaused;
    
        // Start is called before the first frame update
        void Start()
        {
            pauseMenu.SetActive(false);
        }

        // Update is called once per frame
        void Update()
        {
            if (!Input.GetKeyDown(KeyCode.Escape)) return;
            if (IsPaused)
            {
                ResumeGame();
            }

            else
            {
                PauseGame();
            }
        }
        

        /// <summary>
        /// Pause the game (when the clicks on the button)
        /// Remark: it calls the Update method.
        /// </summary>
        public void PauseGame()
        {
            pauseMenu.SetActive(true);
            Time.timeScale = 0f;
            IsPaused = true;
        }
        

        /// <summary>
        /// Resume the game (when the clicks on the button)
        /// Remark: it calls the Update method.
        /// </summary>
        public void ResumeGame()
        {
            pauseMenu.SetActive(false);
            Time.timeScale = 1f;
            IsPaused = false;
        }
        
        /// <summary>
        /// Go to the main menu in the game (when the clicks on the button)
        /// </summary>

        public void GoToMainMenu()
        {
            Time.timeScale = 1f;
            SceneManager.LoadScene("MenuScene");
        }

        /// <summary>
        /// Quit the game (when the clicks on the button)
        /// </summary>
        public void QuitGame()
        {
            Application.Quit();
        }
        
        
    }
}
