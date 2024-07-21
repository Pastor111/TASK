using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using TMPro;

//Contains the ammo information about a gun
[System.Serializable]
public class GunMagazine
{
    public int CurrentMag;
    public int MagLeft;
}


//Class used to easily structure the transform that will point at the enemy
[Serializable]
public class FollowTransform
{
    public Transform t;
    public Vector3 offset;
    public Vector2 limit;
    public bool MinusAngle;
    public float currentAngle;
}


public class GunManager : MonoBehaviour
{   
    //Current weapon/gun
    public Weapon weapon;
    //Information about the players ammo
    public GunMagazine gunMagazine;
    //Transforms that will point at the enemy
    public FollowTransform[] followMouse;
    //Players Body
    public Transform playerBody;
    //Spawn point for all the bullets
    public Transform bulletApper;
    //Bullet prefab
    public GameObject BulletPrefab;
    //Visual effect for a bullet hit
    public GameObject BulletHit;
    //Another shooting visual effect
    public GameObject MuzzleFlash;

    [Space]
    [Space]
    //Text showing the ammo in game
    public TextMeshProUGUI AmmoText;
    //Audio source responsable for playing all the gun sounds
    public AudioSource audioSource;
    //Shooting sound
    public AudioClip ShootingSound;
    //Reload sound
    public AudioClip ReloadSound;

    #region Private
    //Check if player can shoot 
    bool canShoot = true;
    Player player;

    //Closest enemy to the player
    Vector3 closestEnemy;
    //Check if player is shooting (used for mobile input)
    bool isShooting = false;
    #endregion

    // Start is called before the first frame update
    void Start()
    {
        //Get the player component
        player = GetComponent<Player>();
    }

    // Update is called once per frame
    void Update()
    {

        //Update the players input
        GetInput();
        //Update the ammo text on screen
        AmmoText.text = $"{gunMagazine.CurrentMag}/{gunMagazine.MagLeft}";
        
        //Get the closest enemy
        if(GetClosestEnemy(out Vector3 p))
        {
            //If found

            //followMouse[1].offset.z = 0f;


            //Point at it
           for (int i = 0; i < followMouse.Length; i++)
            {
                //LookAtMouse(followMouse[i]);
                LookAtPosition(followMouse[i],p);
            }

            closestEnemy = p;
        }
        else
        {

            //if not found

            Ray r = new Ray(transform.position, transform.right);

            //followMouse[1].offset.z = 90f;

            //make the player look forward
            for (int i = 0; i < followMouse.Length; i++)
            {
                //LookAtMouse(followMouse[i]);
                LookAtPosition(followMouse[i],r.GetPoint(100));
            }

            closestEnemy = r.GetPoint(100);
        }

    }

    #region Mobile Controls

    public void StartShooting()
    {
        isShooting = true;
    }

    public void StopShooting()
    {
        isShooting = false;
    }

    #endregion

    bool GetClosestEnemy(out Vector3 pos)
    {   
        //Get All Objects in a radius
        var col = Physics2D.OverlapCircleAll(transform.position, player.ViewRadius);

        //Check if they found any object
        if(col.Length < 1)
        {
            pos = Vector3.zero;
            return false;
        }

        int lowestDistanceIndex = 0;
        float lowestDistance = 1000f;

        //Loop through all the objects
        for (int i = 0; i < col.Length; i++)
        {   
            //Check if its an enemy
            if(col[i].GetComponent<EnemyBase>() == null)
                continue;

            //Get the distance between the player and the enemy
            float dis = Vector2.Distance(transform.position, col[i].transform.position);

            //Find out if they are the closest one
            if(dis <= lowestDistance)
            {
                lowestDistance = dis;
                lowestDistanceIndex = i;
            }
        }

        if(lowestDistance >= 1000f)
        {
            pos = Vector3.zero;
            return false;
        }
        else
        {

            pos = col[lowestDistanceIndex].transform.position;

            return true;
        }

    }

    void GetInput()
    {
        if(isShooting && gunMagazine.CurrentMag > 0 && canShoot)
        {
            Shoot();
        }

        if(Input.GetKey(KeyCode.F) && gunMagazine.CurrentMag > 0 && canShoot)
        {
            Shoot();
        }

        if(Input.GetKeyDown(KeyCode.R))
        {
            Reload();
        }
    }

    public void Reload()
    {
        //Play reload sound
        audioSource.PlayOneShot(ReloadSound);
        //Get how much ammo to add
        var add = weapon.magazineSize - gunMagazine.CurrentMag;

        //If the ammo required is bigger than the current ammo then set the required amount to the current ammo left
        if(gunMagazine.MagLeft <= add)
            add = gunMagazine.MagLeft;

        //Add ammo
        gunMagazine.CurrentMag += add;
        //Substract ammo
        gunMagazine.MagLeft -= add;
    }
    public void TryShoot()
    {
        if(gunMagazine.CurrentMag > 0 && canShoot)
        {
            Shoot();
        }
    } 

    void Shoot()
    {
        //var mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        //mousePos = GetPointInRadius(mousePos, weapon.Innacuracy);

        //mousePos.z = 0.0f;
        
        //Play shooting sound
        audioSource.PlayOneShot(ShootingSound);
        //Get the dirrection to the enemy
        var dir = (closestEnemy - bulletApper.position).normalized;
        dir.z = 0.0f;

        //Shoot a raycast from the player to the enemies direction
        var rayHit = Physics2D.Raycast(bulletApper.position, dir);

        //Spawn a bullet
        var bullet = Instantiate(BulletPrefab, Vector3.zero, Quaternion.identity);
        Instantiate(MuzzleFlash, bulletApper.position, Quaternion.identity);
        var line = bullet.GetComponent<LineRenderer>();
        line.positionCount = 0;
        line.positionCount = 2;
        //GameObject light = new GameObject("Bullet Light");
        //var l = light.AddComponent<Light2D>();
        //l.lightType = Light2D.LightType.Point;
        //l.color = Color.yellow;
        //l.pointLightOuterRadius = 1.0f;
        //l.pointLightInnerRadius = 0.0f;
        //l.transform.position = bulletApper.position;
        //Destroy(l.gameObject, .1f);

        if(rayHit.collider != null)
        {
            //Hit
            //Set the bullets
            //Set the start and end point of the bullet
            line.SetPosition(0, bulletApper.position);
            line.SetPosition(1, rayHit.point);
            //Instantiate bullet hit effect
            Instantiate(BulletHit, rayHit.point, Quaternion.identity);

            var enemy = rayHit.collider.GetComponent<EnemyBase>();
            
            //if it hit an enemy then damage it
            if(enemy != null)
            {
                enemy.GetDamage(1);
            }
        }
        else
        {
            //No hit

            //Set the bullet start point and raycast 100m forward to get the end point
            line.SetPosition(0, bulletApper.position);
            var point =  new Ray(bulletApper.position, dir).GetPoint(100f);
            line.SetPosition(1, point);
            //Debug.Break();

        }

        canShoot = false;
        gunMagazine.CurrentMag--;
        StartCoroutine(FireDelay(weapon.fireDelay));

        Destroy(line.gameObject, 0.1f);
    }

    void LookAtPosition(FollowTransform t, Vector3 position)
    {
            var mouse_pos = position;
            mouse_pos.z = 0;
            var object_pos = t.t.position;
            mouse_pos.x = mouse_pos.x - object_pos.x;
            mouse_pos.y = mouse_pos.y - object_pos.y;
            mouse_pos.z = 0.0f;
            var angle = Mathf.Atan2(mouse_pos.y, mouse_pos.x) * Mathf.Rad2Deg;

            if(angle > 90 || angle < -90f)
            {
                playerBody.localScale = new Vector3(-Math.Abs(playerBody.localScale.x), playerBody.localScale.y, 1.0f);
                if(t.MinusAngle)
                    angle -= 180f;
            }
            else
            {
                playerBody.localScale = new Vector3(Math.Abs(playerBody.localScale.x), playerBody.localScale.y, 1.0f);
            }

            
            angle = Mathf.Clamp(angle, t.limit.x, t.limit.y);
            
            t.currentAngle = angle;

            t.t.rotation = Quaternion.Euler(0 + t.offset.x, 0 + t.offset.y, angle + t.offset.z);       
    }

    void LookAtMouse(FollowTransform t)
    {
            var mouse_pos = Input.mousePosition;
            mouse_pos.z = 0;
            var object_pos = Camera.main.WorldToScreenPoint(t.t.position);
            mouse_pos.x = mouse_pos.x - object_pos.x;
            mouse_pos.y = mouse_pos.y - object_pos.y;
            var angle = Mathf.Atan2(mouse_pos.y, mouse_pos.x) * Mathf.Rad2Deg;

            if(angle > 90 || angle < -90f)
            {
                playerBody.localScale = new Vector3(-Math.Abs(playerBody.localScale.x), playerBody.localScale.y, 1.0f);
                if(t.MinusAngle)
                    angle -= 180f;
            }
            else
            {
                playerBody.localScale = new Vector3(Math.Abs(playerBody.localScale.x), playerBody.localScale.y, 1.0f);
            }

            
            angle = Mathf.Clamp(angle, t.limit.x, t.limit.y);
            
            t.currentAngle = angle;

            t.t.rotation = Quaternion.Euler(0 + t.offset.x, 0 + t.offset.y, angle + t.offset.z);
            
    }

    
    Vector3 GetPointInRadius(Vector3 centerOfRadius, float radius)
    {
        //Vector3 centerOfRadius = target.position;
        //float radius = Innacuracy;
        Vector3 t = centerOfRadius + (Vector3)(radius * UnityEngine.Random.insideUnitCircle);

        return t;
    }

    private void OnDrawGizmosSelected() 
    {
        Gizmos.DrawLine(transform.position, closestEnemy);        
    }


    IEnumerator FireDelay(float t)
    {
        yield return new WaitForSeconds(t);
        canShoot = true;
    }
}
