using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Source : MonoBehaviour
{
    [SerializeField]
    private float speed = 20;
    [HideInInspector]
    public TrailRenderer trailRenderer;

    private float seconds;
    private List<Vector3> path;

    private void Start()
    {
        trailRenderer = GetComponent<TrailRenderer>();
        trailRenderer.Clear();
    }

    public void AddPosition(Vector3 position)
    {
        if (path == null)
            path = new List<Vector3>();

        path.Add(position);
    }

    public List<Vector3> GetPath()
    {
        return path;
    }

    public IEnumerator CreatePath(Reciever reciever1, Reciever reciever2, Reciever reciever3)
    {
        path.Clear();
        reciever1.GetTimesList().Clear();
        reciever2.GetTimesList().Clear();
        reciever3.GetTimesList().Clear();

        while (trailRenderer.enabled)
        {
            path.Add(transform.position);
            
            reciever1.AddTime(Vector3.Distance(transform.position, reciever1.transform.position) / 1000000);
            reciever2.AddTime(Vector3.Distance(transform.position, reciever2.transform.position) / 1000000);
            reciever3.AddTime(Vector3.Distance(transform.position, reciever3.transform.position) / 1000000);

            yield return new WaitForSeconds(1f);
        }

        yield return false;
    }

    public IEnumerator MoveSource()
    {
        transform.position = path[0];
        trailRenderer.Clear();

        for (int i = 0; i < path.Count; i++)
        {
            Vector3 startPos = transform.position;
            float timer = 0f;
            while (timer <= 1f)
            {
                timer += Time.deltaTime * speed;
                Vector3 newPos = Vector3.Lerp(startPos, path[i], timer);
                transform.position = newPos;

                yield return new WaitForEndOfFrame();
            }

            transform.position = path[i];

            startPos = path[i];
        }

        yield return false;
    }
}