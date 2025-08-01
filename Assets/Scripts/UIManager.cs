using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// This class is responsible for managing the UI elements of the game.
/// </summary>
public class UIManager : MonoBehaviour
{
    public Image fuelImageProgressBar;
    public Gradient fuelGradient;
    public Image lowFuelWarning;
    public Image pauseButtonBg;
    public Text distanceCounter;
    public Text boostCounter;
    public Text rpmCounter;
    public Text fuelCounter;

    public CollectiblesManager collectiblesManager;
    private Vector3 _startPosition; 
    public Button sfxButton;
    public Sprite sfxButtonOn;
    public Sprite sfxButtonOff;
    public AudioManager audioManager;
    public Image coinSprite;
    private bool _a = true;

    public Text timerText;

    // New UI element for score
    public Text scoreCounter;

    // Parameters for score text effect
    public Color normalScoreColor = Color.white;
    public Color highVelocityColor = Color.yellow;
    public float normalFontSize = 24f;
    public float highVelocityFontSize = 36f;

    // Added private field to accumulate score
    private float score = 0f;
    private Vector3 _lastPosition;

    public void UpdateTimerUI(float timeLeft)
    {
        timerText.text = Mathf.CeilToInt(timeLeft).ToString(); // round up
    }

    private void Start()
    {
        _startPosition = collectiblesManager.carBody.transform.position;
        _lastPosition = _startPosition;
        score = 0f;
    }

    /// <summary>
    /// This method is responsible for updating the all the UI elements.
    /// </summary>
    /// <returns>
    /// void
    /// </returns>
    private void Update()
    {
        CheckLowFuelWarning();

        UpdateFuelGUI();

        UpdateRpmCounter();
        UpdateBoostCounter();

        UpdateScore();
    }

    private void UpdateBoostCounter()
    {
        boostCounter.text = Mathf.Abs(collectiblesManager.carBody.linearVelocity.magnitude * .5f).ToString("F0");
    }

    private void UpdateRpmCounter()
    {
        var distance = collectiblesManager.carBody.transform.position - _startPosition;

        distanceCounter.text = distance.magnitude.ToString("F0") + "m";

        rpmCounter.text = Mathf.Abs(collectiblesManager.carBody.linearVelocity.magnitude * 3).ToString("F0");
    }

    private void CheckLowFuelWarning()
    {
        if (collectiblesManager.fuel < 20)
        {
            lowFuelWarning.enabled = true;
            lowFuelWarning.color = new Color(lowFuelWarning.color.r,
                lowFuelWarning.color.g, lowFuelWarning.color.b, Mathf.PingPong(Time.time, 0.5f));
        }
        else
        {
            lowFuelWarning.enabled = false;
        }
    }

    // --- Updated to accumulate score over time ---
    private void UpdateScore()
    {
        Vector3 currentPos = collectiblesManager.carBody.transform.position;
        Vector3 frameDistanceVector = currentPos - _lastPosition;
        float frameDistance = frameDistanceVector.magnitude;

        float velocity = collectiblesManager.carBody.linearVelocity.magnitude;

        // Add only the incremental distance traveled this frame times velocity
        score += frameDistance * velocity;

        scoreCounter.text = "Score: " + Mathf.RoundToInt(score).ToString();

        if (velocity > 10f)  // adjust threshold as needed
        {
            scoreCounter.color = highVelocityColor;
            scoreCounter.fontSize = (int)highVelocityFontSize;
        }
        else
        {
            scoreCounter.color = normalScoreColor;
            scoreCounter.fontSize = (int)normalFontSize;
        }

        _lastPosition = currentPos;
    }

    /// <summary>
    /// This method is responsible for restarting the level. Called when the player runs out of fuel.
    /// </summary>
    /// <returns>
    /// void
    /// </returns>
    public static void RestartLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        Time.timeScale = 1;
    }

    /// <summary>
    /// This method is responsible for pausing the game. It toggles the Time.timeScale between 0 and 1. Called when the pause button is clicked.
    /// </summary>
    /// <returns>
    /// void
    /// </returns>
    public void PauseGame()
    {
        if (_a)
        {
            Time.timeScale = 0;
            pauseButtonBg.color = new Color(1f, 0, 0, 1f);
            _a = false;
        }
        else
        {
            Time.timeScale = 1;
            pauseButtonBg.color = new Color(1, 1f, 1, 1f);
            _a = true;
        }
    }

    private void UpdateFuelGUI()
    {
        fuelImageProgressBar.fillAmount = collectiblesManager.fuel / 100f;

        fuelImageProgressBar.color = fuelGradient.Evaluate(collectiblesManager.fuel / 100f);

        fuelCounter.text = collectiblesManager.fuel.ToString("F0") + "%";

        fuelCounter.color = fuelGradient.Evaluate(collectiblesManager.fuel / 100f);
    }

    public void ToggleSfx()
    {
        sfxButton.image.sprite = sfxButton.image.sprite == sfxButtonOn ? sfxButtonOff : sfxButtonOn;
        audioManager.ToggleAllSfx();
    }
}
