using UnityEngine;

public class RN_SpinReward : MonoBehaviour
{
     public RN_SpinWheel spinWheel;

    private void Start()
    {
        spinWheel.OnSpinCompleted += OnSpinCompleted;
    }

    private void OnSpinCompleted(int rewardIndex)
    {
        // Handle the reward based on the rewardIndex
        Debug.Log("Reward index: " + rewardIndex);
    }

    public void StartSpinWheel()
    {
        spinWheel.StartSpin();
    }
}