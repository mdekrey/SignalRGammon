import { useMemo, useEffect } from "react";
import { HubConnectionBuilder, HubConnection } from "@microsoft/signalr";


export function useNewGameConnection() {
    const connection = useMemo(() => new HubConnectionBuilder().withUrl("/gameHub").build(), []);
    const connected = useMemo(() => connection.start(), [connection]);

    useEffect(() => {
        return () => { connection.stop() };
    }, [connection]);

    return [connection, connected] as [HubConnection, Promise<void>];
}
