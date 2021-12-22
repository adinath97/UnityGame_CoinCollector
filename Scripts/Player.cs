using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] float moveSpeed = 1.25f;
    [SerializeField] Transform startingNode;

    private GridManager myGridManager;

    private float previousDistance = 1.05f, horizontal, speed;

    private bool facingLeft, playerDead, startMovement;

    private Transform tempTargetNode, currentNode, targetNode, previousNode, nextNode;
    private Vector2 direction, nextDirection, previousDirection, movement;

    private Animator anim;
    
    // Start is called before the first frame update
    void Start()
    {
       SetUp();
    }

    // Update is called once per frame
    void Update()
    {
        if(!startMovement) { return; }
        GetPlayerInput();
        Move();
        UpdateAnimation();
    }

    private void SetUp() {
       StartCoroutine(StartRoutine());
       myGridManager = GameObject.FindObjectOfType<GridManager>();
       anim = this.GetComponent<Animator>();
       direction = Vector2.down;
       nextDirection = direction;
       currentNode = startingNode;
       previousNode = currentNode;
       tempTargetNode = ChooseNextNode(ref nextDirection);
    }

    private IEnumerator StartRoutine() {
        yield return new WaitForSeconds(.2f);
        this.GetComponent<SpriteRenderer>().enabled = false;
        yield return new WaitForSeconds(.2f);
        this.GetComponent<SpriteRenderer>().enabled = true;
        yield return new WaitForSeconds(.2f);
        this.GetComponent<SpriteRenderer>().enabled = false;
        yield return new WaitForSeconds(.2f);
        this.GetComponent<SpriteRenderer>().enabled = true;
        yield return new WaitForSeconds(.2f);
        this.GetComponent<SpriteRenderer>().enabled = false;
        yield return new WaitForSeconds(.2f);
        this.GetComponent<SpriteRenderer>().enabled = true;
        startMovement = true;
        MoleMovement moleEnemy = GameObject.FindObjectOfType<MoleMovement>();
        moleEnemy.StartMovement();
        TreantMovement treantEnemy = GameObject.FindObjectOfType<TreantMovement>();
        treantEnemy.StartMovement();
    }

    public void PlayerDead() {
        anim.SetBool("PlayerDead",true);
        playerDead = true;
    }

    private void UpdateAnimation() {
        if(tempTargetNode == currentNode) {
            movement = Vector2.zero;
        } else if(direction == Vector2.right) {
            movement = new Vector2(1f,0);
            facingLeft = false;
        } else if(direction == Vector2.left) {
            movement = new Vector2(-1f,0);
            facingLeft = true;
        } else if(direction == Vector2.up) {
            movement = new Vector2(0,1f);
            facingLeft = false;
        } else if(direction == Vector2.down) {
            movement = new Vector2(0,-1f);
            facingLeft = false;
        }

        if(movement.x < -0.01f || facingLeft)
        {
            gameObject.transform.localScale = new Vector3(-1, 1, 1);
        } else
        {
            gameObject.transform.localScale = new Vector3(1, 1, 1);
        }

        anim.SetFloat("Horizontal",movement.x);
        anim.SetFloat("Vertical",movement.y);
        anim.SetFloat("Speed",movement.sqrMagnitude);

        if(Input.GetAxisRaw("Horizontal") == 1 || Input.GetAxisRaw("Horizontal") == -1 ||Input.GetAxisRaw("Vertical")== 1 || Input.GetAxisRaw("Vertical") == -1) {
            anim.SetFloat("LastHorizontal", Input.GetAxisRaw("Horizontal"));
            anim.SetFloat("LastVertical", Input.GetAxisRaw("Vertical"));
        }        
    }

    private void GetPlayerInput() {
        if(Input.GetKeyDown(KeyCode.UpArrow)) {
            nextDirection = Vector2.up;
        }
        else if(Input.GetKeyDown(KeyCode.RightArrow)) {
            nextDirection = Vector2.right;
        }
        else if(Input.GetKeyDown(KeyCode.LeftArrow)) {
            nextDirection = Vector2.left;
        }
        else if(Input.GetKeyDown(KeyCode.DownArrow)) {
            nextDirection = Vector2.down;
        }
    }

    private void Move() {
        if(playerDead) { return; }
        if(this.transform != tempTargetNode && tempTargetNode != null && !MissedTempTarget(ref previousDistance)) {
            transform.position += (Vector3)direction * moveSpeed * Time.deltaTime;
        }
        else if(this.transform == tempTargetNode || MissedTempTarget(ref previousDistance)) {
            previousNode = currentNode;
            currentNode = tempTargetNode;
            tempTargetNode = ChooseNextNode(ref nextDirection);
        }
    }

    private bool MissedTempTarget(ref float previousDistance) {
        if(tempTargetNode == null) {return true;}
        float currentDistance = Mathf.Sqrt(Mathf.Pow(transform.position.x - tempTargetNode.position.x,2) + Mathf.Pow(transform.position.y - tempTargetNode.position.y,2));
        if(previousDistance > currentDistance) {
            previousDistance = currentDistance;
            return false;
        }
        return true;
    }

    public Transform GetPlayerNode() {
        return tempTargetNode;
    }

    private Transform ChooseNextNode(ref Vector2 nextDirection) {
        Transform tempTargetNode1 = null;

        Transform[] neighborNodes = currentNode.GetComponent<Node>().GetNeighborNodes();
        Vector2[] directions = currentNode.GetComponent<Node>().GetDirections();

        if(nextDirection == Vector2.zero) {
            int currentDirectionIndex = 0;

            //see which direction nextDirection is
            for(int i = 0; i < 4; i++) {
                if(direction == directions[i]) {
                    currentDirectionIndex = i;
                }
            }
            //if not, check if current direction is available
            if(neighborNodes[currentDirectionIndex] != null) {
                tempTargetNode1 = neighborNodes[currentDirectionIndex];
                previousDistance = 1.05f;
                return tempTargetNode1;
            }
            else {
                return currentNode;
            }
        }

        else if(nextDirection != Vector2.zero) {
            int nextDirectionIndex = 0, currentDirectionIndex = 0;

            //see which direction nextDirection is
            for(int i = 0; i < 4; i++) {
                if(nextDirection == directions[i]) {
                    nextDirectionIndex = i;
                }
                if(direction == directions[i]) {
                    currentDirectionIndex = i;
                }
            }

            //check if nextDirection yields a valid node
            if(neighborNodes[nextDirectionIndex] != null) {
                //set new direction and reset nextDirection
                tempTargetNode1 = neighborNodes[nextDirectionIndex];
                direction = nextDirection;
                nextDirection = Vector2.zero;
                previousDistance = 1.05f;
                return tempTargetNode1;
            }
            //if not, check if current direction is available
            else if(neighborNodes[currentDirectionIndex] != null) {
                tempTargetNode1 = neighborNodes[currentDirectionIndex];
                previousDistance = 1.05f;
                return tempTargetNode1;
            }
            else {
                return currentNode;
            }
        } 

        return currentNode;   
    }
}
