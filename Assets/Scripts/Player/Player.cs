using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class Player : MonoBehaviour 
{
    public int MaxHealth;
    public int Health;
    public int MaxShield;
    public int Shield;
    public float ViewRadius;

    [Header("===Graphics===")]
    public SpriteRenderer[] sprites;
    public Material DamageMaterial;
    public AudioClip HurtSound;
    Material startMaterial;
    [Space]
    [Space]
    [Header("===UI===")]
    public Image HealthBar;
    public Image Big_HealthBar;
    public Image ShieldhBar;

    private void Start() 
    {
        Health = MaxHealth;

        startMaterial = sprites[0].material;

        HealthBar.fillAmount = (float)Health / (float)MaxHealth;
        ShieldhBar.fillAmount = (float)Shield / (float)MaxShield;
    }

    private void Update() 
    {
        Big_HealthBar.fillAmount = HealthBar.fillAmount;
        ShieldhBar.fillAmount = (float)Shield / (float)MaxShield;

        Shield = Mathf.Clamp(Shield, 0, MaxShield);
    }

    public void AddHealth(int addhealth)
    {
        Health += addhealth;
        HealthBar.fillAmount = (float)Health / (float)MaxHealth;

        //Freeze Time When Hit();
        //GameManager.FreezeTime(0.1f, 0.0f);
    }

    public void AddShield(int addShield)
    {
        Shield += addShield;
    }

    public void GetDamage(int damage)
    {

        AudioSource.PlayClipAtPoint(HurtSound, transform.position, 1.0f);
        if(Shield <= 0)
        {
            Health -= damage;
            Shield = 0;
        }
        else
        {
            if(Shield < damage)
            {
                var dmg = damage - Shield;
                Shield -= Shield;
                Health -= dmg;
            }
            else
            {
                Shield -= damage;
            }
        }

        for (int i = 0; i < sprites.Length; i++)
        {
            StartCoroutine(ShowDamage(sprites[i], 0.1f));
        }

        HealthBar.fillAmount = (float)Health / (float)MaxHealth;

        //Freeze Time When Hit();
        //GameManager.FreezeTime(0.1f, 0.0f);

        if(Health <= 0)
        {
            GameManager.instance.DoGameOver();
        }
    }

    private void OnDrawGizmosSelected() 
    {
        Gizmos.DrawWireSphere(transform.position, ViewRadius);    
    }

    IEnumerator ShowDamage(SpriteRenderer spr, float t)
    {
        Material m = startMaterial;
        spr.material = DamageMaterial;
        yield return new WaitForSeconds(t);
        spr.material = m;

    }
}