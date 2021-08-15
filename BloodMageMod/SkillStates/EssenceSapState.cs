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
            if (base.isAuthority && base.healthComponent.health < base.healthComponent.fullHealth * (base.characterBody.HasBuff(Modules.Buffs.petrifiedBloodBuff) ? 0.5f : 1)) {
                Ray aimRay = base.GetAimRay();
                RaycastHit hit;
                bool hitSomething = Physics.Raycast(aimRay.origin, aimRay.direction, out hit, 50f, LayerIndex.world.mask | LayerIndex.entityPrecise.mask, QueryTriggerInteraction.Ignore);
                if (!hitSomething) {
                    base.activatorSkillSlot.AddOneStock();
                    outer.SetNextStateToMain();
                    return;
                }
                HurtBox hitHurtBox = hit.collider.GetComponent<HurtBox>();
                if (hitHurtBox != null && hitHurtBox.teamIndex == TeamIndex.Monster) {
                    HealthComponent hitTarget = hitHurtBox.healthComponent;
                    target = hitTarget.gameObject;
                    hitTarget.TakeDamage(new DamageInfo {
                        attacker = base.gameObject,
                        crit = false,
                        damage = healthPercent * base.healthComponent.fullHealth,
                        force = Vector3.zero,
                        inflictor = base.gameObject,
                        position = target.GetComponent<Rigidbody>().position,
                        procCoefficient = 0f,
                        damageType = DamageType.DoT
                    });
                    base.healthComponent.HealFraction(0.1f, new ProcChainMask());
                }
            }
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            if (base.isAuthority && !base.IsKeyDownAuthority()) {
                this.outer.SetNextStateToMain();
                return;
            }

            if (this.target == null) {
                return;
            }

            bool canHeal = this.healthComponent.health < this.healthComponent.fullHealth * (base.characterBody.HasBuff(Modules.Buffs.petrifiedBloodBuff) ? 0.5f : 1);
            if ((!this.target.GetComponent<HealthComponent>().alive || !canHeal) && base.isAuthority) {
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
    }
}
