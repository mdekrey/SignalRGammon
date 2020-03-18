
import React, { createContext, useContext, useMemo } from "react";
import { HubConnection } from "@microsoft/signalr";
import { useNewGameConnection } from "./useNewGameConnection";

export type GameConnectionContextType = {
    connection: HubConnection;
    connected: Promise<void>;
    createGame: (gameType: string) => Promise<string>;
}

const GameConnectionContext = createContext({} as GameConnectionContextType);
export function useGameConnection() {
    return useContext(GameConnectionContext);
}

export function GameConnectionScope(props: { children: React.ReactNode }) {
    const [connection, connected] = useNewGameConnection();
    const context = useMemo((): GameConnectionContextType => ({
        connection,
        connected,
        createGame: async gameType => {
            await connected;
            return await connection.invoke<string>("CreateGame", gameType);
        }
    }), [connection, connected]);

    return (
        <GameConnectionContext.Provider value={context}>
            {props.children}
        </GameConnectionContext.Provider>
    )
}
