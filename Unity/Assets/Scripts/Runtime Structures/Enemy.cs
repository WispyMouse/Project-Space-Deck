namespace SFDDCards
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    public class Enemy : Combatant
    {
        public EnemyModel BaseModel { get; private set; }
        public override string Name => this.BaseModel.Name;
        public override int MaxHealth => this.BaseModel.MaximumHealth;

        public EnemyAttack Intent;

        public bool ShouldBecomeDefeated
        {
            get
            {
                return this.CurrentHealth <= 0;
            }
        }

        public bool DefeatHasBeenSignaled { get; set; } = false;

        public override string Id => this.BaseModel.Id;

        public Enemy(EnemyModel baseModel)
        {
            this.BaseModel = baseModel;
            this.CurrentHealth = baseModel.MaximumHealth;
        }
    }
}