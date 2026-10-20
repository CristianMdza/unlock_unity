using UnityEngine;
using UnityEngine.UI;

public class Unlock : MonoBehaviour
{
    [Header("Pieces")]
    public Transform fixedPiece;        // Static target piece
    public Transform rotatingPiece;     // Rotates to match


    [Header("Settings")]
    public float minAngle = -120f;
    public float maxAngle = 120f;
    public float tolerance = 4f;        // Degrees of allowed error

    [Header("UI")]
    
    public GameObject validation;       // "You Win!" label

    private float currentAngle;         // Current angle of rotating piece
    private float targetAngle;          // Final angle of fixed piece
    public bool IsAligned { get; private set; }

    private bool unlocked = false;

    void Awake()
    {
        // 1. Randomize BOTH pieces initially within the range
        float randFixed = Random.Range(minAngle, maxAngle + 1f);
        

        // Apply them immediately (before Start logic adjusts the fixed one)
        fixedPiece.localRotation = Quaternion.Euler(0f, 0f, randFixed);
        SetAngle(0);
    }

    void Start()
    {
        // 2. Initialize: Adjust FIXED piece to be NOT within tolerance of rotating piece
        // We keep the rotating piece where it is, and find a valid spot for the fixed one.
        const int maxAttempts = 100;
        int attempts = 0;
        float candidateAngle;

        do
        {
            candidateAngle = Random.Range(minAngle, maxAngle + 1f);
            attempts++;
            
            // If we fail too many times, force a position far away
            if (attempts > maxAttempts)
            {
                candidateAngle = currentAngle + (currentAngle >= 0 ? -maxAngle : maxAngle);
                break;
            }
        } while (Mathf.Abs(Mathf.DeltaAngle(candidateAngle, currentAngle)) <= tolerance);

        targetAngle = candidateAngle;
        fixedPiece.localRotation = Quaternion.Euler(0f, 0f, targetAngle);

        // Setup UI
        validation.SetActive(false);
       
        
        // Ensure visuals match current state
        UpdateVisuals();
    }

    /// <summary>
    /// External input entry point. 
    /// Expects a NORMALIZED value (0.0 to 1.0) mapped to minAngle..maxAngle.
    /// </summary>
    public void SetAngle(float normalizedValue)
    {
        // Clamp input to 0-1 range just in case
        normalizedValue = Mathf.Clamp01(normalizedValue);

        // Map 0.0 -> minAngle, 1.0 -> maxAngle
        currentAngle = Mathf.Lerp(maxAngle, minAngle, normalizedValue);
        
        UpdateVisuals();
    }

    private void UpdateVisuals()
    {
        // Rotate the piece
        rotatingPiece.localRotation = Quaternion.Euler(0f, 0f, currentAngle);

        // Check alignment
        float diff = Mathf.Abs(Mathf.DeltaAngle(currentAngle, targetAngle));
        IsAligned = diff <= tolerance;

        // UI Feedback
        //checkButton.interactable = IsAligned;
    }

    public void CheckSolution()
    {
        if (IsAligned)
        {
            //Debug.Log("Unlocked! You win!");
            validation.SetActive(true);
            unlocked = true;
            //checkButton.interactable = false;
        }
    }

    public bool isUnlocked()
    {
        return unlocked;
    }

    /// <summary>
    /// Helper to get Z rotation in degrees (-180..180)
    /// </summary>
    float GetZRotation(Transform t)
    {
        float z = t.localEulerAngles.z;
        if (z > 180f) z -= 360f;
        return z;
    }
}