using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InventoryUIController : MonoBehaviour
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
}