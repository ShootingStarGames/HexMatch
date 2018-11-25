using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexCell : MonoBehaviour {
    public static float moveTime = 0.1f;

    HexMesh hexMesh;
    public bool isShift
    {
        private set;
        get;
    }
    public bool isActive
    {
        get;
        protected set;
    }
    public Vector2Int point
    {
        private set;
        get;
    }

    private void Awake()
    {
        hexMesh = this.GetComponent<HexMesh>();
    }

    public virtual void Init(int y,int x)
    {
        point = new Vector2Int(x, y);
    }

    public void Move()
    {
        if (isShift)
        {
            Invoke("Move", 0.05f);
            return;
        }
        isShift = true;
        Vector3 target;
        target.x = (point.x + point.y * 0.5f - point.y / 2) * (2 * HexMetrics.innerRadius);
        target.y = point.y * HexMetrics.outerRadius * 1.5f;
        target.z = 0;
        StartCoroutine(MoveCoroutine(target));
    }

    IEnumerator MoveCoroutine(Vector2 desc)
    {
        Vector3 src = transform.localPosition;
        float time = moveTime;
        float elapsedTime = 0;
        while (elapsedTime <= time)
        {
            transform.localPosition = Vector3.Lerp(src, desc, elapsedTime / time);
            elapsedTime += Time.deltaTime;
            yield return new WaitForFixedUpdate();
        }

        isShift = false;
        transform.localPosition = desc;
    }

}

public enum ObjectCellType { GIRAFFE, MONKEY, PANDA, PARROT, PENGUIN, BOX }

public abstract class ObjectCell : HexCell
{
    public bool matched;
    public ObjectCellType objectCellType
    {
        protected set;
        get;
    }

    public virtual void Init(int y, int x, ObjectCellType objectCellType)
    {
        base.Init(y, x);
        this.objectCellType = objectCellType;
        this.matched = false;
        SetSprite();
    }

    protected abstract void SetSprite();
}