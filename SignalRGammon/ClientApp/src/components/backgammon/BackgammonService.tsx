import React, { useContext, createContext, useMemo, useCallback } from "react";
import { Observable, from } from "rxjs";
import { switchMap, map } from "rxjs/operators";
import { useGameConnection } from "../../services/gameConnectionContext";
import { fromSignalR } from "../../utils/fromSignalR";
import { BackgammonState } from "./BackgammonState";
import { useRouteMatch } from "react-router";

export type BackgammonAction = {
    // TODO
};

export type BackgammonContextResult = {
    state: Observable<{ state: BackgammonState, action: BackgammonAction } | null>
    roll: () => Promise<boolean>
    otherPlayerUrl: string
    playerColor: "white" | "black";
};

const BackgammonContext = createContext({} as BackgammonContextResult);
export function useBackgammon() {
    return useContext(BackgammonContext);
}

export type BackgammonScopeProps = {
    gameId: string;
    playerColor: "white" | "black";
    children: React.ReactNode;
}
function otherPlayer(playerColor: "white" | "black") { return playerColor === 'white' ? 'black' : 'white'; }
export function BackgammonScope({ gameId, playerColor, children }: BackgammonScopeProps) {
    const {connection, connected} = useGameConnection();
    const { url } = useRouteMatch();
    const otherPlayerUrl = window.location.href.replace(url, url.replace(playerColor, otherPlayer(playerColor)));

    const roll = useCallback(async () => {
        await connected;
        return await connection.invoke<boolean>('Do', gameId, JSON.stringify({ type: 'roll', player: playerColor }))
    }, [connected, connection, playerColor, gameId])

    const state = useMemo(() => from(connected)
        .pipe(
            switchMap(() => fromSignalR(connection.stream('ListenState', gameId))),
            map(json => JSON.parse(json))
        ), [connection, connected, gameId]);
    const value = useMemo(() => ({
        state,
        roll,
        otherPlayerUrl,
        playerColor,
    }), [state, roll, otherPlayerUrl, playerColor]);

    return (
        <BackgammonContext.Provider value={value}>
            {children}
        </BackgammonContext.Provider>
    )
}
