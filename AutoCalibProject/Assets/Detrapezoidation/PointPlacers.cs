using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointPlacers : MonoBehaviour
{
    [SerializeField]
    private GameObject pointObjects;

    public bool createPoints = true;

    //points placed in world space at mouse position
    public GameObject p1 { get; private set; }
    public GameObject p2 { get; private set; }
    public GameObject p3 { get; private set; }
    public GameObject p4 { get; private set; }


    private GameObject temp_p1 = null;
    private GameObject temp_p2 = null;
    private GameObject temp_p3 = null;
    private GameObject temp_p4 = null;


    private void OnMouseDown()
    {
        if (createPoints)
        {
            PutPoint(Input.mousePosition);
        }
    }

    //points creation method
    public void PutPoint(Vector2 mousePosition)
    {
        RaycastHit hit = RayFromCamera(mousePosition, 1000f);
        if (!temp_p1)
        {
            temp_p1 = new GameObject("p1");
            temp_p1.transform.position = hit.point;
        }
        else if (!temp_p2)
        {
            temp_p2 = new GameObject("p2");
            temp_p2.transform.position = hit.point;
        }
        else if (!temp_p3)
        {
            temp_p3 = new GameObject("p3");
            temp_p3.transform.position = hit.point;
        }
        else if (!temp_p4)
        {
            temp_p4 = new GameObject("p4");
            temp_p4.transform.position = hit.point;
            createPoints = false;
            SortPoints(temp_p1, temp_p2, temp_p3, temp_p4);
        }
    }

    //ray from camera to position the point
    public RaycastHit RayFromCamera(Vector3 mousePosition, float rayLength)
    {
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(mousePosition);
        Physics.Raycast(ray, out hit, rayLength);
        return hit;
    }

    //set points in the right order
    private void SortPoints(GameObject t1, GameObject t2, GameObject t3, GameObject t4)
    {
        float meanX = (t1.transform.position.x + t2.transform.position.x + t3.transform.position.x + t4.transform.position.x) / 4;
        float meanY = (t1.transform.position.y + t2.transform.position.y + t3.transform.position.y + t4.transform.position.y) / 4;

        GameObject[] allPoints = new GameObject[] { t1, t2, t3, t4 };

        foreach (GameObject point in allPoints)
        {
            float pX = point.transform.position.x;
            float pY = point.transform.position.y;
            if (pX < meanX && pY < meanY)
            {
                p1 = point;
            }
            else if (pX > meanX && pY < meanY)
            {
                p2 = point;
            }
            else if (pX > meanX && pY > meanY)
            {
                p3 = point;
            }
            else if (pX < meanX && pY > meanY)
            {
                p4 = point;
            }
            else
            {
                Debug.LogErrorFormat("Screen setup toooooo extreme");
            }
        }

    }
}
