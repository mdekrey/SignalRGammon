import React from 'react';
import { Link } from 'react-router-dom';


export function GameSelector(props: { children?: never }) {
    return (
        <>
            <div>
                <Link to="/backgammon">
                    <img src={require('./backgammon/backgammon.svg')} title="Backgammon" alt="Backgammon" />
                    Backgammon
                </Link>
            </div>
            <hr/>
            <div>
                <Link to="/licenses">Licenses</Link>
            </div>
        </>
    );
}
