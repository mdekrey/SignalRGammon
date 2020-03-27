import React, { useContext, createContext, useMemo, useCallback } from "react";
import { Observable, from } from "rxjs";
import { switchMap, map, scan } from "rxjs/operators";
import { useGameConnection } from "../../services/gameConnectionContext";
import { fromSignalR } from "../../utils/fromSignalR";
import { CheckersState, PlayerColor, otherPlayer } from "./CheckersState";
import { useRouteMatch } from "react-router";

export type CheckersAction = {
    // TODO
};

export type ObservedState = { state: CheckersState, action: CheckersAction };

export type CheckersContextResult = {
    state: Observable<ObservedState | null>

    // TODO - actions
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

    const ready = useCallback(async () => {
        await connected;
        return await connection.invoke<boolean>('Do', gameId, JSON.stringify({ type: 'ready', playerColor }));
    }, [connected, connection, gameId])

    const newGame = useCallback(async () => {
        await connected;
        return await connection.invoke<boolean>('Do', gameId, JSON.stringify({ type: 'new-game' }));
    }, [connected, connection, gameId])

    const state = useMemo(() => from(connected)
        .pipe(
            switchMap(() => fromSignalR(connection.stream('ListenState', gameId))),
            map(json => JSON.parse(json) as { state: CheckersState, action: CheckersAction }),
            scan(
                (prev, { state, action }) =>
                    prev
                        ? ({ state, action, })
                        : ({ state, action,  }) ,
                null as ObservedState | null
            )
        ), [connection, connected, gameId]);
    const value = useMemo(() => ({
        state,
        newGame,
        ready,
        otherPlayerUrl,
        playerColor,
    }), [state, newGame, ready, otherPlayerUrl, playerColor]);

    return (
        <CheckersContext.Provider value={value}>
            {children}
        </CheckersContext.Provider>
    )
}
