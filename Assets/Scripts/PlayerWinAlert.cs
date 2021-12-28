using System.Collections.Generic;
using UnityEngine;

public class PlayerWinAlert : MonoBehaviour
{
    private const string ReceiverMethodName = "OnGameOver";
    [SerializeField] private List<GameObject> receivers;
    [SerializeField] private List<GameObject> enemyTanks;

    private void Alert()
    {
        foreach (var receiver in receivers)
        {
            receiver.SendMessage(ReceiverMethodName, SendMessageOptions.DontRequireReceiver);
        }
    }

    private void Update()
    {
        enemyTanks.RemoveAll(enemyTank => enemyTank == null);
        
        if (enemyTanks.Count != 0)
        {
            return;
        }
        
        Alert();
        enabled = false;
    }
}
