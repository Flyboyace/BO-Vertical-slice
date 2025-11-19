using UnityEngine;

public class CoinPickup : MonoBehaviour
{
    [SerializeField] private int coins;
    void Start()
    {
        //UpdateCoinsUI();
    }

    private void AddCoins(int coin)
    {
        coins += coin;
        Debug.Log("Coin count updated: " + coin);
        //UpdateCoinsUI();
    }
}

 