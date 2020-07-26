
import React, { createContext, useContext, useMemo } from "react";
import { HubConnection } from "@microsoft/signalr";
import { useNewGameConnection, FetchHelper } from "./useNewGameConnection";
import { Observable } from "rxjs";

export type GameConnectionContextType = {
    connectionPromise: Promise<HubConnection>;
    createGame: (gameType: string) => Promise<boolean>;
    fetchHelper: FetchHelper;
    doAction: (gameId: string, action: any) => Promise<boolean>;
    getGameState: <TState>(gameId: string) => Observable<TState>;
}

const GameConnectionContext = createContext({} as GameConnectionContextType);
export function useGameConnection() {
    return useContext(GameConnectionContext);
}

export function GameConnectionScope(props: { children: React.ReactNode }) {
    const { connectionPromise, fetchHelper } = useNewGameConnection();
    const context = useMemo((): GameConnectionContextType => ({
        connectionPromise,
        fetchHelper,
        createGame: async gameType => {
            const response = await fetchHelper("/createGame", { method: 'POST', body: gameType });
            if (response.status !== 200) throw new Error("could not create game");

            return await response.json();
        },
        doAction: async (gameId: string, action: any) => {
            const response = await fetchHelper("/doAction", { method: 'POST', body: JSON.stringify(action), headers: { "x-game-id": gameId } });
            if (response.status !== 200) throw new Error("could not process action");

            return await response.json();
        },
        getGameState: function <TState>(gameId: string) {
            return new Observable<TState>((subscriber) => {
                function onMessage(incommingGameId: string, state: string) {
                    if (incommingGameId !== gameId) return;
                    subscriber.next(JSON.parse(state) as TState);
                }
                const connected = connectionPromise.then(async hub => {
                    hub.on("NewPublicState", onMessage);
                    await fetchHelper("/getGameState", { method: 'GET', headers: { "x-game-id": gameId } });
                    return hub;
                });
                return () => { connected.then(hub => hub.off("NewPublicState", onMessage)); }
            })
        }
    }), [connectionPromise, fetchHelper]);

    return (
        <GameConnectionContext.Provider value={context}>
            {props.children}
        </GameConnectionContext.Provider>
    )
}
