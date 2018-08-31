using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Calibrator : MonoBehaviour
{

    #region VARIABLE

    private GameObject p1 { get { return pP.p1; } }
    private GameObject p2 { get { return pP.p2; } }
    private GameObject p3 { get { return pP.p3; } }
    private GameObject p4 { get { return pP.p4; } }

    [SerializeField] [HideInInspector] private GameObject pivot;
    [SerializeField] [HideInInspector] private GameObject cornerT_R;
    [SerializeField] [HideInInspector] private GameObject cornerB_L;
    [SerializeField] [HideInInspector] private GameObject cornerB_R;

    [SerializeField] [HideInInspector] private RenderTexture filmedTexture;
    [SerializeField]
    private Camera sceneCamera;

    public float boardWidth = 1.1f;
    public float boardHeight = 0.75f;

    [SerializeField] [HideInInspector] private PointPlacers pP;

    [SerializeField] [HideInInspector] private Camera calibCam;
    [SerializeField] [HideInInspector] private GameObject plane;

    #endregion

    // --------------------------------------------------------------------

    #region UNITY FUNCTIONS

    void Start()
    {
        if (sceneCamera)
            StartCoroutine(Calibrate());
        else
        {
            Debug.LogError("MISSING ASSETS IN INSPECTOR");
        }
    }

    #endregion

    // --------------------------------------------------------------------

    #region EDITOR FUNCTIONS

    public void SetupCalibrator()
    {
        name = "Calibrator";
        CleanCalibrator();
        CreateCam();
        CreateClickingStuff();
        CreateScreen();
        foreach (Transform item in GetComponentsInChildren<Transform>())
        {
            item.gameObject.layer = 13;
        }
        SetupSize();
    }

    public void CleanCalibrator()
    {
        Transform[] allTr = GetComponentsInChildren<Transform>();
        while (allTr.Length > 1)
        {
            DestroyImmediate(allTr[1].gameObject);
            allTr = GetComponentsInChildren<Transform>();
        }
        sceneCamera.targetTexture = null;
    }

    private void CreateCam()
    {
        if (Camera.main)
        {
            Camera.main.tag = "Untagged";
        }
        calibCam = new GameObject("CalibCam").AddComponent<Camera>();
        calibCam.transform.SetParent(transform);
        calibCam.tag = "MainCamera";
        calibCam.transform.localPosition = new Vector3(0, 1, -13);
        calibCam.clearFlags = CameraClearFlags.SolidColor;
        calibCam.backgroundColor = Color.black;
        //13 = layer Detrapezoidator
        calibCam.cullingMask = 13 << 13;
    }

    private void CreateClickingStuff()
    {
        Canvas clickingCanvas = new GameObject("Clicking Support").AddComponent<Canvas>();
        clickingCanvas.transform.SetParent(transform);
        clickingCanvas.renderMode = RenderMode.ScreenSpaceCamera;
        clickingCanvas.planeDistance = 13.1f;
        clickingCanvas.worldCamera = calibCam;
        RectTransform rT = clickingCanvas.GetComponent<RectTransform>();
        rT.localPosition = new Vector3(0, 1, 0.1f);
        rT.sizeDelta = new Vector2(1920, 1080);
        rT.localScale = Vector3.one * 0.014f;

        GameObject clickZone = new GameObject("clickZone");
        clickZone.transform.SetParent(clickingCanvas.transform);
        clickZone.transform.localPosition = Vector3.zero;
        clickZone.transform.localScale = Vector3.one;
        clickZone.AddComponent<BoxCollider>().size = new Vector3(2500, 1500, 10);
        pP = clickZone.AddComponent<PointPlacers>();
    }

    private void CreateScreen()
    {
        pivot = new GameObject("Pivot - TopLeft");
        pivot.transform.SetParent(transform);
        pivot.transform.localPosition = new Vector3(-5, 5, 3);

        cornerB_L = new GameObject("BottomLeft");
        cornerB_L.transform.SetParent(pivot.transform);
        cornerB_L.transform.localPosition = new Vector3(0, -10, 0);

        cornerB_R = new GameObject("BottomRight");
        cornerB_R.transform.SetParent(pivot.transform);
        cornerB_R.transform.localPosition = new Vector3(10, -10, 0);

        cornerT_R = new GameObject("TopRight");
        cornerT_R.transform.SetParent(pivot.transform);
        cornerT_R.transform.localPosition = new Vector3(10, 0, 0);

        plane = GameObject.CreatePrimitive(PrimitiveType.Plane);
        plane.GetComponent<Renderer>().enabled = false;
        plane.transform.SetParent(pivot.transform);
        plane.transform.localPosition = new Vector3(5, -5, 0);
        plane.transform.localRotation = Quaternion.Euler(90, 180, 0);
        //plane.GetComponent<Renderer>().material = screenMat;
    }

    public void SetupSize()
    {
        float ratio = boardWidth / boardHeight;
        sceneCamera.rect = new Rect(0, 0, (int)(1000 * boardWidth), (int)(1000 * boardHeight));
        pivot.transform.localScale = new Vector3(boardWidth, boardHeight, 1);
    }

    #endregion

    // --------------------------------------------------------------------

    #region RUNTIME FUNCTIONS

    public IEnumerator Calibrate()
    {
        SetupSize();

        print("GetCorners");
        yield return InputCorners();

        RotateCameraOnZ();
        PlaceScreen(); // 1 Place pivot On Corner
        StretchScreen(); // 2 scale to stretch other corner
        FinalAdjustment(); // rotate to place final corner

        SetupTexture();

        print("Done");
    }

    public IEnumerator InputCorners()
    {
        pP.createPoints = true;
        while (!p1 || !p2 || !p3 || !p4)
        {
            yield return new WaitForEndOfFrame();
        }
    }

    private void PlaceScreen()
    {
        pivot.transform.position = p4.transform.position;
    }
    private void StretchScreen()
    {
        while (cornerT_R.transform.position.x < p3.transform.position.x)
        {
            pivot.transform.transform.localScale *= 1.1f;
        }
        while (cornerT_R.transform.position.x > p3.transform.position.x)
        {
            pivot.transform.transform.localScale *= 0.999f;
        }
    }
    private void RotateCameraOnZ()
    {
        float pente = (p4.transform.position.y - p3.transform.position.y) / (p4.transform.position.x - p3.transform.position.x);
        float angle = -Mathf.Atan(pente);
        calibCam.transform.Rotate(0, 0, angle * Mathf.Rad2Deg);
    }
    private void FinalAdjustment()
    {
        Vector3 camPos = Camera.main.transform.position;
        Vector3 cornerPos = cornerB_L.transform.position;
        Vector3 p1Pos = p1.transform.position;

        //Ray rayCorner = new Ray(camPos, cornerPos - camPos);
        //Ray rayP1 = new Ray(camPos, p1Pos - camPos);

        float angle = Vector3.Angle(cornerPos - camPos, p1Pos - camPos);
        float angleP;
        float rotationFactor = -0.001f;
        if (cornerPos.x > p1Pos.x)
        {
            rotationFactor *= -1;
        }

        int i = 0;
        do
        {
            angleP = angle;
            pivot.transform.Rotate(rotationFactor, 0, 0);
            Debug.DrawRay(camPos, cornerPos - camPos);
            Debug.DrawRay(camPos, p1Pos - camPos);

            cornerPos = cornerB_L.transform.position;
            p1Pos = p1.transform.position;

            angle = Vector3.Angle(cornerPos - camPos, p1Pos - camPos);
            i++;
        } while (angle < angleP && i < 1000000);
    }

    private void SetupTexture()
    {
        filmedTexture = new RenderTexture((int)(1000 * boardWidth), (int)(1000 * boardHeight), 1);

        Material newMat = new Material(Shader.Find("Unlit/Texture"));
        newMat.mainTexture = filmedTexture;

        plane.GetComponent<Renderer>().material = newMat;
        plane.GetComponent<Renderer>().enabled = true;
        sceneCamera.targetTexture = filmedTexture;
        sceneCamera.cullingMask = ~(1 << 13);
    }

    #endregion

}
