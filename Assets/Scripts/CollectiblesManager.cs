using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

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

    private bool timerRunning = true;

    private void Start()
    {
        carBody = GetComponent<Rigidbody2D>();
        StartCoroutine(TimerCountdown());
    }

    private void Update()
    {
        fuel -= (Mathf.Clamp(Mathf.Abs(carBody.linearVelocity.x), 0, 0.8f) * Time.deltaTime * 2.5f) * 2;
        if (timeLeft <= 0)
        {
            timerRunning = false;
            Invoke(nameof(RestartLevel), 1f);
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
            fuel = 100f;
            audioManager.PlayOneShotRefuel();
            StartCoroutine(MakeObjectFloatAwayAndFadeOut(other.gameObject));
        }
        else if (other.CompareTag("Coin500")) // adds 5 seconds
        {
            timeLeft += 5;
            audioManager.PlayOneShotCoinSound(); 
            StartCoroutine(MakeObjectFloatAwayAndFadeOut(other.gameObject));
        }
        else if (other.CompareTag("Coin100")) // adds 5 seconds
        {
            timeLeft += 3;
            audioManager.PlayOneShotCoinSound(); 
            StartCoroutine(MakeObjectFloatAwayAndFadeOut(other.gameObject));
        }
        else if (other.CompareTag("Coin25"))
        {
            timeLeft += 2;
            audioManager.PlayOneShotCoinSound(); 
            StartCoroutine(MakeObjectFloatAwayAndFadeOut(other.gameObject));
        }
        else if (other.CompareTag("Coin5"))
        {
            timeLeft += 1;
            audioManager.PlayOneShotCoinSound(); 
            StartCoroutine(MakeObjectFloatAwayAndFadeOut(other.gameObject));
        }
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
}
