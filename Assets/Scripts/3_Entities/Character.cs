using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

// Personnage.
//
// Vous allez appeler des fonctions sur cette classe à partir des états de votre "State Machine". En d'autres termes,
// cette classe est là pour vous rendre service.
//
// Voici les fonctions qui vous seront utiles :
//
//      * NavigateTo : Cette fonction prend une destination en paramètre et déplace le personnage vers cette
//        destination. Vous pouvez vérifier si le personnage est proche d'une destination avec la fonction IsCloseTo.
//        Vous pouvez également annuler un déplacement avec la fonction CancelNavigate.
//      * PickUpTrash : Cette fonction prends un déchet en paramètre et démarre une routine pendant laquelle le
//        personnage ramasse ce déchet. Vous pouvez vérifier si le personnage est en train de ramasser un déchet avec
//        la fonction IsPickingTrash.
//      * ThrowTrash : Cette fonction démarre une routine où le personnage lance un déchet. Vous pouvoir vérifier si le
//        personnage est en train de lancer un déchet avec la fonction IsThrowingTrash.
//      * GreetCharacter : Cette fonction prends un personnage en paramètre et démarre une routine pendant laquelle le
//        personnage actuel salue l'autre personnage (avec un signe de la main). Vous pouvez vérifier si le personnage
//        est en train d'en saluer un autre avec la fonction IsGreetingCharacter.
//      * MakeVisible : Cette fonction rends le personnage visible. Lorsqu'il est visible, ses MeshRenderer et ses
//        colliders sont activés. Le personnage est donc affiché à l'écran et il peut être détecté via les fonctions
//        OnCollisionEnter et OnTriggerEnter.
//      * MakeInvisible : Cette fonction rends le personnage invisible. Lorsqu'il est invisible, ses MeshRenderer et ses
//        colliders sont désactivés. Le personnage est n'est donc plus affiché à l'écran et il peut pas être détecté via
//        les fonctions OnCollisionEnter et OnTriggerEnter.
//
// Cette classe implémente IDestination (voir les commentaires de cette interface pour les détails). Aussi, vous aurez à
// modifier cette classe pour le travail.
public class Character : MonoBehaviour, IDestination
{
    [Header("Identity")]
    [field: SerializeField] public string FullName { get; private set; } = "Character";
    [field: SerializeField] public Sprite Avatar { get; private set; }
    [SerializeField] private GameObject pointer;

    [Header("Movement")]
    [SerializeField, Min(0)] private float destinationReachedDistance = 0.5f;

    private GameController gameController;
    private CharacterStateMachine stateMachine;
    private CharacterVitals vitals;
    private CharacterBlackboard blackboard;
    private CharacterAnimation characterAnimation;
    private NavMeshAgent navMeshAgent;
    private new CapsuleCollider collider;
    private Renderer[] renderers;

    public CharacterStateMachine StateMachine => stateMachine;
    public CharacterVitals Vitals => vitals;
    public CharacterBlackboard Blackboard => blackboard;

    public Vector3 Position => transform.position;
    public bool IsAvailable => isActiveAndEnabled && collider.enabled;

    private void Awake()
    {
        // Get dependencies.
        gameController = Finder.GameController;
        stateMachine = GetComponent<CharacterStateMachine>();
        vitals = GetComponent<CharacterVitals>();
        blackboard = GetComponent<CharacterBlackboard>();
        characterAnimation = GetComponent<CharacterAnimation>();
        navMeshAgent = GetComponent<NavMeshAgent>();
        collider = GetComponent<CapsuleCollider>();
        renderers = GetComponentsInChildren<Renderer>();

        // Init starting position.
        var spawnPoint = ArrayExtensions.Random(Finder.GameController.CityObjects.CharacterSpawnPoints);
        transform.position = spawnPoint.Position;
    }

    public void NavigateTo(IDestination destination)
    {
        IEnumerator Routine()
        {
            // The NavMeshAgent needs time to initialize. Wait for it to be ready before issuing a command.
            while (!navMeshAgent.isOnNavMesh) yield return null;

            // Issue the navigate command.
            navMeshAgent.SetDestination(destination.Position);
        }

        StartCoroutine(Routine());
    }

    public void CancelNavigate()
    {
        navMeshAgent.ResetPath();
    }

    public bool IsCloseTo(IDestination destination)
    {
        return Vector3.Distance(transform.position, destination.Position) < destinationReachedDistance;
    }

    public void PickUpTrash(Trash trash)
    {
        IEnumerator Routine()
        {
            // Cancel navigation.
            navMeshAgent.ResetPath();

            // Play the pick-up animation.
            characterAnimation.PlayPickUpAnimation();

            // Look at the trash.
            transform.LookAt(trash.transform.position, Vector3.up);

            // Wait for animation to end.
            yield return new WaitUntil(() => !characterAnimation.IsPlayingPickUpAnimation());

            // Make trash disappear (should be safe even if trash is cleaned by someone else).
            gameController.ObjectPools.Trash.Release(trash.gameObject);

            // Remove trash from memory.
            // TODO : Enlevez le déchet ramassé de la mémoire du personnage. Par exemple :
            //      blackboard.Trash = null;

#if UNITY_EDITOR
            // Append the character name to the trash GameObject name for debugging.
            trash.gameObject.name = $"Trash (picked up by {FullName})";
#endif
        }

        StartCoroutine(Routine());
    }

    public bool IsPickingTrash()
    {
        return characterAnimation.IsPlayingPickUpAnimation();
    }

    public void ThrowTrash()
    {
        IEnumerator Routine()
        {
            // Cancel navigation.
            navMeshAgent.ResetPath();

            // Play the throw animation.
            characterAnimation.PlayThrowAnimation();

            // Wait for animation to end.
            yield return new WaitUntil(() => !characterAnimation.IsPlayingThrowAnimation());

            // Put trash at feet.
            var trash = gameController.ObjectPools.Trash.Place(transform.position);

            // Mark character as having thrown trash.
            blackboard.ShouldThrowTrash = false;

#if UNITY_EDITOR
            // Append the character name to the trash GameObject name for debugging.
            trash.gameObject.name = $"Trash (thrown by {FullName})";
#endif
        }

        StartCoroutine(Routine());
    }

    public bool IsThrowingTrash()
    {   
        return characterAnimation.IsPlayingThrowAnimation();
    }

    public void GreetCharacter(Character character)
    {
        IEnumerator Routine()
        {
            vitals.LowerLoneliness();
            // Cancel navigation.
            navMeshAgent.ResetPath();

            // Play the greet animation.
            characterAnimation.PlayGreetAnimation();

            // Look at the other character.
            transform.LookAt(character.transform.position, Vector3.up);

            // Wait for animation to end.
            yield return new WaitUntil(() => !characterAnimation.IsPlayingGreetAnimation());

            // Remove friend from memory.
            blackboard.LastSeenFriend = null;

        }

        StartCoroutine(Routine());
    }

    public bool IsGreetingCharacter()
    {
        return characterAnimation.IsPlayingGreetAnimation();
    }

    public void MakeVisible(bool visibility = true)
    {
        // Collider makes this character detectable by other characters.
        collider.enabled = visibility;

        // NavMeshAgent ensure that this character is avoided by the others.
        navMeshAgent.enabled = visibility;

        // Renderers make this character visible to the player (in the camera).
        for (var i = 0; i < renderers.Length; i++)
        {
            renderers[i].enabled = visibility;
        }
    }

    public void MakeInvisible()
    {
        MakeVisible(false);
    }

    public void ShowPointer(bool visibility = true)
    {
        pointer.SetActive(visibility);
    }
    
    public void HidePointer()
    {
        ShowPointer(false);
    }
}