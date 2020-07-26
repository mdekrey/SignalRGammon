import { BackgammonState } from "./BackgammonState";

export const players = Object.freeze<PlayerColor[]>([ 'black', 'white' ]);
type PlayerColor = 'white' | 'black';
type PerPlayer<T> = {
    [color in PlayerColor]: T;
}

export const bar = Symbol();
export type bar = typeof bar;

export const home = Symbol();
export type home = typeof home;

export type Checker = {
    location: bar | home | number; // 0-23
    id: string;
    indexInLocation: number;
    ofCount: number;
}

export type Checkers = PerPlayer<Checker[]>;

type Points = {
    [bar]: Checker["id"][];
    [home]: Checker["id"][];
    [position: number]: Checker["id"][];
}
const pointKeys = Object.freeze<(keyof Points)[]>([ bar, home, 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23 ]);
const makePoints = () => pointKeys.reduce((result, key) => { result[key] = []; return result; }, {} as Points);

export function pointsToCheckers(state: BackgammonState, prev: Checkers = { white: [], black: [] }): Checkers {
    const prevPoints = mapCheckers(prev);

    const { checkers: white, changed: whiteChanged } = doAdjustment(prevPoints, state, 'white');
    const { checkers: black, changed: blackChanged } = doAdjustment(prevPoints, state, 'black');
    return whiteChanged || blackChanged ? { white, black } : prev;
}

function mapCheckers(prev: Checkers): PerPlayer<Points> {
    const result = {
        white: tallyCheckers(prev['white']),
        black: tallyCheckers(prev['black']),
    };

    return result;
}

function tallyCheckers(checkers: Checker[]): Points {
    const result = makePoints();
    for (const checker of checkers) {
        result[checker.location][checker.indexInLocation] = checker.id;
    }
    for (const key of pointKeys) {
        result[key] = result[key].filter(() => true); // removes Empty values
    }
    return result;
}

export function getCheckerCount(key: keyof Points, player: PlayerColor, state: BackgammonState) {
    if (key === bar) {
        return state.bar[player];
    } else if (key === home) {
        return 0;
    } else {
        return state.points[key][player];
    }
}

const makeId = () => Math.random().toString(36).substring(7);

function doAdjustment(points: PerPlayer<Points>, state: BackgammonState, player: PlayerColor): { checkers: Checker[], changed: boolean } {
    let changed = false;
    const unused: Checker["id"][] = [];
    const checkers: Checker[] = [];

    for (const key of pointKeys) {
        const expectedCount = getCheckerCount(key, player, state);
        for (let index = 0; index < points[player][key].length; index++) {
            const id = points[player][key][index];
            if (index < expectedCount) {
                checkers.push({ location: key, id, indexInLocation: index, ofCount: expectedCount });
            } else {
                unused.push(id);
                changed = true;
            }
        }
    }

    for (const key of pointKeys) {
        const expectedCount = getCheckerCount(key, player, state);
        if (points[player][key].length < expectedCount) {
            for (let i = points[player][key].length; i < expectedCount; i++) {
                let id: Checker["id"];
                if (unused.length) {
                    id = unused.pop()!;
                } else {
                    id = makeId();
                }
                checkers.push({ location: key, id, indexInLocation: i, ofCount: expectedCount });
                changed = true;
            }
        }
    }

    checkers.sort((a, b) => a.id.localeCompare(b.id));
    return { checkers, changed };
}
