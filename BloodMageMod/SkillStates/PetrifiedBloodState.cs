using EntityStates;
using RoR2;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using static BloodMageMod.Modules.Buffs;

namespace BloodMageMod.SkillStates
{
    public class PetrifiedBloodState : BaseSkillState
    {

        public static void Hooks()
        {
            On.RoR2.HealthComponent.TakeDamage += TakeDamage;
            On.RoR2.HealthComponent.Heal += PreventHeal;
            On.RoR2.HealthComponent.HealFraction += PreventHealFraction;
            On.RoR2.CharacterBody.FixedUpdate += TickDamage;
        }

        internal const float dotPercent = 0.6f;
        internal const float dotDuration = 4f;
        internal const float dotTickrate = 1f;

        public override void OnEnter() {
            base.OnEnter();
            
            if (base.isAuthority) {
                BuffDef pbDef = petrifiedBloodBuff;
                if (base.characterBody.HasBuff(petrifiedBloodBuff)) {
                    base.characterBody.RemoveBuff(petrifiedBloodBuff);
                } else {
                    base.characterBody.AddBuff(petrifiedBloodBuff);
                    if (base.healthComponent.health / base.healthComponent.fullHealth > 0.5f)
                        base.healthComponent.health = base.healthComponent.fullHealth * 0.5f;
                }
                outer.SetNextStateToMain();
                return;
            }
        }

        public override InterruptPriority GetMinimumInterruptPriority() {
            return InterruptPriority.PrioritySkill;
        }

        public static void TakeDamage(On.RoR2.HealthComponent.orig_TakeDamage orig, HealthComponent self, DamageInfo damageInfo)
        {
            if (damageInfo != null && self.body.HasBuff(petrifiedBloodBuff)) {
                PBDegenComponent degenComp = self.GetComponent<PBDegenComponent>();
                if (!degenComp) degenComp = self.gameObject.AddComponent<PBDegenComponent>();

                if (damageInfo.damageType != DamageType.DoT) {
                    var damage = damageInfo.damage * dotPercent;
                    var time = dotDuration;
                    var ticksPerSecond = dotTickrate;
                    var totalTicks = time * ticksPerSecond;
                    damageInfo.damage -= damage;
                    degenComp.DegenStacks.Add(new DegenStack(1 / ticksPerSecond, totalTicks, damage / totalTicks, damageInfo.attacker));
                }
            }

            orig(self, damageInfo);
        }

        public static void TickDamage(On.RoR2.CharacterBody.orig_FixedUpdate orig, CharacterBody self)
        {
            orig(self);

            if (self.HasBuff(petrifiedBloodBuff) && self.healthComponent.health / self.healthComponent.fullHealth > 0.5f)
                self.healthComponent.health = self.healthComponent.fullHealth * 0.5f;

            PBDegenComponent degenComp = self.GetComponent<PBDegenComponent>();
            if (!degenComp) degenComp = self.gameObject.AddComponent<PBDegenComponent>();

            foreach(DegenStack stack in degenComp.DegenStacks)
            {
                stack.FixedUpdate(self);
            }
            degenComp.DegenStacks.RemoveAll(x => x.ticksLeft <= 0);
        }

        public static float PreventHeal(On.RoR2.HealthComponent.orig_Heal orig, HealthComponent self, float amount, ProcChainMask mask, bool nonRegen = true) {
            if (self.body.HasBuff(petrifiedBloodBuff)) {
                float finalHealth = self.health + amount;
                if (finalHealth / self.fullHealth > 0.5f) {
                    float diff = finalHealth - (self.fullHealth * 0.5f);
                    amount -= diff;
                }
            }

            return orig(self, amount, mask, nonRegen);
        }

        public static float PreventHealFraction(On.RoR2.HealthComponent.orig_HealFraction orig, HealthComponent self, float percent, ProcChainMask mask) {
            if (self.body.HasBuff(petrifiedBloodBuff)) {
                float healAmount = self.fullHealth * percent;
                float finalHealth = self.health + healAmount;
                if (finalHealth / self.fullHealth > 0.5f) {
                    float diff = finalHealth - (self.fullHealth * 0.5f);
                    float diffAsPercent = diff / self.fullHealth;
                    percent -= diffAsPercent;
                }
            }

            return orig(self, percent, mask);
        }
    }

    public class PBDegenComponent : MonoBehaviour
    {
        public List<DegenStack> DegenStacks = new List<DegenStack>();
    }

    public class DegenStack
    {

        public float timePerTick;
        public float timeThisTick = 0;
        public float ticksLeft;
        public float damagePerTick;
        public GameObject attacker;
        
        public DegenStack(float timePerTick, float totalTicks, float damagePerTick, GameObject attacker)
        {
            this.timePerTick = timePerTick;
            this.ticksLeft = totalTicks;
            this.damagePerTick = damagePerTick;
            this.attacker = attacker;
        }

        public void FixedUpdate(CharacterBody characterBody) {
            timeThisTick += Time.fixedDeltaTime;
            if (timeThisTick >= timePerTick)
            {
                timeThisTick -= timePerTick;
                ticksLeft--;
                DamageInfo damageInfo = new DamageInfo {
                    attacker = this.attacker,
                    crit = false,
                    damage = damagePerTick,
                    force = Vector3.zero,
                    inflictor = attacker,
                    position = characterBody.corePosition,
                    procCoefficient = 0f,
                    damageColorIndex = DamageColorIndex.Bleed,
                    damageType = DamageType.DoT
                };

                characterBody.healthComponent.TakeDamage(damageInfo);
            }
        }
    }
}