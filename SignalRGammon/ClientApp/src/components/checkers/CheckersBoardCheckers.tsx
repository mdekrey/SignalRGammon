import React, { useState } from 'react';
import { useCheckers, ValidMove } from './CheckersService';
import { CheckersState, players, PlayerColor } from './CheckersState';
import { Checker } from './svg-parts/Checker';
import { gridSize } from './svg-parts/sizes';


function contains<T>(array: T[], value: T) {
    return array.indexOf(value) !== -1;
}

export function CheckersBoardCheckers({ checkers, validMovesForCurrentPlayer, currentPlayer }: {
    currentPlayer: PlayerColor,
    checkers: CheckersState["checkers"];
    validMovesForCurrentPlayer: ValidMove[];
}) {
    const { playerColor, move } = useCheckers();
    const [selectedChecker, setSelectedChecker] = useState(null as (number | null));
    const currentChecker = selectedChecker && checkers[playerColor][selectedChecker];

    const allowedCheckers = playerColor === currentPlayer
        ? validMovesForCurrentPlayer.map(v => v.checkerIndex)
        : [];
    const allowedMoves = selectedChecker !== null
        ? validMovesForCurrentPlayer.filter(m => m.checkerIndex === selectedChecker)
        : [];

    return (
        <>
            {/* Rendered checkers */}
            {players.map(player =>
                <React.Fragment key={player}>
                    {checkers[player].map((checker, idx) => (
                        checker &&
                        <g style={{ transition: "transform 1s", transform: checkerTranslation(checker.column, checker.row)}} key={idx}>
                            <Checker player={player}
                                selected={idx === selectedChecker && player === playerColor}
                                selectable={contains(allowedCheckers, idx) && player === playerColor}
                                onClick={() => setSelectedChecker(contains(allowedCheckers, idx) && player === playerColor ? idx : null)} />
                        </g>
                    ))}
                </React.Fragment>
            )}

            {/* Click-target checkers */}
            {
                allowedMoves.map((option, idx) =>
                    <g style={{ transform: checkerTranslation(option.column, option.row)}} key={idx}>
                        <Checker player={"transparent"}
                        selectable={true /* TODO */}
                        onClick={() => { setSelectedChecker(null); move(option); }} />
                    </g>
                )
            }
        </>
    );
}

function checkerTranslation(x: number, y: number) {
    return `translate(${(x + 0.5) * gridSize}px, ${(y + 0.5) * gridSize}px)`;
}
