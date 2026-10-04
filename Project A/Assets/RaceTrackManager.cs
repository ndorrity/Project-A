using UnityEngine;

public class RaceTrackManager : MonoBehaviour
{
    public static RaceTrackManager Instance;

    [Header("Race Settings")]
    public int totalCheckpoints = 3;
    public int currentCheckpointIndex = 0;
    public int lapsToWin = 1;
    public int currentLap = 0;

    private bool raceWon = false;
    private float raceTime = 0f;

    void Awake()
    {
        Instance = this;
    }

    void Update()
    {
        if (!raceWon && Time.timeScale > 0)
        {
            raceTime += Time.deltaTime;
        }
    }

    public void CheckpointHit(int checkpointNumber)
    {
        // Must hit checkpoints sequentially to prevent backwards driving/cheating
        if (checkpointNumber == currentCheckpointIndex + 1)
        {
            currentCheckpointIndex = checkpointNumber;
            Debug.Log($"Passed Checkpoint {currentCheckpointIndex} / {totalCheckpoints}");
        }
    }

    public void FinishLineCrossed()
    {
        // Only count lap if all checkpoints were hit in order
        if (currentCheckpointIndex == totalCheckpoints)
        {
            currentLap++;
            currentCheckpointIndex = 0;

            if (currentLap >= lapsToWin)
            {
                TriggerWin();
            }
        }
    }

    private void TriggerWin()
    {
        raceWon = true;
        Time.timeScale = 0f;
    }

    void OnGUI()
    {
        // HUD Overlay
        GUI.Box(new Rect(Screen.width - 200, 20, 180, 55), 
            $"Lap: {currentLap}/{lapsToWin}\nCheckpoint: {currentCheckpointIndex}/{totalCheckpoints}\nTime: {raceTime:F1}s");

        if (raceWon)
        {
            GUI.Box(new Rect(Screen.width / 2 - 125, Screen.height / 2 - 60, 250, 120), 
                $"VICTORY!\n\nFinal Time: {raceTime:F2} seconds\n\nPress [R] to Play Again");

            #if ENABLE_INPUT_SYSTEM
            if (UnityEngine.InputSystem.Keyboard.current != null && UnityEngine.InputSystem.Keyboard.current.rKey.wasPressedThisFrame)
            {
                RestartRace();
            }
            #else
            if (Input.GetKeyDown(KeyCode.R))
            {
                RestartRace();
            }
            #endif
        }
    }

    public void RestartRace()
    {
        Time.timeScale = 1f;
        UnityEngine.SceneManagement.SceneManager.LoadScene(
            UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex);
    }
}
