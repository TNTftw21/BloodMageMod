using EntityStates;
using RoR2;
using System;
using UnityEngine;
using UnityEngine.Networking;

using static BloodMageMod.Modules.Buffs;

namespace BloodMageMod.SkillStates
{
    public class DoomDesireState : BaseSkillState
    {
        public static void Hooks()
        {
            On.RoR2.HealthComponent.TakeDamage += TakeDamage;
            On.RoR2.CharacterBody.OnBuffFinalStackLost += DoomDesireProc;
        }

        private const float skillDuration = 0.5f;
        private const float duration = 3.0f;
        private const float damageCoefficient = 30.0f;
        

        public override void OnEnter()
        {
            base.OnEnter();
            if (this.isAuthority) {
                DoomDesireTracker ddt = this.gameObject.GetComponent<DoomDesireTracker>();
                if (ddt != null && ddt.target != null) {
                    Chat.AddMessage("Detonating current debuff!");
                    ddt.target.body.RemoveBuff(doomDesireBuff);
                    this.characterBody.RemoveBuff(doomDesireBuff);
                } else {
                    Ray aimRay = this.GetAimRay();
                    RaycastHit hit;
                    bool hitSomething = Physics.Raycast(aimRay.origin, aimRay.direction, out hit, 50f, LayerIndex.world.mask | LayerIndex.entityPrecise.mask, QueryTriggerInteraction.Ignore);
                    if (!hitSomething) {
                        this.activatorSkillSlot.AddOneStock();
                        outer.SetNextStateToMain();
                        return;
                    }
                    HurtBox hitHurtBox = hit.collider.GetComponent<HurtBox>();
                    if (hitHurtBox && hitHurtBox.teamIndex == TeamIndex.Monster)
                    {
                        HealthComponent hitTarget = hitHurtBox.healthComponent;
                        if (!ddt)
                            ddt = this.gameObject.AddComponent<DoomDesireTracker>();
                        ddt.target = hitTarget;
                        //Add in a little bit of lead time because of the animation
                        hitTarget.body.AddTimedBuff(doomDesireBuff, duration + (skillDuration / this.characterBody.attackSpeed), 1);
                        this.characterBody.AddTimedBuff(doomDesireBuff, duration + (skillDuration / this.characterBody.attackSpeed), 1);
                    } else
                    {
                        this.activatorSkillSlot.AddOneStock();
                    }
                }
                
                outer.SetNextStateToMain();
                return;
            }
        }

        public override void FixedUpdate()
        {
            this.FixedUpdate();
            if (this.isAuthority && this.fixedAge > skillDuration / this.characterBody.attackSpeed) {
                outer.SetNextStateToMain();
                return;
            }
        }

        public override InterruptPriority GetMinimumInterruptPriority() {
            return InterruptPriority.Skill;
        }

        public static void TakeDamage(On.RoR2.HealthComponent.orig_TakeDamage orig, HealthComponent self, DamageInfo info)
        {
            orig(self, info);

            DoomDesireTracker ddt = self.GetComponent<DoomDesireTracker>();
            if (info != null && !info.rejected && ddt != null && self.body.HasBuff(doomDesireBuff)) {
                ddt.StoredDamage += info.damage;
            }
        }

        public static void DoomDesireProc(On.RoR2.CharacterBody.orig_OnBuffFinalStackLost orig, CharacterBody self, BuffDef buffDef)
        {
            if (buffDef.buffIndex == doomDesireBuff.buffIndex)
            {
                DoomDesireTracker ddt = self.gameObject.GetComponent<DoomDesireTracker>();
                if (ddt) {
                    HealthComponent target = ddt.target;
                    new BlastAttack {
                        baseDamage = ddt.StoredDamage * damageCoefficient,
                        procCoefficient = 1.0f,
                        inflictor = self.gameObject,
                        teamIndex = self.teamComponent.teamIndex,
                        radius = 7f,
                        attacker = self.gameObject,
                        baseForce = 3f,
                        crit = self.RollCrit(),
                        damageType = DamageType.Generic,
                        falloffModel = BlastAttack.FalloffModel.Linear,
                        position = target.body.corePosition
                    }.Fire();
                    ddt.target = null;
                    ddt.StoredDamage = 0;
                }
            }

            orig(self, buffDef);
        }
    }

    public class DoomDesireTracker : MonoBehaviour
    {
        public float StoredDamage = 0f;
        public HealthComponent target;
    }
}
