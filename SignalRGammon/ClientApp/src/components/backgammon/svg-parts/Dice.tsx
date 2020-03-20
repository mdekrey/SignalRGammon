import React from 'react';
import { dieSeparation } from './sizes';

import "./styles.css";
import { Die } from './Die';

export type DiceProps = {
    player: 'white' | 'black';
    values: (number | null)[];
    selectable?: boolean;
    onSelect?: (value: number, index: number) => void;
};

export const Dice = ({ player, values, selectable = false, onSelect }: DiceProps) => {
    return <>
        {values.map((value, index) =>
            value
                ? <g transform={`translate(${((values.length - 1) / 2 - index) * dieSeparation}, 0)`} key={`${value}_${index}`}>
                    <Die player={player} value={value} selectable={selectable} onSelect={onSelect ? () => onSelect(value, index) : undefined} />
                </g>
                : <React.Fragment key={index} />
        )}
    </>;
}
