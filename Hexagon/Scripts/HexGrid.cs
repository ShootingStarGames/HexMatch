using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;


public enum Direction { UP, DOWN, LEFTUP, RIGHTUP, LEFTDOWN, RIGHTDOWN, NONE }

public class HexGrid : MonoBehaviourSingleton<HexGrid> {
    #region value
    [SerializeField]
    int width = 6;
    [SerializeField]
    int height = 6;
    float routineTime;
    int specialCellNum;
    int swapCount;
    int score;
    #endregion
    #region ForRoutine
    ObjectCell selectedCell;
    public bool isShift
    {
        private set;
        get;
    }
    #endregion
    #region Prefabs
    [SerializeField]
    HexCell editCellPrefab;
    [SerializeField]
    HexCell gameCellPrefab;
    [SerializeField]
    CircleCell circleCellPrefab;
    [SerializeField]
    SpecialCell specialCellPrefab;
    #endregion
    #region Struct
    HexCell[,] cells;
    ObjectCell[,] objectCells;

    Vector2Int start;
    List<Vector2Int> subStartList;
    List<ObjectCellType> possibleCellType;
    List<ObjectCellType> impossibleCellType;
    List<Vector2Int> possiblePos;
    #endregion
    #region Editor
    public void CreateCells()
    {
        Clear();
        cells = new HexCell[height,width];
        start = new Vector2Int(0, height / 2);
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                cells[y, x] = CreateCell(x, y, editCellPrefab);
            }
        }
        transform.position =
                  new Vector3(
                      -cells[start.y, start.x].transform.localPosition.y,
                      (cells[start.y, width - 1].transform.localPosition.x + cells[start.y, start.x].transform.localPosition.x) * 0.5f);
    }

    public bool[,] GetBool()
    {
        bool[,] ret = new bool[height, width];

        for (int y = 0, i = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                ret[y, x] = (cells[y, x] as EditCell).isActive;
            }
        }

        return ret;
    }
    #endregion
    #region Common

    public void Clear()
    {
        subStartList = new List<Vector2Int>();
        isShift = false;

        var tempList = transform.Cast<Transform>().ToList();
        foreach (var child in tempList)
        {
            DestroyImmediate(child.gameObject);
        }
    }

    HexCell CreateCell(int x, int y, HexCell prefab)
    {
        Vector3 position;
        position.x = (x + y * 0.5f - y / 2) * (2 * HexMetrics.innerRadius);
        position.y = y * HexMetrics.outerRadius * 1.5f;
        position.z = 0;

        HexCell cell = Instantiate<HexCell>(prefab);
        //cell.gameObject.hideFlags = HideFlags.HideInHierarchy;
        cell.Init(y, x);
        cell.transform.SetParent(transform);
        cell.transform.localPosition = position;

        return cell;
    }
    ObjectCell GetNearestCell(ObjectCell objectCell, Direction direction)
    {
        if (null == objectCell)
            return null;
        int x = objectCell.point.x;
        int y = objectCell.point.y;

        int yy = 0; int xx = 0;

        switch (direction)
        {
            case Direction.UP:
                xx = -1;
                break;
            case Direction.DOWN:
                xx = +1;
                break;
            case Direction.LEFTUP:
                if (y % 2 == 0)
                    xx = -1;
                yy = -1;
                break;
            case Direction.RIGHTUP:
                if (y % 2 == 0)
                    xx = -1;
                yy = +1;
                break;
            case Direction.LEFTDOWN:
                if (y % 2 != 0)
                    xx = +1;
                yy = -1;
                break;
            case Direction.RIGHTDOWN:
                if (y % 2 != 0)
                    xx = +1;
                yy = +1;
                break;
            case Direction.NONE:
                return null;
        }
        int X = x + xx;
        int Y = y + yy;
        if (
            X < 0 || X >= width ||
            Y < 0 || Y >= height
            )
            return null;
        if (!cells[Y, X].isActive)
            return null;
        return objectCells[Y, X];
    }
    #endregion
    #region GameRoutine
    void InitializeForRoutine()
    {
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                if (null != objectCells[y, x])
                {
                    ObjectCell UP = GetNearestCell(objectCells[y, x], Direction.UP);
                    ObjectCell LEFTUP = GetNearestCell(objectCells[y, x], Direction.LEFTUP);
                    ObjectCell RIGHTUP = GetNearestCell(objectCells[y, x], Direction.RIGHTUP);

                    if (null == UP && null == LEFTUP && null == RIGHTUP)
                    {
                        subStartList.Add(objectCells[y,x].point);
                    }
                }
            }
        }

    }
    IEnumerator GameRoutine()
    {
        while (routineTime > 0)
        {
            yield return new WaitForEndOfFrame();
            routineTime -= Time.deltaTime;
        }
    }
    public void Shift(float time)
    {
        float currentTime = routineTime;
        routineTime += time ;
        if(currentTime <= 0)
            StartCoroutine(GameRoutine());
    }

    public void CreateCells(GameData gameData)
    {
        specialCellNum = 8;
        swapCount = 20;
        score = 0;
        UIManager.Instance.UpdateTop(specialCellNum, swapCount, score);

        bool[,] data = gameData.bools;
        height = data.GetLength(0);
        width = data.GetLength(1);
        cells = new HexCell[height, width];
        possibleCellType = 
            new List<ObjectCellType>()
            { ObjectCellType.GIRAFFE, ObjectCellType.MONKEY, ObjectCellType.PANDA, ObjectCellType.PARROT, ObjectCellType.PENGUIN };
        impossibleCellType = new List<ObjectCellType>();
        possiblePos = new List<Vector2Int>();
        start = new Vector2Int(0, height / 2);
        Clear();

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                cells[y, x] = CreateCell(x, y, gameCellPrefab);
                (cells[y, x] as BorderCell).Init(data[y, x]);
            }
        }
        transform.position =
                   new Vector3(
                       -cells[start.y, start.x].transform.localPosition.y,
                       (cells[start.y, width - 1].transform.localPosition.x + cells[start.y, start.x].transform.localPosition.x) * 0.5f);

        Allocate();
        AddSpecialCell(specialCellNum, ObjectCellType.BOX);
        InitializeForRoutine();
    }
    void Allocate()
    {
        objectCells = new ObjectCell[height, width];

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                if (cells[y, x].isActive)
                    objectCells[y, x] = CreateObjectCell(x, y);
            }
        }
    }
    ObjectCell CreateObjectCell(int x, int y)
    {
        possiblePos.Add(new Vector2Int(x, y));
        ObjectCell cell = Instantiate<ObjectCell>(circleCellPrefab);

        cell.transform.SetParent(transform);
        cell.transform.localPosition = cells[y, x].transform.localPosition;
        cell.transform.localScale = Vector3.one * HexMetrics.outerRadius * 1.5f;

        cell.Init(y, x);
        ObjectCell nearCell = GetNearestCell(cell, Direction.LEFTDOWN);
        ObjectCell prevNearCell = GetNearestCell(nearCell, Direction.LEFTDOWN);

        if (null != nearCell && null != prevNearCell)
        {
            if(nearCell.objectCellType == prevNearCell.objectCellType)
            {
                if (!impossibleCellType.Contains<ObjectCellType>(nearCell.objectCellType))
                    impossibleCellType.Add(nearCell.objectCellType);
                possibleCellType.Remove(nearCell.objectCellType);
            }
            nearCell = null;
            prevNearCell = null;
        }
        nearCell = GetNearestCell(cell, Direction.LEFTUP);
        prevNearCell = GetNearestCell(nearCell, Direction.LEFTUP);

        if (null != nearCell && null != prevNearCell)
        {
            if (nearCell.objectCellType == prevNearCell.objectCellType)
            {
                if (!impossibleCellType.Contains<ObjectCellType>(nearCell.objectCellType))
                    impossibleCellType.Add(nearCell.objectCellType);
                possibleCellType.Remove(nearCell.objectCellType);
            }
            nearCell = null;
            prevNearCell = null;
        }
        nearCell = GetNearestCell(cell, Direction.UP);
        prevNearCell = GetNearestCell(nearCell, Direction.UP);

        if (null != nearCell && null != prevNearCell)
        {
            if (nearCell.objectCellType == prevNearCell.objectCellType)
            {
                if (!impossibleCellType.Contains<ObjectCellType>(nearCell.objectCellType))
                    impossibleCellType.Add(nearCell.objectCellType);
                possibleCellType.Remove(nearCell.objectCellType);
            }
            nearCell = null;
            prevNearCell = null;
        }

        ObjectCellType objectCellType = possibleCellType[Random.Range(0, possibleCellType.Count)];

        possibleCellType.AddRange(impossibleCellType);
        impossibleCellType.Clear();

        cell.Init(y, x, objectCellType);

        return cell;
    }
    void AddSpecialCell(int num, ObjectCellType objectCellType)
    {
        for(int i=0;i<num;i++)
        {
            Vector2Int pos = possiblePos[Random.Range(0, possiblePos.Count)];
            possiblePos.Remove(pos);
            ObjectCell cell = Instantiate<ObjectCell>(specialCellPrefab);
            cell.Init(pos.y, pos.x, objectCellType);

            cell.transform.SetParent(transform);
            cell.transform.localPosition = objectCells[pos.y, pos.x].transform.localPosition;
            cell.transform.localScale = Vector3.one * HexMetrics.outerRadius * 1.5f;

            Destroy(objectCells[pos.y, pos.x].gameObject);

            objectCells[pos.y, pos.x] = cell;
        }
    }

    public void SelectCell(ObjectCell objectCell)
    {
        if (null == objectCell)
            return;
        if (objectCell.matched)
            return;
        selectedCell = objectCell;
    }
    public void Swap(Direction direction)
    {
        if (selectedCell == null || routineTime > 0) // 선택된 셀이 없거나 루틴이 아직 진행 중일 경우.
            return;
        Debug.Log("Swap");
        ObjectCell nearestCell = GetNearestCell(selectedCell,direction);
        if (nearestCell == null || nearestCell.matched)
            return;

        objectCells[selectedCell.point.y, selectedCell.point.x] = nearestCell;
        objectCells[nearestCell.point.y, nearestCell.point.x] = selectedCell;

        Vector2Int temp = selectedCell.point;
        selectedCell.Init(nearestCell.point.y, nearestCell.point.x);
        nearestCell.Init(temp.y, temp.x);

        CheckAll(selectedCell);
        CheckAll(nearestCell);

        selectedCell.Move();
        nearestCell.Move();
        Shift(HexCell.moveTime);
        StartCoroutine(GameRoutine());

        if (!selectedCell.matched && !nearestCell.matched) // 매칭이 되지 않는다면 원래 상태로 돌아옴.
        {
            objectCells[selectedCell.point.y, selectedCell.point.x] = nearestCell;
            objectCells[nearestCell.point.y, nearestCell.point.x] = selectedCell;

            temp = selectedCell.point;
            selectedCell.Init(nearestCell.point.y, nearestCell.point.x);
            nearestCell.Init(temp.y, temp.x);

            selectedCell.Move();
            nearestCell.Move();
            Shift(HexCell.moveTime);
        }
        else
        {
            swapCount--;
            UIManager.Instance.UpdateTop(specialCellNum, swapCount, score);
            EffectSideCell();
        }
        StartCoroutine(DestroyAndMoveCo());

        selectedCell = null;
    }
    void CheckAll(ObjectCell objectCell)
    {
        if (objectCell == null)
            return;
        ObjectCellType objectCellType = objectCell.objectCellType;
        if (ObjectCellType.BOX == objectCellType)
            return;

        ObjectCell UP = GetNearestCell(objectCell, Direction.UP);
        ObjectCell DOWN = GetNearestCell(objectCell, Direction.DOWN);
        ObjectCell LEFTUP = GetNearestCell(objectCell, Direction.LEFTUP);
        ObjectCell LEFTDOWN = GetNearestCell(objectCell, Direction.LEFTDOWN);
        ObjectCell RIGHTUP = GetNearestCell(objectCell, Direction.RIGHTUP);
        ObjectCell RIGHTDOWN = GetNearestCell(objectCell, Direction.RIGHTDOWN);

        CheckMatch(objectCell);
        CheckMatch(UP);
        CheckMatch(DOWN);
        CheckMatch(LEFTUP);
        CheckMatch(LEFTDOWN);
        CheckMatch(RIGHTUP);
        CheckMatch(RIGHTDOWN);
    }
    void CheckMatch(ObjectCell objectCell)
    {
        if (objectCell == null)
            return;
        ObjectCellType objectCellType = objectCell.objectCellType;
        if (ObjectCellType.BOX == objectCellType)
            return;

        ObjectCell UP = GetNearestCell(objectCell, Direction.UP);
        ObjectCell DOWN = GetNearestCell(objectCell, Direction.DOWN);
        ObjectCell LEFTUP = GetNearestCell(objectCell, Direction.LEFTUP);
        ObjectCell LEFTDOWN = GetNearestCell(objectCell, Direction.LEFTDOWN);
        ObjectCell RIGHTUP = GetNearestCell(objectCell, Direction.RIGHTUP);
        ObjectCell RIGHTDOWN = GetNearestCell(objectCell, Direction.RIGHTDOWN);

        if(UP != null && DOWN != null)
        {
            if((UP.objectCellType == DOWN.objectCellType) && UP.objectCellType == objectCellType)
            {
                if(!UP.matched)
                    score += 10;
                if(!DOWN.matched)
                    score += 10;
                if(!objectCell.matched)
                    score += 10;

                UP.matched = true;
                DOWN.matched = true;
                objectCell.matched = true;
                UP.GetComponent<SpriteRenderer>().color = Color.clear;
                DOWN.GetComponent<SpriteRenderer>().color = Color.clear;
                objectCell.GetComponent<SpriteRenderer>().color = Color.clear;
            }
        }

        if (LEFTUP != null && RIGHTDOWN != null)
        {
            if ((LEFTUP.objectCellType == RIGHTDOWN.objectCellType) && LEFTUP.objectCellType == objectCellType)
            {
                if (!LEFTUP.matched)
                    score += 10;
                if (!RIGHTDOWN.matched)
                    score += 10;
                if (!objectCell.matched)
                    score += 10;
                LEFTUP.matched = true;
                RIGHTDOWN.matched = true;
                objectCell.matched = true;
                LEFTUP.GetComponent<SpriteRenderer>().color = Color.clear;
                RIGHTDOWN.GetComponent<SpriteRenderer>().color = Color.clear;
                objectCell.GetComponent<SpriteRenderer>().color = Color.clear;
            }
        }

        if (LEFTDOWN != null && RIGHTUP != null)
        {
            if ((LEFTDOWN.objectCellType == RIGHTUP.objectCellType) && LEFTDOWN.objectCellType == objectCellType)
            {
                if (!LEFTDOWN.matched)
                    score += 10;
                if (!RIGHTUP.matched)
                    score += 10;
                if (!objectCell.matched)
                    score += 10;
                LEFTDOWN.matched = true;
                RIGHTUP.matched = true;
                objectCell.matched = true;
                LEFTDOWN.GetComponent<SpriteRenderer>().color = Color.clear;
                RIGHTUP.GetComponent<SpriteRenderer>().color = Color.clear;
                objectCell.GetComponent<SpriteRenderer>().color = Color.clear;
            }
        }

        UIManager.Instance.UpdateTop(specialCellNum, swapCount, score);
    }
    void EffectSideCell()
    {
        List<ObjectCell> sideCell = new List<ObjectCell>();

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                if (null != objectCells[y, x] && objectCells[y, x].matched)
                {
                    ObjectCell UP = GetNearestCell(objectCells[y, x], Direction.UP);
                    ObjectCell DOWN = GetNearestCell(objectCells[y, x], Direction.DOWN);
                    ObjectCell LEFTUP = GetNearestCell(objectCells[y, x], Direction.LEFTUP);
                    ObjectCell LEFTDOWN = GetNearestCell(objectCells[y, x], Direction.LEFTDOWN);
                    ObjectCell RIGHTUP = GetNearestCell(objectCells[y, x], Direction.RIGHTUP);
                    ObjectCell RIGHTDOWN = GetNearestCell(objectCells[y, x], Direction.RIGHTDOWN);
                    
                    if( UP != null && UP.objectCellType == ObjectCellType.BOX)
                    {
                        if(!sideCell.Contains(UP))
                            sideCell.Add(UP);
                    }
                    if (DOWN != null && DOWN.objectCellType == ObjectCellType.BOX)
                    {
                        if (!sideCell.Contains(DOWN))
                            sideCell.Add(DOWN);
                    }
                    if (LEFTUP != null && LEFTUP.objectCellType == ObjectCellType.BOX)
                    {
                        if (!sideCell.Contains(LEFTUP))
                            sideCell.Add(LEFTUP);
                    }
                    if (LEFTDOWN != null && LEFTDOWN.objectCellType == ObjectCellType.BOX)
                    {
                        if (!sideCell.Contains(LEFTDOWN))
                            sideCell.Add(LEFTDOWN);
                    }
                    if (RIGHTUP != null && RIGHTUP.objectCellType == ObjectCellType.BOX)
                    {
                        if (!sideCell.Contains(RIGHTUP))
                            sideCell.Add(RIGHTUP);
                    }
                    if (RIGHTDOWN != null && RIGHTDOWN.objectCellType == ObjectCellType.BOX)
                    {
                        if (!sideCell.Contains(RIGHTDOWN))
                            sideCell.Add(RIGHTDOWN);
                    }
                }
            }
        }

        foreach (var item in sideCell)
        {
            if(item is SpecialCell)
                (item as SpecialCell).Effect();
        }
    }
    IEnumerator DestroyAndMoveCo()
    {
        bool destroyed = false;

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                if (null != objectCells[y, x])
                {
                    bool isDestroy = DestroyMatchAndMove(x, y);
                    if (isDestroy)
                        destroyed = isDestroy;
                }
            }
        }

        if (destroyed)
        {
            Shift(HexCell.moveTime * 3);
            yield return new WaitForSeconds(HexCell.moveTime);
            StartCoroutine(DestroyAndMoveCo());
        }
        else
        {
            Shift(HexCell.moveTime * 3);
            yield return new WaitForSeconds(HexCell.moveTime);
            CreateStartCell();
        }
    }
    bool DestroyMatchAndMove(int x,int y)
    {
        if (!objectCells[y, x].matched)
            return false;
        ObjectCell cell = objectCells[y, x];
        ObjectCell UP = GetNearestCell(cell, Direction.UP);
        ObjectCell LEFTUP = GetNearestCell(cell, Direction.LEFTUP);
        ObjectCell RIGHTUP = GetNearestCell(cell, Direction.RIGHTUP);


        if (null != UP && !UP.matched)
        {
            if (cell is SpecialCell)
                cell = DeleteSpecialCell(cell);

            objectCells[UP.point.y, UP.point.x] = cell;
            objectCells[cell.point.y, cell.point.x] = UP;

            Vector2Int temp = cell.point;
            cell.Init(UP.point.y, UP.point.x);
            UP.Init(temp.y, temp.x);

            cell.Move();
            UP.Move();
            return true;
        }
        else if(null != LEFTUP && !LEFTUP.matched)
        {
            if (cell is SpecialCell)
                cell = DeleteSpecialCell(cell);

            objectCells[LEFTUP.point.y, LEFTUP.point.x] = cell;
            objectCells[cell.point.y, cell.point.x] = LEFTUP;

            Vector2Int temp = cell.point;
            cell.Init(LEFTUP.point.y, LEFTUP.point.x);
            LEFTUP.Init(temp.y, temp.x);

            cell.Move();
            LEFTUP.Move();
            return true;
        }
        else if(null != RIGHTUP && !RIGHTUP.matched)
        {
            if (cell is SpecialCell)
                cell = DeleteSpecialCell(cell);

            objectCells[RIGHTUP.point.y, RIGHTUP.point.x] = cell;
            objectCells[cell.point.y, cell.point.x] = RIGHTUP;

            Vector2Int temp = cell.point;
            cell.Init(RIGHTUP.point.y, RIGHTUP.point.x);
            RIGHTUP.Init(temp.y, temp.x);

            cell.Move();
            RIGHTUP.Move();
            return true;
        }

        return false;
    }

    void ReMatch()
    {
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                if (cells[y, x].isActive && !objectCells[y, x].matched)
                {
                    CheckAll(objectCells[y, x]);
                }
            }
        }

        EffectSideCell();

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                if (null != objectCells[y, x])
                {
                    if(objectCells[y,x].matched)
                    {
                        StartCoroutine(DestroyAndMoveCo());
                        return;
                    }
                }
            }
        }

    }
    
    void CreateStartCell()
    {
        bool isCreated = false;

        for(int i=0;i<subStartList.Count;i++)
        {
            if (objectCells[subStartList[i].y, subStartList[i].x].matched)
            {
                ObjectCellType objectCellType = possibleCellType[Random.Range(0, possibleCellType.Count)];

                objectCells[subStartList[i].y, subStartList[i].x].Init(subStartList[i].y, subStartList[i].x, objectCellType);

                isCreated = true;
            }
        }
        if (isCreated)
        {
            StartCoroutine(DestroyAndMoveCo());
        }

        if(!isCreated)
            ReMatch();
    }

    ObjectCell DeleteSpecialCell(ObjectCell objectCell)
    {
        specialCellNum--;
        score += 100;
        UIManager.Instance.UpdateTop(specialCellNum, swapCount, score);

        if (specialCellNum <= 0)
            UIManager.Instance.Toggle();
        int x = objectCell.point.x;
        int y = objectCell.point.y;

        GameObject obj = objectCell.gameObject;

        ObjectCell cell = Instantiate<ObjectCell>(circleCellPrefab);

        cell.transform.SetParent(transform);
        cell.transform.localPosition = obj.transform.localPosition;
        cell.transform.localScale = Vector3.one * HexMetrics.outerRadius * 1.5f;
        cell.Init(y, x, objectCell.objectCellType);

        Destroy(obj);
        objectCells[y, x] = cell;
        objectCells[y, x].matched = true;
        cell.GetComponent<SpriteRenderer>().color = Color.clear;

        return cell;
    }
    #endregion
    #region Unity
    private void Awake()
    {
        routineTime = 0;
    }
    #endregion
}
