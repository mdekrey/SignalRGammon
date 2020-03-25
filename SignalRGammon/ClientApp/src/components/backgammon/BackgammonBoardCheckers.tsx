import React, { useState } from 'react';
import { useBackgammon } from './BackgammonService';
import { boardPositions } from './svg-parts/sizes';
import { pointTransform, Point } from './svg-parts/Point';
import { Checkers, checkerTranslation } from './svg-parts/Checkers';
import { HomeArea } from './svg-parts/HomeArea';
import { BackgammonState } from './BackgammonState';
import { bar, home, Checkers as CheckersData, players } from './pointsToCheckers';
import { Checker } from './svg-parts/Checker';

const barValue: bar = bar;
const homeValue: home = home;

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

function normalizeGutter(startingPoint: number | bar, playerColor: 'white' | 'black') {
    return (startingPoint === barValue ? barEffectiveValue[playerColor] : startingPoint);
}

function normalizeBearOff(endPoint: number, playerColor: 'white' | 'black') {
    return isHomeEffectiveValue[playerColor](endPoint) ? homeValue : endPoint;
}

function contains<T>(array: T[], value: T) {
    return array.indexOf(value) !== -1;
}

export function BackgammonBoardCheckers({ canRoll, state, checkers, diceRolls }: {
    canRoll: boolean;
    state: BackgammonState;
    checkers: CheckersData;
    diceRolls: BackgammonState["diceRolls"];
}) {
    const { move, bearOff, playerColor } = useBackgammon();
    const [selectedChecker, setSelectedChecker] = useState(null as (number | bar | null));
    const canSelectChecker = selectedChecker === null && !canRoll;
    const allowedPoints = selectedChecker === null
        ? []
        : diceRolls[playerColor].map(die => normalizeBearOff(normalizeGutter(selectedChecker, playerColor) + die * direction[playerColor], playerColor));
    const isPlayerBlack = playerColor === 'black';
    const isPlayerWhite = playerColor === 'white';

    return (
        <>
            <HomeArea selectable={isAllowed(homeValue)} onClick={doBearOff} />
            {state.points.map(({ black, white }, idx) =>
                <g transform={pointTransform(idx)} key={idx}>
                    <Checkers count={black} player="black" selectable={canSelectChecker && isPlayerBlack} selected={selectedChecker === idx} onClick={() => (canSelectChecker && isPlayerBlack) ? setSelectedChecker(idx) : selectPoint(idx)} />
                    <Checkers count={white} player="white" selectable={canSelectChecker && isPlayerWhite} selected={selectedChecker === idx} onClick={() => (canSelectChecker && isPlayerWhite) ? setSelectedChecker(idx) : selectPoint(idx)} />
                    {selectedChecker !== null
                        ? <Point color="transparent" selectable={isAllowed(idx)} onClick={() => selectPoint(idx)} />
                        : null}
                </g>
            )}
            <g transform={boardPositions.blackBar}>
                <Checkers count={state.bar.black} player="black" selectable={canSelectChecker && isPlayerBlack} selected={isPlayerBlack && selectedChecker === barValue} onClick={() => (canSelectChecker && isPlayerBlack) ? setSelectedChecker(barValue) : setSelectedChecker(null)} />
            </g>
            <g transform={boardPositions.whiteBar}>
                <Checkers count={state.bar.white} player="white" selectable={canSelectChecker && isPlayerWhite} selected={isPlayerWhite && selectedChecker === barValue} onClick={() => (canSelectChecker && isPlayerWhite) ? setSelectedChecker(barValue) : setSelectedChecker(null)} />
            </g>
            {players.map(player =>
                checkers[player].map(checker => (
                    <g style={{ transition: "transform 1s" }} transform={` ${pointTransform(checker.location, player)} ${checkerTranslation(checker.indexInLocation, checker.ofCount)}`} key={checker.id}>
                        <Checker player={player} />
                    </g>
            )))}
        </>
    );

    function isAllowed(v: number | home) { return contains(allowedPoints, v); }

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
