import React from 'react';
import { checkerDiameter, checkerRadius, stroke } from './sizes';
import { bar, home } from '../pointsToCheckers';
import { pointPosition } from './Point';

export type CheckersProps = {
    count: number;
    selectable?: boolean;
    selected?: boolean;
    onClick?: () => void;
};

const CheckerGlow = ({ selectable = false, selected = false }: { selectable?: boolean, selected?: boolean }) =>
    <g className={`checker transparent ${selectable ? 'selectable' : ''} ${selected ? 'selected' : ''}`}>
        <circle r={checkerRadius * 0.95} className="glow hitbox" strokeWidth={stroke} />
    </g>;

export function Checkers({ count, selectable = false, selected = false, onClick }: CheckersProps) {
    if (!count)
        return null;
    if (!selectable && !selected)
        return null;
    return (
        <g className={selectable ? "selectable-container" : undefined} onClick={onClick}>
            <g transform={checkerTranslation(count - 1, count)}>
                <CheckerGlow selectable={selectable} selected={selected} />
            </g>
            {selectable
                ? <rect x={-checkerRadius} y={0} width={checkerDiameter} height={checkerDiameter * checkersMaxCount} fill="transparent" />
                : null}
        </g>
    );
}


export function fullCheckerTranslation(checkerIdx: number, count: number, index: number): string;
export function fullCheckerTranslation(checkerIdx: number, count: number, index: number | bar | home, player: 'white' | 'black'): string;
export function fullCheckerTranslation(checkerIdx: number, count: number, index: number | bar | home, player: 'white' | 'black' = 'white') {
    const wallDist = checkerDistanceFromWall(checkerIdx, count);
    const { x, y, reverse } = pointPosition(index, player);

    return `translate(${x}px, ${y + (reverse ? -1 : 1) * wallDist}px)`;
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
