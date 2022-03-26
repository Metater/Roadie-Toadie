using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerManager : MonoBehaviour
{
    public enum MoveDirection
    {
        Right,
        Left,
        Up,
        Down
    }

    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private float velocityCurveCoefficient;
    [SerializeField] public Vector2Int boundX;
    [SerializeField] public Vector2Int boundY;
    [SerializeField] private GameObject spriteGO;
    [SerializeField] private int maxQueuedMoves;
    [SerializeField] private Vector2Int startingPos;
    [SerializeField] private float cameraLerp;
    [SerializeField] private List<Sprite> toadSprites = new List<Sprite>();
    [SerializeField] private float minimumTouchLength;
    [SerializeField] private Text debugText;

    //private float time = 0;

    private Vector2Int lastPos;
    private bool moving = false;
    private Vector2Int targetPos;
    private Queue<(int, int)> queuedMoves = new Queue<(int, int)>();
    private MoveDirection lastQueuedMoveDirection = MoveDirection.Up;

    private SpriteRenderer spriteRenderer;

    private Vector2 touchStartPos;
    //private float touchStartTime;

    private void Start()
    {
        spriteRenderer = spriteGO.GetComponent<SpriteRenderer>();
        SetPos(startingPos);
        lastPos = startingPos;
        targetPos = startingPos;
        Camera.main.transform.position = new Vector3(startingPos.x + 0.5f, startingPos.y + 0.5f, Camera.main.transform.position.z);
    }

    private void Update()
    {
        if (!GameManager.Instance.Loaded) return;
        //time += Time.deltaTime;
        TryEnqueueMove();
        Move();
    }

    private void FixedUpdate()
    {
        float y = Mathf.Lerp(targetPos.y + 0.5f, Camera.main.transform.position.y, cameraLerp);
        Camera.main.transform.position = new Vector3(Camera.main.transform.position.x, y, Camera.main.transform.position.z);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Enemy") Destroy(gameObject);
    }

    private void TryEnqueueMove()
    {
        if (queuedMoves.Count > maxQueuedMoves) return;

        if (Input.touchSupported)
        {
            if (Input.touchCount != 1) return;

            Touch touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Began)
            {
                touchStartPos = touch.position;
                //touchStartTime = time;
            }
            else if (touch.phase == TouchPhase.Ended)
            {
                float touchLength = Vector2.Distance(touch.position, touchStartPos);
                Vector2 touchDiff = touch.position - touchStartPos;
                if (touchLength < minimumTouchLength) // time - touchStartTime < limit
                {
                    MoveInDirection(lastQueuedMoveDirection);
                }
                else
                {
                    MoveDirection moveDirection = MoveDirection.Up;
                    float greatestMagnitude = float.MinValue;
                    float xMag = Mathf.Abs(touchDiff.x);
                    float yMag = Mathf.Abs(touchDiff.y);

                    if (xMag > greatestMagnitude)
                    {
                        greatestMagnitude = xMag;
                        if (touchDiff.x > 0)
                            moveDirection = MoveDirection.Right;
                        else
                            moveDirection = MoveDirection.Left;
                    }
                    if (yMag > greatestMagnitude)
                    {
                        if (touchDiff.y > 0)
                            moveDirection = MoveDirection.Up;
                        else
                            moveDirection = MoveDirection.Down;
                    }
                    MoveInDirection(moveDirection);
                }
            }
        }
        else
        {
            if (Input.GetKeyDown(KeyCode.Space))
                MoveInDirection(lastQueuedMoveDirection);
            else if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
                MoveInDirection(MoveDirection.Up);
            else if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
                MoveInDirection(MoveDirection.Down);
            else if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
                MoveInDirection(MoveDirection.Left);
            else if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
                MoveInDirection(MoveDirection.Right );
        }
    }

    private void MoveInDirection(MoveDirection moveDirection)
    {
        lastQueuedMoveDirection = moveDirection;
        switch (moveDirection)
        {
            case MoveDirection.Up:
                queuedMoves.Enqueue((0, 1));
                break;
            case MoveDirection.Down:
                queuedMoves.Enqueue((0, -1));
                break;
            case MoveDirection.Left:
                queuedMoves.Enqueue((-1, 0));
                break;
            case MoveDirection.Right:
                queuedMoves.Enqueue((1, 0));
                break;
        }
    }

    private void Move()
    {
        if (!moving)
        {
            rb.velocity = Vector2.zero;
            SetPos(GetTargetPos());
            if (queuedMoves.Count > 0)
            {
                (int, int) move = queuedMoves.Dequeue();
                ChangePos(move.Item1, move.Item2);
            }
            return;
        }

        float dist = GetDistance();
        if (dist > 0.025f)
        {
            SetVelTowardsTarget();

            Vector2Int diff = targetPos - lastPos;

            if (diff.x == 1)
                spriteGO.transform.localEulerAngles = new Vector3(0, 0, 270);
            else if (diff.x == -1)
                spriteGO.transform.localEulerAngles = new Vector3(0, 0, 90);
            else if (diff.y == 1)
                spriteGO.transform.localEulerAngles = new Vector3(0, 0, 0);
            else
                spriteGO.transform.localEulerAngles = new Vector3(0, 0, 180);

            float total = 1;
            int maxIndex = toadSprites.Count - 1;
            for (int i = maxIndex; i >= 0; i--)
            {
                if (total > dist) spriteRenderer.sprite = toadSprites[maxIndex - i];
                total -= 1f / (float)toadSprites.Count;
            }
        }
        else
        {
            moving = false;
            lastPos = targetPos;
            spriteRenderer.sprite = toadSprites[0];
        }
    }

    private void SetPos(Vector2 pos)
    {
        gameObject.transform.position = pos;
    }

    private void ChangePos(int x, int y)
    {
        Vector2Int previousTargetPos = targetPos;
        targetPos += new Vector2Int(x, y);
        if (targetPos.x > boundX.x && targetPos.x < boundX.y && targetPos.y > boundY.x && targetPos.y < boundY.y)
        {
            SetVelTowardsTarget();
            moving = true;
        }
        else targetPos = previousTargetPos;
    }

    private void SetVelTowardsTarget()
    {
        Vector2 pos = rb.transform.position;
        Vector2 tar = GetTargetPos();
        float rotation = Mathf.Atan2(pos.y - tar.y, pos.x - tar.x) * 180 / Mathf.PI;
        Quaternion movementVector = Quaternion.Euler(0, 0, rotation + 90);
        rb.velocity = movementVector * Vector2.up * GetVelocity();
    }

    private float GetVelocity()
    {
        return velocityCurveCoefficient * Mathf.Log10(GetDistance() + 0.1f) + velocityCurveCoefficient;
    }
    private Vector2 GetTargetPos()
    {
        return new Vector3(targetPos.x, targetPos.y);
    }
    private float GetDistance()
    {
        return Vector3.Distance(rb.transform.position, GetTargetPos());
    }
    private bool PassedTarget()
    {
        Vector2 diff = lastPos - targetPos;
        Vector2 check = new Vector2(rb.transform.position.x, rb.transform.position.y) - GetTargetPos();
        if (Mathf.Abs(diff.x) > 0.5)
        {
            Debug.Log("H" + Mathf.Sign(diff.x) + ":" + Mathf.Sign(check.x));
            if (Mathf.Sign(diff.x) != Mathf.Sign(check.x)) return false;
        }
        else
        {
            Debug.Log("V" + Mathf.Sign(diff.y) + ":" + Mathf.Sign(check.y));
            if (Mathf.Sign(diff.y) != Mathf.Sign(check.y)) return false;
        }
        return true;
    }
}
