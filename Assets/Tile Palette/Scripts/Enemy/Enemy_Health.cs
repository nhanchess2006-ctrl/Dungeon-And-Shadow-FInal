using UnityEngine;

public class Enemy_Health : Entity_Health
{
    private Enemy enemy;
    private Player_QuestManager questManager;

    protected override void Start()
    {
        base.Start();

        enemy = GetComponent<Enemy>();
        questManager = Player.instance.questManager;
    }

    public override bool TakeDamage(float damage, float elementalDamage,ElementType element, Transform damageDealer)
    {
        if(canTakeDamage == false) 
            return false;

        bool wasHit = base.TakeDamage(damage,elementalDamage,element, damageDealer);

        if (wasHit == false)
            return false;

        if(damageDealer.GetComponent<Player>() != null)
            enemy.TryEnterBattleState(damageDealer);

        return true;    
    }

    protected override void Die()
    {
     // =====================================================
     // CỘNG EXP TRƯỚC KHI QUÁI BỊ XỬ LÝ/XÓA
     // =====================================================

        EnemyExpReward expReward =
        GetComponent<EnemyExpReward>();

      if (expReward != null)
      {
        expReward.GiveExpReward();
      }
        base.Die();
        
      if (QuestManager.Instance != null)
      {
        QuestManager.Instance.NotifyEnemyDeath(this);
      }
    }
}
