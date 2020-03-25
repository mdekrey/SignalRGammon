import React, { useState } from 'react';
import { Link } from 'react-router-dom';
import { useRx } from '../../utils/useRx';
import { useBackgammon } from './BackgammonService';
import { Board } from './svg-parts/Board';
import { Filters, filtersVariables } from './svg-parts/Filters';
import { boardWidth, boardHeight, boardPositions } from './svg-parts/sizes';
import { pointTransform, Point } from './svg-parts/Point';
import { Dice } from './svg-parts/Dice';
import { Checkers } from './svg-parts/Checkers';

import "./PlayBackgammon.css";
import { HomeArea } from './svg-parts/HomeArea';

const barValue = -1;
const homeValue = 24;

const direction = {
    'black': 1,
    'white': -1,
}

const barEffectiveValue = {
    'black': -1,
    'white': 24,
}

const homeEffectiveValue = {
    'black': 24,
    'white': -1,
}

const isHomeEffectiveValue = {
    'black': (v: number) => v >= 24,
    'white': (v: number) => v < 0,
}

function normalizeGutter(startingPoint: number, playerColor: 'white' | 'black') {
    return (startingPoint === barValue ? barEffectiveValue[playerColor] : startingPoint);
}

function normalizeBearOff(endPoint: number, playerColor: 'white' | 'black') {
    return isHomeEffectiveValue[playerColor](endPoint) ? homeValue : endPoint;
}

function contains(array: number[], value: number) {
    return array.indexOf(value) !== -1;
}

export function PlayBackgammonComponent() {
    const { state, roll, move, bearOff, newGame, undo, otherPlayerUrl, playerColor } = useBackgammon();
    const [selectedChecker, setSelectedChecker] = useState(null as (number | null));

    const gameState = useRx(state, undefined);

    if (gameState === undefined) {
        return (
            <h1>Loading...</h1>
        );
    }
    if (gameState === null) {
        return (
            <>
                <h1>The game has expired.</h1>
                <Link to={'/'}>Start a new game!</Link>
            </>
        );
    }

    const canRoll = !gameState.state.winner && gameState.state.diceRolls[playerColor].length === 0
        && (gameState.state.currentPlayer === null || gameState.state.currentPlayer === playerColor);
    const canSelectChecker = selectedChecker === null && !canRoll;
    const isWaiting = !gameState.state.winner && !canRoll && gameState.state.currentPlayer !== playerColor;
    const winner = gameState.state.winner;

    const allowedPoints = selectedChecker === null
        ? []
        : gameState.state.diceRolls[playerColor].map(die => normalizeBearOff(normalizeGutter(selectedChecker, playerColor) + die * direction[playerColor], playerColor));
    const isPlayerBlack = playerColor === 'black';
    const isPlayerWhite = playerColor === 'white';
    const bar = gameState.state.bar;
    const isAllowed = (v: number) => contains(allowedPoints, v);

    return (
        <div className="PlayBackgammon">
            <svg style={filtersVariables()}
                viewBox={boardPositions.viewbox}
                preserveAspectRatio="xMidYMid meet">
                <Filters />
                <g transform={isPlayerWhite ? boardPositions.whiteRotation : boardPositions.blackRotation}>
                    <Board />
                    <HomeArea selectable={isAllowed(homeValue)} onClick={doBearOff} />
                    {gameState.state.currentPlayer && gameState.state.points.map(({ black, white }, idx) =>
                        <g transform={pointTransform(idx)} key={idx}>
                            <Checkers count={black} player="black" selectable={canSelectChecker && isPlayerBlack} selected={selectedChecker === idx}
                                onClick={() => (canSelectChecker && isPlayerBlack) ? setSelectedChecker(idx) : selectPoint(idx)} />
                            <Checkers count={white} player="white" selectable={canSelectChecker && isPlayerWhite} selected={selectedChecker === idx}
                                onClick={() => (canSelectChecker && isPlayerWhite) ? setSelectedChecker(idx) : selectPoint(idx)} />
                            {selectedChecker !== null
                                ? <Point color="transparent" selectable={isAllowed(idx)} onClick={() => selectPoint(idx)} />
                                : null
                            }
                        </g>
                    )}
                    <g transform={boardPositions.blackBar}>
                        <Checkers count={bar.black} player="black" selectable={canSelectChecker && isPlayerBlack} selected={isPlayerBlack && selectedChecker === barValue}
                                onClick={() => (canSelectChecker && isPlayerBlack) ? setSelectedChecker(barValue) : setSelectedChecker(null)} />
                    </g>
                    <g transform={boardPositions.whiteBar}>
                        <Checkers count={bar.white} player="white" selectable={canSelectChecker && isPlayerWhite} selected={isPlayerWhite && selectedChecker === barValue}
                                onClick={() => (canSelectChecker && isPlayerWhite) ? setSelectedChecker(barValue) : setSelectedChecker(null)} />
                    </g>

                    <g transform={boardPositions.whiteDice}>
                        <Dice player="white" values={gameState.state.diceRolls.white} />
                    </g>
                    <g transform={boardPositions.blackDice}>
                        <Dice player="black" values={gameState.state.diceRolls.black} />
                    </g>
                </g>
            </svg>
            {(gameState.state.undo && gameState.state.currentPlayer === playerColor)
                ? <div className="undo-container">
                    <button className="undo-button" onClick={undo}>Undo</button>
                </div>
                : null}
            {isWaiting || canRoll || winner
                ? <div className="overlay">
                    <div className="child">
                        {gameState.state.currentPlayer === null
                            && <div className="share-link">Share this with the other player: <input type="text" value={otherPlayerUrl} onClick={copyUrl} readOnly /></div>}
                        {canRoll && <div className="roll-button-container"><button className="roll-button" onClick={roll} disabled={!gameState}>Roll</button></div>}
                        {isWaiting && <div className="waiting-container"><h1>Waiting on the other player...</h1></div>}
                        {winner && <div className="winner-container">
                            <h1>{winner === playerColor ? "You won!" : "The other player won."}</h1>
                            <button className="new-game-button" onClick={newGame}>Play Again?</button>
                        </div>}
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
        if (contains(allowedPoints, index))
            await move(dieValue, selectedChecker);
    }

    async function doBearOff() {
        if (selectedChecker === null || !gameState) {
            return;
        }
        const minDieValue = (homeEffectiveValue[playerColor] - normalizeGutter(selectedChecker, playerColor)) * direction[playerColor];
        const rolls = gameState.state.diceRolls[playerColor].filter(roll => roll >= minDieValue).sort();
        const dieValue = rolls[0];
        setSelectedChecker(null);
        if (!dieValue) {
            return;
        }
        await bearOff(dieValue, selectedChecker);
    }
}
