using NUnit.Framework.Internal.Commands;
using UnityEngine;

public class NpcMovementStuff : MonoBehaviour
{
    public Rigidbody rb;
    public GameObject[] obsticles;
    public Vector3 acceleration, velocity, position, posDif, targetVec, wanderVec;
    public float maxSpeed = 5, maxForce = 4, fleeDist = 5, dist, flyHeight = 5, flyDist = 0.1f, groundDist, lookAhead, lookDist = 5, avoidanceForce, hideDist = 3;
    public float fastSpeed = 8, slowSpeed = 3, flySpeed = 1, grav = 5, wanderDist, wanderRad, wanderAngle = 0f, wanderRand, rayOfset = 2, smoothTime = 0.05f;
    public int obNum = 0;
    public GameObject targetObject, closestObject;
    public Transform target;
    public Vector3 desiredVelocity, steering, distVec;
    public bool canFly, wander, avoidance, hide;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        position = transform.position;
        target = targetObject.transform;
        closestObject = obsticles[0];
        
    }

    // Update is called once per frame

    private void Update()
    {

    }

    public enum moveType{
        seek,
        flee,
        arive,
        pursuit,
        evade,
        wander,
        hide
    }
    public moveType type;
    void FixedUpdate()
    {
        PlayerMovement playMov = targetObject.GetComponent<PlayerMovement>();
        Vector3 playVel = playMov.moveVec;
        Vector3 desiredVelocity;
        Vector3 distace;
        Vector3 closeDist = new Vector3(999999999,999999999,999999999);
        float playSpeed = playVel.magnitude;
        float orbSpeed = velocity.magnitude;
        for(int i = 1000; i < obsticles.Length; i++)
        {
            distace = obsticles[i].transform.position - transform.position;
            if(distace.magnitude < closeDist.magnitude)
            {
                closestObject = obsticles[i];
                closeDist = distace;
            }
            
        }

        float targetAngle = Mathf.Atan2(velocity.x, velocity.z) * Mathf.Rad2Deg;
        float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref orbSpeed, smoothTime);
        transform.rotation = Quaternion.Euler(0f, angle, 0f);
        wanderVec = transform.forward * wanderDist;
        if (wander && !hide)
        {
            wanderVec.y = transform.position.y;
            targetVec = wanderVec;
            if (wanderAngle >= 360)
            {
                wanderAngle -= 360;
            }
            
            bool randomBool = UnityEngine.Random.value > 0.5f;
            if (randomBool)
            {
                wanderAngle += wanderRand + 3;
            }
            else
            {
                wanderAngle -= wanderRand;
            }

            targetVec.x += wanderRad * Mathf.Sin(wanderAngle * Mathf.Deg2Rad);
            targetVec.z += wanderRad * Mathf.Cos(wanderAngle * Mathf.Deg2Rad);
            
        }
        else if(!wander && hide)
        {
            targetVec = -(target.position - closestObject.transform.position);
        }
        else
        {
            target = targetObject.transform;
            targetVec = target.position;
        }
        distVec = target.position - transform.position;
        dist = distVec.magnitude;
        
        Ray ray = new Ray(transform.position, -transform.up);
        Vector3 mover = new Vector3(0, 0, 0);
        posDif = targetVec - transform.position;
        posDif.y = 0;
        switch (type)
        {
            case moveType.seek:
                wander = false;
                hide = false;

                desiredVelocity = (posDif).normalized * maxSpeed;
                steering = Vector3.ClampMagnitude(desiredVelocity - velocity, maxForce);
                acceleration = steering;
                velocity += acceleration * Time.fixedDeltaTime;
                position += velocity * Time.fixedDeltaTime;
                break;

            case moveType.flee:
                wander = false;
                hide = false;

                if (dist < fleeDist)
                {
                    desiredVelocity = -(posDif).normalized * maxSpeed;
                    steering = Vector3.ClampMagnitude(desiredVelocity - velocity, maxForce);
                    acceleration = steering;
                    velocity += acceleration * Time.fixedDeltaTime;
                    position += velocity * Time.fixedDeltaTime;
                }

            break;

            case moveType.arive:
                wander = false;
                hide = false;

                if (dist > 10)
                {
                    desiredVelocity = (posDif).normalized * fastSpeed;                    
                }
                else if (dist < 10 && dist > 5)
                {
                    desiredVelocity = (posDif).normalized * maxSpeed;                    
                }
                else
                {
                    desiredVelocity = (posDif).normalized * slowSpeed;   
                }
                steering = Vector3.ClampMagnitude(desiredVelocity - velocity, maxForce);
                acceleration = steering;
                velocity += acceleration * Time.fixedDeltaTime;
                position += velocity * Time.fixedDeltaTime;
            break;

            case moveType.pursuit:
                wander = false;
                hide = false;

                lookAhead = dist / (orbSpeed + playSpeed);

                desiredVelocity = (posDif + (playVel * lookAhead)).normalized * maxSpeed;
                steering = Vector3.ClampMagnitude(desiredVelocity - velocity, maxForce);
                acceleration = steering;
                velocity += acceleration * Time.fixedDeltaTime;
                position += velocity * Time.fixedDeltaTime;
                break;

            case moveType.evade:
                wander = false;
                hide = false;

                if (dist < fleeDist)
                {
                    desiredVelocity = -(posDif + playVel).normalized * maxSpeed;
                    steering = Vector3.ClampMagnitude(desiredVelocity - velocity, maxForce);
                    acceleration = steering;
                    velocity += acceleration * Time.fixedDeltaTime;
                    position += velocity * Time.fixedDeltaTime;
                }
            break;

            case moveType.wander:
                wander = true;
                hide = false;
                desiredVelocity = (posDif).normalized * maxSpeed;
                steering = Vector3.ClampMagnitude(desiredVelocity - velocity, maxForce);
                acceleration = steering;
                velocity += acceleration * Time.fixedDeltaTime;
                position += velocity * Time.fixedDeltaTime;

            break;

            case moveType.hide:
                wander = false;
                hide = true;
                desiredVelocity = (posDif ).normalized * maxSpeed;
                steering = Vector3.ClampMagnitude(desiredVelocity - velocity, maxForce);
                acceleration = steering;
                velocity += acceleration * Time.fixedDeltaTime;
                position += velocity * Time.fixedDeltaTime;
            break;
        }

        if (avoidance)
        {
            Vector3 Ray1pos = transform.position + transform.right * rayOfset, Ray2pos = transform.position - transform.right * rayOfset;
            Ray rayR = new Ray(Ray1pos, transform.forward);
            Ray rayL = new Ray(Ray2pos, transform.forward);
            RaycastHit hit;

            if (Physics.Raycast(rayR, out hit, velocity.magnitude * lookDist) || Physics.Raycast(rayL, out hit, velocity.magnitude * lookDist))
            {
                if (hit.collider.CompareTag("Obsticle"))
                {
                    acceleration = steering + (transform.right * avoidanceForce);
                    velocity += acceleration * Time.fixedDeltaTime;
                    position += velocity * Time.fixedDeltaTime;
                    
                }
            }
        }

        rb.MovePosition(position);

        if (canFly) {
            
            if (Physics.Raycast(ray, out RaycastHit hit, 100f))
            {

                float distToTarget = flyHeight - Mathf.Abs(hit.distance);
                if (hit.distance > flyHeight)
                {
                    position.y -= flySpeed * Time.deltaTime;
                }
                else if (hit.distance < flyHeight)
                {
                    position.y += flySpeed * Time.deltaTime;
                }
                else if(distToTarget < flyDist || hit.distance == flyHeight)
                {
                    position.y = flyHeight;
                }
                
            }
        }
        else if(!canFly)
        {
            
            //rb.AddForce(Vector3.down * grav);
            rb.MovePosition(rb.position + Vector3.down * grav * Time.deltaTime);
        }
        
    }

    private void OnDrawGizmos()
    {
        Vector3 circ1 = wanderVec;
        Vector3 circ2 = targetVec;
        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(circ1, wanderRad);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(circ2, .2f);

        Vector3 Ray1pos = transform.position + transform.right * rayOfset, Ray2pos = transform.position - transform.right * rayOfset;
        Gizmos.color = Color.lawnGreen;
        Gizmos.DrawRay(Ray1pos, velocity * lookDist);
        Gizmos.DrawRay(Ray2pos, velocity * lookDist);
        Gizmos.color = Color.blueViolet;
        Gizmos.DrawRay(transform.position, transform.forward * lookDist);
        Gizmos.DrawLine(transform.position, targetVec);
    }
}
