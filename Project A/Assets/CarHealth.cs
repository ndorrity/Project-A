using UnityEngine;

public class CarHealth : MonoBehaviour
{
    [Header("Durability Settings")]
    public float maxHealth = 100f;
    public float currentHealth;

    [Header("Damage Calibration")]
    public float minImpactVelocity = 3.5f;
    public float damageMultiplier = 2.5f;

    private SimpleCarController controller;
    private bool isWrecked = false;

    private void Start()
    {
        currentHealth = maxHealth;
        controller = GetComponent<SimpleCarController>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (isWrecked) return;

        float impactForce = collision.relativeVelocity.magnitude;
        if (impactForce >= minImpactVelocity)
        {
            float damage = (impactForce - minImpactVelocity) * damageMultiplier;
            ApplyDamage(damage);
        }
    }

    public void ApplyDamage(float amount)
    {
        currentHealth = Mathf.Max(0f, currentHealth - amount);

        if (currentHealth <= 0f && !isWrecked)
        {
            TriggerWreck();
        }
    }

    private void TriggerWreck()
    {
        isWrecked = true;
        if (controller != null) controller.enabled = false;
        Debug.Log("Vehicle Totaled! Loss condition reached.");
    }

    private void OnGUI()
    {
        // HUD Durability Display
        GUI.Box(new Rect(20, 20, 180, 35), $"Durability: {Mathf.CeilToInt(currentHealth)} / {maxHealth}");

        if (isWrecked)
        {
            GUI.Box(new Rect(Screen.width / 2 - 120, Screen.height / 2 - 45, 240, 90), 
                "VEHICLE TOTALED!\n(Loss Condition)\n\nPress [R] to Restart");

            #if ENABLE_INPUT_SYSTEM
            if (UnityEngine.InputSystem.Keyboard.current != null && UnityEngine.InputSystem.Keyboard.current.rKey.wasPressedThisFrame)
            {
                RestartScene();
            }
            #else
            if (Input.GetKeyDown(KeyCode.R))
            {
                RestartScene();
            }
            #endif
        }
    }

    private void RestartScene()
    {
        Time.timeScale = 1f;
        UnityEngine.SceneManagement.SceneManager.LoadScene(
            UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex);
    }
}
