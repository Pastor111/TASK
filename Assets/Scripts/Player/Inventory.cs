using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ItemSlot
{
    public Item item;
    public int amount;
}

public class Inventory : MonoBehaviour
{
    [Header("UI")]
    public GameObject ItemObj;
    public GameObject InventoryPanel;
    [Space]
    [Space]
    [Space]
    public Image BigIcon;
    public TextMeshProUGUI NameText;
    public TextMeshProUGUI AmountText;
    public Button UseButton;
    public Button DeleteButton;
    private ItemSlot itemSelected;
    private Dictionary<Item, int> inventory;

    // Start is called before the first frame update
    void Start()
    {
        inventory = new Dictionary<Item, int>();
    }

    // Update is called once per frame
    void Update()
    {
        #if UNITY_EDITOR_WIN
            if(Input.GetKeyDown(KeyCode.I))
            {
                InventoryPanel.SetActive(!InventoryPanel.activeInHierarchy);
                UpdateInventoryUI();
            }
        #endif
    }

    public void UpdateInventoryUI()
    {
        foreach (Transform child in ItemObj.transform.parent)
        {
            if(child.gameObject != ItemObj)
            {
                Destroy(child.gameObject);
            }
        }

        var inv = GetAllItemSlots();

        for (int i = 0; i < inv.Length; i++)
        {
            var slot = Instantiate(ItemObj.gameObject, ItemObj.transform.parent);
            slot.SetActive(true);
            ShowItemInCell(inv[i], slot);
            ShowItemInfo();
        }
    }

    void ShowItemInCell(ItemSlot item, GameObject slot)
    {
        slot.transform.GetChild(0).GetChild(0).GetComponent<Image>().sprite = item.item.image;
        slot.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = item.amount.ToString();
        slot.GetComponent<Button>().onClick.AddListener(()=>
        {
            itemSelected = item;
            ShowItemInfo();
        });
    }

    void ShowItemInfo()
    {
        if(itemSelected == null)
        {
            BigIcon.gameObject.SetActive(false);
            NameText.gameObject.SetActive(false);
            AmountText.gameObject.SetActive(false);
            UseButton.gameObject.SetActive(false);
            DeleteButton.gameObject.SetActive(false);
            return;
        }

        BigIcon.gameObject.SetActive(true);
        NameText.gameObject.SetActive(true);
        AmountText.gameObject.SetActive(true);
        UseButton.gameObject.SetActive(true);
        DeleteButton.gameObject.SetActive(true);


        BigIcon.sprite = itemSelected.item.image;
        NameText.text = itemSelected.item.Name;
        AmountText.text = $"Amount : {itemSelected.amount}";

        UseButton.onClick.RemoveAllListeners();
        UseButton.onClick.AddListener(()=>
        {
            RemoveItem(itemSelected.item);
            if(itemSelected.item.type == Item.ItemType.Health)
            {
                FindObjectOfType<Player>().AddHealth(itemSelected.item.RestoreAmount);
                GameManager.instance.ShowNotification(1f, $"Health +{itemSelected.item.RestoreAmount}", Color.green);
            }
            else if(itemSelected.item.type == Item.ItemType.Shield)
            {
                //Add Shield;
                FindObjectOfType<Player>().AddShield(itemSelected.item.RestoreAmount);
                GameManager.instance.ShowNotification(1f, $"Shield +{itemSelected.item.RestoreAmount}", Color.blue);
            }
            UpdateInventoryUI();
            itemSelected = null;
            ShowItemInfo();
        });

        DeleteButton.onClick.RemoveAllListeners();
        DeleteButton.onClick.AddListener(()=>
        {
            RemoveItem(itemSelected.item);
            //Update UI
            UpdateInventoryUI();
            ShowItemInfo();
            GameManager.instance.ShowNotification(1f, $"-{itemSelected.item.Name}", Color.red);
        });

    }

    public ItemSlot[] GetAllItemSlots()
    {
        List<ItemSlot> slots = new List<ItemSlot>();
        foreach (KeyValuePair<Item, int> item in inventory)
        {
	        slots.Add(new ItemSlot()
            {
                item = item.Key,
                amount = item.Value,
            });
        }
        return slots.ToArray();
    }

    public int GetItemAmount(Item item)
    {
        if(inventory.ContainsKey(item))
        {
            return inventory[item];
        }
        else
        {
            return 0;
        }      
    }

    public void AddItem(Item item)
    {
        if(inventory.ContainsKey(item))
        {
            inventory[item]++;
        }
        else
        {
            inventory.Add(item, 1);
        }
    }

    public void RemoveItem(Item item, int minusAmount = 1)
    {
        int amount = GetItemAmount(item);
        if(amount > 0)
        {
            if(amount <= minusAmount)
            {
                inventory.Remove(item);
            }
            else
            {
                inventory[item] -= minusAmount;
            }
        }
        else
        {
            //No amount in inventory;
        }
    }
}
