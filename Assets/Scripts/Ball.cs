using UnityEngine;
using UnityEngine.EventSystems;

public enum BallColor
{
    White,
    Red,
    Yellow,
    Green,
    Brown,
    Blue,
    Pink,
    Black
}

public class Ball : MonoBehaviour, IPointerClickHandler
{
    [SerializeField]
    private int point;

    public int Point
    {
        get { return point; }
        set { point = value; }
    }

    [SerializeField]
    private MeshRenderer rd;

    [SerializeField]
    private BallColor color;

    public BallColor Color
    {
        get { return color; }
    }

    private void Awake()
    {
        rd = GetComponent<MeshRenderer>();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (GameManager.instance == null)
        {
            return;
        }

        GameManager.instance.AddScore(point);
        GameManager.instance.RemoveBall(color);

        Destroy(gameObject);
    }

    public void SetColorAndPoint(BallColor col)
    {
        color = col;

        switch (col)
        {
            case BallColor.White:
                point = 0;
                rd.material.color = UnityEngine.Color.white;
                break;

            case BallColor.Red:
                point = 1;
                rd.material.color = UnityEngine.Color.red;
                break;

            case BallColor.Yellow:
                point = 2;
                rd.material.color = UnityEngine.Color.yellow;
                break;

            case BallColor.Green:
                point = 3;
                rd.material.color = UnityEngine.Color.green;
                break;

            case BallColor.Brown:
                point = 4;
                rd.material.color = new UnityEngine.Color(0.59f, 0.29f, 0.1f);
                break;

            case BallColor.Blue:
                point = 5;
                rd.material.color = UnityEngine.Color.blue;
                break;

            case BallColor.Pink:
                point = 6;
                rd.material.color = new UnityEngine.Color(1f, 0.4f, 0.7f);
                break;

            case BallColor.Black:
                point = 7;
                rd.material.color = UnityEngine.Color.black;
                break;
        }
    }
}