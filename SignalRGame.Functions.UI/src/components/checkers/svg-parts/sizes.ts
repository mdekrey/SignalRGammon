
export const stroke = 2;

export const checkerRadius = 50;
export const checkerDiameter = 100;

export const padding = 10;
export const gridSize = checkerDiameter + padding;

export const boardWidth = 8 * gridSize;
export const boardHeight = boardWidth;

export const blackRotation = undefined;
export const whiteRotation = `rotate(180)`;

export const boardPositions = Object.freeze({
    viewbox: `0 0 ${boardWidth} ${boardHeight}`,
    blackRotation: undefined,
    whiteRotation: `translate(${boardWidth},${boardHeight}) ${whiteRotation}`,
});
