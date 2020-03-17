import { useMemo, useEffect } from "react";
import { HubConnectionBuilder, HubConnection } from "@microsoft/signalr";
import { from, Observable } from "rxjs";


export function useNewGameConnection() {
    const connection = useMemo(() => new HubConnectionBuilder().withUrl("/gameHub").build(), []);
    const connected = useMemo(() => from(connection.start()), []);

    useEffect(() => {
        return () => { connection.stop() };
    }, []);

    return [connection, connected] as [HubConnection, Observable<void>];
}
