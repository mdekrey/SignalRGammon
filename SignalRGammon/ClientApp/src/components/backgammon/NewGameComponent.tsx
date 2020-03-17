import React, {  } from 'react';
import { useGameConnection } from '../../services/gameConnectionContext';
import { useRouteMatch, useHistory } from 'react-router-dom';

export function NewGameComponent() {
    const [connection, connected] = useGameConnection();
    const { path } = useRouteMatch();
    const history = useHistory();

    return <>
        <p>New Game</p>
        <button onClick={playAsWhite}>Play as White</button>
        <button onClick={playAsBlack}>Play as Black</button>
    </>;

    function playAsWhite() {
        play('white');
    }

    function playAsBlack() {
        play('black');
    }

    async function play(playerColor: 'white' | 'black') {
        await connected;
        const gameId = await connection.invoke<string>("CreateGame", "backgammon");
        history.push(`${path}/${gameId}/${playerColor}`);
    }
}
