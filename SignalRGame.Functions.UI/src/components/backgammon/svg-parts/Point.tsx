import React from 'react';
import { checkerRadius, pointBasePct, boardHeight, pointHeightPct, checkerDiameter, anchorRight, anchorBottom, anchorLeft, anchorTop, boardPositions } from './sizes';

import "./styles.css";
import { bar, home } from '../pointsToCheckers';


export function pointPosition(index: number): { x: number, y: number, reverse: boolean};
export function pointPosition(index: number | bar | home, player: 'white' | 'black'): { x: number, y: number, reverse: boolean};
export function pointPosition(index: number | bar | home, player: 'white' | 'black' = 'white') {
    if (index === home) {
        throw new Error("Home not yet rendered");
    }
    if (index === bar) {
        return player === 'black' ? boardPositions.blackBar : boardPositions.whiteBar;
    }
    if (index < 6) {
        return { x: anchorRight - checkerDiameter * (0.5 + index), y: anchorBottom, reverse: true };
    } else if (index < 12) {
        return { x: anchorLeft + checkerDiameter * (0.5 + 11 - index), y: anchorBottom, reverse: true };
    } else if (index < 18) {
        return { x: anchorLeft + checkerDiameter * (0.5 + index - 12), y: anchorTop, reverse: false };
    } else if (index < 24) {
        return { x: anchorRight - checkerDiameter * (0.5 + 23 - index), y: anchorTop, reverse: false };
    }
    throw new Error("Invalid index " + index);
}

export function pointTransform(index: number): string;
export function pointTransform(index: number | bar | home, player: 'white' | 'black'): string;
export function pointTransform(index: number | bar | home, player: 'white' | 'black' = 'white') {
    const { x, y, reverse } = pointPosition(index, player);
    return `translate(${x}, ${y}) rotate(${reverse ? 180 : 0})`;
}

export type PointProps = {
    color: 'white' | 'black' | 'transparent';
    selectable?: boolean;
    selected?: boolean;
    onClick?: () => void;
}

export const Point = ({ color, selectable = false, selected = false, onClick }: PointProps) =>
    <g className={`point ${color} ${selectable ? 'selectable' : ''} ${selected ? 'selected' : ''}`} onClick={onClick}>
        <path d={`M${-checkerRadius * pointBasePct},0 L${checkerRadius * pointBasePct},0 L0,${boardHeight * pointHeightPct}z`} className={`point ${color}`} />
        {selectable
            ? <rect x={-checkerRadius} width={checkerDiameter} height={(anchorBottom - anchorTop) / 2} fill="transparent" className="hitbox" />
            : null}
    </g>;
