using UnityEngine;
using UnityEngine.SceneManagement;

namespace ModularUIRuntime
{
    public class ModularMenuActions : MonoBehaviour
    {
        [Header("Scene Management")]
        [SerializeField] private string gameSceneName = "GameScene";

        public void StartGame()
        {
            SceneManager.LoadScene(gameSceneName);
        }

        public void LoadSpecificScene(string sceneName)
        {
            SceneManager.LoadScene(sceneName);
        }

        public void OpenPanel(GameObject panelToOpen)
        {
            if (panelToOpen != null)
            {
                panelToOpen.SetActive(true);
            }
        }

        public void ClosePanel(GameObject panelToClose)
        {
            if (panelToClose != null)
            {
                panelToClose.SetActive(false);
            }
        }

        public void QuitGame()
        {
            #if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
            #else
                Application.Quit();
            #endif
        }

        public void OpenURL(string url)
        {
            Application.OpenURL(url);
        }
    }
}