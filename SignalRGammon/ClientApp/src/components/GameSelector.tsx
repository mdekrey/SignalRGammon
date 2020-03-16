import React from 'react';


export function GameSelector(props: { children: React.ReactNode }) {
    return (
        <div>
            <button onClick={onNewGame}>New Game</button>
        </div>
    );

    function onNewGame() {

    }
}
