import React from 'react';
import { checkerRadius, stroke, blackRotation, whiteRotation } from './sizes';

import "./styles.css";

export type CheckerProps = {
    player: 'white' | 'black' | 'transparent'
    isKing?: boolean
    selectable?: boolean
    selected?: boolean
    onClick?: () => void
};

export const Checker = ({ player, isKing = false, selectable = false, selected = false, onClick }: CheckerProps) =>
    <g className={`checker ${player} ${selectable ? "selectable" : ""} ${selected ? "selected" : ""}`} onClick={onClick}>
        <circle r={checkerRadius * 0.95} style={{}}  strokeWidth={stroke} className="glow" />
        <circle r={checkerRadius * 0.95} style={{}}  strokeWidth={stroke} />
        <circle r={checkerRadius * 0.8} style={{}} strokeWidth={stroke} />
        {/* King crown is modified from https://game-icons.net/1x1/lorc/crowned-heart.html */}
        {isKing && <path d="M0 -12 l-12 14 l-16-12 l6 24 h44 l6-24 l-16 12 l-12-14z" strokeWidth={stroke} transform={player === 'white' ? whiteRotation : blackRotation} />}
    </g>;
