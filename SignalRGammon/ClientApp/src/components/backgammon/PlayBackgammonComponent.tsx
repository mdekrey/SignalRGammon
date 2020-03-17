import React from 'react';
import { useRx } from '../../utils/useRx';
import { useBackgammon } from './BackgammonService';

export function PlayBackgammonComponent() {
    const { state, roll, otherPlayerUrl } = useBackgammon();

    const gameState = useRx(state, null);
    console.log(gameState);

    return (
        <div>
            {otherPlayerUrl}
            <button onClick={roll} disabled={!gameState}>Roll</button>
        </div>
    );
}
