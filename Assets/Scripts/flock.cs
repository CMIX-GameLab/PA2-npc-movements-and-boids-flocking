using Unity.VisualScripting;
using UnityEngine;

public class flock : MonoBehaviour
{

    float speed;
    bool youShallNotPass = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        speed = Random.Range(boidStuffs.BS.minSpeed, boidStuffs.BS.maxSpeed);
    }

    // Update is called once per frame
    void Update()
    {
        Bounds b = new Bounds(boidStuffs.BS.transform.position, boidStuffs.BS.area * 2);

        if (!b.Contains(transform.position))
        {
            youShallNotPass = true;
        }
        else
        {
            youShallNotPass = false;
        }

        if (youShallNotPass)
        {
            Vector3 dir2 = boidStuffs.BS.transform.position - transform.position;
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(dir2), boidStuffs.BS.rotSpeed * Time.deltaTime);
        }
        else 
        {
            if (Random.Range(0, 100) < 10)
            {
                speed = Random.Range(boidStuffs.BS.minSpeed, boidStuffs.BS.maxSpeed);
            }
            if (Random.Range(0, 100) < 10)
            {
                flockThing();
            }
        }
        
        this.transform.Translate(0, 0, speed * Time.deltaTime);
    }

    void flockThing()
    {
        GameObject[] obs;
        obs = boidStuffs.BS.allBoid;
        Vector3 middle = Vector3.zero;
        Vector3 avoid = Vector3.zero;
        float speed2 = 0.01f;
        float dist;
        int group = 0;

        foreach(GameObject ob in obs)
        {
            if(ob != this.gameObject)
            {
                dist = Vector3.Distance(ob.transform.position, this.transform.position);
                if(dist <= boidStuffs.BS.closeDis)
                {
                    middle += ob.transform.position;
                    group++;

                    if(dist < 1.0f)
                    {
                        avoid = avoid + (this.transform.position - ob.transform.position);
                    }

                    flock difFlock = ob.GetComponent<flock>();
                    speed2 = speed2 + difFlock.speed;
                }
            }
        }
        if(group > 0)
        {
            middle = middle / group + (boidStuffs.BS.goal - this.transform.position);
            speed = speed2 / group;
            if(speed > boidStuffs.BS.maxSpeed)
            {
                speed = boidStuffs.BS.maxSpeed;
            }

            Vector3 newDir = (middle + avoid) - transform.position;
            if(newDir != Vector3.zero)
            {
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(newDir), boidStuffs.BS.rotSpeed * Time.deltaTime);
            }
        }

    }
}
