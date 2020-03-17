
import React, { createContext, useContext } from "react";
import { HubConnection } from "@microsoft/signalr";
import { Observable } from "rxjs";
import { useNewGameConnection } from "./useNewGameConnection";

const GameConnectionContext = createContext([null!, null!] as [HubConnection, Observable<void>]);
export function useGameConnection() {
    return useContext(GameConnectionContext);
}

export function GameConnectionScope(props: { children: React.ReactNode }) {
    const connection = useNewGameConnection();
    return (
        <GameConnectionContext.Provider value={connection}>
            {props.children}
        </GameConnectionContext.Provider>
    )
}
