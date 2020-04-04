import React, { useState } from 'react';
import { Board } from './svg-parts/Board';
import { Filters } from './svg-parts/Filters';
import { boardWidth, boardHeight, blackDicePosition, whiteDicePosition } from './svg-parts/sizes';
import { Dice } from './svg-parts/Dice';

import "./PlayBackgammon.css";

export function DiceTestComponent() {
    const [dice, setDice] = useState([] as number[]);

    return (
        <div className="PlayBackgammon">
            <svg style={{ width: 'calc(100vw - 20px)', height: 'calc(100vh - 20px)' }}
                viewBox={`0 0 ${boardWidth} ${boardHeight}`}
                preserveAspectRatio="xMidYMid meet">
                <Filters />
                <Board />

                <g transform={whiteDicePosition}>
                    <Dice player="white" values={dice} selectable={true} onSelect={(value, index) => remove(value, index)} />
                </g>
                <g transform={blackDicePosition}>
                    <Dice player="black" values={dice} selectable={true} onSelect={(value, index) => remove(value, index)} />
                </g>
            </svg>
            {dice.length
                ? null
                : <div className="overlay">
                    <div className="child">
                        <div className="roll-button-container"><button className="roll-button" onClick={roll}>Roll</button></div>
                    </div>
                </div>}
        </div>
    );

    function roll() {
        setDice([
            Math.floor(Math.random() * 6 + 1),
            Math.floor(Math.random() * 6 + 1),
            Math.floor(Math.random() * 6 + 1),
            Math.floor(Math.random() * 6 + 1),
        ]);
    }

    function remove(value: number, index: number) {
        setDice(dice.filter((v, i) => i !== index));
    }
}
