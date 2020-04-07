using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;

namespace SignalRGame.ClashOfClones
{
    public readonly struct UnitStats
    {
        public string Name { get; }

        /// <summary>
        /// Base strength of a formation after fully charged
        /// </summary>
        public int Attack { get; }
        /// <summary>
        /// Base HP of a unit when not charged
        /// </summary>
        public int Power { get; }
        /// <summary>
        /// Amount of strength gained per turn while charging
        /// </summary>
        public int ChargePerTurn { get; }
        /// <summary>
        /// Number of turns a formation must charge
        /// </summary>
        public int ChargeTime { get; }

        /// <summary>
        /// Custom rules
        /// </summary>
        public IUnitRule Rule { get; }

        public UnitStats(string name, int attack, int power, int chargePerTurn, int chargeTime, IUnitRule? rule = null)
        {
            this.Name = name;
            this.Attack = attack;
            this.Power = power;
            this.ChargePerTurn = chargePerTurn;
            this.ChargeTime = chargeTime;
            this.Rule = rule ?? NoOpRule.Instance;
        }
    }

    public interface IUnitRule
    {

    }

    public class NoOpRule : IUnitRule
    {
        public static readonly IUnitRule Instance = new NoOpRule();

        private NoOpRule() { }
    }

    public class GameSettings
    {
        public readonly IReadOnlyDictionary<string, UnitStats> Units = new Dictionary<string, UnitStats>()
        {
            { "Human-Swordsmen", new UnitStats("Swordsmen", attack: 11, power: 3, chargePerTurn: 3, chargeTime: 3) },
        }.ToImmutableDictionary();

        public readonly IReadOnlyDictionary<string, UnitStats> Elites = new Dictionary<string, UnitStats>()
        {
            { "Human-Knight", new UnitStats("Knight", attack: 30, power: 6, chargePerTurn: 0, chargeTime: 4) },
        }.ToImmutableDictionary();

        public readonly IReadOnlyDictionary<string, UnitStats> Champions = new Dictionary<string, UnitStats>()
        {
            { "Human-Angel", new UnitStats("Angel", attack: 115, power: 23, chargePerTurn: 15, chargeTime: 6) },
        }.ToImmutableDictionary();

    }
}
