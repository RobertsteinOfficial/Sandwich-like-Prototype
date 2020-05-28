using System.Collections;
using UnityEngine;
using DG.Tweening;

public class Ingredient : MonoBehaviour
{
    public delegate void CompleteAnimation();
    public static event CompleteAnimation OnAnimationComplete;

    /// <summary>
    /// No ingredient = -1
    /// Bread = 0
    /// Other ingredients > 0
    /// </summary>
    [SerializeField] private int iD = -1;
    public int ActualID { get; set; }
    public int XIndex { get; set; }
    public int YIndex { get; set; }
    public Stack ingredientsPile { get; set; }


    //Animation
    public bool FaceUp { get; set; } = true;
    public bool HeadUp { get; set; } = true;
    Sequence flipSequence;
    Vector3 targetPosition;
    Vector3 targetRotation;
    Transform ingredientsToMove;
    readonly float ingredientsHeight = 0.1f;

    private void Awake()
    {
        SetupIngredient();
    }

    /// <summary>
    /// resets the data and the graphics of the Ingredient
    /// </summary>
    public void SetupIngredient()
    {
        ActualID = iD;
        ingredientsPile = new Stack();
        ingredientsPile.Push(iD);

        for (int i = 0; i < transform.childCount; i++)
        {
            Destroy(transform.GetChild(i).gameObject);
        }

    }

    /// <summary>
    /// checks if the ingredientsPile meets the win condition
    /// </summary>
    /// <param name="totalIngredients"></param>
    /// <returns></returns>
    public bool CheckVictory(int totalIngredients)
    {
        if (ingredientsPile.Count == totalIngredients)
        {
            var topId = (int)ingredientsPile.Pop();
            if (topId == 0 && ActualID == 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        else
        {
            return false;
        }
    }

    /// <summary>
    /// animates the graphics, moving it to a target ingredient
    /// </summary>
    /// <param name="target"></param>
    /// <returns></returns>
    public void MoveIngredients(Ingredient target)
    {
        ActualID = -1;
        ingredientsToMove = transform.GetChild(0);

        for (int i = 1; i < transform.childCount; i++)
        {
            transform.GetChild(i).parent = ingredientsToMove;
        }

        //ingredientsToMove.parent = null;
        #region Animation Setup

        float heightMult = ingredientsPile.Count > 1 ? target.ingredientsPile.Count : target.ingredientsPile.Count + 1;
        targetPosition = target.transform.position + Vector3.up * ingredientsHeight * heightMult;

        targetRotation = ingredientsToMove.eulerAngles;

        if (targetPosition.x < transform.position.x)
        {
            // 0 e 0 o !=0 e != 0
            //+180z
            if (HeadUp && FaceUp || !HeadUp && !FaceUp)
            {
                targetRotation.z += 180f;
            }
            else
            {
                targetRotation.z -= 180f;
            }



        }
        else if (targetPosition.x > transform.position.x)
        {
            //-180z
            if (HeadUp && FaceUp || !HeadUp && !FaceUp)
            {
                targetRotation.z -= 180f;
            }
            else
            {
                targetRotation.z += 180f;
            }


        }
        else if (targetPosition.z < transform.position.z)
        {
            //-180x
            if (HeadUp && FaceUp || !HeadUp && !FaceUp)
            {
                targetRotation.x -= 180f;
            }
            else
            {
                targetRotation.x += 180f;
            }


        }
        else if (targetPosition.z > transform.position.z)
        {
            //+180x
            if (HeadUp && FaceUp || !HeadUp && !FaceUp)
            {
                targetRotation.x += 180f;
            }
            else
            {
                targetRotation.x -= 180f;
            }


        }



        flipSequence = DOTween.Sequence().OnComplete(() =>
        {
            ingredientsToMove.parent = target.transform;

            for (int i = 0; i < ingredientsToMove.childCount; i++)
            {
                ingredientsToMove.GetChild(i).parent = target.transform;
            }

            target.ChooseTopIngredient();
            OnAnimationComplete();
        });

        flipSequence.Append(ingredientsToMove.DOMove(ingredientsToMove.position + Vector3.up * (targetPosition.y + 0.2f), .3f)).SetEase(Ease.Linear);
        flipSequence.Append(ingredientsToMove.DOMove(targetPosition, .3f)).SetEase(Ease.OutBounce);
        flipSequence.Insert(0f, ingredientsToMove.DORotate(targetRotation, flipSequence.Duration()));
        #endregion

        flipSequence.Play();


    }

    /// <summary>
    /// reorganizes the hierarchy of the Ingredient, based on the most high model in the scene
    /// reorganizes the hierarchy of the Ingredient, based on the most high model in the scene
    /// </summary>
    public void ChooseTopIngredient()
    {
        for (int i = 0; i < transform.GetChild(0).childCount; i++)
        {
            transform.GetChild(0).GetChild(i).parent = transform;
        }

        Transform topIngredient = transform.GetChild(0);

        for (int i = 0; i < transform.childCount; i++)
        {
            if (transform.GetChild(i).position.y > topIngredient.position.y)
            {
                topIngredient = transform.GetChild(i);
                topIngredient.transform.SetAsFirstSibling();
            }
        }

        for (int i = 0; i < transform.childCount; i++)
        {
            transform.GetChild(i).parent = topIngredient;
        }

        for (int i = 0; i < transform.childCount; i++)
        {
            transform.GetChild(i).parent = topIngredient;
        }

    }

    /// <summary>
    /// sets a new ID for the ingredient
    /// </summary>
    /// <param name="value"> new ID for the ingredient </param>
    public void SetNewID(int value)
    {
        iD = value;
        ActualID = value;
    }

    private void OnDrawGizmos()
    {
        //shows a different gizmo color depending if the tile is empty, is bread,or another ingredient
        if (iD == 0)
        {
            Gizmos.color = Color.yellow;
        }
        else if (iD > 0)
        {
            Gizmos.color = Color.green;
        }
        else
        {
            Gizmos.color = Color.red;
        }

        Gizmos.DrawWireCube(transform.position, Vector3.one * 0.9f);
    }
}
