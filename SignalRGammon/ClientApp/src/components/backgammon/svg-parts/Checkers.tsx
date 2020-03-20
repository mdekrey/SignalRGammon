import React from 'react';
import { checkerDiameter, checkerRadius } from './sizes';
import { Checker } from './Checker';
import { arrayOf } from '../../../utils/arrayOf';

export type CheckersProps = {
    count: number;
    player: 'black' | 'white';
    selectable?: boolean;
    selected?: boolean;
    onClick?: () => void;
};

export function Checkers({ count, player, selectable = false, selected = false, onClick }: CheckersProps) {
    return (<g className={selectable ? "selectable-container" : undefined} onClick={onClick}>
        {arrayOf(count).map((_, idx) => <g transform={`translate(0 ${checkerFactor(idx, count) * checkerDiameter})`} key={idx}>
            <Checker player={player} selectable={count - 1 === idx && selectable} selected={count - 1 === idx && selected} />
        </g>)}
        {selectable
            ? <rect x={-checkerRadius} y={0} width={checkerDiameter} height={checkerDiameter * checkersMaxCount} fill="transparent" />
            : null}
    </g>);
}

const checkersMaxCount = 5;
function checkerFactor(index: number, totalCount: number) {
    const expected = totalCount - 1;
    const max = Math.min(checkersMaxCount, totalCount) - 1;

    return 0.5 + (index === 0 ? 0 : index * (max / expected));
}
