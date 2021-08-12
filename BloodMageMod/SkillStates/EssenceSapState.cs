using EntityStates;
using RoR2;
using System;
using UnityEngine;
using UnityEngine.Networking;

namespace BloodMageMod.SkillStates
{
    public class EssenceSapState : BaseSkillState
    {
        public GameObject hitEffectPrefab = Resources.Load<GameObject>("prefabs/effects/impacteffects/critspark");
        public GameObject tracerEffectPrefab = Resources.Load<GameObject>("prefabs/effects/tracers/tracerbanditshotgun");

        private const float ticksPerSecond = 5f;
        private const float healthPercent = 0.02f;

        private GameObject target;
        private float timeSinceLastTick = 0.0f;

        public override void OnEnter()
        {
            base.OnEnter();
            if (base.isAuthority && base.healthComponent.health < base.healthComponent.fullHealth) {
                Ray aimRay = base.GetAimRay();
                new BulletAttack
                {
                    owner = base.gameObject,
                    weapon = base.gameObject,
                    origin = aimRay.origin,
                    aimVector = aimRay.direction,
                    minSpread = 0f,
                    maxSpread = 0f,
                    bulletCount = 1U,
                    procCoefficient = 0f,
                    damage = healthPercent * base.healthComponent.fullHealth,
                    damageType = DamageType.DoT,
                    force = 0,
                    falloffModel = BulletAttack.FalloffModel.None,
                    tracerEffectPrefab = this.tracerEffectPrefab,
                    hitEffectPrefab = this.hitEffectPrefab,
                    isCrit = false,
                    stopperMask = LayerIndex.world.mask,
                    smartCollision = true,
                    maxDistance = 50f,
                    hitCallback = this.OnHit,
                }.Fire();
            }
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            bool canHeal = this.healthComponent.health < this.healthComponent.fullHealth * (base.characterBody.HasBuff(Modules.Buffs.petrifiedBloodBuff.BuffDef) ? 0.5f : 1);
            if (base.isAuthority && (!base.IsKeyDownAuthority() || !canHeal)) {
                this.outer.SetNextStateToMain();
                return;
            }

            if (this.target == null) {
                return;
            }

            if (!this.target.GetComponent<HealthComponent>().alive && base.isAuthority) {
                this.outer.SetNextStateToMain();
                return;
            }

            timeSinceLastTick += Time.fixedDeltaTime;
            if (timeSinceLastTick >= 1/(ticksPerSecond * base.characterBody.attackSpeed)) {
                timeSinceLastTick = 0.0f;
                HealthComponent tHC = target.GetComponent<HealthComponent>();
                tHC.TakeDamage(new DamageInfo {
                    attacker = base.gameObject,
                    crit = false,
                    damage = healthPercent * base.healthComponent.fullHealth,
                    force = Vector3.zero,
                    inflictor = base.gameObject,
                    position = target.GetComponent<Rigidbody>().position,
                    procCoefficient = 0f,
                    damageType = DamageType.DoT
                });
                this.healthComponent.HealFraction(0.02f, new ProcChainMask());
            }
        }

        public override void OnExit()
        {
            if (this.target == null) {
                base.activatorSkillSlot.AddOneStock();
            }
            this.target = null;
            this.timeSinceLastTick = 0.0f;
            base.OnExit();
        }

        public override InterruptPriority GetMinimumInterruptPriority() {
            return InterruptPriority.PrioritySkill;
        }

        protected bool OnHit(ref BulletAttack.BulletHit bulletHit)
        {
            this.target = bulletHit.entityObject;
            var targetBod = this.target.GetComponent<CharacterBody>();
            if (targetBod != null && targetBod.teamComponent.teamIndex == TeamIndex.Monster) {
                base.healthComponent.HealFraction(healthPercent, new ProcChainMask());
            } else {
                this.target = null;
            }
            return true;
        }
    }
}
