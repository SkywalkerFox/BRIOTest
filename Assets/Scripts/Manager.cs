using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using UnityEngine;

public class Manager : MonoBehaviour
{
    public static bool creating = false;

    public GameObject sourceGO;
    public GameObject recieverGO1;
    public GameObject recieverGO2;
    public GameObject recieverGO3;

    private Source sourceComp;
    private Reciever recieverComp1;
    private Reciever recieverComp2;
    private Reciever recieverComp3;

    private void Awake()
    {
        sourceComp = sourceGO.GetComponent<Source>();
        recieverComp1 = recieverGO1.GetComponent<Reciever>();
        recieverComp2 = recieverGO2.GetComponent<Reciever>();
        recieverComp3 = recieverGO3.GetComponent<Reciever>();

        Setup();

    }

    void Start()
    {
        Debug.Log(Application.persistentDataPath);
    }

    void Update()
    {

    }

    private void Setup()
    {
        using(StreamReader inputReader = new StreamReader("Assets/Resources/input.txt"))
        {
            string line = inputReader.ReadLine();
            string[] lineSplit = line.Split(',');

            recieverComp1.SetPosition(new Vector3(float.Parse(lineSplit[0], CultureInfo.InvariantCulture.NumberFormat), 0,
                float.Parse(lineSplit[1], CultureInfo.InvariantCulture.NumberFormat)));
            recieverComp2.SetPosition(new Vector3(float.Parse(lineSplit[2], CultureInfo.InvariantCulture.NumberFormat), 0,
                float.Parse(lineSplit[3], CultureInfo.InvariantCulture.NumberFormat)));
            recieverComp3.SetPosition(new Vector3(float.Parse(lineSplit[4], CultureInfo.InvariantCulture.NumberFormat), 0,
                float.Parse(lineSplit[5], CultureInfo.InvariantCulture.NumberFormat)));

            while ((line = inputReader.ReadLine()) != null)
            {
                lineSplit = line.Split(',');

                recieverComp1.AddTime(float.Parse(lineSplit[0], CultureInfo.InvariantCulture.NumberFormat));
                recieverComp2.AddTime(float.Parse(lineSplit[1], CultureInfo.InvariantCulture.NumberFormat));
                recieverComp3.AddTime(float.Parse(lineSplit[2], CultureInfo.InvariantCulture.NumberFormat));
            }

            for (int i = 0; i < recieverComp1.GetTimesList().Count; i++)
            {
                sourceComp.AddPosition(FindSourcePosition(recieverComp1.GetPosX(), recieverComp1.GetPosY(), recieverComp1.GetDistanceToSource(i),
                    recieverComp2.GetPosX(), recieverComp2.GetPosY(), recieverComp2.GetDistanceToSource(i),
                    recieverComp3.GetPosX(), recieverComp3.GetPosY(), recieverComp3.GetDistanceToSource(i)));
            }

            inputReader.Close();
        }

    }

    public void Simulate()
    {
        creating = false;
        sourceComp.trailRenderer.enabled = true;

        StartCoroutine(sourceComp.MoveSource());
    }

    public void StopSimulate()
    {
        StopAllCoroutines();

        sourceGO.transform.position = Vector3.zero;
        sourceComp.trailRenderer.Clear();
    }

    public void Create()
    {
        creating = true;

        sourceComp.trailRenderer.enabled = false;
    }

    public void StopCreate()
    {
        recieverComp1.SetPosition(recieverGO1.transform.position);
        recieverComp2.SetPosition(recieverGO2.transform.position);
        recieverComp3.SetPosition(recieverGO3.transform.position);

        creating = false;

        sourceComp.trailRenderer.enabled = false;
    }

    public void CreateTrajectory()
    {
        sourceComp.trailRenderer.enabled = true;

        StartCoroutine(sourceComp.CreatePath(recieverComp1, recieverComp2, recieverComp3));
    }

    public void StopCreateTrajectory()
    {
        StopAllCoroutines();

        sourceGO.transform.position = Vector3.zero;
        sourceComp.trailRenderer.Clear();
        sourceComp.trailRenderer.enabled = false;
    }

    public void SaveTrajectory()
    {
        StopCreateTrajectory();

        recieverComp1.SetPosition(recieverGO1.transform.position);
        recieverComp2.SetPosition(recieverGO2.transform.position);
        recieverComp3.SetPosition(recieverGO3.transform.position);

        // using(StreamWriter outputWriter = new StreamWriter(Application.persistentDataPath + "output.txt"))
        using(StreamWriter outputWriter = new StreamWriter("Assets/Resources/output.txt"))
        {

            for (int i = 0; i < sourceComp.GetPath().Count; i++)
            {
                outputWriter.WriteLine(sourceComp.GetPath() [i].x.ToString("0.00000000", CultureInfo.InvariantCulture.NumberFormat) + "," +
                    sourceComp.GetPath() [i].z.ToString("0.00000000", CultureInfo.InvariantCulture.NumberFormat));
            }
            outputWriter.Close();
        }

        // using(StreamWriter inputWriter = new StreamWriter(Application.persistentDataPath + "input.txt"))
        using(StreamWriter inputWriter = new StreamWriter("Assets/Resources/input.txt"))
        {

            inputWriter.WriteLine(recieverComp1.GetPosX().ToString("0.00000000", CultureInfo.InvariantCulture.NumberFormat) + "," +
                recieverComp1.GetPosY().ToString("0.00000000", CultureInfo.InvariantCulture.NumberFormat) + "," +
                recieverComp2.GetPosX().ToString("0.00000000", CultureInfo.InvariantCulture.NumberFormat) + "," +
                recieverComp2.GetPosY().ToString("0.00000000", CultureInfo.InvariantCulture.NumberFormat) + "," +
                recieverComp3.GetPosX().ToString("0.00000000", CultureInfo.InvariantCulture.NumberFormat) + "," +
                recieverComp3.GetPosY().ToString("0.00000000", CultureInfo.InvariantCulture.NumberFormat));
            for (int i = 0; i < recieverComp1.GetTimesList().Count; i++)
            {
                inputWriter.WriteLine(recieverComp1.GetTime(i).ToString("0.00000000", CultureInfo.InvariantCulture.NumberFormat) + "," +
                    recieverComp2.GetTime(i).ToString("0.00000000", CultureInfo.InvariantCulture.NumberFormat) + "," +
                    recieverComp3.GetTime(i).ToString("0.00000000", CultureInfo.InvariantCulture.NumberFormat));
            }
            inputWriter.Close();
        }
    }

    private Vector3 FindSourcePosition(float x0, float y0, float r0,
        float x1, float y1, float r1,
        float x2, float y2, float r2)
    {
        float x, y, xUp, xDown, yUp, yDown;

        yUp = ((x1 - x2) * ((x1 * x1 - x0 * x0) + (y1 * y1 - y0 * y0) + (r0 * r0 - r1 * r1)) - (x0 - x1) * ((x2 * x2 - x1 * x1) + (y2 * y2 - y1 * y1) + (r1 * r1 - r2 * r2)));
        yDown = (2 * ((y0 - y1) * (x1 - x2) - (y1 - y2) * (x0 - x1)));
        y = yUp / yDown * -1;

        xUp = ((y1 - y2) * ((y1 * y1 - y0 * y0) + (x1 * x1 - x0 * x0) + (r0 * r0 - r1 * r1)) - (y0 - y1) * ((y2 * y2 - y1 * y1) + (x2 * x2 - x1 * x1) + (r1 * r1 - r2 * r2)));
        xDown = (2 * ((x0 - x1) * (y1 - y2) - (x1 - x2) * (y0 - y1)));
        x = xUp / xDown * -1;

        return new Vector3(x, 0, y);
    }
}