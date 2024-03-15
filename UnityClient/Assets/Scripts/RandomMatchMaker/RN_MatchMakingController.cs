using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;

public class RN_MatchmakingController : MonoBehaviour
{
    public List<string> waitingPlayers = new List<string>();
    public ulong maxranVal;
    public ulong ranVal;
    /// <summary>
    /// This function is called when the object becomes enabled and active.
    /// </summary>
    void OnEnable()
    {
        u64_ResourcesConverter.RNRandom += OnRNRandom;
    }

    private void OnRNRandom(ulong val, ulong maxVal)
    {
        ranVal = val;
        maxranVal = maxVal;
    }

    void OnDisable()
    {
        u64_ResourcesConverter.RNRandom -= OnRNRandom;
    }


    public void AddPlayerToMatchmaking(string player)
    {
        waitingPlayers.Add(player);
        // TryMatchPlayers();
    }

    private void TryMatchPlayers()
    {
        if (waitingPlayers.Count >= 2)
        {
            // int randomIndex1 = Random.Range(0, waitingPlayers.Count);
            int randomIndex1 = (int)((float)ranVal/maxranVal * waitingPlayers.Count);
            int randomIndex2 = (int)((float)ranVal/maxranVal * waitingPlayers.Count);
            string player1 = waitingPlayers[randomIndex1];
            waitingPlayers.RemoveAt(randomIndex1);

            // int randomIndex2 = Random.Range(0, waitingPlayers.Count);
            string player2 = waitingPlayers[randomIndex2];
            waitingPlayers.RemoveAt(randomIndex2);

            // Create a new match or assign players to an existing match
            CreateMatch(player1, player2);
        }
    }
     public void MatchAllPlayers()
    {
        while (waitingPlayers.Count >= 2)
        {            int randomIndex1 = (int)((float)ranVal/maxranVal * waitingPlayers.Count);
            int randomIndex2 = (int)((float)ranVal/maxranVal * waitingPlayers.Count);
            // int randomIndex1 = Random.Range(0, waitingPlayers.Count);
            string player1 = waitingPlayers[randomIndex1];
            waitingPlayers.RemoveAt(randomIndex1);

            // int randomIndex2 = Random.Range(0, waitingPlayers.Count);
            string player2 = waitingPlayers[randomIndex2];
            waitingPlayers.RemoveAt(randomIndex2);

            CreateMatch(player1, player2);
        }
    }

    private void CreateMatch(string player1, string player2)
    {
        // Create a new match object or assign players to an existing match
        // You can customize this part based on your game's requirements
        Debug.Log("Matched players: " + player1 + " and " + player2);
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(RN_MatchmakingController))]
public class RN_MatchmakingControllerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        RN_MatchmakingController controller = (RN_MatchmakingController)target;

        if (GUILayout.Button("Match All Players"))
        {
            controller.MatchAllPlayers();
        }

        if (GUILayout.Button("Create Random Player"))
        {
            string randomPlayer = "Player_" + Random.Range(1, 100);
            controller.AddPlayerToMatchmaking(randomPlayer);
        }

        if(GUILayout.Button("Create 10 Players")){
            for(int i = 0; i < 10; i++){
            string randomPlayer = "Player_" + Random.Range(1, 100);
            controller.AddPlayerToMatchmaking(randomPlayer);
        }
        }
    }
}
#endif