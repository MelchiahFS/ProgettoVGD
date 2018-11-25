using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MovementPattern))]
public class EnemyController : Character
{

    public enum MovementType { charging, following, hybridWandering, pureWandering, bouncing };
    public MovementType movementType;
    private GameObject player;
    private float posX, posY;
    private Vector3 lastFramePosition, movementDirection;
    private Animator animator;
    public bool flying; //indica se il nemico vola o no (ossia se è necessario l'algoritmo di pathfinding o no per seguire il target)
    private MovementPattern mp;

        
    void Start()
    {
        mp = GetComponent<MovementPattern>();
        animator = GetComponent<Animator>();
        player = GameObject.FindGameObjectWithTag("Player");
        GetComponent<SpriteRenderer>().sortingLayerName = "Characters";
        gameObject.tag = "Enemy";
        SetRealOffset(gameObject);

        if (!flying)
            mp.astar = GetComponent<AStarAI>();
        else
            gameObject.layer = LayerMask.NameToLayer("Flying");

        lastFramePosition = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        
        //se l'animator possiede le animazioni per le quattro direzioni, imposto i valori degli assi
        if (animator.parameters.Length > 0)
        {
            movementDirection = transform.position - lastFramePosition;

            posX = movementDirection.x;
            posY = movementDirection.y;

            animator.SetFloat("PosX", posX);
            animator.SetFloat("PosY", posY);

            lastFramePosition = transform.position;
        }


        if (movementType == MovementType.following)
        {
            if (flying)
                mp.Follow(player.transform.position);
            else
                mp.FollowAI(player.transform, true);
        }
        else if (movementType == MovementType.pureWandering)
        {
            mp.Wander(flying);
        }
        else if (movementType == MovementType.hybridWandering)
        {
            mp.WanderAndLock(flying);
        }
        else if (movementType == MovementType.charging)
        {
            mp.Charge();
        }
        else if (movementType == MovementType.bouncing)
        {
            mp.Bounce();
        }

    }


}
