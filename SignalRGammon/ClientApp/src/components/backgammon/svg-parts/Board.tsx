import React from 'react';

import "./styles.css";
import { Felt } from './Felt';
import { boardWidth, boardHeight, checkerDiameter, border, anchorLeft, anchorTop, anchorBottom, anchorRight } from './sizes';
import { Point, PointPosition } from './Point';

export const Board = () =>
    <g mask="url(#boardMask)" >
        <Felt width={boardWidth} height={boardHeight} />
        {Array(24).fill(0).map((_, idx) => PointPosition(idx)).map(({ x, y, rotation }, idx) =>
            <g transform={`translate(${x}, ${y}) rotate(${rotation})`} key={idx}>
                <Point color={idx % 2 == 0 ? 'white' : 'black'} />
            </g>
        )}

        <g className="walls">
            <rect width={border} height={boardHeight}></rect>
            <rect width={boardWidth} height={anchorTop}></rect>
            <rect width={border} height={boardHeight} x={boardWidth - border}></rect>
            <rect width={boardWidth} height={anchorTop} y={anchorBottom}></rect>
            <rect width={border} height={boardHeight} x={border + checkerDiameter}></rect>
            <rect width={border} height={boardHeight} x={anchorRight}></rect>
            <rect width={checkerDiameter + border * 2} height={anchorTop} y={(anchorBottom) / 2}></rect>
            <rect width={checkerDiameter + border * 2} height={anchorTop} x={anchorRight} y={(anchorBottom) / 2}></rect>
            <rect width={border * 2} height={boardHeight} x={boardWidth / 2 - border}></rect>
        </g>
    </g>