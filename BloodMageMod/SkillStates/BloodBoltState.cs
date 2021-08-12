using EntityStates;
using RoR2;
using System;
using UnityEngine;
using UnityEngine.Networking;

namespace BloodMageMod.SkillStates {
    public class BloodBoltState : BaseSkillState {

        private const float baseHealthPerTick = 3.0f;
        private const float ticksPerSecond = 5f;
        private const float damageCoefficient = 2.0f;
        public GameObject hitEffectPrefab = Resources.Load<GameObject>("prefabs/effects/impacteffects/critspark");
        public GameObject tracerEffectPrefab = Resources.Load<GameObject>("prefabs/effects/tracers/tracerbanditshotgun");

        private float healthAbsorb = 0.0f;
        private float timeSinceLastTick = 0.0f;

        public override void OnEnter() {
            base.OnEnter();
            if (!this.SelfDamage() && base.isAuthority) {
                this.outer.SetNextStateToMain();
            };
        }

        public override void FixedUpdate() {
            base.FixedUpdate();
            if (base.isAuthority && !base.IsKeyDownAuthority()) {
                this.outer.SetNextStateToMain();
                return;
            }

            timeSinceLastTick += Time.fixedDeltaTime;
            if (timeSinceLastTick >= 1/(ticksPerSecond * base.characterBody.attackSpeed)) {
                timeSinceLastTick = 0.0f;
                //If we can't self-damage anymore, finish the cast.
                if (base.isAuthority && !this.SelfDamage()) {
                    this.outer.SetNextStateToMain();
                    return;
                }
            }
        }

        public override void OnExit()
        {
            if (base.isAuthority && this.healthAbsorb > 0) {
                Ray aimRay = base.GetAimRay();
                
                new BulletAttack {
                    owner = base.gameObject,
                    weapon = base.gameObject,
                    origin = aimRay.origin,
                    aimVector = aimRay.direction,
                    minSpread = 0f,
                    maxSpread = 0f,
                    bulletCount = 1U,
                    procCoefficient = 1f,
                    damage = damageCoefficient * (base.characterBody.damage + this.healthAbsorb),
                    force = 0,
                    falloffModel = BulletAttack.FalloffModel.DefaultBullet,
                    tracerEffectPrefab = this.tracerEffectPrefab,
                    hitEffectPrefab = this.hitEffectPrefab,
                    isCrit = false,
                    stopperMask = LayerIndex.world.mask,
                    smartCollision = true,
                    maxDistance = 300f
                }.Fire();
            }
            this.healthAbsorb = 0.0f;
            this.timeSinceLastTick = 0.0f;
            base.OnExit();
        }

        public override InterruptPriority GetMinimumInterruptPriority() {
            return InterruptPriority.Skill;
        }

        private bool SelfDamage() {
            float healthToTake = RoR2.Run.instance.compensatedDifficultyCoefficient * baseHealthPerTick;
            if (base.characterBody.healthComponent.combinedHealth > baseHealthPerTick) {
                DamageInfo info = new DamageInfo();
                info.damage = baseHealthPerTick;
                info.damageType = DamageType.DoT;
                this.characterBody.healthComponent.TakeDamage(info);
                this.healthAbsorb += baseHealthPerTick;
                return true;
            }
            return false;
        }
    }
}