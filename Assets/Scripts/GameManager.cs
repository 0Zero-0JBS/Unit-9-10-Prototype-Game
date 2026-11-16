using UnityEngine;

public class GameManager : MonoBehaviour
{
    //The Tetrominos are those colourful blocks that come in different shapes/arrangements.
    //Tetrominos and Grid code.
    public GameObject[] Tetrominos;
    public float movementFrequency = 0.8f;
    private float passedTime = 0;
    private GameObject currentTetromino;

    //Touchscreen controls code.
    private Vector2 startPos;
    public float swipeLimit = 50f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        SpawnTetromino();
    }

    // Update is called once per frame
    void Update()
    {
        passedTime += Time.deltaTime;
        if (passedTime >= movementFrequency)
        {
            passedTime -= movementFrequency;
            MoveTetromino(Vector3.down);
        }

        void UserInput()
        {
            if (Input.touchCount == 0) return;

            Touch touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Began)
            {
                startPos = touch.position;
            }
            else if (touch.phase == TouchPhase.Ended)
            {
                Vector2 swipe = touch.position - startPos;

                //Tap = rotate Tetrominos by 90 degrees.
                if (swipe.magnitude < swipeLimit)
                {
                    currentTetromino.transform.Rotate(0, 0, -90);
                }
                else if (Mathf.Abs(swipe.x) > Mathf.Abs(swipe.y))
                {
                    if (swipe.x > 0)
                    {
                        //Swipe right = move Tetrominos right by how much swiped.
                        MoveTetromino(Vector3.right);
                    }
                    else
                    {
                        //Swipe left = move Tetrominos left by how much swiped.
                        MoveTetromino(Vector3.left);
                    }
                }
                else if (swipe.y < 0)
                {
                    //Swipe down = move Tetrominos down by how much swiped.
                    MoveTetromino(Vector3.down);
                }
            }
        }
    }

    void SpawnTetromino()
    {
        int index = Random.Range(0, Tetrominos.Length);
        currentTetromino = Instantiate(Tetrominos[index], new Vector3(1.5f, 5.5f, 0), Quaternion.identity);
    }

    void MoveTetromino(Vector3 direction)
    {
        currentTetromino.transform.position += direction;
        if(!IsValidPosition())
        {
            currentTetromino.transform.position -= direction;
            if(direction == Vector3.down)
            {
                //GetComponent<GridScript>().UpdateGrid(currentTetromino.transform);
                CheckForLines();
                SpawnTetromino();
            }
        }
    }

    bool IsValidPosition()
    {
        return true;
    }

    void CheckForLines()
    {
        
    }
}
