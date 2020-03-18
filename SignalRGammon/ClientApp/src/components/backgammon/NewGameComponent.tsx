import React from 'react';
import { useGameConnection } from '../../services/gameConnectionContext';
import { useRouteMatch, useHistory } from 'react-router-dom';
import { Checker } from './svg-parts/Checker';
import { Filters } from './svg-parts/Filters';
import { checkerDiameter, checkerRadius } from './svg-parts/sizes';
import { Felt } from './svg-parts/Felt';

function Selector({ player }: { player: 'white' | 'black' }) {
    const padding = 1.6;
    return (
        <svg style={{ width: '20vmin', height: '20vmin' }} viewBox={`0 0 ${checkerDiameter * padding} ${checkerDiameter * padding}`}>
            <Filters />
            <Felt width={checkerDiameter * padding} height={checkerDiameter * padding} />
            <g transform={`translate(${checkerRadius * padding} ${checkerRadius * padding})`}>
                <Checker player={player} />
            </g>
        </svg>
    )
}

export function NewGameComponent() {
    const {createGame} = useGameConnection();
    const { path } = useRouteMatch();
    const history = useHistory();

    return <>
        <p style={{ textAlign: 'center' }}>New Game &mdash; Choose your Checkers</p>
        <div style={{ textAlign: 'center' }}>
            <button onClick={playAsWhite} className="lookless"><Selector player='white' /></button>
            <button onClick={playAsBlack} className="lookless"><Selector player='black' /></button>
        </div>
    </>;

    function playAsWhite() {
        play('white');
    }

    function playAsBlack() {
        play('black');
    }

    async function play(playerColor: 'white' | 'black') {
        const gameId = await createGame('backgammon');
        history.push(`${path}/${gameId}/${playerColor}`);
    }
}
