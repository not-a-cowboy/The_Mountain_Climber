using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance { get; private set; }

    [Header("Emergency Use")]
    [SerializeField] private float emergencyHPCost = 25f;

    [Header("References — assign in Inspector")]
    [SerializeField] private PlayerController playerController;
    [SerializeField] private PlayerHealth playerHealth;
    [SerializeField] private PlayerGlow playerGlow;

    private int[] counts = new int[3];

    private bool[] active = new bool[3];

    public int GetCount(int slot) => counts[slot];
    public bool IsActive(int slot) => active[slot];

    public event System.Action OnInventoryChanged;

    private PlayerInputActions playerInput;

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;

        playerInput = new PlayerInputActions();
    }

    private void OnEnable()
    {
        playerInput.Player.UseSlot1.performed += _ => TryActivate(0);
        playerInput.Player.UseSlot2.performed += _ => TryActivate(1);
        playerInput.Player.UseSlot3.performed += _ => TryActivate(2);
        playerInput.Enable();
    }

    private void OnDisable()
    {
        playerInput.Player.UseSlot1.performed -= _ => TryActivate(0);
        playerInput.Player.UseSlot2.performed -= _ => TryActivate(1);
        playerInput.Player.UseSlot3.performed -= _ => TryActivate(2);
        playerInput.Disable();
    }

    public void AddToSlot(PowerUpType type)
    {
        int slot = SlotFor(type);
        counts[slot]++;
        OnInventoryChanged?.Invoke();
    }

    private void TryActivate(int slot)
    {
        if (GameManager.Instance == null || GameManager.Instance.IsGameOver) return;

        bool hasItem = counts[slot] > 0;

        if (!hasItem)
        {
            if (playerHealth != null)
                playerHealth.DrainHP(emergencyHPCost);
        }
        else
        {
            counts[slot]--;
            OnInventoryChanged?.Invoke();
        }

        ApplyEffect(slot);
    }

    private void ApplyEffect(int slot)
    {
        switch (slot)
        {
            case 0: // HigherJump
                StartCoroutine(ActivateWithGlow(slot, 5f, () =>
                    playerController.ApplyHigherJump(5f, 1.5f)));
                break;

            case 1: // Invulnerability
                StartCoroutine(ActivateWithGlow(slot, 5f, () =>
                    playerController.ApplyInvulnerability(5f)));
                break;

            case 2: // ScoreMultiplier
                StartCoroutine(ActivateWithGlow(slot, 5f, () =>
                    GameManager.Instance.ActivateScoreMultiplier(5f, 2f)));
                break;
        }
    }

    private IEnumerator ActivateWithGlow(int slot, float duration, System.Action applyEffect)
    {
        active[slot] = true;
        applyEffect.Invoke();
        playerGlow?.SetGlow(GlowColorFor(slot));

        yield return new WaitForSeconds(duration);

        active[slot] = false;

        if (!active[0] && !active[1] && !active[2])
            playerGlow?.ClearGlow();
        else
        {
            for (int i = 0; i < 3; i++)
                if (active[i]) playerGlow?.SetGlow(GlowColorFor(i));
        }
    }

    private static int SlotFor(PowerUpType type)
    {
        return type switch
        {
            PowerUpType.HigherJump => 0,
            PowerUpType.Invulnerability => 1,
            PowerUpType.ScoreMultiplier => 2,
            _ => 0
        };
    }

    private static Color GlowColorFor(int slot)
    {
        return slot switch
        {
            0 => Color.yellow,
            1 => Color.green,
            2 => Color.cyan,
            _ => Color.white
        };
    }
}
