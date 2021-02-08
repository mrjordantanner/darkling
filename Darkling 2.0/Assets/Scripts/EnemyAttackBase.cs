using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAttackBase : DamagerBase//, IEnemyAttack
{
    // Base code for all enemy attacks
    
    [HideInInspector]
    public PlayerCharacter player;
    [HideInInspector]
    public EnemyCharacter enemy;
    [HideInInspector]
    public SphereCollider attackRangeCollider;


    public float CalculateDistanceFromPlayerSquared()
    {
        var distanceFromPlayerSquared = (player.transform.position - transform.position).sqrMagnitude;
        return distanceFromPlayerSquared;
    }

}

public interface IEnemyAttack
{
    void Attack();
    float AnticipationTime { get; }
    float AttackTime { get; }
    float ResetTime { get; }
    //float KnockBack { get; set; }


}

