using SignalRGammon.Clash.StateComponents;
using System.Linq;
using System.Threading.Tasks;

namespace SignalRGammon.Clash
{
    public readonly struct ClashState
    {
        public Player CurrentPlayer { get; }
        public Player? Winner { get; }
        public PlayerState<bool> IsReady { get; }

        public PlayerState<ArmyConfiguration> ArmyConfiguration { get; }
        public PlayerState<int> Health { get; }
        public PlayerState<int> LimitAmount { get; }
        public PlayerState<ArmyLayout> Battlefield { get; }

        public ClashState(Player CurrentPlayer, 
            Player? Winner,
            PlayerState<bool> IsReady,
            PlayerState<ArmyConfiguration> ArmyConfiguration,
            PlayerState<int> Health,
            PlayerState<int> LimitAmount,
            PlayerState<ArmyLayout> Battlefield
        )
        {
            this.CurrentPlayer = CurrentPlayer;
            this.Winner = Winner;
            this.IsReady = IsReady;
            this.ArmyConfiguration = ArmyConfiguration;
            this.Health = Health;
            this.LimitAmount = LimitAmount;
            this.Battlefield = Battlefield;
        }

        public ClashState With(
            Player? CurrentPlayer = null,
            Player? Winner = null,
            PlayerState<bool>? IsReady = null,
            PlayerState<ArmyConfiguration>? ArmyConfiguration = null,
            PlayerState<int>? Health = null,
            PlayerState<int>? LimitAmount = null,
            PlayerState<ArmyLayout>? Battlefield = null
        )
        {
            return new ClashState(
                CurrentPlayer: CurrentPlayer ?? this.CurrentPlayer,
                Winner: Winner ?? this.Winner,
                IsReady: IsReady ?? this.IsReady,
                ArmyConfiguration: ArmyConfiguration ?? this.ArmyConfiguration,
                Health: Health ?? this.Health,
                LimitAmount: LimitAmount ?? this.LimitAmount,
                Battlefield: Battlefield ?? this.Battlefield
            );
        }
    }
}
