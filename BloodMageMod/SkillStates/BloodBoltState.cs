using EntityStates;
using RoR2;
using R2API;
using System;
using UnityEngine;
using UnityEngine.Networking;

namespace BloodMageMod.SkillStates {
    public class BloodBoltState : BaseSkillState {

        private const float baseHealthPerSecond = 15f;
        private const float ticksPerSecond = 2f;
        private const float baseHealthPerTick = baseHealthPerSecond / ticksPerSecond;
        private const float damageCoefficient = 2.0f;
        private const float releaseDuration = 0.15f;
        private const float recoverDuration = 0.15f;
        public static readonly GameObject hitEffectPrefab = Resources.Load<GameObject>("prefabs/effects/impacteffects/critspark");
        public static readonly GameObject tracerEffectPrefab = Resources.Load<GameObject>("prefabs/effects/tracers/tracerbanditshotgun");

        private float healthAbsorb = 0.0f;
        private float timeSinceLastTick = 0.0f;
        private bool hasReleased = false;
        private bool hasFired = false;

        public override void OnEnter() {
            base.OnEnter();
            if (!this.SelfDamage() && this.isAuthority) {
                outer.SetNextStateToMain();
                return;
            };
            AudioSource source = this.gameObject.GetComponentInParent<AudioSource>() ?? this.gameObject.AddComponent<AudioSource>();
            source.clip = Modules.Assets.mainAssetBundle.LoadAsset<AudioClip>("assets/sounds/skills/bloodbolt_charge.wav");
            source.loop = true;
            source.Play();
        }

        public override void FixedUpdate() {
            base.FixedUpdate();

            //Skill has been released, but hasn't recognized it yet.
            if (!hasReleased && !IsKeyDownAuthority()) {
                this.hasReleased = true;
                this.timeSinceLastTick = 0f;
                this.gameObject.GetComponent<AudioSource>()?.Stop();
                return;
            }

            timeSinceLastTick += Time.fixedDeltaTime;

            //Skill hasn't been released, and we've waited long enough for a tick.
            if (!hasReleased && timeSinceLastTick >= 1/(ticksPerSecond * this.characterBody.attackSpeed)) {
                timeSinceLastTick = 0.0f;
                if (!SelfDamage()) {
                    this.hasReleased = true;
                    this.gameObject.GetComponent<AudioSource>()?.Stop();
                    return;
                }
            }

            //Skill has been released, we haven't fired, and we've waited long enough to fire
            if (this.hasReleased && !this.hasFired && timeSinceLastTick >= releaseDuration / characterBody.attackSpeed) {
                timeSinceLastTick = 0f;
                this.Fire();
            }

            //We have fired and we've waited the recovery period
            if (this.hasFired && timeSinceLastTick >= recoverDuration / characterBody.attackSpeed) {
                outer.SetNextStateToMain();
                return;
            }
        }

        public override void OnExit()
        {
            if (!hasFired) this.Fire();
            this.gameObject.GetComponent<AudioSource>()?.Stop();
            base.OnExit();
        }

        public override InterruptPriority GetMinimumInterruptPriority() {
            return InterruptPriority.Skill;
        }

        private bool SelfDamage() {
            float healthToTake = RoR2.Run.instance.compensatedDifficultyCoefficient * baseHealthPerTick;
            if (this.characterBody.healthComponent.combinedHealth > healthToTake) {
                this.characterBody.healthComponent.health -= healthToTake;
                this.healthAbsorb += healthToTake;
                if (this.characterBody.HasBuff(Modules.Buffs.doomDesireBuff))
                {
                    DoomDesireTracker ddt = this.gameObject.GetComponent<DoomDesireTracker>() ?? this.gameObject.AddComponent<DoomDesireTracker>();
                    ddt.StoredDamage += healthToTake;
                }
                return true;
            }
            return false;
        }

        private void Fire() {
            this.hasFired = true;
            if (this.healthAbsorb > 0) {
                AudioSource source = this.gameObject.GetComponent<AudioSource>() ?? this.gameObject.AddComponent<AudioSource>();
                source.loop = false;
                source.clip = Modules.Assets.mainAssetBundle.LoadAsset<AudioClip>("assets/sounds/skills/bloodbolt_fire.wav");
                source.Play();
                Ray aimRay = this.GetAimRay();
                
                new BulletAttack {
                    owner = this.gameObject,
                    weapon = this.gameObject,
                    origin = aimRay.origin,
                    aimVector = aimRay.direction,
                    minSpread = 0f,
                    maxSpread = 0f,
                    bulletCount = 1U,
                    procCoefficient = 1f,
                    damage = damageCoefficient * (this.characterBody.damage + this.healthAbsorb),
                    force = 0,
                    falloffModel = BulletAttack.FalloffModel.DefaultBullet,
                    tracerEffectPrefab = tracerEffectPrefab,
                    hitEffectPrefab = hitEffectPrefab,
                    isCrit = false,
                    stopperMask = LayerIndex.world.mask,
                    smartCollision = true,
                    maxDistance = 300f
                }.Fire();
            }
            this.healthAbsorb = 0;
        }
    }
}