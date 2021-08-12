using EntityStates;
using RoR2;
using System;
using UnityEngine;
using UnityEngine.Networking;

namespace BloodMageMod.SkillStates
{
    public class DoomDesireState : BaseSkillState
    {
        public static void Hooks()
        {
            On.RoR2.HealthComponent.TakeDamage += TakeDamage;
            On.RoR2.CharacterBody.OnBuffFinalStackLost += DoomDesireProc;
        }

        private const float duration = 10.0f;
        private const float damageCoefficient = 30.0f;

        public override void OnEnter()
        {
            base.OnEnter();
            if (base.isAuthority) {
                Ray aimRay = base.GetAimRay();
                RaycastHit hit;
                bool hitSomething = Physics.Raycast(aimRay.origin, aimRay.direction, out hit, 50f, LayerIndex.world.mask | LayerIndex.entityPrecise.mask, QueryTriggerInteraction.Ignore);
                if (!hitSomething) {
                    base.activatorSkillSlot.AddOneStock();
                    outer.SetNextStateToMain();
                    return;
                }
                Log.LogInfo("Layer index of object: " + hit.rigidbody.gameObject.layer);
                if (hit.rigidbody.gameObject.GetComponent<CharacterBody>().teamComponent.teamIndex == TeamIndex.Monster)
                {
                    HealthComponent hitTarget = hit.rigidbody.gameObject.GetComponent<HealthComponent>();
                    DoomDesireTracker ddt = base.gameObject.GetComponent<DoomDesireTracker>();
                    if (!ddt)
                        ddt = base.gameObject.AddComponent<DoomDesireTracker>();
                    ddt.target = hitTarget;
                    hitTarget.body.AddTimedBuff(Modules.Buffs.doomDesireBuff.BuffDef, duration, 1);
                    base.characterBody.AddTimedBuff(Modules.Buffs.doomDesireBuff.BuffDef, duration, 1);
                } else
                {
                    base.activatorSkillSlot.AddOneStock();
                }
                outer.SetNextStateToMain();
                return;
            }
            
        }

        public override InterruptPriority GetMinimumInterruptPriority() {
            return InterruptPriority.Skill;
        }

        public static void TakeDamage(On.RoR2.HealthComponent.orig_TakeDamage orig, HealthComponent self, DamageInfo info)
        {
            DoomDesireTracker ddt = self.GetComponent<DoomDesireTracker>();
            if (info != null && ddt != null && self.body.HasBuff(Modules.Buffs.doomDesireBuff.BuffDef)) {
                ddt.StoredDamage += info.damage;
            }

            orig(self, info);
        }

        public static void DoomDesireProc(On.RoR2.CharacterBody.orig_OnBuffFinalStackLost orig, CharacterBody self, BuffDef buffDef)
        {
            if (buffDef.buffIndex == Modules.Buffs.doomDesireBuff.BuffDef.buffIndex)
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
