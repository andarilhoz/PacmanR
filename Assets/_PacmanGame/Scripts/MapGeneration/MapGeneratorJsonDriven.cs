using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using _PacmanGame.Scripts.Pathfind;

namespace _PacmanGame.Scripts
{
    [System.Serializable]
    public class MapGeneratorJsonDriven : MonoBehaviour, IMapGenerator
    {

        public GameObject WallPrefab;
        public GameObject PointPrefab;
        public GameObject MegaPointPrefab;
        public GameObject ThinWallPrefab;
        public GameObject PlayerPrefab;
        public GameObject BlinkyPrefab;
        public GameObject TeleportPrefab;

        private const float TILE_OFFSET = 0.255f;
        
        private int[,] map;
        private Vector2[,] realWorldPos;

        public struct MapDetails
        {
            public int[,] mapGrid;
            public Vector2[,] realWorldPosGrid;
        }

        public MapDetails GenerateMap()
        {
            var itemDictionary = new Dictionary<ItemTypes, GameObject>
            {
                {ItemTypes.Empty, null},
                {ItemTypes.Wall, WallPrefab},
                {ItemTypes.Point, PointPrefab},
                {ItemTypes.PlayerPos, PlayerPrefab},
                {ItemTypes.Teleport, TeleportPrefab},
                {ItemTypes.ThinWall, WallPrefab},
                {ItemTypes.Blinky, BlinkyPrefab}
            };  
            
            var jsonData = ReadJson();
            map = DuplicateMap(jsonData);
            realWorldPos = new Vector2[map.GetLength(0), map.GetLength(1)];
            InstantiateArray(map, itemDictionary);
            return new MapDetails() { mapGrid = map, realWorldPosGrid = realWorldPos};
        }

        private int[,] DuplicateMap(int[,] originalMap)
        {
            VerticalFlip(originalMap);
            var rightJson = RemoveLastColumn(originalMap);  
            HorizontalFlip(rightJson);
            
            var duplicatedMap = new int[originalMap.GetLength(0), (originalMap.GetLength(1) * 2) - 1];

            //Get left side
            for (var i = 0; i < originalMap.GetLength(0); i++)
            {
                for (var j = 0; j < originalMap.GetLength(1); j++)
                {
                    duplicatedMap[i, j] = originalMap[i, j];
                }    
            }
            //Get right side
            for (var i = 0; i < rightJson.GetLength(0); i++)
            {
                for (var j = 0; j < rightJson.GetLength(1); j++)
                {
                    duplicatedMap[i, j + originalMap.GetLength(1)] = rightJson[i, j];
                }    
            }

            return duplicatedMap;
        }

        private void InstantiateArray(int[,] data, Dictionary<ItemTypes, GameObject> dictionary)
        {
            for (var row = 0; row < data.GetLength(0); row++)
            {
                for (var column = 0; column < data.GetLength(1); column++)
                {
                    var currentTile = data[row, column];
                    if ( ((ItemTypes) currentTile).Equals(ItemTypes.Empty) ) continue;

                    var yOffset = (data.GetLength(0) * TILE_OFFSET) / 2;
                    var xOffset = (data.GetLength(1) * TILE_OFFSET) /2;
                    
                    var position = new Vector2( column * TILE_OFFSET - xOffset, row * TILE_OFFSET - yOffset);
                    
                    if ( ((ItemTypes) currentTile).Equals(ItemTypes.Empty) ) continue;
                    var item = Instantiate(dictionary[(ItemTypes) currentTile], transform, false);
                    if(((ItemTypes) currentTile).Equals(ItemTypes.Blinky)) GetComponent<Pathfinding>().StartPosition = item.transform;
                    if(((ItemTypes) currentTile).Equals(ItemTypes.PlayerPos)) GetComponent<Pathfinding>().TargetPosition= item.transform;
                    realWorldPos[row, column] = position;
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

        private static int[,] ReadJson()
        {
            var mapAsset = Resources.Load<TextAsset>("map");
            /* TODO remove newtonsoft, replace with regex */
            var mapinfo = Newtonsoft.Json.JsonConvert.DeserializeObject<int[,]>(mapAsset.text);
            return mapinfo;
        }
    }
}

