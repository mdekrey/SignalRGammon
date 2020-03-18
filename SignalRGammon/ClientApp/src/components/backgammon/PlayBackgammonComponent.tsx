import React from 'react';
import { Link } from 'react-router-dom';
import { useRx } from '../../utils/useRx';
import { useBackgammon } from './BackgammonService';
import { Board } from './svg-parts/Board';
import { Filters } from './svg-parts/Filters';
import { boardWidth, boardHeight } from './svg-parts/sizes';
import { pointTransform } from './svg-parts/Point';
import { Dice } from './svg-parts/Dice';
import { Checkers } from './svg-parts/Checkers';

import "./PlayBackgammon.css";

export function PlayBackgammonComponent() {
    const { state, roll, otherPlayerUrl, playerColor } = useBackgammon();

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
                            <Checkers count={black} player="black" selectable={playerColor === 'black' && !canRoll} />
                            <Checkers count={white} player="white" selectable={playerColor === 'white' && !canRoll} />

                        </g>
                    )}
                    <g transform={`translate(${boardWidth / 4}, ${boardHeight / 2})`}>
                        <Dice player="white" values={gameState.state.diceRolls.white} selectable={playerColor === 'white'} />
                    </g>
                    <g transform={`translate(${boardWidth * 3 / 4}, ${boardHeight / 2})`}>
                        <Dice player="black" values={gameState.state.diceRolls.black} selectable={playerColor === 'black'} />
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
}
