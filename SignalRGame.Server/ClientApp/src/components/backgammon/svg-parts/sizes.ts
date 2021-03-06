
export const stroke = 2;

export const checkerRadius = 50;
export const checkerDiameter = 100;

export const dieSize = 100;
export const pipRadius = 10;
export const pipPercentage = 0.3;
export const dieSeparation = 150;

export const barWidth = 0.75 * checkerDiameter;
export const border = 0.375 * checkerDiameter;

export const boardWidth = 14 * checkerDiameter + border * 6;
export const boardHeight = 6 * 2 * checkerDiameter + border * 2;

export const pointBasePct = 0.75;
export const pointHeightPct = 1/3;


export const anchorTop = border;
export const anchorBottom = boardHeight - border;
export const anchorLeft = border * 2 + checkerDiameter;
export const anchorRight = boardWidth - (border * 2 + checkerDiameter);

export const whiteDicePosition = `translate(${(anchorRight - anchorLeft) / 4 + anchorLeft - border / 2}, ${boardHeight / 2}) rotate(20)`;
export const blackDicePosition = `translate(${(anchorRight - anchorLeft) * 3 / 4 + anchorLeft + border / 2}, ${boardHeight / 2}) rotate(-20)`;

export const boardPositions = Object.freeze({
    viewbox: `0 0 ${boardWidth} ${boardHeight}`,
    blackRotation: undefined,
    whiteRotation: `translate(${boardWidth},${boardHeight}) rotate(180)`,
    blackBar: { x: boardWidth / 2, y: boardHeight / 2, reverse: true },
    whiteBar: { x: boardWidth / 2, y: boardHeight / 2, reverse: false },
    whiteDice: `translate(${boardWidth / 4}, ${boardHeight / 2})`,
    blackDice: `translate(${boardWidth * 3 / 4}, ${boardHeight / 2})`,
});
