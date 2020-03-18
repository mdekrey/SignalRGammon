import React from 'react';
import { checkerRadius, stroke } from './sizes';

import "./styles.css";

export const Felt = ({ width, height }: { width: number; height: number }) =>
    <g className="felt">
        <rect width={width} height={height} />
        <rect width={width} height={height} />
    </g>;
