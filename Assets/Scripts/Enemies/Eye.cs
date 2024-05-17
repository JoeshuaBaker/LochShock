using UnityEngine;

public class Eye : BasicEnemy
{
    public override void Update()
    {
        base.Update();
        animator.SetFloat("RandomTransition", Random.Range(0f, 1f));
    }
    public override int EnemyId()
    {
        return 1;
    }
}