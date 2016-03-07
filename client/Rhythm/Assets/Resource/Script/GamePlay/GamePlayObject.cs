using UnityEngine;
using System.Collections;

public class GamePlayObject : MonoBehaviour {
    public GameObject player;
    public GameObject rival;
    
    public void startGame()
    {
        player.SetActive(true);
        rival.SetActive(true);
    }
}
