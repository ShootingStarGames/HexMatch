using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexagonManager : MonoBehaviourSingleton<HexagonManager> {

    Vector3 startTouchPos, endTouchPos;
    TouchPhase touchPhase = TouchPhase.Ended;

    public void GameStart()
    {
        GameData data = GameDataManager.Instance.LoadData();
        if (null == data)
        {
            bool[,] array =
                {
                    { false, false, true, true, true , false},
                    { false, true , true, true, true , false},
                    { false, true , true, true, true , true },
                    { true , true , true, true, true , true },
                    { false, true , true, true, true , true },
                    { false, true , true, true, true , false},
                    { false, false, true, true, true , false}
                };
            data = new GameData(array);
        }
        HexGrid.Instance.CreateCells(data);
    }
    private void Start()
    {
        GameStart();
    }

    void Update()
    {
#if UNITY_EDITOR
        if (TouchPhase.Ended == touchPhase)
        {
            if (Input.GetMouseButtonDown(0))
            {
                touchPhase = TouchPhase.Began;
                HexGrid.Instance.SelectCell(null);
                startTouchPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

                RaycastHit2D hitInformation = Physics2D.Raycast(startTouchPos, Camera.main.transform.forward);

                if (hitInformation.collider != null)
                {
                    GameObject touchedObject = hitInformation.transform.gameObject;
                    if (!touchedObject.GetComponent<ObjectCell>())
                        return;
                    HexGrid.Instance.SelectCell(touchedObject.GetComponent<ObjectCell>());
                }
            }
        }
        else
        {
            if (Input.GetMouseButton(0))
            {
                endTouchPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

                if((endTouchPos- startTouchPos).sqrMagnitude > 100)
                {
                    float angle = Quaternion.FromToRotation(Vector3.up, endTouchPos - startTouchPos).eulerAngles.z;

                    if(-30 < angle && angle <= 30) // UP
                    {
                        HexGrid.Instance.Swap(Direction.UP);
                    }
                    else if(30 < angle && angle <= 90) // LEFTUP
                    {
                        HexGrid.Instance.Swap(Direction.LEFTUP);
                    }
                    else if (90 < angle && angle <= 150) // LEFTDOWN
                    {
                        HexGrid.Instance.Swap(Direction.LEFTDOWN);
                    }
                    else if (150 < angle && angle <= 210) // DOWN
                    {
                        HexGrid.Instance.Swap(Direction.DOWN);
                    }
                    else if (210 < angle && angle <= 270) // RIGHTDOWN
                    {
                        HexGrid.Instance.Swap(Direction.RIGHTDOWN);
                    }
                    else if (270 < angle || angle <= -30) // RIGHTUP
                    {
                        HexGrid.Instance.Swap(Direction.RIGHTUP);
                    }
                    touchPhase = TouchPhase.Ended;
                }
            }
            else
            {
                touchPhase = TouchPhase.Ended;
            }

        }
#else
        if (TouchPhase.Ended == touchPhase)
        {            
            if (Input.touchCount == 1)
            {
                touchPhase = TouchPhase.Began;
                HexGrid.Instance.SelectCell(null);
                startTouchPos = Camera.main.ScreenToWorldPoint(Input.GetTouch(0).position);

                RaycastHit2D hitInformation = Physics2D.Raycast(startTouchPos, Camera.main.transform.forward);

                if (hitInformation.collider != null)
                {
                    GameObject touchedObject = hitInformation.transform.gameObject;
                    if (!touchedObject.GetComponent<ObjectCell>())
                        return;
                    HexGrid.Instance.SelectCell(touchedObject.GetComponent<ObjectCell>());
                }
            }
        }
        else
        {
            if (Input.touchCount == 1)
            {
                endTouchPos = Camera.main.ScreenToWorldPoint(Input.GetTouch(0).position);

                if ((endTouchPos - startTouchPos).sqrMagnitude > 100)
                {
                    float angle = Quaternion.FromToRotation(Vector3.up, endTouchPos - startTouchPos).eulerAngles.z;

                    if (-30 < angle && angle <= 30) // UP
                    {
                        HexGrid.Instance.Swap(Direction.UP);
                    }
                    else if (30 < angle && angle <= 90) // LEFTUP
                    {
                        HexGrid.Instance.Swap(Direction.LEFTUP);
                    }
                    else if (90 < angle && angle <= 150) // LEFTDOWN
                    {
                        HexGrid.Instance.Swap(Direction.LEFTDOWN);
                    }
                    else if (150 < angle && angle <= 210) // DOWN
                    {
                        HexGrid.Instance.Swap(Direction.DOWN);
                    }
                    else if (210 < angle && angle <= 270) // RIGHTDOWN
                    {
                        HexGrid.Instance.Swap(Direction.RIGHTDOWN);
                    }
                    else if (270 < angle || angle <= -30) // RIGHTUP
                    {
                        HexGrid.Instance.Swap(Direction.RIGHTUP);
                    }
                    touchPhase = TouchPhase.Ended;
                }
            }
            else
            {
                touchPhase = TouchPhase.Ended;
            }
        }
#endif
    }
}
