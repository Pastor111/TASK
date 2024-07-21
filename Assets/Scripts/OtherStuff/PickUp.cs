using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickUp : MonoBehaviour
{
    public enum PickUpType{Bullets, Item}
    public PickUpType type;
    public Item item;
    public int Amount;
    public AudioClip PickUpSound;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D other) 
    {
        if(other.CompareTag("Player"))
        {
            if(type == PickUpType.Bullets)
            {
                FindObjectOfType<GunManager>().gunMagazine.MagLeft += Amount;
                GameManager.instance.ShowNotification(1f, $"Ammo +{Amount}", Color.white);
               
            }
            else if(type == PickUpType.Item)
            {
                FindObjectOfType<Inventory>().AddItem(item);
                GameManager.instance.ShowNotification(1f, $"Picked up {item.Name}", Color.white);
            }
            else
            {
                
            }
            AudioSource.PlayClipAtPoint(PickUpSound, transform.position, 1.0f);
            Destroy(gameObject);
        }
    }
}
