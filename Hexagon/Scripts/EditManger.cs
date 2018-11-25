using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EditManger : MonoBehaviour
{
    bool isClicked = false;
    public void CreateCell()
    {
        HexGrid.Instance.CreateCells();
    }

    public void SaveCell()
    {
        GameDataManager.Instance.SaveData(HexGrid.Instance.GetBool());
    }

    public void BackScene()
    {
        SceneManager.LoadScene("Title");
    }
    private void Update()
    {

#if UNITY_EDITOR
        if (Input.GetMouseButtonDown(0))
        {
            Vector2 touchPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            RaycastHit hit;

            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit))
            {
                GameObject touchedObject = hit.transform.gameObject;
                if (!touchedObject.GetComponent<EditCell>())
                    return;
                touchedObject.GetComponent<EditCell>().Toggle();
            }
        }
#else    
        if (Input.touchCount == 1)
        {
            if(isClicked)
                return;
            isClicked = true;
            Vector2 touchPos = Camera.main.ScreenToWorldPoint(Input.GetTouch(0).position);

            RaycastHit hit;

            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit))
            {
                GameObject touchedObject = hit.transform.gameObject;
                if (!touchedObject.GetComponent<EditCell>())
                    return;
                touchedObject.GetComponent<EditCell>().Toggle();
            }
        }
        else if(Input.touchCount == 0)
        {
            isClicked = false;
        }
#endif
    }
}
