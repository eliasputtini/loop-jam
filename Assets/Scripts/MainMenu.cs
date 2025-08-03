using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    // Método para iniciar o Level1
    public void PlayLevel1()
    {
        SceneManager.LoadScene("Level1");
    }

    // Método para fechar o jogo
    public void QuitGame()
    {
        #if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
        #else
                    Application.Quit();
        #endif
    }
}

