
export const stroke = 2;

export const checkerRadius = 50;
export const checkerDiameter = 100;


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