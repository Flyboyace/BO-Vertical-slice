using UnityEngine;
using UnityEngine.SceneManagement;

public class CoinPickup : MonoBehaviour
{
    [SerializeField] private int coins;
    [SerializeField] private int greenStar;
    void Start()
    {
        
    }
    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Coin")
        {
            AddCoins();
            Destroy(collision.gameObject);
        }
        if (collision.gameObject.tag == "GreenStar")
        {
         //TBD   
        }
    }

    private void AddCoins()
    {
        coins += 1;
        Debug.Log("Coin count updated: " + coins);
    }
    private void GreenStar()
    { 
       //TBD 
    }
}

 