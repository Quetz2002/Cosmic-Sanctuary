using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    // I use this list to store the rewards the player collects in the planets
    public List<RewardItem> collectedRewards = new List<RewardItem>();

    private void Awake()
    {
        // I am making sure only one GameManager exists across all scenes
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void AddReward(RewardItem newReward)
    {
        // I add the item to the persistent list when found on a planet
        if (!collectedRewards.Contains(newReward))
        {
            collectedRewards.Add(newReward);
        }
    }
}