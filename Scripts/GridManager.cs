using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridManager : MonoBehaviour
{
   public static Transform[,] gameGrid = new Transform[9,9];
   [SerializeField] Transform finalNode;
   [SerializeField] Transform allNodes;

   private void Awake() {
       PopulateGameGrid();
   }

   private void PopulateGameGrid() {
        //add each child node to the gameGrid as appropriate
        int rowIndex = 0, columnIndex = 0;
        for(int nodeIndex = allNodes.transform.childCount - 1; nodeIndex >= 0; nodeIndex--) {
            gameGrid[rowIndex,columnIndex] = allNodes.GetChild(nodeIndex).gameObject.transform;
            rowIndex++;
            if(rowIndex > 8) {
                columnIndex++;
                rowIndex = 0;
            }
        }

        gameGrid[8,8] = finalNode;
   }

   public Transform GetNodeAtPosition(int row, int column) {
       return gameGrid[row,column];
   }
}
