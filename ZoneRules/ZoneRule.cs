using BaldiPlusRandomZone.EndlessSupport;
using System;

namespace BaldiPlusRandomZone.ZoneRules
{
    public class ZoneRule
    {
        public ZoneRule(int positiveWeight, int negativeWeight)
        {
            this.positiveWeight = positiveWeight;
            this.negativeWeight = negativeWeight;
        }

        public ZoneRule(int positiveWeight) => this.positiveWeight = positiveWeight;

        public virtual void Initialize(BasicZoneManager gameManager, EnvironmentController ec) { }
        public virtual void ModifyLevelObject(LevelGenerationParameters level, int levelId) { }
        public virtual void ModifySceneObject(SceneObject level) { }
        public virtual void OnGameUpdate(BaseGameManager gameManager) { }
        public virtual void OnSpoopModeBegin(EnvironmentController ec) { }
        public virtual void AfterAllRoomsPlaced(RoomController room, LevelBuilder lb, Random random) { }
        public virtual void Generate(LevelBuilder lb, Random random) { }

        public virtual void LoadPreparation(Random random) { }

        public ZoneRuleType ChoseRandomRuleType(Random random)
        {
            ZoneRuleType type = (ZoneRuleType)random.Next(0, 2);
            SetRuleType(type);
            return type;
        }

        public virtual void SetRuleType(ZoneRuleType type)
        {
            this.type = type;
            weight = type == ZoneRuleType.Positive ? positiveWeight : negativeWeight;
            powerBonus = type == ZoneRuleType.Positive ? positivePowerBonus : negativePowerBonus;
        }

        public virtual ZoneRuleCategory category => ZoneRuleCategory.Empty;
        public virtual ZoneRuleType type { get; set; }

        public int weight = 0;
        public int positiveWeight = 0;
        public int negativeWeight = 0;

        public float powerBonus = 1f;
        public virtual float positivePowerBonus => 1f;
        public virtual float negativePowerBonus => 1f;
    }
}
