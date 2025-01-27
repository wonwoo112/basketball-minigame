using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class BallBehaviour : MonoBehaviour
{
    [SerializeField] public float initUpperBound;
    [SerializeField] public float initBallSpeed;
    [SerializeField] public float minDribbleHeight;
    private bool isDribbling;
    private float upperBound;
    private float ballSpeed;
    private bool goingUp;
    private float lowerBound;
    private bool leftDribble;
    private bool rightDribble;
    private Direction holdingHand;
    private float xSpeed;
    private const float leftPos = -1.0f;
    private const float rightPos = 1.0f;
    private const float acrossDist = rightPos - leftPos;
    enum Direction {
        left, right
    }
    private Direction dribblingDirection;
    bool canDribble() {
        return goingUp && minDribbleHeight < transform.position.y && initUpperBound > transform.position.y;
    }
    void keyHandler() {
        if (Input.GetKeyDown(KeyCode.A) && canDribble()) {
            leftDribble = true;
            rightDribble = false;
            dribblingDirection = Direction.left;
        }
        else if (Input.GetKeyDown(KeyCode.D) && canDribble()) {
            leftDribble = false;
            rightDribble = true;
            dribblingDirection = Direction.right;
        }
        else {
            leftDribble = false;
            rightDribble = false;
        }
    }
    float targetX(Direction d) {
        switch (d) {
            case Direction.right: return rightPos;
            case Direction.left: return leftPos;
            default: return 0.0f;
        }
    }
    float getNewX() {
        // function for getting the new x position of the ball
        if (isDribbling) {
            float oldX = transform.position.x, newX;
            if (holdingHand == dribblingDirection) {
                newX = targetX(dribblingDirection);
            }
            else {
                newX = oldX + xSpeed * Time.deltaTime;
            }
            return newX;
        }
        else {
            return transform.position.x;
        }
    }
    float getNewY() {
        // function for getting the new y position of the ball 
        if (isDribbling) {
            float oldY = transform.position.y, newY;
            if (goingUp) {
                newY = oldY + ballSpeed * Time.deltaTime;
                
                if (newY > initUpperBound) {
                    newY = initUpperBound;
                    isDribbling = false;
                }
            }
            else {
                newY = oldY - ballSpeed * Time.deltaTime;
                if (newY <= lowerBound) {
                    newY = lowerBound;
                }   
            }
            return newY;
        }
        else {
            return initUpperBound;
        }
        
    }
    void updateState(float newY) {
        // function for updating the state of the ball 
        if (!isDribbling) return;
        if (goingUp) {    
            if (leftDribble) {
                upperBound = newY;
                if (holdingHand == Direction.right) {
                    xSpeed = -acrossDist / ((upperBound - lowerBound) / ballSpeed);
                }
                goingUp = false;
            }
            if (rightDribble) {
                upperBound = newY;
                if (holdingHand == Direction.left) {
                    xSpeed = acrossDist / ((upperBound - lowerBound) / ballSpeed);
                }
                goingUp = false;
            }
        }
        else if (newY <= lowerBound) {
            goingUp = true;
            xSpeed = 0.0f;
            holdingHand = dribblingDirection;
        }
    }
    void Start()
    {
        lowerBound = transform.localScale.y / 2;
        transform.position = new Vector3(0.0f, lowerBound, 0.0f);
        goingUp = true;
        ballSpeed = initBallSpeed;
        upperBound = initUpperBound;
        isDribbling = true;
        xSpeed = 0.0f;
    }

    // Update is called once per frame
    void Update()
    {
        keyHandler();
        float newY = getNewY();
        updateState(newY);
        float newX = getNewX();
        transform.position = new Vector3(newX, newY, 0.0f);
    }
}
