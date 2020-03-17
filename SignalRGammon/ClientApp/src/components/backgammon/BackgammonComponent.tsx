import React, { useMemo, useEffect, useState } from 'react';
import { BehaviorSubject } from 'rxjs';
import { HubConnectionBuilder } from '@microsoft/signalr';

export interface BackgammonState
{
    currentPlayer: 'White' | 'Black';
    whiteDiceRolls: number[];
    blackDiceRolls: number[];
    points: { white: number, black: number }[];
    fence: { white: number, black: number };
}

export function BackgammonComponent(props: { children: React.ReactNode }) {
    const gameState = useMemo(() => new BehaviorSubject<BackgammonState | null>(null), []);
    const connection = useMemo(() => new HubConnectionBuilder().withUrl("/gameHub").build(), []);
    const [gameId, setGameId] = useState<null | string>(null);

    useEffect(function makeGame() {

        connection.start().then(async function () {
            const gameId = await connection.invoke("CreateGame", "backgammon");
            setGameId(gameId);
            connection.stream('ListenState', gameId)
                .subscribe(gameState);
        }).catch(function (err) {
            return console.error(err.toString());
        });

    }, [connection]);

    useEffect(function logGameState() {
        gameState.subscribe(v => console.log(v));
    });

    return (
        <div>
            <button onClick={onRoll} disabled={!gameId}>Roll</button>
        </div>
    );

    function onRoll() {
        connection.send('Do', gameId, JSON.stringify({ type: 'roll', player: 'Black' }));
    }
}
