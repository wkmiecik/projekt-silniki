using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.Playables;
using DG.Tweening;

public class UIManager : MonoBehaviour
{
    // Camera
    [Header("Camera")]
    Camera cam;
    [HideInInspector] public bool uiBlockingInput = false;


    // Ship ui
    [Header("Ship HP")]
    [SerializeField] TMP_Text shipHpText;


    [Header("Player tool tip")]
    [SerializeField] TMP_Text playerToolTipText;


    [SerializeField] Texture2D cursorTexture;


    // Resources ui
    [Header("Resources")]
    [SerializeField] TMP_Text coinsText;
    [SerializeField] TMP_Text woodText;
    [SerializeField] TMP_Text ironText;


    // Equipment ui
    [Header("Equipment")]
    [SerializeField] ShipEquipment shipEquipment;
    [SerializeField] GameObject scroll;
    [SerializeField] Animator scrollAnimator;

    [HideInInspector] public bool scrollOpened = false;
    bool animationPlaying = false;
    [SerializeField] float scrollAnimationDuration = .6f;


    // Equipment levels
    [Header("Equipment levels")]
    [SerializeField] TMP_Text cannonLevelText;
    [SerializeField] TMP_Text cannonballsLevelText;
    [SerializeField] TMP_Text shipLevelText;


    // Upgrades info panel
    [Header("Equipment upgrade info")]
    [SerializeField] GameObject infoPanel;
    [SerializeField] GameObject infoPanelMaxLevel;
    EquipmentLevels equipmentLevels;
    [SerializeField] Vector3 infoPanelOffset;

    [SerializeField] TMP_Text cost1_text;
    [SerializeField] Image cost1_icon;

    [SerializeField] TMP_Text cost2_text;
    [SerializeField] Image cost2_icon;

    [SerializeField] TMP_Text bonus1_text;
    [SerializeField] TMP_Text bonus2_text;


    // Chests !!!do wywalenia caly ten kod!!!
    [Header("Chests")]
    [SerializeField] GameObject chestsInfoPanel;
    [SerializeField] Vector3 chestInfoPanelOffset;
    [SerializeField] Sprite OpenedChestSprite;

    [SerializeField] GameObject gainedResourcePanel;
    [SerializeField] TMP_Text gainedResourceText;

    [SerializeField] GameObject gainedResourcePanel2;
    [SerializeField] TMP_Text gainedResourceText2;

    [SerializeField] Vector3 gainedResourcePanelOffset;
    [SerializeField] Ease ease1;
    [SerializeField] Ease ease2;



    void Start() {
        cam = Camera.main;

        equipmentLevels = ObjectManager.Instance.equipmentLevels;

        Cursor.SetCursor(cursorTexture, Vector2.zero, CursorMode.Auto);
    }

    void Update() {
        // Rotate ship hp to face camera
        shipHpText.transform.rotation = cam.transform.rotation;
        playerToolTipText.transform.rotation = cam.transform.rotation;

        // Show panel with upgrade info next to mouse when hovering over upgrade icon
        if (infoPanel.activeSelf) {
            infoPanel.transform.position = Input.mousePosition + infoPanelOffset;
        }
        if (infoPanelMaxLevel.activeSelf) {
            infoPanelMaxLevel.transform.position = Input.mousePosition + infoPanelOffset;
        }
        if(chestsInfoPanel.activeSelf) {
            chestsInfoPanel.transform.position = Input.mousePosition + chestInfoPanelOffset;
        }
    }

    public void EnablePlayerTipText() {
        playerToolTipText.gameObject.SetActive(true);
    }
    public void DisablePlayerTipText() {
        playerToolTipText.gameObject.SetActive(false);
    }



    public void OpenChest(Image chest) {
        chest.sprite = OpenedChestSprite;
        chestsInfoPanel.SetActive(false);

        OpenChestSequence(chest, ResourceType.iron, gainedResourcePanel, gainedResourceText, ironText.transform.position);

        OpenChestSequence(chest, ResourceType.coin, gainedResourcePanel2, gainedResourceText2, coinsText.transform.position).SetDelay(.72f);
    }
    Tween OpenChestSequence(Image chest, ResourceType resourceType, GameObject panel, TMP_Text text, Vector3 finishPos) {
        panel.SetActive(true);
        panel.transform.position = chest.transform.position + gainedResourcePanelOffset;

        Sequence sequence = DOTween.Sequence();
        sequence.Append(panel.transform.DOMoveY(panel.transform.position.y + 150f, .55f)).SetEase(ease1)
            .Join(panel.transform.DOScale(1f, .55f))
            .OnComplete(() => OpenChestSequence1(chest, resourceType, panel, text, finishPos));
        sequence.PlayForward();
        return sequence;
    }
    void OpenChestSequence1(Image chest, ResourceType resourceType, GameObject panel, TMP_Text text, Vector3 finishPos) {
        Sequence sequence = DOTween.Sequence();
        sequence.Append(panel.transform.DOMove(new Vector3(finishPos.x + 160, finishPos.y), .6f)).SetEase(ease2)
            .Join(panel.transform.DOScale(.8f, .6f))
            .Join(text.DOColor(new Color(0, 0, 0, 0), .5f))
            .OnComplete(() => OpenChestFinish(resourceType, panel, text));
        sequence.PlayForward();
    }
    void OpenChestFinish(ResourceType resourceType, GameObject panel, TMP_Text text) {
        panel.SetActive(false);
        ObjectManager.Instance.resourcesManager.AddResource(resourceType, int.Parse(text.text));
    }



    public void UpgradeIconClicked(int equipmentType) {
        shipEquipment.UpgradeEquipment((ShipEquipment.EquipmentType)equipmentType);
    }


    // Hovering over upgrade icon
    public void ShowHoverInfoPanel(int equipmentType) {
        ShipEquipment.EquipmentType type = (ShipEquipment.EquipmentType)equipmentType;
        int levelIndex = shipEquipment.GetEquipmentLevel(type) - 1;
        if (levelIndex >= equipmentLevels.GetUpgrades(type).Length) {
            infoPanelMaxLevel.SetActive(true);
            infoPanel.SetActive(false);
            return;
        }

        cost1_icon.sprite = equipmentLevels.GetUpgrades(type)[levelIndex].cost_1_icon;
        cost1_text.text = equipmentLevels.GetUpgrades(type)[levelIndex].cost_1_amount.ToString();
        cost2_icon.sprite = equipmentLevels.GetUpgrades(type)[levelIndex].cost_2_icon;
        cost2_text.text = equipmentLevels.GetUpgrades(type)[levelIndex].cost_2_amount.ToString();
        bonus1_text.text = equipmentLevels.GetUpgrades(type)[levelIndex].bonus_1;
        bonus2_text.text = equipmentLevels.GetUpgrades(type)[levelIndex].bonus_2;
        infoPanel.SetActive(true);
        infoPanelMaxLevel.SetActive(false);
    }

    public void HideHoverInfoPanel() {
        infoPanel.SetActive(false);
        infoPanelMaxLevel.SetActive(false);
    }



    // Equipment srcoll open/close
    public void OpenEquipment() {
        if (!animationPlaying) {
            scroll.SetActive(true);
            uiBlockingInput = true;
            scrollOpened = true;

            animationPlaying = true;
            StartCoroutine(SetScrollOpenedAfterDelay(scrollAnimationDuration));

            DisablePlayerTipText();
            ObjectManager.Instance.cameraController.SwitchToShipMenuCamera();
            ObjectManager.Instance.ship.SetSailsVisible();
        }
    }

    public void CloseEquipment() {
        if (!animationPlaying) {
            uiBlockingInput = false;
            scrollOpened = false;

            animationPlaying = true;
            scrollAnimator.SetTrigger("Close");
            StartCoroutine(SetScrollClosedAfterDelay(scrollAnimationDuration));

            EnablePlayerTipText();
            ObjectManager.Instance.cameraController.SwitchToShipCamera();
            ObjectManager.Instance.ship.SetSailsTransparent();
        }
    }

    IEnumerator SetScrollOpenedAfterDelay(float time) {
        yield return new WaitForSeconds(time);

        animationPlaying = false;
    }

    IEnumerator SetScrollClosedAfterDelay(float time) {
        yield return new WaitForSeconds(time);

        animationPlaying = false;
        scroll.SetActive(false);
    }



    // Equipment levels
    public void SetEquipmentLevelText(ShipEquipment.EquipmentType equipmentType, int level) {
        switch (equipmentType) {
            case (ShipEquipment.EquipmentType.cannon):
                cannonLevelText.text = $"{level} lvl";
                return;
            case (ShipEquipment.EquipmentType.cannonBalls):
                cannonballsLevelText.text = $"{level} lvl";
                return;
            case (ShipEquipment.EquipmentType.ship):
                shipLevelText.text = $"{level} lvl";
                return;
        }
        throw new UnityException("wrong equipment type");
    }



    // Resources view
    public void SetShipHPtext(int hp) {
        shipHpText.text = $"{hp} HP";
    }

    public void SetCoinsText(int coins) {
        coinsText.text = $"{coins}";
    }

    public void SetWoodText(int wood) {
        woodText.text = $"{wood}";
    }

    public void SetIronText(int iron) {
        ironText.text = $"{iron}";
    }
}
