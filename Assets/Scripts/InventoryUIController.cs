using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InventoryUIController : Singleton<InventoryUIController>
{
    [SerializeField] private Button[] weaponsImages = new Button[3];
    [SerializeField] private Button[] magazinesImages = new Button[3];
    [SerializeField] private Button[] healersImages = new Button[3];

    [SerializeField] private TextMeshProUGUI weaponsCountText;
    [SerializeField] private TextMeshProUGUI magazinesCountText;
    [SerializeField] private TextMeshProUGUI healersCountText;

    [SerializeField] private TextMeshProUGUI descriptionText;
    [SerializeField] private Button applyButton;
    [SerializeField] private Button exitButton;

    private Dictionary<bool, Image> weaponsSlots = new Dictionary<bool, Image>();
    private Dictionary<bool, Image> magazinesSlots = new Dictionary<bool, Image>();
    private Dictionary<bool, Image> healersSlots = new Dictionary<bool, Image>();

    protected override void Awake()
    {
        base.Awake();
    }
}