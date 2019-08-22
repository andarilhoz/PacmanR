using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

namespace _PacmanGame.Scripts
{
    [System.Serializable]
    public class MapGenerator : MonoBehaviour
    {
        public GameObject WallPrefab;
        public GameObject PointPrefab;
        public GameObject PowerDotPrefab;
        public GameObject ThinWallPrefab;
        public GameObject PlayerPrefab;
        public GameObject BlinkyPrefab;
        public GameObject PinkyPrefab;
        public GameObject InkyPrefab;
        public GameObject ClydePrefab;
        public GameObject TeleportPrefab;

        public const float TILE_OFFSET = 0.255f;
        public const float SCREEN_OFFSET = .125f;


        public (int[,], Vector2[,]) GenerateMap()
        {
            var jsonData = ReadJson();
            var map = DuplicateMap(jsonData);
            return (map, GetRealWorldPosMap(map));
        }

        private Vector2[,] GetRealWorldPosMap(int[,] map)
        {
            var realWorldPos = new Vector2[map.GetLength(0), map.GetLength(1)];
            for (var row = 0; row < map.GetLength(0); row++)
            {
                for (var column = 0; column < map.GetLength(1); column++)
                {
                    var pos = position(map, column, row);
                    realWorldPos[row, column] = pos;
                }
            }

            return realWorldPos;
        }

        public void InstantiateMap(int[,] map, Vector2[,] realWorldPos)
        {
            var itemDictionary = new Dictionary<ItemTypes, GameObject>
            {
                {ItemTypes.Empty, null},
                {ItemTypes.Wall, WallPrefab},
                {ItemTypes.Point, PointPrefab},
                {ItemTypes.PowerDot, PowerDotPrefab},
                {ItemTypes.PlayerPos, PlayerPrefab},
                {ItemTypes.Teleport, TeleportPrefab},
                {ItemTypes.ThinWall, ThinWallPrefab},
                {ItemTypes.Blinky, BlinkyPrefab},
                {ItemTypes.Pinky, PinkyPrefab},
                {ItemTypes.Inky, InkyPrefab},
                {ItemTypes.Clyde, ClydePrefab}
            };


            InstantiateArray(map, realWorldPos, itemDictionary);
        }

        private static int[,] DuplicateMap(int[,] originalMap)
        {
            VerticalFlip(originalMap);
            var rightJson = RemoveLastColumn(originalMap);
            HorizontalFlip(rightJson);

            var duplicatedMap = new int[originalMap.GetLength(0), originalMap.GetLength(1) * 2 - 1];

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
                    var value = ((ItemTypes) rightJson[i, j]).IsGhost() ? 0 : rightJson[i, j];
                    duplicatedMap[i, j + originalMap.GetLength(1)] = value;
                }
            }

            return duplicatedMap;
        }

        private static Vector2 position(int[,] data, int column, int row)
        {
            var yOffset = data.GetLength(0) * TILE_OFFSET / 2;
            var xOffset = data.GetLength(1) * TILE_OFFSET / 2;

            return new Vector2(column * TILE_OFFSET - xOffset + SCREEN_OFFSET, row * TILE_OFFSET - yOffset);
        }

        private void InstantiateArray(int[,] data, Vector2[,] realWorldPos,
            IReadOnlyDictionary<ItemTypes, GameObject> dictionary)
        {
            for (var row = 0; row < data.GetLength(0); row++)
            {
                for (var column = 0; column < data.GetLength(1); column++)
                {
                    var currentTile = data[row, column];

                    if ( ((ItemTypes) currentTile).Equals(ItemTypes.Empty) )
                    {
                        continue;
                    }

                    if ( ((ItemTypes) currentTile).Equals(ItemTypes.Empty) )
                    {
                        continue;
                    }

                    var item = Instantiate(dictionary[(ItemTypes) currentTile], transform, false);
                    item.transform.localPosition = realWorldPos[row, column];
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
                    if ( j == cols - 1 )
                    {
                        continue;
                    }

                    result[i, j] = inputMatrix[i, j];
                }
            }

            return result;
        }

        private static void VerticalFlip(int[,] inputMatrix)
        {
            var rows = inputMatrix.GetLength(0);
            var cols = inputMatrix.GetLength(1);

            for (var i = 0; i <= cols - 1; i++)
            {
                var j = 0;
                var k = rows - 1;
                while (j < k)
                {
                    var temp = inputMatrix[j, i];
                    inputMatrix[j, i] = inputMatrix[k, i];
                    inputMatrix[k, i] = temp;
                    j++;
                    k--;
                }
            }
        }

        private static void HorizontalFlip(int[,] inputMatrix)
        {
            var rows = inputMatrix.GetLength(0);
            var cols = inputMatrix.GetLength(1);

            for (var i = 0; i <= rows - 1; i++)
            {
                var j = 0;
                var k = cols - 1;
                while (j < k)
                {
                    var temp = inputMatrix[i, j];
                    inputMatrix[i, j] = inputMatrix[i, k];
                    inputMatrix[i, k] = temp;
                    j++;
                    k--;
                }
            }
        }

        private static int[,] ReadJson()
        {
            /* Originally mad with Newtonsoft,
             * but since this is a no plugin/sdk project
             * i replaced it with regex.
             */

            var mapAsset = Resources.Load<TextAsset>("map");

            var rowsRegex = new Regex(@"(\[[0-9].+?([0-9].+?)(]))");
            var columnRegex = new Regex(@"\d+");

            var oneLineString = mapAsset.text.Replace("\r\n", "");
            var removeSpaces = oneLineString.Replace(" ", "");

            var matches = rowsRegex.Matches(removeSpaces);
            var rowsSize = matches.Count;
            var columnSize = columnRegex.Matches(matches[0].ToString()).Count;

            var returnArray = new int[rowsSize, columnSize];

            for (var i = 0; i < matches.Count; i++)
            {
                var rowData = columnRegex.Matches(matches[i].Value);
                for (var j = 0; j < rowData.Count; j++)
                {
                    returnArray[i, j] = int.Parse(rowData[j].Value);
                }
            }

            return returnArray;
        }
    }
}