export interface BackgammonState {
    currentPlayer: 'white' | 'black';
    diceRolls: {
        white: number[];
        black: number[];
    }
    points: {
        white: number;
        black: number;
    }[];
    bar: {
        white: number;
        black: number;
    };
}
