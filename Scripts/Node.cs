using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node : MonoBehaviour
{
    [SerializeField] Transform[] neighboringNodes = new Transform[4] {null,null,null,null};

    private Vector2[] directions = new Vector2[] {Vector2.up, Vector2.right, Vector2.down, Vector2.left};

    //node order: top, right, down, left (clockwise)

    public Transform[] GetNeighborNodes() {
        return neighboringNodes;
    }

    public Vector2[] GetDirections() {
        return directions;
    }
}
