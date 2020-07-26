import React from 'react';
import { Link } from 'react-router-dom';

import './GameSelector.css';

export function GameSelector(props: { children?: never }) {
    return (
        <>
            <div className="games-list">
                <Link to="/backgammon">
                    <img src={require('./backgammon/backgammon.svg')} title="Backgammon" alt="Backgammon" />
                    Backgammon
                </Link>
                <Link to="/checkers">
                    <img src={require('./checkers/empty-chessboard.svg')} title="Checkers" alt="Checkers" />
                    Checkers
                </Link>
            </div>
            <hr/>
            <div>
                <Link to="/licenses">Licenses</Link>
            </div>
        </>
    );
}
