using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

namespace _PacmanGame.Scripts
{
    [System.Serializable]
    public class MapGeneratorJsonDriven : MonoBehaviour, IMapGenerator
    {

        public GameObject WallPrefab;
        public GameObject PointPrefab;
        public GameObject EmptyPrefab;
        public GameObject MegaPointPrefab;
        public GameObject ThinWallPrefab;
        public GameObject PlayerPrefab;

        private const float TILE_OFFSET = 0.255f;
        
        public void GenerateMap()
        {
            var itemDictionary = new Dictionary<ItemTypes, GameObject>
            {
                {ItemTypes.Empty, EmptyPrefab},
                {ItemTypes.Wall, WallPrefab},
                {ItemTypes.Point, PointPrefab},
                {ItemTypes.PlayerPos, PlayerPrefab},
                {ItemTypes.ThinWall, WallPrefab}
            };  
            
            var jsonData = ReadJson();
            VerticalFlip(jsonData);
            
            InstantiateArray(jsonData, 1, itemDictionary);

            var rightJson = RemoveLastColumn(jsonData);  
            HorizontalFlip(rightJson);
            
            InstantiateArray(rightJson, 11, itemDictionary);
        }

        private void InstantiateArray(int[,] data, int offset, Dictionary<ItemTypes, GameObject> dictionary)
        {
            for (var i = 0; i < data.GetLength(0); i++)
            {
                for (var j = 0; j < data.GetLength(1); j++)
                {
                    var currentTile = data[i, j];
                    var yOffset = data.GetLength(1) * TILE_OFFSET;
                    var xOffset = (data.GetLength(0)/2) * TILE_OFFSET;
                    var position = new Vector2( (j * TILE_OFFSET ) + (offset * TILE_OFFSET)  - yOffset, (i * TILE_OFFSET) - xOffset );
                    var item = Instantiate(dictionary[(ItemTypes) currentTile], transform, false);
                    item.transform.localPosition = position;
                }
            }
        }

        private static int[,] RemoveLastColumn(int[,] inputMatrix)
        {
            var rows = inputMatrix.GetLength(0);
            var cols = inputMatrix.GetLength(1);
            var result = new int[rows, cols - 1];

            for (var i = 0; i < rows; i++)
            {
                for (var j = 0; j < cols; j++)
                {
                    if ( j == cols - 1 ) continue;
                    result[i, j] = inputMatrix[i, j];
                }
            }

            return result;
        }

        private static void VerticalFlip(int[,] inputMatrix)
        {
            var rows = inputMatrix.GetLength(0);
            var cols = inputMatrix.GetLength(1);
    
            for(var i = 0 ;i<= cols-1;i++)
            {
                var j = 0;
                var k= rows-1;
                while(j<k)
                {
                    var temp = inputMatrix[j,i];
                    inputMatrix[j,i] = inputMatrix[k,i];
                    inputMatrix[k,i] = temp;
                    j++;
                    k--;
                }
            }
        }
        
        private static void HorizontalFlip(int[,] inputMatrix)
        {
            var rows = inputMatrix.GetLength(0);
            var cols = inputMatrix.GetLength(1);
    
            for(var i = 0 ;i<= rows-1;i++)
            {
                var j = 0;
                var k= cols-1;
                while(j<k)
                {
                    var temp = inputMatrix[i,j];
                    inputMatrix[i,j] = inputMatrix[i,k];
                    inputMatrix[i,k] = temp;
                    j++;
                    k--;
                }
            }
        }
        

        public enum ItemTypes
        {
            Empty = 0,
            Wall = 1,
            Point = 2,
            PlayerPos = 3,
            ThinWall = 9
        }

        private static int[,] ReadJson()
        {
            var mapAsset = Resources.Load<TextAsset>("map");
            var mapinfo = Newtonsoft.Json.JsonConvert.DeserializeObject<int[,]>(mapAsset.text);
            return mapinfo;
        }
    }
}

