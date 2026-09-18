using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class PlayerController : MonoBehaviour
{
    private float elapsedTime = 0f;
    private float score = 0f;

    public float scoreMultiplier = 10f;
    public float thrustForce = 1f;

    private Rigidbody2D rb;

    public UIDocument uiDocument;
    private Label scoreText;
    private Label highScoreText;
    private Label topScoresText;
    private Button restartButton;

    public GameObject explosionEffect;

    private int highScore = 0;
    private int[] topScores = new int[3];

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        scoreText = uiDocument.rootVisualElement.Q<Label>("ScoreLabel");
        highScoreText = uiDocument.rootVisualElement.Q<Label>("HighScoreLabel");
        topScoresText = uiDocument.rootVisualElement.Q<Label>("TopScoresLabel");
        restartButton = uiDocument.rootVisualElement.Q<Button>("RestartButton");

        // Load the saved Top 3 scores
        for (int i = 0; i < 3; i++)
        {
            topScores[i] = PlayerPrefs.GetInt("TopScore" + i, 0);
        }

        // Bring old high score into the Top 3 system
        int oldHighScore = PlayerPrefs.GetInt("HighScore", 0);

        if (oldHighScore > topScores[0])
        {
            topScores[2] = topScores[1];
            topScores[1] = topScores[0];
            topScores[0] = oldHighScore;

            SaveTopScores();
        }

        highScore = topScores[0];

        UpdateScoreDisplay();

        restartButton.style.display = DisplayStyle.None;
        restartButton.clicked += ReloadScene;
    }

    void SaveTopScores()
    {
        for (int i = 0; i < 3; i++)
        {
            PlayerPrefs.SetInt("TopScore" + i, topScores[i]);
        }

        PlayerPrefs.Save();
    }

    void UpdateScoreDisplay()
    {
        highScoreText.text = "High Score: " + highScore;

        if (topScoresText != null)
        {
            topScoresText.text =
                "TOP 3 SCORES\n" +
                "1. " + topScores[0] + "\n" +
                "2. " + topScores[1] + "\n" +
                "3. " + topScores[2];
        }
    }

    void Update()
    {
        elapsedTime += Time.deltaTime;

        score = Mathf.FloorToInt(elapsedTime * scoreMultiplier);

        scoreText.text = "Score: " + score;

        if (Mouse.current.leftButton.isPressed)
        {
            // Calculate mouse direction
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(
                Mouse.current.position.value
            );

            Vector2 direction =
                (mousePos - transform.position).normalized;

            // Move player in direction of mouse
            transform.up = direction;
            rb.AddForce(direction * thrustForce);
        }
    }

    void AddScoreToLeaderboard()
    {
        int finalScore = Mathf.FloorToInt(score);

        // New #1 score
        if (finalScore > topScores[0])
        {
            topScores[2] = topScores[1];
            topScores[1] = topScores[0];
            topScores[0] = finalScore;
        }

        // New #2 score
        else if (finalScore < topScores[0] && finalScore > topScores[1])
        {
            topScores[2] = topScores[1];
            topScores[1] = finalScore;
        }

        // New #3 score
        else if (finalScore < topScores[1] && finalScore > topScores[2])
        {
            topScores[2] = finalScore;
        }

        // Update the high score
        highScore = topScores[0];

        // Save the Top 3
        SaveTopScores();

        // Update the screen
        UpdateScoreDisplay();
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        AddScoreToLeaderboard();

        Instantiate(
            explosionEffect,
            transform.position,
            transform.rotation
        );

        Destroy(gameObject);

        restartButton.style.display = DisplayStyle.Flex;
    }

    void ReloadScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}