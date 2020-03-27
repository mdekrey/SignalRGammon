export const players = Object.freeze<PlayerColor[]>([ 'black', 'white' ]);
export type PlayerColor = 'white' | 'black';
export type PerPlayer<T> = {
    [color in PlayerColor]: T;
}
export function otherPlayer(playerColor: PlayerColor) { return playerColor === 'white' ? 'black' : 'white'; }

export type SingleChecker = {
    column: number;
    row: number;
    isKing: number;
};

export interface CheckersState {
    winner: PlayerColor | null;
    currentPlayer: PlayerColor;
    isReady: PerPlayer<boolean>;
    checkers: PerPlayer<(SingleChecker | null)[]>;
}
