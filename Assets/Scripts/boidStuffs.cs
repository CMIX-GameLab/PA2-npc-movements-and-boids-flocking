using UnityEngine;

public class boidStuffs : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    public static boidStuffs BS;
    public GameObject boid;
    public GameObject[] allBoid;
    public int numBoid = 20;
    public Vector3 area = new Vector3(5, 5, 5);
    public Vector3 goal = Vector3.zero;

    public float minSpeed = 2;
    public float maxSpeed = 5;

    public float closeDis = 5;
    public float rotSpeed = 2;

    void Start()
    {
        allBoid = new GameObject[numBoid];
        for(int i = 0; i < numBoid; i++)
        {
            Vector3 pos = this.transform.position + new Vector3(Random.Range(-area.x, area.x), Random.Range(-area.y, area.y), Random.Range(-area.z, area.z));

            allBoid[i] = Instantiate(boid, pos, Quaternion.identity);
        }
        BS = this;
        goal = this.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if(Random.Range(0, 1000) < 10)
        {
            goal = this.transform.position + new Vector3(Random.Range(-area.x, area.x), Random.Range(-area.y, area.y), Random.Range(-area.z, area.z));
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.bisque;
        Gizmos.DrawWireSphere(goal,.3f);

        Gizmos.color = new Color(0f, 0.5f, 1f, 0.3f);
        Gizmos.DrawCube(transform.position, area * 2.5f);
        Gizmos.color = Color.black;
        Gizmos.DrawWireCube(transform.position, area * 2.5f);
    }
}
