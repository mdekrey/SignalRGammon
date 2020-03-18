import React from 'react';
import { checkerRadius, stroke } from './sizes';

import "./styles.css";

export const Checker = ({ player, selectable = false, selected = false }: { player: 'white' | 'black', selectable?: boolean, selected?: boolean }) =>
    <g className={`checker ${player} ${selectable ? 'selectable' : ''} ${selected ? 'selected' : ''}`}>
        <circle r={checkerRadius * 0.95} style={{}}  strokeWidth={stroke} />
        <circle r={checkerRadius * 0.8} style={{}} strokeWidth={stroke} />
    </g>;
