using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    [SerializeField]
    private int playerScore;

    public int PlayerScore
    {
        get { return playerScore; }
        set { playerScore = value; }
    }

    [SerializeField]
    private GameObject[] ballPositions;

    [SerializeField]
    private GameObject ballPrefab;

    [SerializeField]
    private GameObject cueball;

    public GameObject CueBall
    {
        get { return cueball; }
    }

    [SerializeField]
    private GameObject ballLine;

    [SerializeField]
    private GameObject cam;

    [SerializeField]
    private float xInput = 0f;

    [SerializeField]
    private UnityEngine.UI.Slider powerSlider;

    [SerializeField]
    private float maxPower = 50f;

    [SerializeField]
    private float chargeSpeed = 1f;

    [SerializeField]
    private float rotationSpeed = 100f;

    [SerializeField]
    private TMP_Text guiScore;

    public TMP_Text GuiScore
    {
        get { return guiScore; }
        set { guiScore = value; }
    }

    [SerializeField]
    private GameObject gameOverPanel;

    [SerializeField]
    private TMP_Text gameOverScore;

    private float currentPower = 0f;
    private bool charging = false;
    private bool powerIncreasing = true;
    private bool gameOver = false;

    private List<BallColor> remainingBalls = new List<BallColor>();

    public static GameManager instance;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        Time.timeScale = 1f;

        gameOver = false;
        charging = false;

        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(false);
        }

        if (powerSlider != null)
        {
            powerSlider.gameObject.SetActive(false);
        }

        if (Settings.fromSave && PlayerPrefs.GetInt("HasSaveGame", 0) == 1)
        {
            LoadGame();
        }
        else
        {
            StartNewGame();
        }

        UpdateScore();
    }

    private void Update()
    {
        if (Keyboard.current == null)
        {
            return;
        }

        if (Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            if (cueball != null)
            {
                Rigidbody rb = cueball.GetComponent<Rigidbody>();

                if (rb != null && rb.linearVelocity.magnitude >= 0.1f)
                {
                    return;
                }
            }

            if (gameOver)
            {
                ReturnToTitle();
            }
            else
            {
                SaveAndReturnToTitle();
            }

            return;
        }

        if (gameOver)
        {
            return;
        }

        if (Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            StartCharging();
        }

        if (Keyboard.current.spaceKey.isPressed && charging)
        {
            ChargePower();
        }

        if (Keyboard.current.spaceKey.wasReleasedThisFrame && charging)
        {
            ShootBall();
        }

        if (Keyboard.current.rKey.wasPressedThisFrame)
        {
            StopBall();
        }

        if (Keyboard.current.leftArrowKey.isPressed ||
            Keyboard.current.aKey.isPressed)
        {
            xInput = -1f;
        }
        else if (Keyboard.current.rightArrowKey.isPressed ||
                 Keyboard.current.dKey.isPressed)
        {
            xInput = 1f;
        }
        else
        {
            xInput = 0f;
        }

        RotateBall();
    }

    private void StartNewGame()
    {
        playerScore = 0;

        remainingBalls.Clear();

        remainingBalls.Add(BallColor.Red);
        remainingBalls.Add(BallColor.Yellow);
        remainingBalls.Add(BallColor.Green);
        remainingBalls.Add(BallColor.Brown);
        remainingBalls.Add(BallColor.Blue);
        remainingBalls.Add(BallColor.Pink);
        remainingBalls.Add(BallColor.Black);

        SpawnBalls();
        ResetCueBall();

        PlayerPrefs.DeleteKey("HasSaveGame");
        PlayerPrefs.Save();
    }

    private void SpawnBalls()
    {
        foreach (BallColor color in remainingBalls)
        {
            int index = GetBallPositionIndex(color);

            if (index < 0 || index >= ballPositions.Length)
            {
                continue;
            }

            GameObject obj = Instantiate(
                ballPrefab,
                ballPositions[index].transform.position,
                Quaternion.identity
            );

            Ball ball = obj.GetComponent<Ball>();

            if (ball != null)
            {
                ball.SetColorAndPoint(color);
            }
        }
    }

    private int GetBallPositionIndex(BallColor color)
    {
        switch (color)
        {
            case BallColor.Red:
                return 1;

            case BallColor.Yellow:
                return 2;

            case BallColor.Green:
                return 3;

            case BallColor.Brown:
                return 4;

            case BallColor.Blue:
                return 5;

            case BallColor.Pink:
                return 6;

            case BallColor.Black:
                return 7;
        }

        return -1;
    }

    private void StartCharging()
    {
        if (cueball == null)
        {
            return;
        }

        Rigidbody rb = cueball.GetComponent<Rigidbody>();

        if (rb == null)
        {
            return;
        }

        if (rb.linearVelocity.magnitude >= 0.1f)
        {
            return;
        }

        charging = true;
        currentPower = 0f;
        powerIncreasing = true;

        if (powerSlider != null)
        {
            powerSlider.value = 0f;
            powerSlider.gameObject.SetActive(true);
        }
    }

    private void ChargePower()
    {
        if (powerIncreasing)
        {
            currentPower += chargeSpeed * Time.deltaTime;

            if (currentPower >= 1f)
            {
                currentPower = 1f;
                powerIncreasing = false;
            }
        }
        else
        {
            currentPower -= chargeSpeed * Time.deltaTime;

            if (currentPower <= 0f)
            {
                currentPower = 0f;
                powerIncreasing = true;
            }
        }

        if (powerSlider != null)
        {
            powerSlider.value = currentPower;
        }
    }

    private void ShootBall()
    {
        if (cueball == null)
        {
            return;
        }

        Rigidbody rb = cueball.GetComponent<Rigidbody>();

        if (rb == null)
        {
            return;
        }

        if (rb.linearVelocity.magnitude >= 0.1f)
        {
            charging = false;

            if (powerSlider != null)
            {
                powerSlider.gameObject.SetActive(false);
            }

            return;
        }

        float shootPower = maxPower * currentPower;

        if (ballLine != null)
        {
            ballLine.SetActive(false);
        }

        if (cam != null)
        {
            cam.transform.SetParent(null);
        }

        rb.AddForce(
            cueball.transform.forward * shootPower,
            ForceMode.Impulse
        );

        charging = false;

        if (powerSlider != null)
        {
            powerSlider.gameObject.SetActive(false);
        }

        if (cam != null)
        {
            cam.transform.position = new Vector3(
                0f,
                30f,
                -42f
            );

            cam.transform.eulerAngles = new Vector3(
                45f,
                0f,
                0f
            );
        }
    }

    private void RotateBall()
    {
        if (cueball == null || charging)
        {
            return;
        }

        Rigidbody rb = cueball.GetComponent<Rigidbody>();

        if (rb == null)
        {
            return;
        }

        if (rb.linearVelocity.magnitude < 0.1f)
        {
            cueball.transform.Rotate(
                0f,
                xInput * rotationSpeed * Time.deltaTime,
                0f
            );
        }
    }

    private void StopBall()
    {
        if (cueball == null)
        {
            return;
        }

        Rigidbody rb = cueball.GetComponent<Rigidbody>();

        if (rb == null)
        {
            return;
        }

        charging = false;

        if (powerSlider != null)
        {
            powerSlider.gameObject.SetActive(false);
        }

        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        Vector3 currentPosition = cueball.transform.position;
        float currentRotation = cueball.transform.eulerAngles.y;

        cueball.transform.position = currentPosition;

        cueball.transform.eulerAngles = new Vector3(
            0f,
            currentRotation,
            0f
        );

        Physics.SyncTransforms();

        CameraBehindBall();

        if (ballLine != null)
        {
            ballLine.SetActive(true);
        }
    }

    private void CameraBehindBall()
    {
        if (cueball == null || cam == null)
        {
            return;
        }

        cam.transform.SetParent(cueball.transform);

        cam.transform.localPosition = new Vector3(
            0f,
            7f,
            -15f
        );

        cam.transform.localEulerAngles = new Vector3(
            30f,
            0f,
            0f
        );
    }

    public void AddScore(int score)
    {
        PlayerScore += score;
        UpdateScore();
    }

    public void RemoveBall(BallColor color)
    {
        remainingBalls.Remove(color);
    }

    public void UpdateScore()
    {
        if (GuiScore != null)
        {
            GuiScore.text = "Score: " + PlayerScore.ToString();
        }
    }

    public void GameOver()
    {
        if (gameOver)
        {
            return;
        }

        gameOver = true;
        charging = false;

        if (powerSlider != null)
        {
            powerSlider.gameObject.SetActive(false);
        }

        if (ballLine != null)
        {
            ballLine.SetActive(false);
        }

        if (gameOverScore != null)
        {
            gameOverScore.text =
                "Score: " + PlayerScore.ToString();
        }

        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
        }

        PlayerPrefs.DeleteKey("HasSaveGame");
        PlayerPrefs.Save();

        Time.timeScale = 0f;
    }

    public void ReturnToTitle()
    {
        Time.timeScale = 1f;

        PlayerPrefs.DeleteKey("HasSaveGame");
        PlayerPrefs.Save();

        SceneManager.LoadScene("Title");
    }

    public void SaveAndReturnToTitle()
    {
        if (gameOver)
        {
            ReturnToTitle();
            return;
        }

        SaveGame();

        Time.timeScale = 1f;

        SceneManager.LoadScene("Title");
    }

    public void SaveGame()
    {
        if (gameOver)
        {
            return;
        }

        if (cueball != null)
        {
            Rigidbody cueRb = cueball.GetComponent<Rigidbody>();

            if (cueRb != null)
            {
                cueRb.linearVelocity = Vector3.zero;
                cueRb.angularVelocity = Vector3.zero;
            }

            PlayerPrefs.SetFloat(
                "cueBallPosX",
                cueball.transform.position.x
            );

            PlayerPrefs.SetFloat(
                "cueBallPosY",
                cueball.transform.position.y
            );

            PlayerPrefs.SetFloat(
                "cueBallPosZ",
                cueball.transform.position.z
            );

            PlayerPrefs.SetFloat(
                "cueBallRotY",
                cueball.transform.eulerAngles.y
            );
        }

        Ball[] balls = FindObjectsByType<Ball>(
            FindObjectsInactive.Exclude,
            FindObjectsSortMode.None
        );

        foreach (BallColor color in System.Enum.GetValues(typeof(BallColor)))
        {
            PlayerPrefs.SetInt(
                "Ball_" + color,
                remainingBalls.Contains(color) ? 1 : 0
            );
        }

        foreach (Ball ball in balls)
        {
            if (ball == null)
            {
                continue;
            }

            Rigidbody ballRb = ball.GetComponent<Rigidbody>();

            if (ballRb != null)
            {
                ballRb.linearVelocity = Vector3.zero;
                ballRb.angularVelocity = Vector3.zero;
            }

          string key = "BallPos_" + ball.Color;

            PlayerPrefs.SetFloat(
                key + "_X",
                ball.transform.position.x
            );

            PlayerPrefs.SetFloat(
                key + "_Y",
                ball.transform.position.y
            );

            PlayerPrefs.SetFloat(
                key + "_Z",
                ball.transform.position.z
            );

            PlayerPrefs.SetFloat(
                key + "_RotX",
                ball.transform.eulerAngles.x
            );

            PlayerPrefs.SetFloat(
                key + "_RotY",
                ball.transform.eulerAngles.y
            );

            PlayerPrefs.SetFloat(
                key + "_RotZ",
                ball.transform.eulerAngles.z
            );
        }

        PlayerPrefs.SetInt(
            "PlayerScore",
            PlayerScore
        );

        PlayerPrefs.SetInt(
            "HasSaveGame",
            1
        );

        PlayerPrefs.Save();
    }

    public void LoadGame()
    {
        remainingBalls.Clear();

        foreach (BallColor color in System.Enum.GetValues(typeof(BallColor)))
        {
            if (PlayerPrefs.GetInt(
                "Ball_" + color,
                0
            ) == 1)
            {
                remainingBalls.Add(color);
            }
        }

        PlayerScore = PlayerPrefs.GetInt(
            "PlayerScore",
            0
        );

        SpawnBalls();

        if (cueball != null)
        {
            float x = PlayerPrefs.GetFloat(
                "cueBallPosX",
                0f
            );

            float y = PlayerPrefs.GetFloat(
                "cueBallPosY",
                0.95f
            );

            float z = PlayerPrefs.GetFloat(
                "cueBallPosZ",
                -25f
            );

            float rotY = PlayerPrefs.GetFloat(
                "cueBallRotY",
                0f
            );

            Rigidbody rb = cueball.GetComponent<Rigidbody>();

            if (rb != null)
            {
                rb.linearVelocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
            }

            cueball.transform.position = new Vector3(
                x,
                y,
                z
            );

            cueball.transform.eulerAngles = new Vector3(
                0f,
                rotY,
                0f
            );
        }

        Ball[] balls = FindObjectsByType<Ball>(
            FindObjectsInactive.Exclude,
            FindObjectsSortMode.None
        );

        foreach (Ball ball in balls)
        {
            if (ball == null)
            {
                continue;
            }

            string key = "BallPos_" + ball.Color;

            float ballX = PlayerPrefs.GetFloat(
                key + "_X",
                ball.transform.position.x
            );

            float ballY = PlayerPrefs.GetFloat(
                key + "_Y",
                ball.transform.position.y
            );

            float ballZ = PlayerPrefs.GetFloat(
                key + "_Z",
                ball.transform.position.z
            );

            float ballRotX = PlayerPrefs.GetFloat(
                key + "_RotX",
                ball.transform.eulerAngles.x
            );

            float ballRotY = PlayerPrefs.GetFloat(
                key + "_RotY",
                ball.transform.eulerAngles.y
            );

            float ballRotZ = PlayerPrefs.GetFloat(
                key + "_RotZ",
                ball.transform.eulerAngles.z
            );

            Rigidbody ballRb = ball.GetComponent<Rigidbody>();

            if (ballRb != null)
            {
                ballRb.linearVelocity = Vector3.zero;
                ballRb.angularVelocity = Vector3.zero;
            }

            ball.transform.position = new Vector3(
                ballX,
                ballY,
                ballZ
            );

            ball.transform.eulerAngles = new Vector3(
                ballRotX,
                ballRotY,
                ballRotZ
            );
        }

        Physics.SyncTransforms();

        CameraBehindBall();

        if (ballLine != null)
        {
            ballLine.SetActive(true);
        }
    }

    public void ResetCueBall()
    {
        if (cueball == null)
        {
            return;
        }

        Rigidbody rb = cueball.GetComponent<Rigidbody>();

        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }

        cueball.transform.position = new Vector3(
            0f,
            0.95f,
            -25f
        );

        cueball.transform.eulerAngles = new Vector3(
            0f,
            0f,
            0f
        );

        Physics.SyncTransforms();

        CameraBehindBall();

        if (ballLine != null)
        {
            ballLine.SetActive(true);
        }
    }
}