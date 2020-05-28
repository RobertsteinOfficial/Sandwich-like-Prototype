using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MatrixManager : MonoBehaviour
{
    public delegate void CancelAction();
    public static event CancelAction OnActionCanceled;

    [Header("Board Dimension")]
    [SerializeField] private int boardHeight;
    [SerializeField] private int boardLength;

    [Header("Ingredients")]
    [SerializeField] private Transform[] ingredients;

    [Header("UI")]
    [SerializeField] private GameObject victoryPanel;

    public Ingredient selectedIngredient { get; set; }

    int totalIngredients = 0;
    Ingredient[,] logicMatrix;
    Ingredient targetIngredient;
    bool isSwiping;


    private void Start()
    {
        SetupLogicMatrix();
    }

    /// <summary>
    /// clears the ingredients cached for the previous swap
    /// </summary>
    public void ResetIngredients()
    {
        selectedIngredient = null;
        targetIngredient = null;
    }

    /// <summary>
    /// spawns the food on the grid based on the Ingredients' IDs
    /// </summary>
    public void SetupLogicMatrix()
    {
        ShowVictory(false);
        totalIngredients = 0;
        logicMatrix = new Ingredient[boardLength, boardHeight];

        for (int i = 0; i < boardHeight * boardLength; i++)
        {
            Ingredient temp = transform.GetChild(i).GetComponent<Ingredient>();
            temp.SetupIngredient();
            if (temp.ActualID >= 0)
            {
                Transform mesh = Instantiate(ingredients[temp.ActualID], temp.transform);
                mesh.localPosition = Vector3.zero;
                ++totalIngredients;
            }

            temp.XIndex = (int)(temp.transform.position.x + 1.5f);
            temp.YIndex = (int)(temp.transform.position.z + 1.5f);

            logicMatrix[temp.XIndex, temp.YIndex] = temp;
        }
    }

    /// <summary>
    /// given a direction, returns the ID of a neighbour Ingredient of selectedIngredient
    /// </summary>
    /// <param name="dir"></param>
    /// <returns></returns>
    public int GetTargetIngredient(SwipeDIrection dir)
    {
        if (selectedIngredient != null)
        {

            int x = -1, y = -1;

            //find the index of the destination ingredient
            switch (dir)
            {

                case SwipeDIrection.Up:
                    if (selectedIngredient.YIndex < boardHeight)
                    {
                        x = selectedIngredient.XIndex;
                        y = selectedIngredient.YIndex + 1;
                    }
                    else
                    {
                        ResetIngredients();
                        return -1;
                    }
                    break;
                case SwipeDIrection.Down:
                    if (selectedIngredient.YIndex > 0)
                    {
                        x = selectedIngredient.XIndex;
                        y = selectedIngredient.YIndex - 1;
                    }
                    else
                    {
                        ResetIngredients();
                        return -1;
                    }
                    break;
                case SwipeDIrection.Right:
                    if (selectedIngredient.XIndex < boardLength)
                    {
                        x = selectedIngredient.XIndex + 1;
                        y = selectedIngredient.YIndex;
                    }
                    else
                    {
                        ResetIngredients();
                        return -1;
                    }
                    break;
                case SwipeDIrection.Left:
                    if (selectedIngredient.XIndex > 0)
                    {
                        x = selectedIngredient.XIndex - 1;
                        y = selectedIngredient.YIndex;
                    }
                    else
                    {
                        ResetIngredients();
                        return -1;
                    }
                    break;
                default:
                    ResetIngredients();
                    break;
            }

            targetIngredient = logicMatrix[x, y];

            return targetIngredient.ActualID;
        }
        else
        {
            ResetIngredients();
            return -1;
        }
    }

    /// <summary>
    /// transfers the food data to one Ingredient to another, also calling the animation code in the starting Ingredient
    /// </summary>
    /// <returns></returns>
    public IEnumerator Impile()
    {
        if (isSwiping)
        {
            ResetIngredients();
            yield return null;
        }
        else
        {

            if (targetIngredient.ActualID < 0)
            {
                ResetIngredients();
                OnActionCanceled();
                yield return null;
            }
            else
            {
                isSwiping = true;
                /*yield return StartCoroutine(*/
                selectedIngredient.MoveIngredients(targetIngredient)/*)*/;

                while (selectedIngredient.ingredientsPile.Count != 0)
                {
                    targetIngredient.ingredientsPile.Push(selectedIngredient.ingredientsPile.Pop());
                }

                if (targetIngredient.CheckVictory(totalIngredients))
                {
                    ShowVictory(true);
                }

            }
        }

        ResetIngredients();
        isSwiping = false;
    }

    /// <summary>
    /// activates / deactivates the victory UI
    /// </summary>
    /// <param name="value"> true to activate, false to deactivate </param>
    void ShowVictory(bool value)
    {
        if (victoryPanel != null)
        {
            victoryPanel.SetActive(value);
        }
    }

}
