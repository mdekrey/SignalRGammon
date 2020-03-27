import React from 'react';

import "./styles.css";
import { boardWidth, boardHeight } from './sizes';

export function filtersVariables(): Object {
    return {
        '--dropshadow': `url(${window.location.pathname}#dropshadow)`,
        '--noise': `url(${window.location.pathname}#noise)`,
        '--selectionGlow': `url(${window.location.pathname}#selectionGlow)`,
        '--boardMask': `url(${window.location.pathname}#boardMask)`,
    };
}

export const Filters = () =>
    <defs>
        <filter id="dropshadow" height="130%">
            <feGaussianBlur in="SourceAlpha" stdDeviation="3" /> {/* stdDeviation is how much to blur */}
            <feOffset dx="2" dy="2" result="offsetblur" /> {/* how much to offset */}
            <feComponentTransfer>
                <feFuncA type="linear" slope="0.5" /> {/* slope is the opacity of the shadow */}
            </feComponentTransfer>
            <feMerge>
                <feMergeNode /> {/* this contains the offset blurred image */}
                <feMergeNode in="SourceGraphic" /> {/* this contains the element that the filter is applied to */}
            </feMerge>
        </filter>
        <filter id='noise'>
            <feTurbulence type='fractalNoise' baseFrequency='.2' numOctaves='3' stitchTiles='stitch' />
        </filter>

        <filter id="selectionGlow" height="300%" width="300%" x="-75%" y="-75%">
            {/* Thicken out the original shape */}
            <feMorphology operator="dilate" radius="4" in="SourceAlpha" result="thicken" />

            {/* Use a gaussian blur to create the soft blurriness of the glow */}
            <feGaussianBlur in="thicken" stdDeviation="2" result="blurred" />

            {/* Change the colour */}
            <feFlood floodColor="rgb(255,186,0)" result="glowColor" />

            {/* Color in the glows */}
            <feComposite in="glowColor" in2="blurred" operator="in" result="softGlow_colored" />

            {/*	Layer the effects together */}
            <feMerge>
                <feMergeNode in="softGlow_colored" />
                <feMergeNode in="SourceGraphic" />
            </feMerge>

        </filter>
        <mask id="boardMask">
            <rect x="0" y="0" width={boardWidth} height={boardHeight} fill="white" />
        </mask>
    </defs>
