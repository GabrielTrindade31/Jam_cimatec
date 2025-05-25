using System;
using UnityEngine;
using UnityEngine.EventSystems;

[Serializable]
public class ButtonUpgrade : MonoBehaviour, IPointerEnterHandler
{
    private UpgradeUI upgradeUI;
    public Sprite icon;
    public int cost;
    public float defaultIncrease;
    [TextArea] public string description;
    public Stat stat;
    public void Setup(UpgradeUI upgradeUI)
    {
        this.upgradeUI = upgradeUI;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        upgradeUI.currentButton = this;
    }
}
