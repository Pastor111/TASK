using System.Collections;
using System.Collections.Generic;
using Pathfinding;
using UnityEngine;

public class EnemyBase : MonoBehaviour
{
    public int Health;
    [HideInInspector]
    public int InitialHealth;
    public Seeker seeker;

    [HideInInspector]
    public Transform target;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public virtual void GetDamage(int damage = 1)
    {
        Health -= damage;
    }
}
