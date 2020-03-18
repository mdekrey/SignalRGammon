import React, { useMemo } from 'react';
import { dieSize, pipRadius, pipPercentage, stroke } from './sizes';

import "./styles.css";

const pipFactor = pipPercentage * dieSize;

const positions = ((p) => [
    [ { x: 0, y: 0 } ],
    [ { x: p, y: p }, { x: -p, y: -p } ],
    [ { x: 0, y: 0 }, { x: p, y: p }, { x: -p, y: -p } ],
    [ { x: p, y: p }, { x: p, y: -p }, { x: -p, y: -p }, { x: -p, y: p } ],
    [ { x: 0, y: 0 }, { x: p, y: p }, { x: p, y: -p }, { x: -p, y: -p }, { x: -p, y: p } ],
    [ { x: p, y: p }, { x: p, y: 0 }, { x: p, y: -p }, { x: -p, y: -p }, { x: -p, y: 0 }, { x: -p, y: p } ],
])(pipFactor);

export const Die = ({ player, value, selectable = false }: { player: 'white' | 'black', value: number, selectable?: boolean }) => {
    const rotation = useMemo(() => Math.random() * 360, []);
    return (
        <g className={`die ${player} ${selectable ? 'selectable' : ''}`} transform={`rotate(${rotation})`}>
            <rect width={dieSize} height={dieSize} x={-dieSize / 2} y={-dieSize / 2} strokeWidth={stroke} />
            {(positions[value - 1] || []).map(({ x, y }, idx) =>
            <circle r={pipRadius} className="pip" key={idx} cx={x} cy={y} />
            )}
        </g>
    );
}
