using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SwipeDIrection
{
    Up,
    Down,
    Right,
    Left
}

public class InputManager : MonoBehaviour
{

    [SerializeField] private float maxSwipeDist;


    SwipeDIrection swipeDir;
    Vector3 startSwipePos;
    Ray ray;
    RaycastHit hit;
    Camera cam;
    MatrixManager board;
    bool canSwipe;

    private void Start()
    {
        cam = Camera.main;
        board = FindObjectOfType<MatrixManager>();
        Ingredient.OnAnimationComplete += EnableSwipe;
        MatrixManager.OnActionCanceled += EnableSwipe;
        canSwipe = true;
    }

    private void Update()
    {
        #region MK testing
        if (canSwipe)
        {

            if (Input.GetMouseButtonDown(0))
            {
                ray = cam.ScreenPointToRay(Input.mousePosition);

                Physics.Raycast(ray, out hit);

                if (hit.transform != null)
                {
                    Ingredient temp = hit.transform.GetComponent<Ingredient>();
                    if (temp.ActualID >= 0)
                    {
                        board.selectedIngredient = temp;
                        startSwipePos = Input.mousePosition;
                    }
                }
            }

            if (Input.GetMouseButtonDown(1))
            {
                if (board.GetTargetIngredient(CheckInputDirection(Input.mousePosition)) >= 0)
                {
                    canSwipe = false;
                    StartCoroutine(board.Impile());
                }

            }
        }

        #endregion

        if (Input.touchCount > 0 && canSwipe)
        {
            Touch touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Began)
            {
                ray = cam.ScreenPointToRay(touch.position);
                Physics.Raycast(ray, out hit);

                if (hit.transform != null)
                {
                    Ingredient temp = hit.transform.GetComponent<Ingredient>();
                    if (temp.ActualID >= 0)
                    {
                        startSwipePos = touch.position;
                        board.selectedIngredient = temp;
                    }
                }
            }
            else if (touch.phase == TouchPhase.Moved || touch.phase == TouchPhase.Canceled || touch.phase == TouchPhase.Ended)
            {
                if (Vector3.Distance(startSwipePos, touch.position) > maxSwipeDist)
                {
                    if (board.GetTargetIngredient(CheckInputDirection(touch.position)) >= 0)
                    {
                        canSwipe = false;
                        StartCoroutine(board.Impile());
                    }
                }
            }
        }


    }

    /// <summary>
    /// checks the direction of the swipe
    /// </summary>
    /// <param name="pos"> last position of the finger when the function is called </param>
    /// <returns></returns>
    private SwipeDIrection CheckInputDirection(Vector3 pos)
    {
        Vector3 dir = pos - startSwipePos;

        float positiveX = Mathf.Abs(dir.x);
        float positiveZ = Mathf.Abs(dir.y);

        if (positiveX > positiveZ)
        {
            swipeDir = (dir.x > 0) ? SwipeDIrection.Right : SwipeDIrection.Left;
        }
        else
        {
            swipeDir = (dir.y > 0) ? SwipeDIrection.Up : SwipeDIrection.Down;
        }
        return swipeDir;
    }

    void EnableSwipe()
    {
        canSwipe = true;
    }

    private void OnDisable()
    {
        Ingredient.OnAnimationComplete -= EnableSwipe;
        MatrixManager.OnActionCanceled -= EnableSwipe;
    }
}
