using SignalRGame.ClashOfClones.StateComponents;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SignalRGame.ClashOfClones
{
    public class Defaults
    {
        public static readonly ArmyConfiguration DefaultArmyConfiguration = new ArmyConfiguration(
            15,
            new StandardUnitConfiguration<string>("Human-Swordsmen", "Human-Swordsmen", "Human-Swordsmen"), 
            new SpecialUnitConfiguration[0]
        );
        public static readonly ArmyConfiguration StaffedArmyConfiguration = new ArmyConfiguration(
            30,
            new StandardUnitConfiguration<string>("Human-Swordsmen", "Human-Swordsmen", "Human-Swordsmen"),
            new []
            {
                new SpecialUnitConfiguration("Human-Knight", 10),
                new SpecialUnitConfiguration("Human-Angel", 5),
            }
        );
        public static readonly ClashState DefaultState = new ClashState(
                CurrentPlayer: Player.White,
                Winner: null,
                IsReady: new PlayerState<bool>(false, false), 
                ArmyConfiguration: new PlayerState<ArmyConfiguration>(DefaultArmyConfiguration, DefaultArmyConfiguration),
                Health: new PlayerState<int>(100, 100),                
                LimitAmount: new PlayerState<int>(0, 0),
                Battlefield: new PlayerState<ArmyLayout>()
            );
    }
}
