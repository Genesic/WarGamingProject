using UnityEngine;
using System.Collections;

public class GamePlayObject : MonoBehaviour {
    public GameObject player;
    public GameObject rival;
    
    public void startGame()
    {
        Debug.Log("in!!");
        player.SetActive(true);
        rival.SetActive(true);
    }
}
