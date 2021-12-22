using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoleMovement : MonoBehaviour
{

    [SerializeField] float moveSpeed = 1f;

    [SerializeField] AudioClip playerDeathSFX;

    private AudioSource myAudioSource;

    private GameObject player;
    private GridManager myGridManager;
    private GameManager myGameManager;

    private bool startMovement, differentPosition;

    private List<Transform> nodesThatWork = new List<Transform>();

    private List<int> oneDimensionalCoordinates = new List<int>() {0,8};

    private Transform tempTargetNode, currentNode, targetNode, previousNode, nextNode;
    private Vector2 direction, nextDirection, previousDirection, movement;

    private float horizontal, speed;

    private Animator anim;

    private float previousDistance = 1.05f;

    private void Awake() {
        ObtainPosition();
    }

    // Start is called before the first frame update
    void Start()
    {
        SetUp();
    }

    // Update is called once per frame
    void Update()
    {
        if(!startMovement || !differentPosition) {return;}
        Move();
        UpdateAnimation();
    }

    private void SetUp() {
        direction = Vector2.left;
        myGameManager = GameObject.FindObjectOfType<GameManager>();
        anim = this.GetComponent<Animator>();
        player = GameObject.FindGameObjectWithTag("Player");
        targetNode = GetNodeClosestToPlayer();
        //Debug.Log(targetNode.gameObject.name);
        //Debug.Log(currentNode.gameObject.name);
        tempTargetNode = ChooseNextNode();
        previousNode = currentNode;
        myAudioSource = this.GetComponent<AudioSource>();
    }

    private void ObtainPosition() {
        this.GetComponent<SpriteRenderer>().enabled = false;
        GameObject treant = GameObject.Find("Treant");
        myGridManager = GameObject.FindObjectOfType<GridManager>();
        Transform testPosition = myGridManager.GetNodeAtPosition(oneDimensionalCoordinates[Random.Range(0,oneDimensionalCoordinates.Count)],oneDimensionalCoordinates[Random.Range(0,oneDimensionalCoordinates.Count)]);
        do {
            testPosition = myGridManager.GetNodeAtPosition(oneDimensionalCoordinates[Random.Range(0,oneDimensionalCoordinates.Count)],oneDimensionalCoordinates[Random.Range(0,oneDimensionalCoordinates.Count)]);
        } while(treant.transform.position == testPosition.position);
        currentNode = testPosition;
        transform.position = currentNode.position;
        differentPosition = true;
        this.GetComponent<SpriteRenderer>().enabled = true;
        Debug.Log(this.GetComponent<SpriteRenderer>().enabled);
        treant.GetComponent<TreantMovement>().PositionObtained();
    }

    public void StartMovement() {
        startMovement = true;
    }

    private void UpdateAnimation() {
        if(tempTargetNode == currentNode) {
            movement = Vector2.zero;
        } else if(direction == Vector2.right) {
            movement = new Vector2(1f,0);
        } else if(direction == Vector2.left) {
            movement = new Vector2(-1f,0);
        } else if(direction == Vector2.up) {
            movement = new Vector2(0,1f);
        } else if(direction == Vector2.down) {
            movement = new Vector2(0,-1f);
        }

        horizontal = movement.x > 0.01f ? movement.x : movement.x < -0.01f ? 1 : 0;
        speed = movement.y > 0.01f ? movement.y : movement.y < -0.01f ? 1 : 0;

        if(movement.x < -0.01f)
        {
            gameObject.transform.localScale = new Vector3(-1, 1, 1);
        } else
        {
            gameObject.transform.localScale = new Vector3(1, 1, 1);
        }

        anim.SetFloat("Horizontal", horizontal);
        anim.SetFloat("Vertical", movement.y);
        anim.SetFloat("Speed", speed);
    }

    private Transform GetNodeClosestToPlayer() {
        int x = 0, y = 0;

        x = Mathf.RoundToInt(4f + player.transform.position.x);
        y = Mathf.RoundToInt(4f + player.transform.position.y);
        
        Transform targetNode1 = myGridManager.GetNodeAtPosition(x,y);

        return targetNode1;
    }

    private void Move() {
        if(this.transform != tempTargetNode && tempTargetNode != null && !MissedTempTarget(ref previousDistance)) {
            transform.position += (Vector3)direction * moveSpeed * Time.deltaTime;
        }
        else if(this.transform == tempTargetNode || MissedTempTarget(ref previousDistance)) {
            previousDistance = 1.05f;
            targetNode = player.GetComponent<Player>().GetPlayerNode();
            previousNode = currentNode;
            currentNode = tempTargetNode;
            tempTargetNode = ChooseNextNode();
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

    private Transform ChooseNextNode() {
        Transform tempTargetNode1 = null;

        nodesThatWork.Clear();
        Transform[] neighborNodes = currentNode.GetComponent<Node>().GetNeighborNodes();
        Vector2[] directions = currentNode.GetComponent<Node>().GetDirections();
        

        float maxDistance = 1000f;

        for(int i = 0; i < 4; i++) {
            if(neighborNodes[i] != null && directions[i] != -1 * direction) {
                float distanceFromTarget = Mathf.Sqrt(Mathf.Pow(neighborNodes[i].position.x - targetNode.position.x,2) + Mathf.Pow(neighborNodes[i].position.y - targetNode.position.y,2));
                if(distanceFromTarget <= maxDistance) {
                    nodesThatWork.Add(neighborNodes[i]);
                    maxDistance = distanceFromTarget;
                    tempTargetNode1 = neighborNodes[i];
                    nextDirection = directions[i];
                }
            }
        }

        direction = nextDirection;
        return tempTargetNode1;     
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if(other.gameObject.tag == "Player") {
            myAudioSource.PlayOneShot(playerDeathSFX);
            myGameManager.RemoveALife();
        }
    }
}
