using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CoinManager : MonoBehaviour
{
    public int coinCount;
    public TextMeshProUGUI coinText;

    public GameObject door;
    private bool doorDestroyed;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        coinText.text = "Coin Count: " + coinCount.ToString();

        if(coinCount == 3 && !doorDestroyed)
        {
            doorDestroyed = true;
            Destroy(door);
        }
    }
}
