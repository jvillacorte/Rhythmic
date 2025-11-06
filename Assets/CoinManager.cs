using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CoinManager : MonoBehaviour
{
    public int coinCount;
    public TextMeshProUGUI coinText;

    public GameObject door;
    public GameObject portal;
    private bool portalActivated;
    private bool doorDestroyed;
    void Start()
    {
        if (portal != null)
        {
            portal.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        coinText.text = "Coin Count: " + coinCount.ToString();

        if (coinCount == 3 && !doorDestroyed)
        {
            doorDestroyed = true;
            Destroy(door);
        }
        
        if (coinCount == 5 && !portalActivated)
        {
            portalActivated = true;
            if (portal != null)
            {
                portal.SetActive(true);
            }

        }
    }
}
