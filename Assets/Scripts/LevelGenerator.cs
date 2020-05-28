using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class LevelGenerator : MonoBehaviour
{
    [Tooltip("Number of maximum ingredients that will spawn in the level, bread excluded")]
    [SerializeField] private int maxIngredientsPerLevel;

    public int BoardLength { get; set; }
    public int BoardHeight { get; set; }
    public int NOfIngredients { get; set; }

    int[,] newLevel;
    List<Vector2> occupiedCells;

    private void Start()
    {
        newLevel = new int[BoardLength, BoardHeight];
    }

    /// <summary>
    /// generates a new random matrix for a new level
    /// </summary>
    /// <returns></returns>
    public int[,] SetupNewLevel()
    {
        //reset the matrix
        for (int i = 0; i < BoardLength; ++i)
        {
            for (int j = 0; j < BoardHeight; ++j)
            {
                newLevel[i, j] = -1;
            }
        }

        occupiedCells = new List<Vector2>();

        //allocate the first slice of bread
        int x = Random.Range(0, BoardLength);
        int y = Random.Range(0, BoardHeight);

        occupiedCells.Add(new Vector2(x, y));

        newLevel[x, y] = 0;

        //allocate the second slice of bread
        Vector2 secondBread = GetRandomAdjacent(x, y);
        newLevel[(int)secondBread.x, (int)secondBread.y] = 0;
        occupiedCells.Add(secondBread);

        //allocate the rest of the ingredients
        Vector2 newIngredient;
        int iterations = 0;

        for (int i = 0; i < maxIngredientsPerLevel;)
        {
            int randomIndex = Random.Range(0, occupiedCells.Count);

            newIngredient = GetRandomAdjacent((int)occupiedCells[randomIndex].x, (int)occupiedCells[randomIndex].y);
            iterations++;

            if (newIngredient.x >= 0)
            {
                if (newLevel[(int)newIngredient.x, (int)newIngredient.y] == -1)
                {
                    newLevel[(int)newIngredient.x, (int)newIngredient.y] = Random.Range(1, NOfIngredients);
                    ++i;
                }

            }


            if (iterations > 1000)
            {
                break;
            }

        }

        return newLevel;
    }

    /// <summary>
    /// returns the coordinates of a random adjacent element of the matrix
    /// </summary>
    /// <param name="x"> x coordinate of the element </param>
    /// <param name="y"> y coordinate of the element </param>
    Vector2 GetRandomAdjacent(int x, int y)
    {
        List<Vector2> adjacents = new List<Vector2>();

        for (int dx = (x > 0 ? -1 : 0); dx <= (x < BoardLength - 1 ? 1 : 0); ++dx)
        {
            for (int dy = (y > 0 ? -1 : 0); dy <= (y < BoardHeight - 1 ? 1 : 0); ++dy)
            {
                if (dx != 0 && dy == 0 || dx == 0 && dy != 0)
                {
                    Vector2 temp = new Vector2(x + dx, y + dy);

                    adjacents.Add(temp);
                }
            }
        }


        if (adjacents.Count > 0)
        {

            int randomValue = Random.Range(0, adjacents.Count);

            return adjacents[randomValue];
        }
        else
        {
            Debug.Log("fail getrandomadjacent");
            return new Vector2(-1, -1);
        }
    }

    public void Save(GameDataWriter writer)
    {
        writer.Write(BoardLength);
        writer.Write(BoardHeight);

        for (int i = 0; i < BoardLength; i++)
        {
            for (int j = 0; j < BoardHeight; j++)
            {
                writer.Write(newLevel[i, j]);
            }
        }
    }

    public int[,] Load(GameDataReader reader)
    {
        int length = reader.ReadInt();
        int height = reader.ReadInt();

        for (int i = 0; i < length; i++)
        {
            for (int j = 0; j < height; j++)
            {
                newLevel[i, j] = reader.ReadInt();
            }
        }

        return newLevel;
    }
}
