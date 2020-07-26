import React, { useContext, createContext, useMemo, useCallback } from "react";
import { Observable } from "rxjs";
import { scan } from "rxjs/operators";
import { useGameConnection } from "../../services/gameConnectionContext";
import { BackgammonState } from "./BackgammonState";
import { useRouteMatch } from "react-router";
import { bar, Checkers, pointsToCheckers } from "./pointsToCheckers";

export type BackgammonAction = null | {
    // TODO
};

export type ObservedState = { state: BackgammonState, action: BackgammonAction, checkers: Checkers };

export type BackgammonContextResult = {
    state: Observable<ObservedState | null>
    roll: () => Promise<boolean>
    move: (dieValue: number, startingPointIndex: number | bar) => Promise<boolean>
    bearOff: (dieValue: number, startingPointIndex: number | bar) => Promise<boolean>
    newGame: () => Promise<boolean>
    undo: () => Promise<boolean>
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
    const {getGameState, doAction} = useGameConnection();
    const { url } = useRouteMatch();
    const otherPlayerUrl = window.location.href.replace(url, url.replace(playerColor, otherPlayer(playerColor)));

    const roll = useCallback(async () => {
        return await doAction(gameId, ({ type: 'roll', player: playerColor }));
    }, [playerColor, gameId, doAction])

    const move = useCallback(async (dieValue: number, startingPoint: number | bar) => {
        return await doAction(gameId, ({ type: 'move', player: playerColor, dieValue, startingPointNumber: startingPoint === bar ? -1 : startingPoint }));
    }, [playerColor, gameId, doAction])

    const bearOff = useCallback(async (dieValue: number, startingPoint: number | bar) => {
        return await doAction(gameId, ({ type: 'bear-off', player: playerColor, dieValue, startingPointNumber: startingPoint === bar ? -1 : startingPoint }));
    }, [playerColor, gameId, doAction])

    const newGame = useCallback(async () => {
        return await doAction(gameId, ({ type: 'new-game' }));
    }, [gameId, doAction])

    const undo = useCallback(async () => {
        return await doAction(gameId, ({ type: 'undo' }));
    }, [gameId, doAction])

    const state = useMemo(() => getGameState<{ state: BackgammonState, action: BackgammonAction | null }>(gameId)
        .pipe(
            scan(
                (prev, {state, action}) =>
                    prev
                        ? ({ state, action, checkers: pointsToCheckers(state, prev.checkers) })
                        : ({ state, action, checkers: pointsToCheckers(state) }) ,
                null as ObservedState | null
            )),
        [getGameState, gameId]);
    const value = useMemo(() => ({
        state,
        roll,
        move,
        bearOff,
        newGame,
        undo,
        otherPlayerUrl,
        playerColor,
    }), [state, roll, move, bearOff, newGame, undo, otherPlayerUrl, playerColor]);

    return (
        <BackgammonContext.Provider value={value}>
            {children}
        </BackgammonContext.Provider>
    )
}
