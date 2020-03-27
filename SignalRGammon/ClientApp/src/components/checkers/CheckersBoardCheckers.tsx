import React, { useState } from 'react';
import { useCheckers } from './CheckersService';
import { CheckersState, players } from './CheckersState';
import { Checker } from './svg-parts/Checker';
import { gridSize } from './svg-parts/sizes';

const direction = {
    'black': 1,
    'white': -1,
}

export function CheckersBoardCheckers({ checkers }: {
    checkers: CheckersState["checkers"];
}) {
    const { playerColor } = useCheckers();
    const [selectedChecker, setSelectedChecker] = useState(null as (number | null));
    const canSelectChecker = selectedChecker === null;

    return (
        <>
            {/* Rendered checkers */}
            {players.map(player =>
                <React.Fragment key={player}>
                    {checkers[player].map((checker, idx) => (
                        checker &&
                        <g style={{ transition: "transform 1s", pointerEvents: "none", transform: checkerTranslation(checker.column, checker.row)}} key={idx}>
                            <Checker player={player} />
                        </g>
                    ))}
                </React.Fragment>
            )}
        </>
    );
}

function checkerTranslation(x: number, y: number) {
    return `translate(${(x + 0.5) * gridSize}px, ${(y + 0.5) * gridSize}px)`;
}
