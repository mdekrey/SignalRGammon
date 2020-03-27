import React, { useContext, createContext, useMemo, useCallback } from "react";
import { Observable, from } from "rxjs";
import { switchMap, map } from "rxjs/operators";
import { useGameConnection } from "../../services/gameConnectionContext";
import { fromSignalR } from "../../utils/fromSignalR";
import { CheckersState, PlayerColor, otherPlayer } from "./CheckersState";
import { useRouteMatch } from "react-router";

export type CheckersAction = {
    // TODO
};

export type ValidMove = {
    checkerIndex: number;
    isJump: boolean;
    moves: number[][];
};

export type ObservedState = {
    state: CheckersState;
    validMovesForCurrentPlayer: ValidMove[];
    action: CheckersAction;
};

export type CheckersContextResult = {
    state: Observable<ObservedState | null>

    // TODO - actions
    move: (move: ValidMove) => Promise<boolean>
    ready: () => Promise<boolean>
    newGame: () => Promise<boolean>

    otherPlayerUrl: string
    playerColor: PlayerColor;
};

const CheckersContext = createContext({} as CheckersContextResult);
export function useCheckers() {
    return useContext(CheckersContext);
}

export type CheckersScopeProps = {
    gameId: string;
    playerColor: PlayerColor;
    children: React.ReactNode;
}
export function CheckersScope({ gameId, playerColor, children }: CheckersScopeProps) {
    const {connection, connected} = useGameConnection();
    const { url } = useRouteMatch();
    const otherPlayerUrl = window.location.href.replace(url, url.replace(playerColor, otherPlayer(playerColor)));

    const move = useCallback(async (move: ValidMove) => {
        await connected;
        return await connection.invoke<boolean>('Do', gameId, JSON.stringify({ type: 'move', player: playerColor, pieceIndex: move.checkerIndex, destination: move.moves }));
    }, [connected, connection, gameId, playerColor])

    const ready = useCallback(async () => {
        await connected;
        return await connection.invoke<boolean>('Do', gameId, JSON.stringify({ type: 'ready', player: playerColor }));
    }, [connected, connection, gameId, playerColor])

    const newGame = useCallback(async () => {
        await connected;
        return await connection.invoke<boolean>('Do', gameId, JSON.stringify({ type: 'new-game' }));
    }, [connected, connection, gameId])

    const state = useMemo(() => from(connected)
        .pipe(
            switchMap(() => fromSignalR(connection.stream('ListenState', gameId))),
            map(json => JSON.parse(json) as ObservedState),
        ), [connection, connected, gameId]);
    const value = useMemo(() => ({
        state,
        newGame,
        ready,
        move,
        otherPlayerUrl,
        playerColor,
    }), [state, newGame, ready, move, otherPlayerUrl, playerColor]);

    return (
        <CheckersContext.Provider value={value}>
            {children}
        </CheckersContext.Provider>
    )
}
