import React, { useState } from 'react';
import { Link } from 'react-router-dom';
import { useRx } from '../../utils/useRx';
import { useBackgammon } from './BackgammonService';
import { Board } from './svg-parts/Board';
import { Filters } from './svg-parts/Filters';
import { boardWidth, boardHeight } from './svg-parts/sizes';
import { pointTransform, Point } from './svg-parts/Point';
import { Dice } from './svg-parts/Dice';
import { Checkers } from './svg-parts/Checkers';

import "./PlayBackgammon.css";

const direction = {
    'black': 1,
    'white': -1,
}

const barEffectiveValue = {
    'black': -1,
    'white': 24,
}

function normalizeGutter(startingPoint: number, playerColor: 'white' | 'black') {
    return (startingPoint === -1 ? barEffectiveValue[playerColor] : startingPoint);
}

export function PlayBackgammonComponent() {
    const { state, roll, move, otherPlayerUrl, playerColor } = useBackgammon();
    const [selectedChecker, setSelectedChecker] = useState(null as (number | null));

    const gameState = useRx(state, undefined);

    if (gameState === undefined) {
        return (
            <h1>Loading...</h1>
        );
    } else if (gameState === null) {
        return (
            <>
                <h1>The game has expired.</h1>
                <Link to={'/'}>Start a new game!</Link>
            </>
        );
    }

    const canRoll = gameState.state.diceRolls[playerColor].length === 0
        && (gameState.state.currentPlayer === null || gameState.state.currentPlayer === playerColor);
    const isWaiting = !canRoll && gameState.state.currentPlayer !== playerColor;

    const allowedPoints = selectedChecker === null
        ? []
        : gameState.state.diceRolls[playerColor].map(die => normalizeGutter(selectedChecker, playerColor) + die * direction[playerColor]);

    return (
        <div className="PlayBackgammon">
            <svg style={{ width: 'calc(100vw - 20px)', height: 'calc(100vh - 20px)' }}
                viewBox={`0 0 ${boardWidth} ${boardHeight}`}
                preserveAspectRatio="xMidYMid meet">
                <Filters />
                <g transform={playerColor === 'white' ? `translate(${boardWidth},${boardHeight}) rotate(180)` : undefined}>
                    <Board />
                    {gameState.state.currentPlayer && gameState.state.points.map(({ black, white }, idx) =>
                        <g transform={pointTransform(idx)} key={idx}>
                            <Checkers count={black} player="black" selectable={selectedChecker === null && playerColor === 'black' && !canRoll} selected={selectedChecker === idx}
                                onClick={(selectedChecker === null && playerColor === 'black' && !canRoll && black) ? (() => setSelectedChecker(idx)) : undefined} />
                            <Checkers count={white} player="white" selectable={selectedChecker === null && playerColor === 'white' && !canRoll} selected={selectedChecker === idx}
                                onClick={(selectedChecker === null && playerColor === 'white' && !canRoll && white) ? (() => setSelectedChecker(idx)) : undefined} />
                            {selectedChecker !== null
                                ? <Point color="transparent" selectable={allowedPoints.indexOf(idx) !== -1} onClick={() => selectPoint(idx)} />
                                : null
                            }
                        </g>
                    )}
                    <g transform={`translate(${boardWidth / 2}, ${boardHeight / 2}) rotate(180)`}>
                        <Checkers count={gameState.state.bar.black} player="black" selectable={selectedChecker === null && playerColor === 'black' && !canRoll} selected={playerColor === 'black' && selectedChecker === -1}
                                onClick={(selectedChecker === null && playerColor === 'black' && !canRoll && gameState.state.bar.black) ? (() => setSelectedChecker(-1)) : undefined} />
                    </g>
                    <g transform={`translate(${boardWidth / 2}, ${boardHeight / 2})`}>
                        <Checkers count={gameState.state.bar.white} player="white" selectable={selectedChecker === null && playerColor === 'white' && !canRoll} selected={playerColor === 'white' && selectedChecker === -1}
                                onClick={(selectedChecker === null && playerColor === 'white' && !canRoll && gameState.state.bar.white) ? (() => setSelectedChecker(-1)) : undefined} />
                    </g>

                    <g transform={`translate(${boardWidth / 4}, ${boardHeight / 2})`}>
                        <Dice player="white" values={gameState.state.diceRolls.white} />
                    </g>
                    <g transform={`translate(${boardWidth * 3 / 4}, ${boardHeight / 2})`}>
                        <Dice player="black" values={gameState.state.diceRolls.black} />
                    </g>
                </g>
            </svg>
            {isWaiting || canRoll
                ? <div className="overlay">
                    <div className="child">
                        {gameState.state.currentPlayer === null
                            && <div className="share-link">Share this with the other player: <input type="text" value={otherPlayerUrl} onClick={copyUrl} readOnly /></div>}
                        {canRoll && <div className="roll-button-container"><button className="roll-button" onClick={roll} disabled={!gameState}>Roll</button></div>}
                        {isWaiting && <div className="waiting-container"><h1>Waiting on the other player...</h1></div>}
                    </div>
                </div>
                : null}
        </div>
    );

    function copyUrl(ev: React.MouseEvent<HTMLInputElement>) {
        ev.currentTarget.select();
        document.execCommand('copy');
    }

    async function selectPoint(index: number) {
        if (selectedChecker === null) {
            return;
        }
        const dieValue = (index - normalizeGutter(selectedChecker, playerColor)) * direction[playerColor];
        setSelectedChecker(null);
        await move(dieValue, selectedChecker);
    }
}