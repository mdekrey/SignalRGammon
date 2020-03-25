import React from 'react';
import { checkerDiameter, checkerRadius } from './sizes';
import { Checker } from './Checker';
import { arrayOf } from '../../../utils/arrayOf';
import { bar, home } from '../pointsToCheckers';
import { pointPosition } from './Point';

export type CheckersProps = {
    count: number;
    player: 'black' | 'white';
    selectable?: boolean;
    selected?: boolean;
    onClick?: () => void;
};

export function Checkers({ count, player, selectable = false, selected = false, onClick }: CheckersProps) {
    if (!count)
        return null;
    return (<g className={selectable ? "selectable-container" : undefined} onClick={onClick}>
        {arrayOf(count).map((_, idx) => <g transform={checkerTranslation(idx, count)} key={idx}>
            <Checker player={player} selectable={count - 1 === idx && selectable} selected={count - 1 === idx && selected} />
        </g>)}
        {selectable
            ? <rect x={-checkerRadius} y={0} width={checkerDiameter} height={checkerDiameter * checkersMaxCount} fill="transparent" />
            : null}
    </g>);
}


export function fullCheckerTranslation(checkerIdx: number, count: number, index: number): string;
export function fullCheckerTranslation(checkerIdx: number, count: number, index: number | bar | home, player: 'white' | 'black'): string;
export function fullCheckerTranslation(checkerIdx: number, count: number, index: number | bar | home, player: 'white' | 'black' = 'white') {
    const wallDist = checkerDistanceFromWall(checkerIdx, count);
    const { x, y, reverse } = pointPosition(index, player);

    return `translate(${x} ${y + (reverse ? -1 : 1) * wallDist})`;
}

export function checkerTranslation(idx: number, count: number) {
    return `translate(0 ${checkerDistanceFromWall(idx, count)})`;
}

function checkerDistanceFromWall(idx: number, count: number) {
    return checkerFactor(idx, count) * checkerDiameter;
}

const checkersMaxCount = 5;
function checkerFactor(index: number, totalCount: number) {
    const expected = totalCount - 1;
    const max = Math.min(checkersMaxCount, totalCount) - 1;

    return 0.5 + (index === 0 ? 0 : index * (max / expected));
}
