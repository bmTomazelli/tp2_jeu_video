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

    [Header("UI- Limit color bar change")]
    [SerializeField] private float vitalGreenLimit = 0.15f;
    [SerializeField] private float vitalYellowLimit = 0.60f;

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

        index = 0;
        Bind(index);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Z)) Bind(PrevIndex());
        if (Input.GetKeyDown(KeyCode.X)) Bind(NextIndex());

        UpdateHeader();
        UpdateVitals();
    }

    private int PrevIndex()
    {
        return (index - 1 + characters.Length) % characters.Length;
    }
    private int NextIndex() 
    { 
        return (index + 1) % characters.Length; 
    }

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

        float hungry = currentVitals.Hunger / 100f;
        float loneliness = currentVitals.Loneliness / 100f;
        float sleepiness = currentVitals.Sleepiness / 100f;

        if (hungryBar) hungryBar.size = hungry;
        if (socializeBar) socializeBar.size = loneliness;
        if (sleepinessBar) sleepinessBar.size = sleepiness;

        if (hungryFill) hungryFill.color = VitalColor(hungry);
        if (socializeFill) socializeFill.color = VitalColor(loneliness);
        if (sleepinessFill) sleepinessFill.color = VitalColor(sleepiness);
    }

    private Color VitalColor(float vital)
    {
        
        if (vital > vitalYellowLimit)
            return Color.red;

        
        if (vital > vitalGreenLimit)
            return Color.yellowNice;

        
        return Color.green;
    }
}
