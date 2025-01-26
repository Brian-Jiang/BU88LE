using UnityEngine;

public class MoveTest : MonoBehaviour
{
    public float speed = 2f;
    public float distance = 3f;
    
    private Vector3 startPosition;
    private bool movingRight = true;

    void Start()
    {
        startPosition = transform.position;
    }

    void Update()
    {
        float move = speed * Time.deltaTime;
        
        if (movingRight)
            transform.position += new Vector3(move, 0, 0);
        else
            transform.position -= new Vector3(move, 0, 0);

        if (Vector3.Distance(startPosition, transform.position) >= distance)
            movingRight = !movingRight;
    }
}

