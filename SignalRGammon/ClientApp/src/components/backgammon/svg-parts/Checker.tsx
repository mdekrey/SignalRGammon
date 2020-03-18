import React from 'react';
import { checkerRadius, stroke } from './sizes';

import "./styles.css";

export const Checker = ({ player, selectable = false }: { player: 'white' | 'black', selectable?: boolean }) =>
    <g className={`checker ${player} ${selectable ? 'selectable' : ''}`}>
        <circle r={checkerRadius * 0.95} style={{}}  strokeWidth={stroke} />
        <circle r={checkerRadius * 0.8} style={{}} strokeWidth={stroke} />
    </g>;
