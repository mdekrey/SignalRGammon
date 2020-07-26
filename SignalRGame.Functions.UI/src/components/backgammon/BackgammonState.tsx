export interface BackgammonState {
    winner: 'white' | 'black' | null;
    currentPlayer: 'white' | 'black' | null;
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
    undo: null | BackgammonState;
}
