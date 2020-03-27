import React from 'react';

import "./styles.css";
import { Felt } from './Felt';
import { boardWidth, boardHeight, gridSize } from './sizes';

export const Board = () =>
    <g mask="url(#boardMask)" >
        <Felt width={boardWidth} height={boardHeight} />

        {Array(64).fill(0).map((_, idx) => {
            const y = Math.floor(idx / 8);
            const x = idx % 8;
            if ((x + y) % 2 === 0)
                return null;
            return <rect key={idx} x={x * gridSize} y={y * gridSize} width={gridSize} height={gridSize} className="tile" />;
        })}
    </g>