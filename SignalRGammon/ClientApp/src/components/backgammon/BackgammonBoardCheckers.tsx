import React, { useState } from 'react';
import { useBackgammon } from './BackgammonService';
import { boardPositions } from './svg-parts/sizes';
import { pointTransform, Point } from './svg-parts/Point';
import { Checkers } from './svg-parts/Checkers';
import { HomeArea } from './svg-parts/HomeArea';
import { BackgammonState } from './BackgammonState';

const barValue = -1;
const homeValue = 24;

const direction = {
    'black': 1,
    'white': -1,
}

const barEffectiveValue = {
    'black': -1,
    'white': 24,
}

const homeEffectiveValue = {
    'black': 24,
    'white': -1,
}

const isHomeEffectiveValue = {
    'black': (v: number) => v >= 24,
    'white': (v: number) => v < 0,
}

function normalizeGutter(startingPoint: number, playerColor: 'white' | 'black') {
    return (startingPoint === barValue ? barEffectiveValue[playerColor] : startingPoint);
}

function normalizeBearOff(endPoint: number, playerColor: 'white' | 'black') {
    return isHomeEffectiveValue[playerColor](endPoint) ? homeValue : endPoint;
}

function contains(array: number[], value: number) {
    return array.indexOf(value) !== -1;
}

export function BackgammonBoardCheckers({ canRoll, state }: {
    canRoll: boolean;
    state: BackgammonState;
}) {
    const { move, bearOff, playerColor } = useBackgammon();
    const [selectedChecker, setSelectedChecker] = useState(null as (number | null));
    const canSelectChecker = selectedChecker === null && !canRoll;
    const allowedPoints = selectedChecker === null
        ? []
        : state.diceRolls[playerColor].map(die => normalizeBearOff(normalizeGutter(selectedChecker, playerColor) + die * direction[playerColor], playerColor));
    const isPlayerBlack = playerColor === 'black';
    const isPlayerWhite = playerColor === 'white';
    const bar = state.bar;

    return (
        <>
            <HomeArea selectable={isAllowed(homeValue)} onClick={doBearOff} />
            {state.currentPlayer && state.points.map(({ black, white }, idx) => <g transform={pointTransform(idx)} key={idx}>
                <Checkers count={black} player="black" selectable={canSelectChecker && isPlayerBlack} selected={selectedChecker === idx} onClick={() => (canSelectChecker && isPlayerBlack) ? setSelectedChecker(idx) : selectPoint(idx)} />
                <Checkers count={white} player="white" selectable={canSelectChecker && isPlayerWhite} selected={selectedChecker === idx} onClick={() => (canSelectChecker && isPlayerWhite) ? setSelectedChecker(idx) : selectPoint(idx)} />
                {selectedChecker !== null
                    ? <Point color="transparent" selectable={isAllowed(idx)} onClick={() => selectPoint(idx)} />
                    : null}
            </g>)}
            <g transform={boardPositions.blackBar}>
                <Checkers count={bar.black} player="black" selectable={canSelectChecker && isPlayerBlack} selected={isPlayerBlack && selectedChecker === barValue} onClick={() => (canSelectChecker && isPlayerBlack) ? setSelectedChecker(barValue) : setSelectedChecker(null)} />
            </g>
            <g transform={boardPositions.whiteBar}>
                <Checkers count={bar.white} player="white" selectable={canSelectChecker && isPlayerWhite} selected={isPlayerWhite && selectedChecker === barValue} onClick={() => (canSelectChecker && isPlayerWhite) ? setSelectedChecker(barValue) : setSelectedChecker(null)} />
            </g>
        </>
    );

    function isAllowed(v: number) { return contains(allowedPoints, v); }

    async function selectPoint(index: number) {
        if (selectedChecker === null) {
            return;
        }
        const dieValue = (index - normalizeGutter(selectedChecker, playerColor)) * direction[playerColor];
        setSelectedChecker(null);
        if (contains(allowedPoints, index))
            await move(dieValue, selectedChecker);
    }

    async function doBearOff() {
        if (selectedChecker === null) {
            return;
        }
        const minDieValue = (homeEffectiveValue[playerColor] - normalizeGutter(selectedChecker, playerColor)) * direction[playerColor];
        const rolls = state.diceRolls[playerColor].filter(roll => roll >= minDieValue).sort();
        const dieValue = rolls[0];
        setSelectedChecker(null);
        if (!dieValue) {
            return;
        }
        await bearOff(dieValue, selectedChecker);
    }
}
