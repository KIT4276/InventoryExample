using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InventorySlotView : MonoBehaviour
{
    [SerializeField] private Image _itemIcon;
    [SerializeField] private GameObject _lockView;
    [SerializeField] private GameObject _unlockView;
    [SerializeField] private GameObject _countBack;
    [SerializeField] private TMP_Text _countText;
    [SerializeField] private TMP_Text _lockedSlotNumberText;
    [SerializeField] private Button _button;

    private Action<InventorySlotView> _onClick;
    private InventorySlotData _slotData;
    private int _slotIndex;

    public InventorySlotData SlotData => _slotData;
    public int SlotIndex => _slotIndex;

    private void Awake()
    {
        if (_button != null)
            _button.onClick.AddListener(HandleClick);
    }

    public void BindClick(Action<InventorySlotView> onClick)
    {
        _onClick = onClick;
    }

    public void Render(int slotIndex, int slotNumber, InventorySlotData slotData, ItemDefinition itemDefinition)
    {
        _slotIndex = slotIndex;
        _slotData = slotData;

        if (slotData == null)
        {
            SetVisible(_itemIcon, false);
            SetActive(_countBack, false);
            SetVisible(_lockedSlotNumberText, false);
            SetActive(_lockView, false);
            SetActive(_unlockView, false);
            return;
        }

        bool isLocked = !slotData.IsUnlocked;
        bool isEmpty = slotData.IsEmpty;

        SetActive(_lockView, isLocked);
        SetActive(_unlockView, !isLocked);

        if (_lockedSlotNumberText != null)
        {
            _lockedSlotNumberText.gameObject.SetActive(isLocked);
            if (isLocked)
                _lockedSlotNumberText.text = slotNumber.ToString();
        }

        if (isLocked)
        {
            SetVisible(_itemIcon, false);
            SetActive(_countBack, false);
            return;
        }

        if (isEmpty || itemDefinition == null)
        {
            SetVisible(_itemIcon, false);
            SetActive(_countBack, false);
            return;
        }

        if (_itemIcon != null)
        {
            _itemIcon.sprite = itemDefinition.Icon;
            _itemIcon.enabled = itemDefinition.Icon != null;
            _itemIcon.gameObject.SetActive(true);
        }

        if (_countText != null)
        {
            bool showCount = slotData.Count > 1;
            SetActive(_countBack, showCount);

            if (showCount)
                _countText.text = slotData.Count.ToString();
        }
    }

    private void HandleClick()
    {
        _onClick?.Invoke(this);
    }

    private static void SetVisible(Graphic graphic, bool isVisible)
    {
        if (graphic == null)
            return;

        graphic.gameObject.SetActive(isVisible);
    }

    private static void SetActive(GameObject target, bool isActive)
    {
        if (target == null)
            return;

        target.SetActive(isActive);
    }
}
