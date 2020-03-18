import React from 'react';
import { dieSeparation } from './sizes';

import "./styles.css";
import { Die } from './Die';

export const Dice = ({ player, values, selectable = false }: { player: 'white' | 'black', values: (number | null)[], selectable?: boolean }) => {
    return <>
        {values.map((value, index) =>
            value
                ? <g transform={`translate(${((values.length - 1) / 2 - index) * dieSeparation}, 0)`} key={`${value}_${index}`}>
                    <Die player={player} value={value} selectable={selectable} />
                </g>
                : <React.Fragment key={index} />
        )}
    </>;
}
