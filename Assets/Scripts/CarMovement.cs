using UnityEngine;

/// <summary>
/// This class is in charge of moving the player's car.
/// </summary>
public class CarMovement : MonoBehaviour
{
    [SerializeField] private Rigidbody2D frontWheel;
    [SerializeField] private Rigidbody2D backWheel;
    [SerializeField] private Rigidbody2D carBody;
    [Range(0, 1000)]
    [SerializeField] private float speed = 150f;
    [Range(0, 1000)]
    [SerializeField] private float rotationSpeed = 500f;

    public AudioSource carEngine;
    public AudioSource goofyCarHorn;

    private float _jumpTimer;
    private const float JumpDelay = 1.5f;
    private float _moveInput;

    private CollectiblesManager collectiblesManager;

    private void Start()
    {
        // Play the car engine sound and loop it.
        carEngine.Play();
        carEngine.loop = true;

        // Find and cache the reference to the CollectiblesManager
        // Encontrar a primeira instância de CollectiblesManager
        collectiblesManager = Object.FindFirstObjectByType<CollectiblesManager>();


    }

    /// <summary>
    /// Handles the input for movement, horn, and jumping.
    /// </summary>
    private void Update()
    {
        _moveInput = Input.GetAxis("Horizontal");

        carEngine.pitch = Mathf.Abs(_moveInput); // Adjust pitch only by input magnitude

        _jumpTimer -= Time.deltaTime;

        if (Input.GetKeyDown(KeyCode.H))
        {
            goofyCarHorn.Play();
        }

        if (Input.GetKeyDown(KeyCode.Space) && _jumpTimer <= 0 && collectiblesManager.Fuel > 0f)
        {
            // Jump
            carBody.AddForce(Vector2.up * 500f);
            _jumpTimer = JumpDelay;
        }

        // Optionally stop engine sound if out of fuel
        if (collectiblesManager.Fuel <= 0f && carEngine.isPlaying)
        {
            carEngine.Stop();
        }
        else if (collectiblesManager.Fuel > 0f && !carEngine.isPlaying)
        {
            carEngine.Play();
        }
    }

    /// <summary>
    /// Applies torque to simulate car movement and rotation, only if there is fuel.
    /// </summary>
    private void FixedUpdate()
    {
        if (collectiblesManager == null || collectiblesManager.Fuel <= 0f)
            return;

        frontWheel.AddTorque(-_moveInput * speed * Time.fixedDeltaTime);
        backWheel.AddTorque(-_moveInput * speed * Time.fixedDeltaTime);
        carBody.AddTorque(-_moveInput * rotationSpeed * Time.fixedDeltaTime);
    }
}
