using UnityEngine;

public class Hole : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        Ball b = other.GetComponent<Ball>();

        if (b == null)
        {
            return;
        }

        if (b.Point == 0)
        {
            GameManager.instance.GameOver();
            return;
        }

        GameManager.instance.AddScore(b.Point);
        GameManager.instance.RemoveBall(b.Color);

        Destroy(b.gameObject);
    }
}