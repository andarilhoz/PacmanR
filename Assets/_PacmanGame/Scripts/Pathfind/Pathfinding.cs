using System.Collections.Generic;
using UnityEngine;

namespace _PacmanGame.Scripts.Pathfind
{
    public class Pathfinding : MonoBehaviour
    {
        private Grid grid;
        public Transform StartPosition;
        public Transform TargetPosition;


        private void Awake()
        {
            grid = GetComponent<Grid>();
        }

        private void Update()
        {
            if ( StartPosition == null || TargetPosition == null ) return;
            FindPath(StartPosition.position, TargetPosition.position);
        }

        private void FindPath(Vector2 aStartPos, Vector2 aTargetPos)
        {
            var StartNode = grid.NodeFromWorldPostiion(aStartPos);
            var TargetNode = grid.NodeFromWorldPostiion(aTargetPos);

            var OpenList = new List<Node>();
            var ClosedList = new HashSet<Node>();
            
            OpenList.Add(StartNode);

            while (OpenList.Count > 0)
            {
                var CurrentNode = OpenList[0];
                for (var i = 1; i < OpenList.Count; i++)
                {
                    if ( OpenList[i].FCost < CurrentNode.FCost || OpenList[i].FCost == CurrentNode.FCost &&
                         OpenList[i].hCost < CurrentNode.hCost )
                    {
                        CurrentNode = OpenList[i];
                    }
                }
                
                CurrentNode.dirt = true;
                OpenList.Remove(CurrentNode);
                ClosedList.Add(CurrentNode);
                
                
                if ( CurrentNode == TargetNode )
                {
                    GetFinalPath(StartNode, TargetNode);
                }

                foreach (var neighborNode in grid.GetNeighboringNodes(CurrentNode))
                {
                    if ( neighborNode.IsWall || ClosedList.Contains(neighborNode) ) continue;

                    var MoveCost = CurrentNode.gCost + GetManhattenDistance(CurrentNode, neighborNode);
                    if ( MoveCost < neighborNode.FCost || !OpenList.Contains(neighborNode) )
                    {
                        neighborNode.gCost = MoveCost;
                        neighborNode.hCost = GetManhattenDistance(neighborNode, TargetNode);
                        neighborNode.Parent = CurrentNode;

                        if ( !OpenList.Contains(neighborNode)  )
                        {
                            OpenList.Add(neighborNode);
                        }
                    }
                }
            }
        }
        private void GetFinalPath(Node aStartNode, Node aEndNode)
        {
            List<Node> FinalPath = new List<Node>();
            Node CurrentNode = aEndNode;

            while (CurrentNode != aStartNode)
            {
                FinalPath.Add(CurrentNode);
                CurrentNode = CurrentNode.Parent;
            }
            
            FinalPath.Reverse();
            grid.FinalPath = FinalPath;
        }

        private int GetManhattenDistance(Node nodeA, Node nodeB)
        {
            var fX = Mathf.Abs(nodeA.gridX - nodeB.gridX);
            var fY = Mathf.Abs(nodeA.gridY - nodeB.gridY);

            return fX + fY;
        }
    }
}