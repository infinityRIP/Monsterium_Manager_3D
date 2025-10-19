using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventorySlot : MonoBehaviour, IPointerClickHandler , IPointerEnterHandler , IPointerExitHandler, IBeginDragHandler, IDragHandler, IEndDragHandler, IDropHandler
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
    public Image itemImg;

    [SerializeField] float hoverDelay = 0.10f;

    public GameObject descriptionUI;
    public GameObject selectedShader;
    public TMP_Text descriptionText;
    public bool thisItemSelected;

    [SerializeField] public CanvasGroup cg;

    Coroutine hoverCo;
    Coroutine showCo;
    bool isHovering;
    bool HasTooltip => !string.IsNullOrWhiteSpace(description);

    private InventoryManager inventoryManager;

    // ====== DRAG & DROP ======
    static InventorySlot draggingFrom;   // สล็อตต้นทาง
    static string dragId;
    static Sprite dragIcon;
    static int dragAmount;
    static List<ItemStatSpec> dragMods;
    static string dragName, dragDesc;
    static int dragMax;

    private void Start()
    {
        inventoryManager = GameObject.Find("InventoryCanvas").GetComponent<InventoryManager>();
        //descriptionUI = transform.Find("Description").gameObject;
        amountText.enabled = false;
        itemImg.enabled = false;
        //descriptionText = transform.Find("DescriptionText").GetComponent<TMP_Text>();
    }
    void Update()
    {
        if (isHovering && TooltipController.I && TooltipController.I.gameObject.activeSelf)
            TooltipController.I.Reposition(Input.mousePosition);
    }
    public void Additem(string id, string name, Sprite icon, int amount, string description, List<ItemStatSpec> modifiers, int maxNumberOfItem)
    {
        if (isFull) return;

        this.modifiers = modifiers;

        this.id = id;

        this.displayName = name;

        this.icon = icon;
        itemImg.sprite = icon;
        itemImg.enabled = true;                  

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

    public void OnPointerEnter(UnityEngine.EventSystems.PointerEventData e)
    {
        isHovering = true;
        if (showCo != null) StopCoroutine(showCo);
        showCo = StartCoroutine(ShowAfterDelay());
    }

    public void OnPointerExit(UnityEngine.EventSystems.PointerEventData e)
    {
        isHovering = false;
        if (showCo != null) { StopCoroutine(showCo); showCo = null; }
        TooltipController.I?.Hide();
    }

    IEnumerator ShowAfterDelay()
    {
        showCo = null;

        if (!isHovering || !HasTooltip || TooltipController.I == null) yield break;

        TooltipController.I.PrimeAtMouse($"{displayName}\n{description}");
    }

    // เลื่อนตามเมาส์หลังจากโชว์
    void LateUpdate() // ใช้ LateUpdate ให้ตำแหน่ง sync หลัง UI อัปเดต
    {
        if (isHovering && TooltipController.I != null)
            TooltipController.I.Reposition(Input.mousePosition);
    }

    public void OnBeginDrag(PointerEventData e)
    {
        if (string.IsNullOrEmpty(id) || icon == null || amount <= 0) return;

        draggingFrom = this;
        dragId = id;
        dragIcon = icon;
        dragAmount = amount;
        dragMods = modifiers;
        dragName = displayName;
        dragDesc = description;
        dragMax = maxNumberOfItem;

        // ทำภาพเงาลอยตามเมาส์
        UIDragGhost.I.Show(dragIcon);
        UIDragGhost.I.Move(e.position);

        cg.blocksRaycasts = false;

        itemImg.enabled = false;
        amountText.enabled = false;
        descriptionUI?.SetActive(false);
    }

    public void OnDrag(PointerEventData e)
    {
        if (draggingFrom != null) UIDragGhost.I.Move(e.position);
    }

    public void OnEndDrag(PointerEventData e)
    {
        UIDragGhost.I.Hide();

        if (draggingFrom != null)
        {
            draggingFrom.itemImg.enabled = true;
            draggingFrom.amountText.enabled = draggingFrom.amount > 1;
            draggingFrom.cg.blocksRaycasts = true;
        }
        draggingFrom = null;
        itemImg.enabled = false ;
    }

    public void OnDrop(PointerEventData e)
    {
        if (draggingFrom == null) return;

        if (draggingFrom == this)
        {
            itemImg.enabled = true;
            amountText.enabled = amount > 1;
            cg.blocksRaycasts = true;
            return;
        }

        if (string.IsNullOrEmpty(id))
        {
            // MOVE
            SetItem(dragId, dragName, dragIcon, dragAmount, dragDesc, dragMods, dragMax);
            draggingFrom.ClearItem();
        }
        else if (id == dragId && amount < maxNumberOfItem)  
        {
            int space = maxNumberOfItem - amount;
            int move = Mathf.Min(space, dragAmount);
            amount += move;
            amountText.text = amount.ToString();
            amountText.enabled = amount > 1;

            draggingFrom.amount -= move;
            draggingFrom.amountText.text = draggingFrom.amount.ToString();
            draggingFrom.amountText.enabled = draggingFrom.amount > 1;
            if (draggingFrom.amount <= 0) draggingFrom.ClearItem();
            else { draggingFrom.itemImg.enabled = true; }

            draggingFrom.cg.blocksRaycasts = true;
        }
        else
        {
            var tid = id; var tname = displayName; var ticon = icon;
            var tamt = amount; var tdesc = description; var tmods = modifiers;
            var tmax = maxNumberOfItem;

            SetItem(dragId, dragName, dragIcon, dragAmount, dragDesc, dragMods, dragMax);
            draggingFrom.SetItem(tid, tname, ticon, tamt, tdesc, tmods, tmax);
        }

    }

    // ====== Utilities ======
    public void SetItem(string id, string name, Sprite icon, int amount,
                        string description, List<ItemStatSpec> mods, int maxNum)
    {
        this.id = id;
        this.displayName = name;
        this.icon = icon;
        this.amount = amount;
        this.description = description;
        this.modifiers = mods;
        this.maxNumberOfItem = maxNum;

        itemImg.sprite = icon;
        itemImg.enabled = true;
        amountText.text = amount.ToString();
        amountText.enabled = amount > 1;
        isFull = amount >= maxNumberOfItem;
    }

    void ClearItem()
    {
        id = displayName = description = null;
        modifiers = null;
        icon = null;
        amount = 0; isFull = false;

        itemImg.sprite = null;
        itemImg.enabled = false;               
        amountText.enabled = false;
    }


}
