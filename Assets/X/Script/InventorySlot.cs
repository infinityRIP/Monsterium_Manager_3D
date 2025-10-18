using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventorySlot : MonoBehaviour, IPointerClickHandler , IPointerEnterHandler , IPointerExitHandler
{
    public List<ItemStatSpec> modifiers;
    public string id;
    public string displayName;
    [TextArea(3, 12)] public string description;
    public Sprite icon;
    public int amount;
    public bool isFull;

    [SerializeField]
    private int maxNumberOfItem;

    [SerializeField]
    private TMP_Text amountText;

    [SerializeField]
    private Image itemImg;

    [SerializeField] float hoverDelay = 0.50f;

    public GameObject descriptionUI;
    public GameObject selectedShader;
    public TMP_Text descriptionText;
    public bool thisItemSelected;


    Coroutine hoverCo;
    bool isHovering;
    bool HasTooltip => !string.IsNullOrWhiteSpace(description);

    private InventoryManager inventoryManager;


    private void Start()
    {
        inventoryManager = GameObject.Find("InventoryCanvas").GetComponent<InventoryManager>();
        descriptionUI = transform.Find("Description").gameObject;
        amountText.enabled = false;
        //descriptionText = transform.Find("DescriptionText").GetComponent<TMP_Text>();
    }
    public void Additem(string id, string name, Sprite icon, int amount, string description, List<ItemStatSpec> modifiers, int maxNumberOfItem)
    {
        if (isFull) return;

        this.modifiers = modifiers;

        this.id = id;

        this.displayName = name;

        this.icon = icon;
        itemImg.sprite = icon;

        this.description = description;
        descriptionText.text = description;

        this.maxNumberOfItem = maxNumberOfItem;

        this.amount += amount;
        amountText.text = this.amount.ToString();
        amountText.enabled = this.amount > 1;

        if (this.amount >= maxNumberOfItem)
        {
            amountText.text = maxNumberOfItem.ToString();
            amountText.enabled = true;
            isFull = true;
        }

    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            OnLeftClick();
        }
        if (eventData.button == PointerEventData.InputButton.Right)
        {
            OnRightClick();
        }
    }

    public void OnLeftClick()
    {
        

    }
    public void OnRightClick()
    {

    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        isHovering = true;

        // เริ่มจับเวลาใหม่ทุกครั้งที่ผู้ใช้เข้ามา hover
        if (hoverCo != null) StopCoroutine(hoverCo);
        hoverCo = StartCoroutine(ShowAfterDelay());
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isHovering = false;

        // ยกเลิกคิวโชว์ ถ้ายังนับเวลาไม่ครบ
        if (hoverCo != null) { StopCoroutine(hoverCo); hoverCo = null; }

        inventoryManager.closeDescription();

    }

    IEnumerator ShowAfterDelay()
    {
        yield return new WaitForSecondsRealtime(hoverDelay);

        if (isHovering && HasTooltip)
        {
            descriptionUI.SetActive(true);
        }
        else
        {
            hoverCo = null;
        }
    }   

}
