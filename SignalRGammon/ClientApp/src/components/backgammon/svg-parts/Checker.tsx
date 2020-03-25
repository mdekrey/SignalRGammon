import React from 'react';
import { checkerRadius, stroke } from './sizes';

import "./styles.css";

export const Checker = ({ player }: { player: 'white' | 'black' }) =>
    <g className={`checker ${player}`}>
        <circle r={checkerRadius * 0.95} style={{}}  strokeWidth={stroke} />
        <circle r={checkerRadius * 0.8} style={{}} strokeWidth={stroke} />
    </g>;
