import React from 'react';
import { checkerRadius, pointBasePct, boardHeight, pointHeightPct, checkerDiameter, anchorRight, anchorBottom, anchorLeft, anchorTop } from './sizes';

import "./styles.css";

const pointPosition = (index: number) => {
    if (index < 6) {
        return { x: anchorRight - checkerDiameter * (0.5 + index), y: anchorBottom, rotation: 180 };
    } else if (index < 12) {
        return { x: anchorLeft + checkerDiameter * (0.5 + 11 - index), y: anchorBottom, rotation: 180 };
    } else if (index < 18) {
        return { x: anchorLeft + checkerDiameter * (0.5 + index - 12), y: anchorTop, rotation: 0 };
    } else if (index < 24) {
        return { x: anchorRight - checkerDiameter * (0.5 + 23 - index), y: anchorTop, rotation: 0 };
    }
    throw new Error("Invalid index " + index);
}

export const pointTransform = (index: number) => {
    const { x, y, rotation } = pointPosition(index);
    return `translate(${x}, ${y}) rotate(${rotation})`;
}

export const Point = ({ color }: { color: 'white' | 'black' }) =>
    <g className={`point ${color}`}>
        <path d={`M${-checkerRadius * pointBasePct},0 L${checkerRadius * pointBasePct},0 L0,${boardHeight * pointHeightPct}z`} className={`point ${color}`} />
    </g>;
