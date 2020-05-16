import React, { useMemo } from 'react';
import { Link, useHistory, useRouteMatch } from 'react-router-dom';
import { useRx } from '../utils/useRx';
import { GameApi, GetGameTypesResponse200 } from '../api-generated';

import './GameSelector.css';
import { map, tap } from 'rxjs/operators';

export function GameSelector(props: { children?: never }) {
    const getType$ = useMemo(() => new GameApi({ basePath: '' }).getGameTypes().pipe(
        tap(v => console.log(v)),
        map(resp => resp.statusCode === 200 ? resp.data : []),
    ), []);
    const gameTypes = useRx(getType$, undefined);
    return (
        <>
            <div className="games-list">
                {!gameTypes
                    ? <div>Loading...</div>
                    : gameTypes.map(gameType => <GameTypeLink key={gameType.gameType} gameType={gameType.gameType} title={gameType.displayName} iconUrl={gameType.iconUrl} />)}
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


function GameTypeLink({ gameType, title, iconUrl }: { children?: never; gameType: string; title: string; iconUrl: string; }) {
    const history = useHistory();
    const { path } = useRouteMatch();
    return (
        <button onClick={startGame}>
            <img src={iconUrl} title={title} alt={title} />
            { title }
        </button>
    );

    async function startGame() {
        const game = await new GameApi({ basePath: '' }).createGame({ gameType: gameType }).pipe(
            map(resp => resp.statusCode === 200 ? resp.data : null)
        ).toPromise();
        if (!game) {
            return;
        }
        history.push(`${path}/${gameType}/${game.id}`, { server: game.server });
    }
}
