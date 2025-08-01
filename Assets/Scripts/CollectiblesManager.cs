using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro; // Add this for TextMeshPro

/// <summary>
/// This class is in charge of managing the collectibles in the game, including fuel, coins and their respective animations when collected.
/// </summary>
/// <remarks>
/// - We get fuel by overlapping with it.
/// - We lose fuel by moving.
/// </remarks>
/// <value>
/// fuel: The fuel value.
/// coins: The coins value.
/// </value>
public class CollectiblesManager : MonoBehaviour
{
    public float fuel = 100f;
    public float Fuel => fuel;
    public float timeLeft = 10f; // start with 10 seconds
    public Rigidbody2D carBody;
    public AudioManager audioManager;
    public UIManager uiManager;

    [Header("Text Effect Settings")]
    public GameObject textPrefab; // Assign a TextMeshPro prefab in the inspector
    public Canvas worldCanvas; // Assign your world space canvas

    [Header("Game Over UI")]
    public GameObject gameOverUI;

    private bool timerRunning = true;
    private HashSet<GameObject> collectedItems = new HashSet<GameObject>(); // Track collected items

    private void Start()
    {
        carBody = GetComponent<Rigidbody2D>();
        StartCoroutine(TimerCountdown());
    }

    private void Update()
    {
        fuel -= (Mathf.Clamp(Mathf.Abs(carBody.linearVelocity.x), 0, 0.8f) * Time.deltaTime * 2.5f) * 2;
        if (timeLeft <= 0 && timerRunning)
        {
            timerRunning = false;
            EnterSlowMotion();
            uiManager.SetGameOver(); // Informa ao UIManager que o jogo acabou
        }

        uiManager.UpdateTimerUI(timeLeft); // assumes you have a method for this
    }

    private IEnumerator TimerCountdown()
    {
        while (timerRunning)
        {
            yield return new WaitForSeconds(1f);
            timeLeft--;
        }
    }

    private void RestartLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Fuel"))
        {
            other.enabled = false; // Disable collider immediately
            fuel = 100f;
            audioManager.PlayOneShotRefuel();
            StartCoroutine(MakeObjectFloatAwayAndFadeOut(other.gameObject));
        }
        else if (other.CompareTag("Coin500")) // adds 5 seconds
        {
            other.enabled = false; // Disable collider immediately
            timeLeft += 5;
            audioManager.PlayOneShotCoinSound();
            SpawnFloatingText("+5s", other.transform.position);
            StartCoroutine(MakeObjectFloatAwayAndFadeOut(other.gameObject));
        }
        else if (other.CompareTag("Coin100")) // adds 3 seconds
        {
            other.enabled = false; // Disable collider immediately
            timeLeft += 3;
            audioManager.PlayOneShotCoinSound();
            SpawnFloatingText("+3s", other.transform.position);
            StartCoroutine(MakeObjectFloatAwayAndFadeOut(other.gameObject));
        }
        else if (other.CompareTag("Coin25"))
        {
            other.enabled = false; // Disable collider immediately
            timeLeft += 2;
            audioManager.PlayOneShotCoinSound();
            SpawnFloatingText("+2s", other.transform.position);
            StartCoroutine(MakeObjectFloatAwayAndFadeOut(other.gameObject));
        }
        else if (other.CompareTag("Coin5"))
        {
            other.enabled = false; // Disable collider immediately
            timeLeft += 1;
            audioManager.PlayOneShotCoinSound();
            SpawnFloatingText("+1s", other.transform.position);
            StartCoroutine(MakeObjectFloatAwayAndFadeOut(other.gameObject));
        }
    }

    private void SpawnFloatingText(string text, Vector3 position)
    {
        if (textPrefab == null || worldCanvas == null) return;

        GameObject textObj = Instantiate(textPrefab, worldCanvas.transform);
        textObj.transform.position = position;

        TextMeshProUGUI textComponent = textObj.GetComponent<TextMeshProUGUI>();
        if (textComponent != null)
        {
            textComponent.text = text;
        }

        StartCoroutine(FloatingTextEffect(textObj));
    }

    private static IEnumerator FloatingTextEffect(GameObject textObj)
    {
        const float duration = 1.5f;
        float elapsedTime = 0f;
        Vector3 startPosition = textObj.transform.position;
        Vector3 endPosition = startPosition + Vector3.up * 2f; // Float up by 2 units

        TextMeshProUGUI textComponent = textObj.GetComponent<TextMeshProUGUI>();
        Color originalColor = textComponent.color;

        while (elapsedTime < duration)
        {
            float progress = elapsedTime / duration;

            // Move up with easing
            textObj.transform.position = Vector3.Lerp(startPosition, endPosition, progress);

            // Fade out
            Color color = originalColor;
            color.a = Mathf.Lerp(1f, 0f, progress);
            textComponent.color = color;

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        Destroy(textObj);
    }

    private static IEnumerator MakeObjectFloatAwayAndFadeOut(GameObject obj)
    {
        const float duration = 1f;
        float elapsedTime = 0f;
        var startPosition = obj.transform.position;
        var endPosition = new Vector3(obj.transform.position.x, 5, obj.transform.position.z);
        var spriteRenderer = obj.GetComponent<SpriteRenderer>();

        while (elapsedTime < duration)
        {
            var newY = Mathf.Lerp(startPosition.y, endPosition.y, elapsedTime / duration);
            obj.transform.position = new Vector3(obj.transform.position.x, newY, obj.transform.position.z);
            var color = spriteRenderer.color;
            color.a = Mathf.Lerp(1f, 0f, elapsedTime / duration);
            spriteRenderer.color = color;
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        Destroy(obj, 1f);
    }
    private void EnterSlowMotion()
    {
        Time.timeScale = 0.1f;
        Time.fixedDeltaTime = 0.02f * Time.timeScale;

        StartCoroutine(ShowGameOverUIAfterDelay(3f));
    }

    private IEnumerator ShowGameOverUIAfterDelay(float delay)
    {
        yield return new WaitForSecondsRealtime(delay);

        Time.timeScale = 0f; // Pausa o jogo completamente depois da transição

        if (gameOverUI != null)
        {
            gameOverUI.SetActive(true); // Ativa o Canvas ou painel de Game Over
        }
    }

}