import React from 'react';
import { Link } from 'react-router-dom';
import { useRx } from '../../utils/useRx';
import { useBackgammon } from './BackgammonService';
import { Board } from './svg-parts/Board';
import { Filters, filtersVariables } from './svg-parts/Filters';
import { boardPositions } from './svg-parts/sizes';
import { Dice } from './svg-parts/Dice';

import "./PlayBackgammon.css";
import { BackgammonBoardCheckers } from './BackgammonBoardCheckers';

export function PlayBackgammonComponent() {
    const { state, roll, newGame, undo, otherPlayerUrl, playerColor } = useBackgammon();

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

    console.log(gameState.checkers);

    const canRoll = !gameState.state.winner && gameState.state.diceRolls[playerColor].length === 0
        && (gameState.state.currentPlayer === null || gameState.state.currentPlayer === playerColor);
    const isWaiting = !gameState.state.winner && !canRoll && gameState.state.currentPlayer !== playerColor;
    const winner = gameState.state.winner;

    return (
        <div className="PlayBackgammon">
            <svg style={filtersVariables()}
                viewBox={boardPositions.viewbox}
                preserveAspectRatio="xMidYMid meet">
                <Filters />
                <g transform={playerColor === 'white' ? boardPositions.whiteRotation : boardPositions.blackRotation}>
                    <Board />
                    {gameState.state.currentPlayer && <BackgammonBoardCheckers canRoll={canRoll} state={gameState.state} checkers={gameState.checkers} diceRolls={gameState.state.diceRolls} />}

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

}
