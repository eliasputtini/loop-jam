using UnityEngine;
using UnityEngine.SceneManagement; // Necess�rio para carregar cenas

public class MainMenu : MonoBehaviour
{
    // Esse m�todo pode ser chamado, por exemplo, por um bot�o
    public void PlayLevel1()
    {
        SceneManager.LoadScene("Level1");
    }
}
