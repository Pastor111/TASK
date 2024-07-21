using System.Collections;
using System.Collections.Generic;
using Pathfinding;
using UnityEngine;
using UnityEngine.UI;

public class BasicEnemy : EnemyBase
{

    public float Speed;
    public float nextWaypointDistance = 3;
    public float PathUpdateTime = 0.1f;
    public float EnemyRadius;
    [Space]
    [Space]
    [Header("Death")]
    public int DropProbability;
    public GameObject[] DropItems;
    [Space]
    [Space]
    [Header("Graphics")]
    public Animator anim;
    public SpriteRenderer[] sprites;
    public Material DamageMaterial;
    [Space]
    [Space]
    [Header("UI")]
    public Image HealthBar;
    [Space]
    [Space]
    [Header("Death")]
    public float RoarTimer;
    public AudioSource audioSource;
    public AudioClip ZombieHit;
    public AudioClip Roar;

    bool isDead
    {
        get
        {
            return Health <= 0;
        }
    }
    Rigidbody2D rb;
    Path path;
    int currentWaypoint;
    bool reachedEndOfPath;

    // Start is called before the first frame update
    void Start()
    {
        InitialHealth = Health;
        StartCoroutine(RoarPlay());
        target = GameObject.FindGameObjectWithTag("Player").transform;
        rb = GetComponent<Rigidbody2D>();
        StartCoroutine(UpdatePath());
        HealthBar.fillAmount = Health / InitialHealth;
    }

    // Update is called once per frame
    void Update()
    {
        if(isDead)
            return;
        
        float d = Vector2.Distance(transform.position, target.position);

        if(d <= EnemyRadius)
            Move();
        else
            anim.SetBool("Walking", false);
    }

    void Move()
    {
        if (path == null) {
            // We have no path to follow yet, so don't do anything
            return;
        }

         reachedEndOfPath = false;

        // The distance to the next waypoint in the path

        float d = Vector2.Distance(transform.position, path.vectorPath[currentWaypoint]);

        if(d <= nextWaypointDistance)
        {
            if(currentWaypoint + 1 < path.vectorPath.Count)
                currentWaypoint++;
            else
                Debug.Log("Done");
        }

        //var speedFactor = reachedEndOfPath ? Mathf.Sqrt(distanceToWaypoint/nextWaypointDistance) : 1f;

        //Vector2 dir = (path.vectorPath[currentWaypoint] - transform.position).normalized;

        Vector2 pos = Vector2.MoveTowards(rb.position, path.vectorPath[currentWaypoint], Speed * Time.deltaTime);
        //pos = Vector2.ClampMagnitude(pos, 1.0f);
        anim.SetBool("Walking", true);

        if((rb.position.x - path.vectorPath[currentWaypoint].x) > 0.0f)
        {
            transform.localScale = new Vector3(-Mathf.Abs(transform.localScale.x), transform.localScale.y, 1.0f);
        }
        else
        {
            transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, 1.0f);
        }

        //Vector2 velocity = dir * Speed * speedFactor * Time.deltaTime;

        rb.MovePosition(pos);

    }

    IEnumerator UpdatePath()
    {
        while(true)
        {
            yield return new WaitForSeconds(PathUpdateTime);
            seeker.StartPath(transform.position, target.position, OnPathComplete);
        }

    }

    public void OnPathComplete (Path p) {
        Debug.Log("Yay, we got a path back. Did it have an error? " + p.error);

         if (!p.error) {
            path = p;
            // Reset the waypoint counter so that we start to move towards the first point in the path
            currentWaypoint = 0;
        }
    }

    public override void GetDamage(int damage = 1)
    {

        if(isDead)
            return;


        base.GetDamage(damage);


        HealthBar.fillAmount = (float)Health / (float)InitialHealth;

        audioSource.PlayOneShot(ZombieHit);

        if(Health <= 0)
        {
            //Dead
            anim.SetTrigger("Dead");
            anim.SetBool("Walking", false);
            Destroy(gameObject, 5.0f);
            GetComponent<Collider2D>().enabled = false;
            GameManager.instance.AddScore();
        }
        else
        {
            //Alive
            for (int i = 0; i < sprites.Length; i++)
            {
                StartCoroutine(ShowDamage(sprites[i], 0.1f));
            }
        }
    }

    private void OnDestroy() 
    {
        int x = Random.Range(0, 100);

        if(x <= DropProbability)
        {
            var obj = Instantiate(DropItems[Random.Range(0, DropItems.Length)], transform.position, Quaternion.identity);
            Destroy(obj, 5.0f);
        }    
    }

    private void OnCollisionEnter2D(Collision2D other) 
    {
        if(other.collider.CompareTag("Player"))
        {
            if(!isDead)
                target.GetComponent<Player>().GetDamage(10);
        }    
    }

    IEnumerator ShowDamage(SpriteRenderer spr, float t)
    {
        Material m = spr.material;
        spr.material = DamageMaterial;
        yield return new WaitForSeconds(t);
        spr.material = m;

    }

    IEnumerator RoarPlay()
    {
        while(!isDead)
        {
            yield return new WaitForSeconds(RoarTimer);
            audioSource.PlayOneShot(Roar);
        }
    }
}
