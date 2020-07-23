import React from 'react';
import { Link } from 'react-router-dom';
import { useRx } from '../../utils/useRx';
import { useCheckers } from './CheckersService';
import { Board } from './svg-parts/Board';
import { Filters, filtersVariables } from './svg-parts/Filters';
import { boardPositions } from './svg-parts/sizes';

import "./PlayCheckers.css";
import { CheckersBoardCheckers } from './CheckersBoardCheckers';

export function PlayCheckersComponent() {
    const { state, ready, newGame, otherPlayerUrl, playerColor } = useCheckers();

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

    const isReady = gameState.state.state.isReady.white && gameState.state.state.isReady.black;
    const isWaiting = isReady ? (!gameState.state.state.winner && gameState.state.state.currentPlayer !== playerColor) : gameState.state.state.isReady[playerColor];
    const winner = gameState.state.state.winner;

    console.log(gameState.state.validMovesForCurrentPlayer);

    return (
        <div className="PlayCheckers">
            <svg style={filtersVariables()}
                viewBox={boardPositions.viewbox}
                preserveAspectRatio="xMidYMid meet">
                <Filters />
                <g transform={playerColor === 'white' ? boardPositions.whiteRotation : boardPositions.blackRotation}>
                    <Board />
                    <CheckersBoardCheckers
                        checkers={gameState.state.state.checkers}
                        currentPlayer={gameState.state.state.currentPlayer}
                        validMovesForCurrentPlayer={gameState.state.validMovesForCurrentPlayer} />
                </g>
            </svg>
            {isWaiting || winner || !isReady
                ? <div className="overlay">
                    <div className="child">
                        {(!isReady)
                            && <div className="share-link">Share this with the other player: <input type="text" value={otherPlayerUrl} onClick={copyUrl} readOnly /></div>}
                        {!gameState.state.state.isReady[playerColor]
                            && <div className="ready-button-container"><button className="ready-button" onClick={ready} disabled={!gameState}>Ready!</button></div>}
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
