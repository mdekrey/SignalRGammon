import React from 'react';
import { checkerDiameter, anchorRight, border, anchorTop, anchorBottom } from './sizes';

export type HomeAreaProps = {
    selectable?: boolean;
    selected?: boolean;
    onClick?: () => void;
};

export function HomeArea({ selectable = false, selected = false, onClick }: HomeAreaProps) {
    return (<g className={`home ${selectable ? "selectable" : undefined} ${selected ? "selected" : undefined}`} onClick={onClick}>
        <rect x={anchorRight + border} y={anchorTop} width={checkerDiameter} height={anchorBottom - anchorTop} fill="transparent" />
    </g>);
}
