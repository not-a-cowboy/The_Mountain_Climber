using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HUDManager : MonoBehaviour
{
    [System.Serializable]
    public class InventorySlot
    {
        public Image icon;
        public TextMeshProUGUI countText;
        public Sprite sprite;
    }

    [Header("Inventory Slots (0=Jump, 1=Invul, 2=ScoreMult, 3=Launch)")]
    [SerializeField] private InventorySlot[] slots = new InventorySlot[4];

    [Header("Health Bar")]
    [SerializeField] private Slider healthSlider;
    [SerializeField] private Image healthFill;

    [Header("References")]
    [SerializeField] private PlayerHealth playerHealth;

    private static readonly Color healthColorFull = new Color(0.247f, 0.749f, 0.745f, 1.0f);
    private static readonly Color healthColorLow = new Color(0.247f, 0.090f, 0.714f, 1.0f);

    [System.Serializable]
    public class PowerUpTimer
    {
        [Tooltip("The root timer panel (inactive by default in the scene)")]
        public GameObject panel;

        [Tooltip("The child TMP text named 'Timer' inside the panel")]
        public TMP_Text timerText;

        [HideInInspector] public float remaining;
        [HideInInspector] public Coroutine coroutine;
    }

    [Header("Power-Up Timer Panels (0=Jump, 1=Shield, 2=Score, 3=Launch)")]
    [SerializeField] private PowerUpTimer jumpTimer;
    [SerializeField] private PowerUpTimer shieldTimer;
    [SerializeField] private PowerUpTimer scoreTimer;
    [SerializeField] private PowerUpTimer launchTimer;

    public static HUDManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    private void Start()
    {
        if (playerHealth != null)
        {
            playerHealth.OnHPChanged += UpdateHealthBar;
            UpdateHealthBar(playerHealth.CurrentHP, playerHealth.MaxHP);
        }

        if (InventoryManager.Instance != null)
            InventoryManager.Instance.OnInventoryChanged += RefreshSlots;

        RefreshSlots();

        SetPanelActive(jumpTimer, false);
        SetPanelActive(shieldTimer, false);
        SetPanelActive(scoreTimer, false);
        SetPanelActive(launchTimer, false);
    }

    private void OnDestroy()
    {
        if (playerHealth != null)
            playerHealth.OnHPChanged -= UpdateHealthBar;

        if (InventoryManager.Instance != null)
            InventoryManager.Instance.OnInventoryChanged -= RefreshSlots;
    }

    private void RefreshSlots()
    {
        if (InventoryManager.Instance == null) return;

        for (int i = 0; i < slots.Length; i++)
        {
            int count = InventoryManager.Instance.GetCount(i);

            if (slots[i].icon != null)
            {
                slots[i].icon.sprite = slots[i].sprite;
                Color c = slots[i].icon.color;
                c.a = count > 0 ? 1f : 0.3f;
                slots[i].icon.color = c;
            }

            if (slots[i].countText != null)
            {
                slots[i].countText.text = count >= 0 ? count.ToString() : "";
                slots[i].countText.enabled = count >= 0;
            }
        }
    }

    private void UpdateHealthBar(float current, float max)
    {
        if (healthSlider != null)
            healthSlider.value = current / max;

        if (healthFill != null)
            healthFill.color = Color.Lerp(healthColorLow, healthColorFull, current / max);
    }

    public void TrackJumpTimer(float duration) => TrackTimer(jumpTimer, duration);
    public void TrackShieldTimer(float duration) => TrackTimer(shieldTimer, duration);
    public void TrackScoreTimer(float duration) => TrackTimer(scoreTimer, duration);
    public void TrackLaunchTimer(float duration) => TrackTimer(launchTimer, duration);

    private void SetPanelActive(PowerUpTimer entry, bool active)
    {
        if (entry == null || entry.panel == null) return;
        entry.panel.SetActive(active);
    }

    private void TrackTimer(PowerUpTimer entry, float duration)
    {
        if (entry == null || entry.panel == null || entry.timerText == null) return;

        if (entry.coroutine != null)
        {
            entry.remaining += duration;
            return;
        }

        entry.remaining = duration;
        SetPanelActive(entry, true);
        entry.coroutine = StartCoroutine(RunTimer(entry));
    }

    private IEnumerator RunTimer(PowerUpTimer entry)
    {
        while (entry.remaining > 0f)
        {
            entry.remaining -= Time.deltaTime;
            entry.timerText.text = Mathf.CeilToInt(Mathf.Max(0f, entry.remaining)).ToString();
            yield return null;
        }

        entry.timerText.text = "0";
        SetPanelActive(entry, false);
        entry.coroutine = null;
        entry.remaining = 0f;
    }
}