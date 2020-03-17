import React, { useMemo, useState } from 'react';
import { from } from 'rxjs';
import { switchMap, tap } from 'rxjs/operators';
import { fromSignalR } from '../../utils/fromSignalR';
import { useRx } from '../../utils/useRx';
import { useGameConnection } from '../../services/gameConnectionContext';
import { BackgammonState } from './BackgammonState';

export function BackgammonComponent() {
    const [gameId, setGameId] = useState<null | string>(null);
    const [connection, connected] = useGameConnection();

    const createGameAndListen = useMemo(() => connected
        .pipe(
            switchMap(() => from(connection.invoke<string>("CreateGame", "backgammon"))),
            tap(setGameId),
            switchMap(gameId => fromSignalR<BackgammonState>(connection.stream('ListenState', gameId)))
        ), [connected]);
    const gameState = useRx(createGameAndListen, null);
    console.log(gameState);

    return (
        <div>
            <button onClick={onRoll} disabled={!gameId}>Roll</button>
        </div>
    );

    function onRoll() {
        connection.invoke<boolean>('Do', gameId, JSON.stringify({ type: 'roll', player: 'Black' }))
            .then(v => console.log('was roll allowed:', v));
    }
}
