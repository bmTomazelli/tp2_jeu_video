using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HudController : MonoBehaviour
{
    [Header("UI - Labels & Avatar")]
    [SerializeField] private TMP_Text nameLabel;
    [SerializeField] private Image avatarImage;

    [Header("UI - Vital Bars (Scrollbars)")]
    [SerializeField] private Scrollbar hungryBar;
    [SerializeField] private Scrollbar socializeBar;
    [SerializeField] private Scrollbar sleepinessBar;

    [Header("UI - Vital Colors (opcional)")]
    [SerializeField] private Image hungryFill;
    [SerializeField] private Image socializeFill;
    [SerializeField] private Image sleepinessFill;

    private Character[] characters;
    private int index;
    private Character current;
    private CharacterVitals currentVitals;

    private void Awake()
    {
        characters = Finder.GameController.CityObjects.Characters;
        if (characters == null || characters.Length == 0)
        {
            Debug.LogWarning("No character found.");
            enabled = false;
            return;
        }

        index = Mathf.Clamp(index, 0, characters.Length - 1);
        Bind(index);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Z)) Bind(PrevIndex());
        if (Input.GetKeyDown(KeyCode.X)) Bind(NextIndex());

        UpdateHeader();
        UpdateVitals();
    }

    private int PrevIndex() => (index - 1 + characters.Length) % characters.Length;
    private int NextIndex() => (index + 1) % characters.Length;

    private void Bind(int newIndex)
    {
        if (current) current.HidePointer();

        index = newIndex;
        current = characters[index];
        current.ShowPointer(true);

        currentVitals = current.GetComponent<CharacterVitals>();

        if (avatarImage) avatarImage.sprite = current.Avatar;

        UpdateHeader();
        UpdateVitals();
    }

    private void UpdateHeader()
    {
        if (!current || !nameLabel) return;

        string state = current.StateMachine != null
            ? current.StateMachine.CurrentStateName
            : "Idle";

        nameLabel.text = $"{current.FullName}\n({state})";
    }

    private void UpdateVitals()
    {
        if (!current || !currentVitals) return;

        float h = Mathf.Clamp01(currentVitals.Hunger / 100f);
        float l = Mathf.Clamp01(currentVitals.Loneliness / 100f);
        float s = Mathf.Clamp01(currentVitals.Sleepiness / 100f);

        if (hungryBar) hungryBar.size = h;
        if (socializeBar) socializeBar.size = l;
        if (sleepinessBar) sleepinessBar.size = s;

        if (hungryFill) hungryFill.color = VitalColor(h);
        if (socializeFill) socializeFill.color = VitalColor(l);
        if (sleepinessFill) sleepinessFill.color = VitalColor(s);
    }

    private Color VitalColor(float v)
    {
        if (v > 0.40f) return Color.green;
        if (v > 0.15f) return Color.yellow;
        return Color.red;
    }
}
