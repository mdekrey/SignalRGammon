import React, { useMemo } from 'react';
import { Link, useHistory, useRouteMatch } from 'react-router-dom';
import { useRx } from '../utils/useRx';
import { GameApi, GetGamesResponse200 } from '../api-generated';

import './GameSelector.css';
import { map } from 'rxjs/operators';

export function GameSelector(props: { children?: never }) {
    const getType$ = useMemo(() => new GameApi({ basePath: '' }).getGames().pipe(
        map(resp => resp.statusCode === 200 ? resp.data : [])
    ), []);
    const gameTypes = useRx(getType$, undefined);
    return (
        <>
            <div className="games-list">
                {gameTypes === undefined
                    ? <div>Loading...</div>
                    : gameTypes.map(gameType => <GameTypeLink gameType={gameType} />)}
                {/* <Link to="/backgammon">
                    <img src={require('./backgammon/backgammon.svg')} title="Backgammon" alt="Backgammon" />
                    Backgammon
                </Link>
                <Link to="/checkers">
                    <img src={require('./checkers/empty-chessboard.svg')} title="Checkers" alt="Checkers" />
                    Checkers
                </Link> */}
            </div>
            <hr/>
            <div>
                <Link to="/licenses">Licenses</Link>
            </div>
        </>
    );
}


function GameTypeLink({ gameType }: { children?: never; gameType: GetGamesResponse200[0] }) {
    const history = useHistory();
    const { path } = useRouteMatch();
    return (
        <button onClick={startGame}>
            {/* <img src={require('./backgammon/backgammon.svg')} title="Backgammon" alt="Backgammon" /> */}
            { gameType }
        </button>
    );

    async function startGame() {
        const game = await new GameApi({ basePath: '' }).createGame({ gameType }).pipe(
            map(resp => resp.statusCode === 200 ? resp.data : null)
        ).toPromise();
        if (!game) {
            return;
        }
        history.push(`${path}/${gameType}/${game.id}`, { server: game.server });
    }
}
