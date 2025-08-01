using UnityEngine;
using UnityEngine.SceneManagement; // Necessário para carregar cenas

public class MainMenu : MonoBehaviour
{
    // Esse método pode ser chamado, por exemplo, por um botão
    public void PlayLevel1()
    {
        SceneManager.LoadScene("Level1");
    }
}
