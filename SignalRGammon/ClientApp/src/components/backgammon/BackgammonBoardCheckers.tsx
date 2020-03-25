import React, { useState } from 'react';
import { useBackgammon } from './BackgammonService';
import { pointTransform, Point } from './svg-parts/Point';
import { Checkers, fullCheckerTranslation } from './svg-parts/Checkers';
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

    return (
        <>
            <HomeArea selectable={isAllowed(homeValue)} onClick={doBearOff} />
            {players.map(player =>
                <React.Fragment key={player}>
                    {checkers[player].map(checker => (
                        <g style={{ transition: "transform 1s", pointerEvents: "none" }} transform={fullCheckerTranslation(checker.indexInLocation, checker.ofCount, checker.location, player)} key={checker.id}>
                            <Checker player={player} />
                        </g>
                    ))}
                </React.Fragment>
            )}
            {state.points.map((point, idx) =>
                <g transform={pointTransform(idx)} key={idx}>
                    {players.map(player =>
                        <Checkers key={player}
                                  count={point[player]}
                                  player="transparent"
                                  selectable={canSelectChecker && playerColor === player}
                                  selected={selectedChecker === idx}
                                  onClick={() => (canSelectChecker && playerColor === player) ? setSelectedChecker(idx) : selectPoint(idx)} />
                    )}
                    {selectedChecker !== null
                        ? <Point color="transparent" selectable={isAllowed(idx)} onClick={() => selectPoint(idx)} />
                        : null}
                </g>
            )}
            {players.map(player =>
                <g transform={pointTransform(bar, player)} key={player}>
                    <Checkers
                        count={state.bar[player]} player={"transparent"}
                        selectable={canSelectChecker && playerColor === player}
                        selected={playerColor === player && selectedChecker === barValue}
                        onClick={() => (canSelectChecker && playerColor === player) ? setSelectedChecker(barValue) : setSelectedChecker(null)} />
                </g>
            )}
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
