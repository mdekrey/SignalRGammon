import React from 'react';
import { Link } from 'react-router-dom';


export function GameSelector(props: { children?: never }) {
    return (
        <>
            <div>
                <Link to="/backgammon">Backgammon</Link>
            </div>
            <hr/>
            <div>
                <Link to="/licenses">Licenses</Link>
            </div>
        </>
    );
}
