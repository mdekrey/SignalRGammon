export interface BackgammonState {
    currentPlayer: 'White' | 'Black';
    whiteDiceRolls: number[];
    blackDiceRolls: number[];
    points: {
        white: number;
        black: number;
    }[];
    fence: {
        white: number;
        black: number;
    };
}
