import React from 'react';
import { checkerRadius, stroke } from './sizes';

import "./styles.css";

export const Checker = ({ player, selectable = false, selected = false, onClick }: { player: 'white' | 'black' | 'transparent', selectable?: boolean, selected?: boolean, onClick?: () => void }) =>
    <g className={`checker ${player} ${selectable ? "selectable" : ""} ${selected ? "selected" : ""}`} onClick={onClick}>
        <circle r={checkerRadius * 0.95} style={{}}  strokeWidth={stroke} className="glow" />
        <circle r={checkerRadius * 0.95} style={{}}  strokeWidth={stroke} />
        <circle r={checkerRadius * 0.8} style={{}} strokeWidth={stroke} />
    </g>;
