import React from 'react';
import { checkerDiameter } from './sizes';
import { Checker } from './Checker';
import { arrayOf } from '../../../utils/arrayOf';
export function Checkers({ count, player, selectable = false }: {
    count: number;
    player: 'black' | 'white';
    selectable?: boolean;
}) {
    return (<g className={selectable ? "selectable-container" : undefined}>
        {arrayOf(count).map((_, idx) => <g transform={`translate(0 ${(idx + 0.5) * checkerDiameter})`} key={idx}>
            <Checker player={player} selectable={count - 1 === idx && selectable} />
        </g>)}
    </g>);
}
