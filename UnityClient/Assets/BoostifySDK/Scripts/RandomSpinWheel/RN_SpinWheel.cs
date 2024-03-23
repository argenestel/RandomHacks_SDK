using System;
using System.Collections;
using Playsphere.RandomSDK.U64;
using UnityEngine;


public class RN_SpinWheel : MonoBehaviour
{
    public int numberOfRewards = 8;
    public float spinDuration = 3f;
    public float slowDownDuration = 1f;
    public float maxSpeed = 500f;
    public float startAngle = 0f;
    private float currentAngle;
    private float targetAngle;
    private float spinStartTime;
    private bool isSpinning;
    ulong val;
    ulong maxValue;
    public RNMoveU64ClientController rNMoveU64ClientController;
    public event Action<int> OnSpinCompleted;

    private void Start()
    {
        currentAngle = startAngle;
    }
    
  /// <summary>
  /// This function is called when the object becomes enabled and active.
  /// </summary>
  void OnEnable()
  {
      u64_ResourcesConverter.RNRandom += OnRNRandom;
  }
    void OnDisable()
  {
      u64_ResourcesConverter.RNRandom -= OnRNRandom;
  }

    private void OnRNRandom(ulong value0, ulong maxValue)
    {
        this.val = value0;
        this.maxValue = maxValue;

        spinStartTime = Time.time;
        targetAngle = currentAngle + 360f * (float)val/maxValue * (5f) ;//UnityEngine.Random.Range(2f, 5f);

        print((float)val/maxValue);
        isSpinning = true;

        StartCoroutine(SpinCoroutine());
    }

    public void StartSpin()
    {
        if (isSpinning)
            return;

        rNMoveU64ClientController.CallRandomNumber();
        
    }

    private IEnumerator SpinCoroutine()
    {
        while (isSpinning)
        {
            float elapsedTime = Time.time - spinStartTime;
            float t = Mathf.Clamp01(elapsedTime / spinDuration);
            float slowDownt = Mathf.Clamp01((elapsedTime - (spinDuration - slowDownDuration)) / slowDownDuration);

            float speed = Mathf.Lerp(maxSpeed, 0f, slowDownt);
            float deltaAngle = speed * Time.deltaTime;

            currentAngle += deltaAngle;

            if (t >= 1f)
            {
                isSpinning = false;
                currentAngle = targetAngle;

                int rewardIndex = Mathf.FloorToInt(currentAngle / (360f / numberOfRewards)) % numberOfRewards;
                OnSpinCompleted?.Invoke(rewardIndex);
            }

            transform.rotation = Quaternion.Euler(0f, 0f, -currentAngle);

            yield return null;
        }
    }
}