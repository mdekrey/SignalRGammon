import React, { useContext, createContext, useMemo, useCallback } from "react";
import { Observable } from "rxjs";
import { useGameConnection } from "../../services/gameConnectionContext";
import { CheckersState, PlayerColor, otherPlayer } from "./CheckersState";
import { useRouteMatch } from "react-router";
import { map } from "rxjs/operators";

export type CheckersAction = {
    // TODO
};

export type ValidMove = {
    checkerIndex: number;
    isJump: boolean;
    column: number;
    row: number;
};

export type ObservedState = {
    state: { state: CheckersState; validMovesForCurrentPlayer: ValidMove[]; };
    action: CheckersAction | null;
};

export type CheckersContextResult = {
    state: Observable<ObservedState>

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
    const {doAction, getGameState} = useGameConnection();
    const { url } = useRouteMatch();
    const otherPlayerUrl = window.location.href.replace(url, url.replace(playerColor, otherPlayer(playerColor)));

    const move = useCallback(async (move: ValidMove) => {
        return await doAction(gameId, { type: 'move', player: playerColor, pieceIndex: move.checkerIndex, column: move.column, row: move.row });
    }, [doAction, gameId, playerColor])

    const ready = useCallback(async () => {
        return await doAction(gameId, { type: 'ready', player: playerColor });
    }, [doAction, gameId, playerColor])

    const newGame = useCallback(async () => {
        return await doAction(gameId, { type: 'new-game' });
    }, [doAction, gameId])

    const state = useMemo(() => getGameState<ObservedState["state"]>(gameId)
        .pipe(
            map((state) => ({ state, action: null }) as ObservedState),
        ), [getGameState, gameId]);
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
